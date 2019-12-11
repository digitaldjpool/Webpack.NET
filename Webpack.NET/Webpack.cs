using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webpack.NET
{
    /// <summary>
    /// Implementation of configured webpack instance.
    /// </summary>
    /// <seealso cref="Webpack.NET.IWebpack" />
    public class Webpack : IWebpack
    {
        private static bool _isInitialized;
        private static HttpServerUtilityBase _httpServerUtility;
        private static WebpackConfig _config;
        private static IWebpack s_instance;

        public static IWebpack Instance
        {
            get
            {
                if (!_isInitialized)
                {
                    throw new ApplicationException("Webpack not initialized");
                }

                if (_config.DisableCaching)
                {
                    Reload();
                }

                return s_instance;
            }
            internal set
            {
                _isInitialized = true;
                s_instance = value;
            }
        }

        public static void Initialize(HttpServerUtilityBase httpServerUtility, WebpackConfig config)
        {
            if (_isInitialized)
            {
                throw new ApplicationException("Already initialized");
            }

            _httpServerUtility = httpServerUtility ?? throw new ArgumentNullException(nameof(httpServerUtility));
            _config = config;
            var _webpackBuilder = new WebpackBuilder(_httpServerUtility);
            s_instance = _webpackBuilder.Build(_config);
            _isInitialized = true;
        }

        public static void Reload()
        {
            if (!_isInitialized)
            {
                throw new ApplicationException("Not yet initialized. Can't reload.");
            }

            var _webpackBuilder = new WebpackBuilder(_httpServerUtility);
            s_instance = _webpackBuilder.Build(_config);
            _isInitialized = true;
        }

        private readonly IEnumerable<WebpackAssetsDictionary> assets;

        internal Webpack(IEnumerable<WebpackAssetsDictionary> webpackAssetsDictionaries)
        {
            assets = webpackAssetsDictionaries ?? throw new ArgumentNullException(nameof(webpackAssetsDictionaries));
        }

        /// <summary>
        /// Gets the asset URL.
        /// </summary>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="assetType">Type of the asset.</param>
        /// <param name="required">If set to <c>true</c> throws an <see cref="AssetNotFoundException" /> when the asset could not be found; otherwise, returns <c>null</c>.</param>
        /// <returns>
        /// The asset URL.
        /// </returns>
        /// <exception cref="AssetNotFoundException"></exception>
        [Obsolete("The assets-webpack-plugin can return an array of assets for a single entry. This is typically used with 'entrypoint'. This method is kept for backwards compatibility, but in the event it is used when multiple assets exist, the first one will be returned.", false)]
        public string GetAssetUrl(string assetName, string assetType, bool required = true)
        {
            var allAssets = GetAssetsUrl(assetName, assetType, required);
            if (required && (allAssets == null || !allAssets.Any()))
            {
                throw new AssetNotFoundException(assetName, assetType);
            }
            return allAssets?.FirstOrDefault();
        }

        /// <summary>
        /// Gets a list of URLS for the multiple assets. This is typically used when the
        /// assets-webpack-plugin is configured to return "entrypoints." In this configuration the
        /// manifest file has an array of files for each asset.
        /// </summary>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="assetType">Type of the asset.</param>
        /// <param name="required">if set to <c>true</c> [required].</param>
        /// <returns></returns>
        /// <exception cref="AssetNotFoundException">
        /// </exception>
        public List<string> GetAssetsUrl(string assetName, string assetType, bool required = true)
        {
            var matchingDictionary = this.assets
                .Where(a => a.ContainsKey(assetName))
                .FirstOrDefault();

            if (matchingDictionary == null)
            {
                if (required)
                {
                    throw new AssetNotFoundException(assetName, assetType);
                }

                return new List<string>(0);
            }

            WebpackAsset webpackAsset = matchingDictionary[assetName];

            if (matchingDictionary[assetName] == null)
            {
                if (required)
                {
                    throw new AssetNotFoundException(assetName, assetType);
                }

                return new List<string>(0);
            }

            if (!webpackAsset.ContainsKey(assetType))
            {
                if (required)
                {
                    throw new AssetNotFoundException(assetName, assetType);
                }

                return new List<string>(0);
            }

            List<string> finalList = new List<string>(webpackAsset[assetType].Count);
            foreach (var assetUrl in webpackAsset[assetType])
            {
                string rootFolder = matchingDictionary.RootFolder;
                if (String.IsNullOrEmpty(rootFolder) || Uri.IsWellFormedUriString(assetUrl, UriKind.Absolute))
                {
                    // No root folder set or asset is already an absolute URL
                    finalList.Add(assetUrl);
                }
                else
                {
                    // Combine root folder and asset URL
                    finalList.Add(JoinUrlSegments(rootFolder, assetUrl));
                }
            }
            return finalList;
        }


        /// <summary>
        /// Joins the URL segments ensuring '/' characters are used properly.
        /// </summary>
        /// <param name="first">The first url segment.</param>
        /// <param name="second">The second url segment.</param>
        private string JoinUrlSegments(string first, string second)
        {
            var slash = new[] { '/' };
            return $"{first.Replace('\\', '/').TrimEnd(slash)}/{second.Replace('\\', '/').TrimStart(slash).TrimEnd(slash)}";
        }
    }
}