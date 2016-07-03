using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Extensions;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Helbreath.Launcher.Tests
{

    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void Two_Versions_Then_Check_If_Newer_ReturnTrue()
        {
            // arrange
            var gameUpdater = new GameUpdater(new RestClient("http://local.host.com"));
            var oldVersion = 0.1;
            var newVersion = 0.2;

            // act
            var result = gameUpdater.CheckVersion(newVersion, oldVersion);

            // assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public void Two_Versions_Then_Check_If_Newer_ReturnFalse()
        {
            //arrange
            var gameUpdater = new GameUpdater(new RestClient("http://local.host.com"));
            var oldVersion = 0.2;
            var newVersion = 0.2;

            //act
            var result = gameUpdater.CheckVersion(newVersion, oldVersion);

            //assert
            Assert.AreEqual(false, result);
        }

        [Test]
        public void DownloadFileFromServer_Then_Make_Sure_It_Exists()
        {
            //arrange
            var client = new RestClient("https://s3-eu-west-1.amazonaws.com/helbreath-files/updates/");
            var gameUpdater = new GameUpdater(client);
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
        public void DownloadFileFromServer_ThenUnzipFileToFolder()
        {
            // arrange
            var client = new RestClient("https://s3-eu-west-1.amazonaws.com/helbreath-files/updates/");
            var gameUpdater = new GameUpdater(client);
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
            Assert.That(Directory.Exists("C:/tempTest/my-game-patch-0.3"));
        }

        [Test]
        public void Read_Then_Read_Version_From_File_Return_CurrentVersion()
        {
            //arrange
            var currentVersionProvider = new InMememoryCurrentVersionProvider();
            var expectedVersion = 0.1;

            //act
            var result = currentVersionProvider.GetCurrentVersionFromComputer();

            //assert
            Assert.AreEqual(expectedVersion, result);
        }
    }
}
