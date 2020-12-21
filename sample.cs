using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace c
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                var currentDirectory = Directory.GetCurrentDirectory();
                var singleFile = GetSingleFile(currentDirectory);
                var arguments = $"\"{currentDirectory}\" ";
                if (!string.IsNullOrEmpty(singleFile))
                {
                    arguments += $" --goto \"{singleFile}\"";
                }

                var p = new Process
                {
                    StartInfo =
                    {
                        FileName = @"C:\Program Files\Microsoft VS Code\Code.exe",
                        UseShellExecute = false,
                        Arguments = arguments,
                        WorkingDirectory = currentDirectory,
                        WindowStyle = ProcessWindowStyle.Normal,
                        LoadUserProfile = true,
                        CreateNoWindow = false,
                    }
                };

                p.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}