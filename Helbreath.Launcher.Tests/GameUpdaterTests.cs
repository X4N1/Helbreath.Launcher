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

        private GameUpdater _gameUpdater;

        private IVersionProvider _versionProvider;

        [SetUp]
        public void Setup()
        {
            TestHelpers.CleanupTestFolder();
            TestHelpers.SetupCultureInfo();
            this._restClient = new RestClient("https://s3-eu-west-1.amazonaws.com/helbreath-files/updates/");
            var restclientMock = new Mock<RestClient>();
            restclientMock.Setup(x => x.Execute(It.IsAny<IRestRequest>())).Returns(new RestResponse
            {
                Content = "{ 'version' : 0.4 }"
            });
            this._versionProvider = new VersionProvider(restclientMock.Object, TestHelpers.GetTestDataFolder("TestData/Version.json"));
            this._gameUpdater = new GameUpdater(_restClient, TestHelpers.GetTestDataFolder("TestData"), this._versionProvider);
        }

        [Test]
        public void DownloadFileFromServer_Then_Make_Sure_It_Exists()
        {
            //arrange
            var gameVersion = new GameVersion
            {
                Version = 0.4
            };

            //act
            var result = this._gameUpdater.DownloadFileFromServer(gameVersion);

            //assert
            Assert.That(result);
        }
        
        [Test]
        public void DownloadFileFromServer_ThenUnzipFileToFolder_DirectoryShouldExists()
        {
            // arrange
            var gameVersion = new GameVersion
            {
                Version = 0.4
            };
            var expectedPathDirectory = string.Format("TestData/my-game-patch-{0}", gameVersion.Version);

            //act
            if (this._gameUpdater.DownloadFileFromServer(gameVersion))
            {
                this._gameUpdater.UnzipDownloadedFiles(gameVersion);
            }

            //assert
        }

        [Test]
        public void UpdateGame_When_All_Correct()
        {
            // arrange
            var expectedGameVersion = new GameVersion
            {
                Version = 0.4
            };

            //act
            var result = this._gameUpdater.Update();
            

            // assert
            Assert.That(result.HasSucceed);
            Assert.AreEqual(expectedGameVersion.Version, result.GameVersion.Version);
        }
        
    }
}
