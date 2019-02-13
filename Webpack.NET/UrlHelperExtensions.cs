using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Webpack.NET
{
    /// <summary>
    /// Extension methods for <see cref="UrlHelper"/>.
    /// </summary>
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Gets the URL of the webpack asset.
        /// </summary>
        /// <param name="urlHelper">The URL helper.</param>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="assetType">Type of the asset.</param>
        /// <param name="required">If set to <c>true</c> throws an <see cref="AssetNotFoundException" /> when the asset could not be found; otherwise, returns <c>null</c>.</param>
        /// <returns>
        /// The URL of the webpack asset.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">urlHelper</exception>
        [Obsolete("This method is marked as obsolete as there are possibly multiple files given an assetName & assetType.")]
        public static string WebpackAsset(this UrlHelper urlHelper, string assetName = "main", string assetType = "js", bool required = true)
        {
            if (urlHelper == null) throw new ArgumentNullException(nameof(urlHelper));

            var webpack = urlHelper.RequestContext.HttpContext.Application.GetWebpack();
            var assetUrl = webpack.GetAssetUrl(assetName, assetType, required);

            return (assetUrl != null) ? urlHelper.Content(assetUrl) : null;
        }

        /// <summary>
        /// Gets the absolute URL of the webpack asset.
        /// </summary>
        /// <param name="urlHelper">The URL helper.</param>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="assetType">Type of the asset.</param>
        /// <param name="required">If set to <c>true</c> throws an <see cref="AssetNotFoundException" /> when the asset could not be found; otherwise, returns <c>null</c>.</param>
        /// <returns>
        /// The absolute URL of the webpack asset.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">urlHelper</exception>
        [Obsolete("This method is marked as obsolete as there are possibly multiple files given an assetName & assetType.")]
        public static string AbsoluteWebpackAsset(this UrlHelper urlHelper, string assetName = "main", string assetType = "js", bool required = true)
        {
            if (urlHelper == null) throw new ArgumentNullException(nameof(urlHelper));

            var assetUrl = urlHelper.WebpackAsset(assetName, assetType, required);
            return (assetUrl != null) ? new Uri(urlHelper.RequestContext.HttpContext.Request.Url, assetUrl).AbsoluteUri : null;
        }

        /// <summary>
        /// Gets the URsL of the webpack assets.
        /// </summary>
        /// <param name="urlHelper">The URL helper.</param>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="assetType">Type of the asset.</param>
        /// <param name="required">If set to <c>true</c> throws an <see cref="AssetNotFoundException" /> when the asset could not be found; otherwise, returns <c>null</c>.</param>
        /// <returns>
        /// The URL of the webpack asset.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">urlHelper</exception>
        public static List<string> WebpackAssets(this UrlHelper urlHelper, string assetName = "main", string assetType = "js", bool required = true)
        {
            if (urlHelper == null) throw new ArgumentNullException(nameof(urlHelper));

            var webpack = urlHelper.RequestContext.HttpContext.Application.GetWebpack();
            var assetUrl = webpack.GetAssetsUrl(assetName, assetType, required);

            List<string> list = new List<string>();
            foreach (var w in assetUrl)
            {
                list.Add(urlHelper.Content(w));
            }
            return list;
        }

        /// <summary>
        /// Gets the absolute URL of the webpack assets.
        /// </summary>
        /// <param name="urlHelper">The URL helper.</param>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="assetType">Type of the asset.</param>
        /// <param name="required">If set to <c>true</c> throws an <see cref="AssetNotFoundException" /> when the asset could not be found; otherwise, returns <c>null</c>.</param>
        /// <returns>
        /// The absolute URL of the webpack asset.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">urlHelper</exception>
        public static List<string> AbsoluteWebpackAssets(this UrlHelper urlHelper, string assetName = "main", string assetType = "js", bool required = true)
        {
            if (urlHelper == null) throw new ArgumentNullException(nameof(urlHelper));

            var assetUrl = urlHelper.WebpackAssets(assetName, assetType, required);

            List<string> list = new List<string>();
            foreach (var w in assetUrl)
            {
                list.Add((assetUrl != null) ? new Uri(urlHelper.RequestContext.HttpContext.Request.Url, w).AbsoluteUri : null);
            }

            return list;
        }
    }
}