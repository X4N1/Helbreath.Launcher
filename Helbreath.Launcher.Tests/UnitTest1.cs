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
            var gameUpdater = new GameUpdater(new RestClient("http://local.host.com"), TestHelpers.GetTestDataFolder("updates"));
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
            var gameUpdater = new GameUpdater(new RestClient("http://local.host.com"), TestHelpers.GetTestDataFolder("updates"));
            var oldVersion = 0.2;
            var newVersion = 0.2;

            //act
            var result = gameUpdater.CheckVersion(newVersion, oldVersion);

            //assert
            Assert.AreEqual(false, result);
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
