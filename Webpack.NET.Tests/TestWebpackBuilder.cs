using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Moq;
using NUnit.Framework;

namespace Webpack.NET.Tests
{
    [TestFixture]
    public class TestWebpackBuilder
    {
        private WebpackConfig _config;
        private WebpackBuilder _builder;
        private Mock<HttpServerUtilityBase> _server;
        private List<string> _tempFiles;

        [SetUp]
        public void Setup()
        {
            _server = new Mock<HttpServerUtilityBase>();
            _config = new WebpackConfig();
            _builder = new WebpackBuilder(_server.Object);
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
            Assert.Throws<ArgumentNullException>(() => new Webpack(null));
        }

        [Test]
        public void Assets_Are_Lazily_Loaded()
        {
            _config.AssetManifestPath = "~/scripts/manifest.json";
            _config.AssetOutputPath = "~/dist";

            SetupManifestFile(_config, @"{ ""file"": { ""js"": ""file.js"" } }");

            _server.Verify(s => s.MapPath(It.IsAny<string>()), Times.Never());

            _builder.Build(_config);

            _server.Verify(s => s.MapPath(It.IsAny<string>()), Times.Once());

            _server.Verify(s => s.MapPath(It.IsAny<string>()), Times.Once());
        }

        private void SetupManifestFile(WebpackConfig config, string manifestContent)
        {
            var tempFile = Path.GetTempFileName();
            _tempFiles.Add(tempFile);
            config.AssetManifestPath = tempFile;
            _server.Setup(s => s.MapPath(config.AssetManifestPath)).Returns(tempFile);
            File.WriteAllText(tempFile, manifestContent);
        }
    }
}