using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace Helbreath.Launcher.Tests
{
    [TestFixture]
    public class VersionProviderTests
    {
        private IVersionProvider _versionProvider;

        [SetUp]
        public void Setup()
        {
            TestHelpers.CleanupTestFolder();
            Mock<RestClient> restclientMock = new Mock<RestClient>();
            restclientMock.Setup(x => x.Execute(It.IsAny<IRestRequest>())).Returns(new RestResponse
            {
                Content = "{ 'version' : 1 }"
            });
            var restclient = restclientMock.Object;
            _versionProvider = new VersionProvider(restclient, TestHelpers.GetTestDataFolder("TestData/Version.json"));
        }

        [Test]
        public void GetVersionFromFile_Then_Make_Sure_That_Version_File_Exist()
        {
            _versionProvider.GetVersionFromFile();
           
            Assert.That(File.Exists(TestHelpers.GetTestDataFolder("TestData/Version.json")));
        }

        [Test]
        public void NonExisting_File_GetVersionFromFile_Then_Read_Version_Should_Return_Base_Version()
        {
            var result = _versionProvider.GetVersionFromFile();
           
            Assert.AreEqual(0.1, result.Version);
        }

        [Test]
        public void UpdateVersionInFile_Then_Read_New_Version_Return_Correct_GameVersion()
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

        [Test]
        public void GetVersionFromInternet_Then_Deserialize_Version_Return_GameVersion()
        {
            //arrange
            Mock<RestClient> restclientMock = new Mock<RestClient>();
            var expectedGameVersion = new GameVersion
            {
                Version = 0.5
            };
            var jsonToReturn = JsonConvert.SerializeObject(expectedGameVersion);
            restclientMock.Setup(x => x.Execute(It.IsAny<IRestRequest>())).Returns(new RestResponse
            {
                Content = jsonToReturn
            });
            var restclient = restclientMock.Object;
            _versionProvider = new VersionProvider(restclient, TestHelpers.GetTestDataFolder("TestData/Version.json"));

            //act
            var result = _versionProvider.GetVersionFromInternet();

            //assert
            Assert.AreEqual(expectedGameVersion.Version, result.Version);
        }
    }
}
