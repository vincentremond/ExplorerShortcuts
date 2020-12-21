using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExeGenerator
{
    public class ConfigLoader
    {
        public class YamlConfig
        {
            public string Command { get; set; }
            public string Arguments { get; set; }
        }

        public static IEnumerable<CommandConfig> GetConfig(string fileName)
        {
            var configFile = LocateFile(fileName);
            var contents = File.ReadAllText(configFile);
            var d = new YamlDotNet.Serialization.Deserializer();
            var yamlConfigs = d.Deserialize<Dictionary<string, YamlConfig>>(contents);
            return yamlConfigs.Select(
                command => new CommandConfig(
                    Shortcut: command.Key,
                    Command: command.Value.Command,
                    Arguments: command.Value.Arguments
                )
            ).ToArray();
        }

        private static string LocateFile(string fileName)
        {
            var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            do
            {
                var file = currentDirectory
                    .GetFiles(fileName, SearchOption.TopDirectoryOnly)
                    .FirstOrDefault();
                if (file != null)
                {
                    return file.FullName;
                }

                currentDirectory = currentDirectory.Parent;
            } while (currentDirectory != null);

            throw new ApplicationException($"Failed to find '{fileName}' config file");
        }
    }
}