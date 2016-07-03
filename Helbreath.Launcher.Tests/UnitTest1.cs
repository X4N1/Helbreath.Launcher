﻿using System;
using System.IO;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Extensions;

namespace Helbreath.Launcher.Tests
{

    [TestFixture]
    public class UnitTest1
    {

        private IVersionProvider _versionProvider;

        [SetUp]
        public void Setup()
        {
            Mock<RestClient> restclientMock = new Mock<RestClient>();
            restclientMock.Setup(x => x.Execute(It.IsAny<IRestRequest>())).Returns(new RestResponse
            {
                Content = "{ 'version' : 1 }"
            });
            var restclient = restclientMock.Object;
            _versionProvider = new VersionProvider(restclient, "C:/Version.txt");
        }

        [Test]
        public void Two_Versions_Then_Check_If_Newer_ReturnTrue()
        {
            // arrange
            var gameUpdater = new GameUpdater();
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
            var gameUpdater = new GameUpdater();
            var oldVersion = 0.2;
            var newVersion = 0.2;

            //act
            var result = gameUpdater.CheckVersion(newVersion, oldVersion);

            //assert
            Assert.AreEqual(false, result);
        }

        [Test]
        public void Send_Request_To_Website_Then_Return_ReturnLastesVersion()
        {
            //arrange
            //act
            var result = _versionProvider.GetVersionFromInternet();

            //assert
            Assert.AreEqual(1, result.Version);
        }

        [Test]
        public void GetUpdatesFromWebsite_Then_Download_And_Copy_To_Root_Folder()
        {
            //arrange
            var client = new RestClient("https://s3-eu-west-1.amazonaws.com/livecoding-tv/my-game-patch-0.3.txt");
            var request = new RestRequest(Method.GET);

            //act
            client.DownloadData(request).SaveAs("C:/my-game-patch-0.3.txt");
            var updateExist = File.Exists("C:/my-game-patch-0.3.txt");

            //assert
            Assert.That(updateExist);
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

        [Test]
        public void Check_For_Version_File_Then_CreateVersionFile()
        {
            //arrange
            //act
            _versionProvider.GetVersionFromFile();

            //assert
            Assert.That(File.Exists("C:/Version.txt"));
        }

        [Test]
        public void Check_For_Version_File_Then_ReadFile()
        {
            //arrange
            //act
            var result = _versionProvider.GetVersionFromFile();

            //assert
            Assert.AreEqual(0.5, result.Version);
        }

        [Test]
        public void Update_Version_File_With_New_Version()
        {
            //arrange
            var newVersion = 0.5;
            var gameVersionToUpdate = new GameVersion
            {
                Version = newVersion
            };

            //act
            var result = _versionProvider.UpdateVersionInFile(gameVersionToUpdate);

            //assert
            Assert.AreEqual(newVersion, result.Version);
        }

        public class GameUpdater
        {
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
}
