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

        private readonly IVersionProvider _versionProvider;

        private readonly string _basePath;

        public GameUpdater(IRestClient restClient, string basePath, IVersionProvider versionProvider)
        {
            this._restClient = restClient;
            this._basePath = basePath;
            _versionProvider = versionProvider;
            this.SetupCurrentCulture();
        }

        public GameUpdaterResult Update()
        {
            var result = new GameUpdaterResult();
            var localVersion = _versionProvider.GetVersionFromFile();
            var remoteVersion = _versionProvider.GetVersionFromInternet();
            var isUpdate = this.CheckVersion(remoteVersion.Version, localVersion.Version);

            if (isUpdate)
            {
                if (this.DownloadFileFromServer(remoteVersion))
                {
                    this.UnzipDownloadedFiles(remoteVersion);
                    result.GameVersion = _versionProvider.UpdateVersionInFile(remoteVersion);
                    result.HasSucceed = true;
                }
            }

            return result;
        }

        public bool DownloadFileFromServer(GameVersion gameVersion)
        {
            this.SetupCurrentCulture();
            var fileToDownload = $"my-game-patch-{gameVersion.Version}.zip";
            var locationOfFileToSave = Path.Combine(_basePath, fileToDownload);

            var restRequest = new RestRequest(fileToDownload, Method.GET);
            _restClient.DownloadData(restRequest).SaveAs(locationOfFileToSave);

            return File.Exists(locationOfFileToSave);
        }

        public void UnzipDownloadedFiles(GameVersion gameVersion)
        {
            this.SetupCurrentCulture();
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

                        var fullZipToPath = Path.Combine(_basePath, entryFileName).ToLower();
                        var directoryName = Path.GetDirectoryName(fullZipToPath);

                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(directoryName);
                        }
                        if (!fullZipToPath.Contains(".txt") && !fullZipToPath.Contains(".exe") && !fullZipToPath.Contains(".cfg"))
                        {
                            zipEntry = zipInputStream.GetNextEntry();
                        }
                        else
                        {
                            using (var streamWriter = File.Create(fullZipToPath))
                            {
                                StreamUtils.Copy(zipInputStream, streamWriter, buffer);
                            }
                            zipEntry = zipInputStream.GetNextEntry();
                        }
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
