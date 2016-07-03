using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RestSharp;

namespace Helbreath.Launcher.Tests
{
    public class GameUpdaterTests
    {
        private IRestClient _restClient;

        [SetUp]
        public void Setup()
        {
            TestHelpers.CleanupTestFolder();
            this._restClient = new RestClient("https://s3-eu-west-1.amazonaws.com/helbreath-files/updates/");
        }

        [Test]
        public void DownloadFileFromServer_Then_Make_Sure_It_Exists()
        {
            //arrange
            var gameUpdater = new GameUpdater(_restClient, TestHelpers.GetTestDataFolder("TestData"));
            var gameVersion = new GameVersion
            {
                Version = 0.3
            };

            //act
            var result = gameUpdater.DownloadFileFromServer(gameVersion);

            //assert
            Assert.That(result);
        }

        [Test]
        public void DownloadFileFromServer_ThenUnzipFileToFolder_DirectoryShouldExists()
        {
            // arrange
            var gameUpdater = new GameUpdater(_restClient, TestHelpers.GetTestDataFolder("TestData"));
            var gameVersion = new GameVersion
            {
                Version = 0.3
            };

            //act
            if (gameUpdater.DownloadFileFromServer(gameVersion))
            {
                gameUpdater.UnzipDownloadedFiles(gameVersion);
            }

            //assert
            Assert.That(Directory.Exists(TestHelpers.GetTestDataFolder("TestData/my-game-patch-0.3")));
        }
    }
}
