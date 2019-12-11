namespace Webpack.NET
{
    /// <summary>
    /// Configuration properties for webpack integration.
    /// </summary>
    public class WebpackConfig
    {
        /// <summary>
        /// Gets or sets the server-relative path to the JSON file output by https://github.com/kossnocorp/assets-webpack-plugin, 
        /// eg. "~/scripts/webpack-assets.json"
        /// </summary>
        public string AssetManifestPath { get; set; }

        /// <summary>
        /// Gets or sets the server-relative path to the output of the webpack assets, eg. "~/scripts".
        /// This can be used to prefix the urls in the webpack-assets.json file with a CDN
        /// </summary>
        public string AssetOutputPath { get; set; }

        /// <summary>
        /// If caching is disabled then the webpack-assets.json file will be read every page load. 
        /// This is usful in scenarios where front-end development is happening with hot-reload (i.e. npm --watch)
        /// </summary>
        public bool DisableCaching { get; set; }
    }
}