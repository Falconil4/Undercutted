using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Undercutted
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
                
        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;
        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            if (this.pluginInterface == null) return;
            this.pluginInterface.SavePluginConfig(this);
        }
    }
}
