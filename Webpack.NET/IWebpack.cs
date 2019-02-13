using System.Collections.Generic;

namespace Webpack.NET
{
    /// <summary>
    /// Represents a configured webpack instance.
    /// </summary>
    internal interface IWebpack
    {
        /// <summary>
        /// Gets the asset URL.
        /// </summary>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="assetType">Type of the asset.</param>
        /// <param name="required">If set to <c>true</c> throws an <see cref="AssetNotFoundException" /> when the asset could not be found; otherwise, returns <c>null</c>.</param>
        /// <returns>
        /// The asset URL.
        /// </returns>
        /// <exception cref="AssetNotFoundException">Asset '<paramref name="assetName" />' with type '<paramref name="assetType" />' could not be found.</exception>
        string GetAssetUrl(string assetName, string assetType, bool required = true);

        /// <summary>
        /// Gets a list of URLS for the multiple assets. This is typically used when the
        /// assets-webpack-plugin is configured to return "entrypoints." In this configuration the
        /// manifest file has an array of files for each asset.
        /// </summary>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="assetType">Type of the asset.</param>
        /// <param name="required">if set to <c>true</c> [required].</param>
        /// <returns></returns>
        List<string> GetAssetsUrl(string assetName, string assetType, bool required = true);
    }
}