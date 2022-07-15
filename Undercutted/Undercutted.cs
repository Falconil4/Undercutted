using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Command;
using Dalamud.Hooking;
using Dalamud.IoC;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using System.Diagnostics;

namespace Undercutted
{
    public sealed unsafe class Undercutted : IDalamudPlugin
    {
        public string Name => "Undercutted";
        private const string commandName = "/undercutted";

        private ConfigurationUI ConfigurationUi { get; init; }
        
        public static DalamudPluginInterface? PluginInterface { get; private set; }
        public static CommandManager? CommandManager { get; private set; }
        public static ClientState? ClientState { get; private set; }
        public static ExcelSheet<Item>? GameItemsList { get; private set; }
        public static Configuration? Configuration { get; set; }

        public Undercutted(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager,
            ClientState clientState,
            DataManager dataManager)
        {
            PluginInterface = pluginInterface;
            CommandManager = commandManager;
            ClientState = clientState;
            GameItemsList = dataManager.GetExcelSheet<Item>();
            
            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(PluginInterface);
            ConfigurationUi = new ConfigurationUI(Configuration);

            CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Open Undercutted main UI"
            });

            PluginInterface.UiBuilder.Draw += this.ConfigurationUi.Draw;
            PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        private void OnCommand(string command, string args) => DrawConfigUI();
        
        private void DrawConfigUI()
        {
            this.ConfigurationUi.SettingsVisible = true;
        }

        public void Dispose()
        {
            ConfigurationUi.Dispose();
            CommandManager?.RemoveHandler(commandName);
        }
    }
}
