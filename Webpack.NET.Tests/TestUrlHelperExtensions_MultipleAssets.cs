using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;

namespace Webpack.NET.Tests
{
    [TestFixture]
    public class TestUrlHelperExtensions_MultipleAssets
    {
        [Test]
        public void WebpackAssets_Throws_On_Null_UrlHelper()
        {
            UrlHelper urlHelper = null;
            Assert.Throws<ArgumentNullException>(() => urlHelper.WebpackAssets());
            Assert.Throws<ArgumentNullException>(() => urlHelper.AbsoluteWebpackAssets());
        }

        [Test]
        public void WebpackAssets_Throws_On_No_Matching_Resource()
        {
            var urlHelper = SetupUrlHelper("http://server/");
            var webpack = new Mock<IWebpack>();
            webpack.Setup(w => w.GetAssetsUrl("non-existant", "non-existant", true)).Throws<AssetNotFoundException>();
            //urlHelper.RequestContext.HttpContext.Application.ConfigureWebpack(webpack.Object);
            Webpack.Instance = webpack.Object;

            Assert.Throws<AssetNotFoundException>(() => urlHelper.WebpackAssets("non-existant", "non-existant"));
            Assert.Throws<AssetNotFoundException>(() => urlHelper.AbsoluteWebpackAssets("non-existant", "non-existant"));
        }

        [Test]
        public void WebpackAssets_Returns_Expected_Url()
        {
            var urlHelper = SetupUrlHelper("http://server/");
            var webpack = new Mock<IWebpack>();
            webpack.Setup(w => w.GetAssetsUrl("asset-name", "ext", true)).Returns(new List<string>() { "/scripts/assets/asset.hash.js", "/scripts/assets/asset.hash2.js" });
            webpack.Setup(w => w.GetAssetsUrl("asset-name", "ext", false)).Returns(new List<string>() { "/scripts/assets/asset.hash.js", "/scripts/assets/asset.hash2.js" });
            webpack.Setup(w => w.GetAssetsUrl("asset-name-querystring", "ext", true)).Returns(new List<string>() { "/scripts/assets/asset.hash.js?i=1%2b1", "/scripts/assets/asset.hash2.js?i=1%2b1" });
            webpack.Setup(w => w.GetAssetsUrl("asset-name-querystring", "ext", false)).Returns(new List<string>() { "/scripts/assets/asset.hash.js?i=1%2b1", "/scripts/assets/asset.hash2.js?i=1%2b1" });

            // Setup the static instance with the Mock
            Webpack.Instance = webpack.Object;

            Assert.That(urlHelper.WebpackAssets("asset-name", "ext").FirstOrDefault(), Is.EqualTo("/scripts/assets/asset.hash.js"));
            Assert.That(urlHelper.WebpackAssets("asset-name", "ext").Skip(1).FirstOrDefault(), Is.EqualTo("/scripts/assets/asset.hash2.js"));
            Assert.That(urlHelper.WebpackAssets("asset-name", "ext", false).FirstOrDefault(), Is.EqualTo("/scripts/assets/asset.hash.js"));
            Assert.That(urlHelper.WebpackAssets("asset-name", "ext", false).Skip(1).FirstOrDefault(), Is.EqualTo("/scripts/assets/asset.hash2.js"));

            Assert.That(urlHelper.AbsoluteWebpackAssets("asset-name", "ext").FirstOrDefault(), Is.EqualTo("http://server/scripts/assets/asset.hash.js"));
            Assert.That(urlHelper.AbsoluteWebpackAssets("asset-name", "ext").Skip(1).FirstOrDefault(), Is.EqualTo("http://server/scripts/assets/asset.hash2.js"));
            Assert.That(urlHelper.AbsoluteWebpackAssets("asset-name", "ext", false).FirstOrDefault(), Is.EqualTo("http://server/scripts/assets/asset.hash.js"));
            Assert.That(urlHelper.AbsoluteWebpackAssets("asset-name", "ext", false).Skip(1).FirstOrDefault(), Is.EqualTo("http://server/scripts/assets/asset.hash2.js"));

            Assert.That(urlHelper.WebpackAssets("asset-name-querystring", "ext").FirstOrDefault(), Is.EqualTo("/scripts/assets/asset.hash.js?i=1%2b1"));
            Assert.That(urlHelper.WebpackAssets("asset-name-querystring", "ext").Skip(1).FirstOrDefault(), Is.EqualTo("/scripts/assets/asset.hash2.js?i=1%2b1"));
            Assert.That(urlHelper.WebpackAssets("asset-name-querystring", "ext", false).FirstOrDefault(), Is.EqualTo("/scripts/assets/asset.hash.js?i=1%2b1"));
            Assert.That(urlHelper.WebpackAssets("asset-name-querystring", "ext", false).Skip(1).FirstOrDefault(), Is.EqualTo("/scripts/assets/asset.hash2.js?i=1%2b1"));

            Assert.That(urlHelper.AbsoluteWebpackAssets("asset-name-querystring", "ext").FirstOrDefault(), Is.EqualTo("http://server/scripts/assets/asset.hash.js?i=1%2b1"));
            Assert.That(urlHelper.AbsoluteWebpackAssets("asset-name-querystring", "ext").Skip(1).FirstOrDefault(), Is.EqualTo("http://server/scripts/assets/asset.hash2.js?i=1%2b1"));
            Assert.That(urlHelper.AbsoluteWebpackAssets("asset-name-querystring", "ext", false).FirstOrDefault(), Is.EqualTo("http://server/scripts/assets/asset.hash.js?i=1%2b1"));
            Assert.That(urlHelper.AbsoluteWebpackAssets("asset-name-querystring", "ext", false).Skip(1).FirstOrDefault(), Is.EqualTo("http://server/scripts/assets/asset.hash2.js?i=1%2b1"));
        }

        [Test]
        public void WebpackAssets_Returns_Empty_List_When_Not_Required_And_No_Matching_Resource()
        {
            var urlHelper = SetupUrlHelper("http://server/");
            var webpack = new Mock<IWebpack>();
            webpack.Setup(w => w.GetAssetsUrl("non-existant", "non-existant", false)).Returns(new List<string>(0));

            // Setup the static instance with the Mock
            Webpack.Instance = webpack.Object;

            Assert.That(urlHelper.WebpackAssets("non-existant", "non-existant", false), Is.Empty);
            Assert.That(urlHelper.AbsoluteWebpackAssets("non-existant", "non-existant", false), Is.Empty);
        }

        public static UrlHelper SetupUrlHelper(string baseUrl)
        {
            var routes = new RouteCollection();

            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.ApplicationPath).Returns("/");
            request.SetupGet(x => x.Url).Returns(new Uri(baseUrl, UriKind.Absolute));
            request.SetupGet(x => x.ServerVariables).Returns(new System.Collections.Specialized.NameValueCollection());

            var response = new Mock<HttpResponseBase>();
            response.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>())).Returns((string url) => url);
            response.Setup(x => x.AddCacheDependency(It.IsAny<CacheDependency>()));

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);

            var application = new MockHttpApplicationState();
            context.SetupGet(x => x.Application).Returns(application);

            return new UrlHelper(new RequestContext(context.Object, new RouteData()), routes);
        }
    }
}