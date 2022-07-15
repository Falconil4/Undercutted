using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace HotbarTimers
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

        public ConfigurationUI(Configuration configuration)
        {
            Configuration = configuration;
        }

        public void Dispose() {}
        public void Draw() => DrawSettingsWindow();

        public void DrawSettingsWindow()
        {
            if (!SettingsVisible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(600, 600), ImGuiCond.Appearing);
            if (ImGui.Begin("Undercutted Settings", ref this.settingsVisible,
                ImGuiWindowFlags.NoCollapse))
            {
                
                

                ImGui.End();
            }
        }

        private void SaveConfig()
        {
            Configuration.Save();
            Undercutted.Configuration = Configuration;
        }
    }
}
