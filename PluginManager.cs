using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Plugins
{
    public class PluginManager
    {
        IList<IConsumingPlugin> plugins = null;
        
        private PluginManager()
        {
            Init();
        }

        private void Init()
        {
            plugins = new List<IConsumingPlugin>();

            string pluginPath = ConfigurationManager.AppSettings["PluginPath"];
            if (string.IsNullOrEmpty(pluginPath))
                pluginPath = @".\Output\Plugins";
            BuildPlugins(pluginPath);
        }

        static PluginManager instance;
        public static PluginManager Instance
        {
            get
            {
                lock (typeof(PluginManager))
                {
                    if (null == instance)
                        instance = new PluginManager();
                }
                return instance;
            }
        }

        private void BuildPlugins(string pluginPath)
        {
            try
            {
                plugins.Clear();
                if (!string.IsNullOrEmpty(pluginPath))
                {
                    DirectoryInfo pluginDirInfo = new DirectoryInfo(pluginPath);
                    if (pluginDirInfo.Exists)
                    {
                        var pluginFiles = pluginDirInfo.GetFiles("*.dll");
                        foreach (string pluginFile in pluginFiles.Select(f => f.FullName))
                        {
                            try
                            {
                                Assembly lib = Assembly.LoadFrom(pluginFile);
                                if (lib == null)
                                {
                                    Console.WriteLine("Warning: Cannot load the assembly, name=\"{0}\"",
                                        pluginFile);
                                }
                                else
                                {
                                    var types = lib.GetTypes();
                                    Type type = types.FirstOrDefault(t => t.GetInterface("IConsumingPlugin") != null);
                                    if (type != null)
                                    {
                                        var plugin = Activator.CreateInstance(type) as IConsumingPlugin;
                                        plugins.Add(plugin);
                                    }
                                }
                            }
                            catch (FileLoadException flexc)
                            {
                                Console.Error.WriteLine(flexc);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Warning: Cannot access the plugins, path=\"{0}\"", pluginPath);
                    }
                }
                else
                {
                    Console.WriteLine("Warning: Cannot access the plugins, path is not valid.");
                }
            }
            catch (Exception exc)
            {
                Console.Error.WriteLine(exc);
            }
        }

        public IEnumerable<IConsumingPlugin> GetAvailablePlugins()
        {
            return plugins;
        }

        public IEnumerable<IConsumingPlugin> GetAvailablePluginsAt(string pluginPath)
        {
            BuildPlugins(pluginPath);
            return plugins;
        }
    }
}
