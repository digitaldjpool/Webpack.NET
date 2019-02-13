using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Moq;
using NUnit.Framework;

namespace Webpack.NET.Tests
{
    [TestFixture]
    public class TestWebpack_MultipleAssets
    {
        private WebpackConfig _config;
        private Mock<HttpServerUtilityBase> _server;
        private List<string> _tempFiles;

        [SetUp]
        public void Setup()
        {
            _config = new WebpackConfig();
            _server = new Mock<HttpServerUtilityBase>();
            _tempFiles = new List<string>();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var tempFile in _tempFiles)
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
        }

        [Test]
        public void Constructor_Throws_On_Null_Parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new Webpack(null, _server.Object));
            Assert.Throws<ArgumentNullException>(() => new Webpack(new[] { new WebpackConfig() }, null));
        }

        [Test]
        public void GetAssetsUrl_Throws_On_No_Configurations_Specified()
        {
            var webpack = new Webpack(new WebpackConfig[0], _server.Object);
            Assert.Throws<AssetNotFoundException>(() => webpack.GetAssetsUrl("any", "any"));
        }

        [Test]
        public void GetAssetsUrl_Returns_Empty_List_When_Not_Required_And_No_Configurations_Specified()
        {
            var webpack = new Webpack(new WebpackConfig[0], _server.Object);
            Assert.That(webpack.GetAssetsUrl("any", "any", false), Is.Empty);
        }

        [Test]
        public void GetAssetsUrl_Throws_On_No_Matching_Resource()
        {
            _config.AssetManifestPath = "~/scripts/manifest.json";
            SetupManifestFile(_config.AssetManifestPath, @"{ ""file"": { ""js"": ""file.js"" } }");

            var webpack = new Webpack(new[] { _config }, _server.Object);
            Assert.Throws<AssetNotFoundException>(() => webpack.GetAssetsUrl("non-existant", "js"));
            Assert.Throws<AssetNotFoundException>(() => webpack.GetAssetsUrl("file", "non-existant"));
            Assert.Throws<AssetNotFoundException>(() => webpack.GetAssetsUrl("non-existant", "non-existant"));
        }

        [Test]
        public void GetAssetsUrl_Returns_Empty_List_When_Not_Required_And_No_Matching_Resource()
        {
            _config.AssetManifestPath = "~/scripts/manifest.json";
            SetupManifestFile(_config.AssetManifestPath, @"{ ""file"": { ""js"": ""file.js"" } }");

            var webpack = new Webpack(new[] { _config }, _server.Object);
            Assert.That(webpack.GetAssetsUrl("non-existant", "js", false), Is.Empty);
            Assert.That(webpack.GetAssetsUrl("file", "non-existant", false), Is.Empty);
            Assert.That(webpack.GetAssetsUrl("non-existant", "non-existant", false), Is.Empty);
        }

        [Test]
        public void Assets_Can_Be_Retrieved_When_Entrypoints_Provides_Multiple_Assets()
        {
            var config1 = new WebpackConfig { AssetManifestPath = "~/scripts/manifest5.json", AssetOutputPath = "" };

            SetupManifestFile(config1.AssetManifestPath, @"{
                                                            ""main"": {
                                                                ""js"": [
                                                                      ""/dist/first-script.js"",
                                                                      ""/dist/second-script.js"",
                                                                      ""/dist/third-script.js""
                                                                      ],
                                                                ""css"": [
                                                                    ""/dist/first-style.css"",
                                                                    ""/dist/second-style.css"",
                                                                    ]
                                                                }
                                                            }");

            var webpack = new Webpack(new[] { config1 }, _server.Object);
            Assert.That(webpack.GetAssetsUrl("main", "js").Count, Is.EqualTo(3));
            Assert.That(webpack.GetAssetsUrl("main", "js")[0], Is.EqualTo("/dist/first-script.js"));
            Assert.That(webpack.GetAssetsUrl("main", "js")[1], Is.EqualTo("/dist/second-script.js"));
            Assert.That(webpack.GetAssetsUrl("main", "js")[2], Is.EqualTo("/dist/third-script.js"));

            Assert.That(webpack.GetAssetsUrl("main", "css").Count, Is.EqualTo(2));
            Assert.That(webpack.GetAssetsUrl("main", "css")[0], Is.EqualTo("/dist/first-style.css"));
            Assert.That(webpack.GetAssetsUrl("main", "css")[1], Is.EqualTo("/dist/second-style.css"));
        }

        private void SetupManifestFile(string manifestPath, string manifestContent)
        {
            var tempFile = Path.GetTempFileName();
            _tempFiles.Add(tempFile);
            _server.Setup(s => s.MapPath(manifestPath)).Returns(tempFile);
            File.WriteAllText(tempFile, manifestContent);
        }
    }
}