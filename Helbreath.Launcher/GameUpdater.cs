using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using RestSharp;
using RestSharp.Extensions;

namespace Helbreath.Launcher
{
    public class GameUpdater
    {
        private readonly IRestClient _restClient;

        public GameUpdater(IRestClient restClient)
        {
            this._restClient = restClient;
        }

        public bool DownloadFileFromServer(GameVersion gameVersion)
        {
            var currentCulture = (System.Globalization.CultureInfo)CultureInfo.CurrentCulture.Clone();
            currentCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;

            var fileToDownload = $"my-game-patch-{gameVersion.Version}.zip";
            var restRequest = new RestRequest(fileToDownload, Method.GET);
            _restClient.DownloadData(restRequest).SaveAs($"C:/tempTest/{fileToDownload}");
            return File.Exists($"C:/tempTest/{fileToDownload}");
        }

        public void UnzipDownloadedFiles(GameVersion gameVersion)
        {
            var outFolder = "C:/tempTest/";
            var fileToUnzip = $"C:/tempTest/my-game-patch-{gameVersion.Version}.zip";

            using (var streamToUnzip = File.OpenRead(fileToUnzip))
            {
                using (var zipInputStream = new ZipInputStream(streamToUnzip))
                {
                    var zipEntry = zipInputStream.GetNextEntry();
                    while (zipEntry != null)
                    {
                        var entryFileName = zipEntry.Name;

                        byte[] buffer = new byte[4096];

                        var fullZipToPath = Path.Combine(outFolder, entryFileName);
                        var directoryName = Path.GetDirectoryName(fullZipToPath);

                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(directoryName);
                        }

                        using (var streamWriter = File.Create(fullZipToPath))
                        {
                            StreamUtils.Copy(zipInputStream, streamWriter, buffer);
                        }
                        zipEntry = zipInputStream.GetNextEntry();
                    }
                }
            }
        }

        public bool CheckVersion(double newerVersion, double oldVersion)
        {
            if (newerVersion > oldVersion)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
