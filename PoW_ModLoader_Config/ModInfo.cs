using System.Collections.Generic;
using System.IO;

using IniParser;
using IniParser.Model;

using Mono.Cecil;

namespace PoW_ModLoader_Config
{
    internal class ModInfo
    {
        private FileIniDataParser _Parser;
        public string ConfigFilePath;
        public IniData ConfigFileData;

        private int _LoadOrderIndex;

        public int LoadOrderIndex
        {
            get
            {
                return _LoadOrderIndex;
            }
            set
            {
                _LoadOrderIndex = value;
            }
        }

        public bool _Enabled;

        public bool Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                _Enabled = value;
                Save();
            }
        }

        public string ModName { get; set; }
        public string PluginName { get; set; }
        public string Version { get; set; }

        public ModInfo(string cfgFilePath, string pluginName, string modName, string version)
        {
            ConfigFilePath = cfgFilePath;
            ModName = modName;
            PluginName = pluginName;
            Version = version;

            _Parser = new FileIniDataParser();
            _Parser.Parser.Configuration.CommentString = "#";

            if (!File.Exists(ConfigFilePath))
            {
                //There is no config for this yet
                string content = "[Generic]\n";
                content += "LoadOrderIndex = -1\n";
                content += "Enabled = True";
                File.WriteAllText(ConfigFilePath, content);
            }

            ConfigFileData = _Parser.ReadFile(ConfigFilePath);

            var loadOrderString = ConfigFileData["Generic"]["LoadOrderIndex"];
            if (loadOrderString == null)
            {
                _LoadOrderIndex = -1;
            }
            else
            {
                _LoadOrderIndex = int.Parse(loadOrderString);
            }
            var enabledString = ConfigFileData["Generic"]["Enabled"];
            if (enabledString == null)
            {
                _Enabled = true;
            }
            else
            {
                _Enabled = bool.Parse(enabledString);
            }
        }

        public void Save()
        {
            ConfigFileData["Generic"]["LoadOrderIndex"] = LoadOrderIndex.ToString();
            ConfigFileData["Generic"]["Enabled"] = Enabled.ToString();

            _Parser.WriteFile(ConfigFilePath, ConfigFileData);
        }

        public static List<ModInfo> GetAllModsInDll(string dllPath)
        {
            var result = new List<ModInfo>();
            //We now need to use Mono.Cecil because we cannot do assembly.GetTypes() without having all dependencies loaded.
            //Mono.Cecil allows us to inspect the dll without having the references.
            var module = ModuleDefinition.ReadModule(dllPath);
            //We now try to find all mods in this plugin
            foreach (TypeDefinition type in module.Types)
            {
                if (!type.HasCustomAttributes)
                    continue;

                if (!type.HasInterfaces)
                    continue;

                var interfaces = type.Interfaces;
                bool inheritsIPoWMod = false;
                foreach (var inter in interfaces)
                {
                    if (inter.InterfaceType.FullName == "ModAPI.IPoWMod")
                    {
                        inheritsIPoWMod = true;
                        break;
                    }
                }

                if (!inheritsIPoWMod)
                    continue;

                bool isBepInPlugin = false;
                string pluginName = null;
                string modName = null;
                string modVersion = null;
                foreach (CustomAttribute attribute in type.CustomAttributes)
                {
                    if (attribute.AttributeType.FullName == "BepInEx.BepInPlugin")
                    {
                        isBepInPlugin = true;
                        pluginName = (string)attribute.ConstructorArguments[0].Value;
                        modName = (string)attribute.ConstructorArguments[1].Value;
                        modVersion = (string)attribute.ConstructorArguments[2].Value;
                        break;
                    }
                }

                if (!isBepInPlugin)
                    continue;

                var curDir = Directory.GetCurrentDirectory();
                var cfgFilePath = curDir + Path.DirectorySeparatorChar + "BepInEx" + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + pluginName + ".cfg";
                result.Add(new ModInfo(cfgFilePath, pluginName, modName, modVersion));
            }

            return result;
        }
    }
}