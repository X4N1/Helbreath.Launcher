using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RestSharp;

namespace Helbreath.Launcher
{
    public class VersionProvider : IVersionProvider
    {
        private readonly IRestClient _restClient;

        private readonly string _basePathToVersionFile;

        private const double BaseVersion = 0.1;

        private const string VersionFile = "Version.txt";

        public VersionProvider(IRestClient restClient, string basePathToVersionFile)
        {
            this._restClient = restClient;
            this._basePathToVersionFile = basePathToVersionFile;
        }

        public GameVersion GetVersionFromInternet()
        {
            var restRequest = new RestRequest(Method.GET);
            var response = _restClient.Get(restRequest);
            var gameVersion = JsonConvert.DeserializeObject<GameVersion>(response.Content);
            return gameVersion;
        }

        public GameVersion GetVersionFromFile()
        {
            GameVersion result;

            if (!this.VersionFileExist())
            {
                result = new GameVersion
                {
                    Version = BaseVersion
                };
                using (var stream = File.CreateText(_basePathToVersionFile))
                {
                    var jsonToWrite = JsonConvert.SerializeObject(result);
                    stream.Write(jsonToWrite);
                }
                return result;
            }
            else
            {
                result = new GameVersion
                {
                    Version = BaseVersion
                };
                var jsonToRead = File.ReadAllText(_basePathToVersionFile);
                result = JsonConvert.DeserializeObject<GameVersion>(jsonToRead);
            }

            return result;
        }

        public GameVersion UpdateVersionInFile(GameVersion gameVersion)
        {
            GameVersion result;
            var jsonToWrite = JsonConvert.SerializeObject(gameVersion);
            File.WriteAllText(_basePathToVersionFile, jsonToWrite);
            var jsonToRead = File.ReadAllText(_basePathToVersionFile);
            result = JsonConvert.DeserializeObject<GameVersion>(jsonToRead);
            return result;
        }

        private bool VersionFileExist()
        {
            return File.Exists(_basePathToVersionFile);
        }
    }
}
