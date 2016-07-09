using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Helbreath.Launcher.Tests
{
    public class TestHelpers
    {
        public static void CleanupTestFolder()
        {
            var directoryInfo = new DirectoryInfo(GetTestDataFolder("TestData"));

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                if (file.Name != "Readme.md")
                {
                    file.Delete();
                }
            }
            foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        public static string GetTestDataFolder(string testDataFolder)
        {
            string startupPath = System.AppDomain.CurrentDomain.BaseDirectory;
            var pathItems = startupPath.Split(Path.DirectorySeparatorChar);
            string projectPath = String.Join(Path.DirectorySeparatorChar.ToString(), pathItems.Take(pathItems.Length - 3));
            return Path.Combine(projectPath, "Helbreath.Launcher.Tests", testDataFolder);
        }

        public static void SetupCultureInfo()
        {
            var currentCulture = (System.Globalization.CultureInfo)CultureInfo.CurrentCulture.Clone();
            currentCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
