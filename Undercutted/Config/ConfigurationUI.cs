using ImGuiNET;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Undercutted.Core;

namespace Undercutted
{
    class ConfigurationUI : IDisposable
    {
        private readonly Configuration Configuration;

        private bool settingsVisible = false;
        public bool SettingsVisible
        {
            get { return this.settingsVisible; }
            set { this.settingsVisible = value; }
        }

        private bool UpdateInProgress { get; set; } = false;

        private MarketBoardData? MarketBoardData { get; set; }

        public ConfigurationUI(Configuration configuration)
        {
            Configuration = configuration;
            UpdateData();
        }

        public void Dispose() {}

        private static string SearchText = "";
        public void Draw()
        {
            if (!SettingsVisible)
            {
                return;
            }

            if (ImGui.Begin("Undercutted", ref this.settingsVisible,
                ImGuiWindowFlags.NoCollapse))
            {
                if (ImGui.Button(UpdateInProgress ? "Refreshing..." : "Refresh prices")) {
                    UpdateData();
                    Draw();
                }

                List<string> columns = new() { "Item", "Listings #", "Lowest price", "By", "Delete", "Last update" };
                if (ImGui.BeginTable("ItemsTable", columns.Count, ImGuiTableFlags.Borders))
                {
                    ImGui.TableSetupColumn("Item", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("Listings #", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("Lowest price", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("By", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("Delete", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableHeadersRow();

                    for (int row = 0; row < Configuration.ItemNames.Count; row++)
                    {
                        ImGui.TableNextRow();

                        string itemName = Configuration.ItemNames[row];
                        ItemCurrentData? item = MarketBoardData?.Items.FirstOrDefault(item => item.ItemName == itemName);

                        //item name
                        ImGui.TableNextColumn();
                        ImGui.SetNextItemWidth(-1);
                        
                        ImGui.Text(itemName);
                        ImGui.SameLine();
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetColumnWidth() - 20);
                        if (ImGui.BeginCombo($"###{row}itemNameCombo", string.Empty, ImGuiComboFlags.NoPreview))
                        {
                            ImGui.InputText("Search", ref SearchText, 512);

                            ExcelSheet<Item>? list = Undercutted.GameItemsList;
                            if (list != null)
                            {
                                foreach (Item gameItem in list)
                                {
                                    string name = gameItem.Name.RawString;
                                    if (!String.IsNullOrEmpty(SearchText) && !name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) continue;
                                    bool selected = name.Equals(itemName, StringComparison.OrdinalIgnoreCase);

                                    if (ImGui.Selectable(name, selected))
                                    {
                                        Configuration.ItemNames[row] = name;
                                        SaveConfig();
                                    }
                                }
                            }
                            
                            ImGui.EndCombo();
                        }

                        if (item != null && item.Listings.Count > 0)
                        {
                            //listing #
                            ImGui.TableNextColumn();
                            string listingsCount = item.Listings.Count.ToString();
                            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetColumnWidth() -
                                ImGui.CalcTextSize(listingsCount).X - 5);
                            ImGui.Text(listingsCount);

                            //Lowest price
                            ImGui.TableNextColumn();
                            string price = item.Listings[0].PricePerUnit.ToString("#,##");
                            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetColumnWidth() - 
                                ImGui.CalcTextSize(price).X - 5);
                            ImGui.Text(price);

                            //By
                            ImGui.TableNextColumn();
                            ImGui.SetNextItemWidth(-1);
                            ImGui.Text(item.Listings[0].RetainerName.ToString());

                            //Last update
                            ImGui.TableNextColumn();
                            ImGui.Text(item.LastUpload.ToString());
                        }

                        //Delete
                        ImGui.TableSetColumnIndex(columns.Count - 1);
                        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(255, 0, 0, 255));
                        if (ImGui.Button($"X###{row}delete", new Vector2(-1, 20)))
                        {
                            Configuration.ItemNames.RemoveAt(row);
                            SaveConfig();
                        };
                        ImGui.PopStyleColor(1);

                    }

                    ImGui.EndTable();
                }

                if (ImGui.Button("Add new item"))
                {
                    Configuration.ItemNames.Add("");
                }

                ImGui.End();
            }
        }

        private void SaveConfig()
        {
            Configuration.ItemNames.RemoveAll(name => String.IsNullOrEmpty(name));
            Configuration.Save();
            Undercutted.Configuration = Configuration;
        }

        private async void UpdateData()
        {
            string? playerHomeWorld = Undercutted.ClientState?.LocalPlayer?.HomeWorld?.GameData?.Name?.RawString;
            if (playerHomeWorld == null) return;
            if (Configuration.ItemNames.Count == 0) return;
            
            UpdateInProgress = true;
            MarketBoardData = await ApiConsumer.GetData(playerHomeWorld, Configuration.ItemNames);
            UpdateInProgress = false;
        }
    }
}
