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

        private readonly string _basePath;

        public GameUpdater(IRestClient restClient, string basePath)
        {
            this._restClient = restClient;
            this._basePath = basePath;
            this.SetupCurrentCulture();
        }

        public bool DownloadFileFromServer(GameVersion gameVersion)
        {
            var fileToDownload = $"my-game-patch-{gameVersion.Version}.zip";
            var locationOfFileToSave = Path.Combine(_basePath, fileToDownload);

            var restRequest = new RestRequest(fileToDownload, Method.GET);
            _restClient.DownloadData(restRequest).SaveAs(locationOfFileToSave);

            return File.Exists(locationOfFileToSave);
        }

        public void UnzipDownloadedFiles(GameVersion gameVersion)
        {
            var fileToUnzip = $"my-game-patch-{gameVersion.Version}.zip";
            var locationOfUnzip = Path.Combine(_basePath, fileToUnzip);

            using (var streamToUnzip = File.OpenRead(locationOfUnzip))
            {
                using (var zipInputStream = new ZipInputStream(streamToUnzip))
                {
                    var zipEntry = zipInputStream.GetNextEntry();
                    while (zipEntry != null)
                    {
                        var entryFileName = zipEntry.Name;

                        byte[] buffer = new byte[4096];

                        var fullZipToPath = Path.Combine(_basePath, entryFileName);
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

        private void SetupCurrentCulture()
        {
            var currentCulture = (System.Globalization.CultureInfo)CultureInfo.CurrentCulture.Clone();
            currentCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
