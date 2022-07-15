using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace HotbarTimers
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

    public enum FontType { Type1, Type2, Type3, Type4, Type5 }
}
