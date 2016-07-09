using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
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

            var currentCulture = (System.Globalization.CultureInfo)CultureInfo.CurrentCulture.Clone();
            currentCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
        }

        [Test]
        public void DownloadFileFromServer_Then_Make_Sure_It_Exists()
        {
            //arrange
            var gameUpdater = new GameUpdater(_restClient, TestHelpers.GetTestDataFolder("TestData"));
            var gameVersion = new GameVersion
            {
                Version = 0.4
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
                Version = 0.4
            };
            var expectedPathDirectory = string.Format("TestData/my-game-patch-{0}", gameVersion.Version);

            //act
            if (gameUpdater.DownloadFileFromServer(gameVersion))
            {
                gameUpdater.UnzipDownloadedFiles(gameVersion);
            }

            //assert
        }

        [Test]
        public void UpdateGame_When_All_Correct()
        {
            // arrange
            var restclientMock = new Mock<RestClient>();
            restclientMock.Setup(x => x.Execute(It.IsAny<IRestRequest>())).Returns(new RestResponse
            {
                Content = "{ 'version' : 0.4 }"
            });
            var expectedGameVersion = new GameVersion
            {
                Version = 0.4
            };
            var gameUpdater = new GameUpdater(_restClient, TestHelpers.GetTestDataFolder("TestData"));
            var versionProvider = new VersionProvider(restclientMock.Object, TestHelpers.GetTestDataFolder("TestData/Version.json"));
            bool result;
            GameVersion gameVersion = new GameVersion();

            //act
            var localVersion = versionProvider.GetVersionFromFile();
            var remoteVersion = versionProvider.GetVersionFromInternet();
            var isUpdate = gameUpdater.CheckVersion(remoteVersion.Version, localVersion.Version);

            if (isUpdate)
            {
                if (gameUpdater.DownloadFileFromServer(remoteVersion))
                {
                    gameUpdater.UnzipDownloadedFiles(remoteVersion);
                    gameVersion = versionProvider.UpdateVersionInFile(remoteVersion);
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            // assert
            Assert.That(result);
            Assert.AreEqual(expectedGameVersion.Version, gameVersion.Version);
        }
        
    }
}
