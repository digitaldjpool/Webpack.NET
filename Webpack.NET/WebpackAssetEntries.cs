using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Webpack.NET
{
    /// <summary>
    /// A list of entries for the specific asset. Used when webpack plugin is configured to return 'entrypoints'
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{System.String}"/>
    [JsonConverter(typeof(SingleOrArrayConverter))]
    internal class WebpackAssetEntries : List<string>
    {
    }

    /// <summary>
    /// Will handle situations where a value is either a string or string[]
    /// </summary>
    /// <remarks>Based on https://stackoverflow.com/a/18997172/255194</remarks>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    internal class SingleOrArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            WebpackAssetEntries assetList = new WebpackAssetEntries();

            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
            {
                assetList.AddRange(token.ToObject<List<string>>());
            }
            else
            {
                assetList.Add(token.ToObject<string>());
            }

            return assetList;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}