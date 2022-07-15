using ImGuiNET;
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

        private MarketBoardData? MarketBoardData { get; set; }

        public ConfigurationUI(Configuration configuration)
        {
            Configuration = configuration;
            UpdateData();
        }

        public void Dispose() {}

        public void Draw()
        {
            if (!SettingsVisible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(500, 200), ImGuiCond.Appearing);
            if (ImGui.Begin("Undercutted", ref this.settingsVisible,
                ImGuiWindowFlags.NoCollapse))
            {
                if (ImGui.Button("Refresh")) {
                    UpdateData();
                    Draw();
                }

                if (ImGui.BeginTable("ItemsTable", 4, ImGuiTableFlags.Borders))
                {
                    ImGui.TableSetupColumn("Item", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("Listings #", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("Lowest price", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("By", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableHeadersRow();

                    for (int row = 0; row < MarketBoardData?.Items?.Count; row++)
                    {
                        ItemCurrentData item = MarketBoardData.Items[row];
                        if (item.ItemID == 0) continue;
                        int columnIndex = 0;
                        ImGui.TableNextRow();

                        //item name
                        ImGui.TableSetColumnIndex(columnIndex++);
                        ImGui.SetNextItemWidth(-1);
                        ImGui.Text(item.ItemName?.ToString());

                        //listing #
                        ImGui.TableSetColumnIndex(columnIndex++);
                        ImGui.Text(item.Listings.Count.ToString());

                        //Lowest price
                        ImGui.TableSetColumnIndex(columnIndex++);
                        ImGui.Text(item.Listings[0].PricePerUnit.ToString());

                        //By
                        ImGui.TableSetColumnIndex(columnIndex++);
                        ImGui.SetNextItemWidth(-1);
                        ImGui.Text(item.Listings[0].RetainerName.ToString());
                    }

                    ImGui.EndTable();
                }

                ImGui.End();
            }
        }

        private void SaveConfig()
        {
            Configuration.Save();
            Undercutted.Configuration = Configuration;
        }

        private async void UpdateData()
        {
            string? playerHomeWorld = Undercutted.ClientState?.LocalPlayer?.HomeWorld?.GameData?.Name;
            if (playerHomeWorld == null) return;

            List<string> itemNames = new() { "Perfect Mortar" };
            MarketBoardData = await ApiConsumer.GetData(playerHomeWorld, itemNames);
        }
    }
}
