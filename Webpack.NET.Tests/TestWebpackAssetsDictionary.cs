using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Webpack.NET.Tests
{
    [TestFixture]
    public class TestWebpackAssetsDictionary
    {
        private string _tempFile;

        [SetUp]
        public void Setup()
        {
            _tempFile = Path.GetTempFileName();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
        }

        [Test]
        public void FromFile_Throws_On_Invalid_Filename()
        {
            Assert.Throws<ArgumentNullException>(() => WebpackAssetsDictionary.FromFile(null));
            Assert.Throws<ArgumentException>(() => WebpackAssetsDictionary.FromFile(string.Empty));
        }

        [Test]
        public void FromFile_Throws_On_NonExistant_File()
        {
            Assert.Throws<FileNotFoundException>(() => WebpackAssetsDictionary.FromFile("non-existant.json"));
        }

        [Test]
        public void FromFile_Reads_Valid_Content()
        {
            File.WriteAllText(_tempFile, @"
			{
				""file"": { ""js"": ""file.hash.js"", ""other"": ""file.hash.other"" },
				""file2"": { ""js"": ""file2.hash.js"" },
                ""file3"": { ""js"": [""file3-1.js"", ""file3-2.js""], ""css"": [""style-1.css"", ""style-2.css""] }
			}");

            var assets = WebpackAssetsDictionary.FromFile(_tempFile);

            Assert.That(assets["file"]["js"].First(), Is.EqualTo("file.hash.js"));
            Assert.That(assets["file"]["other"].First(), Is.EqualTo("file.hash.other"));
            Assert.That(assets["file2"]["js"].First(), Is.EqualTo("file2.hash.js"));

            Assert.That(assets["file3"]["js"].First(), Is.EqualTo("file3-1.js"));
            Assert.That(assets["file3"]["js"].Skip(1).First(), Is.EqualTo("file3-2.js"));
            Assert.That(assets["file3"]["css"].First(), Is.EqualTo("style-1.css"));
            Assert.That(assets["file3"]["css"].Skip(1).First(), Is.EqualTo("style-2.css"));
        }
    }
}