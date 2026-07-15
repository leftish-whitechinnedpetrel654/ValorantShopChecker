using System;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CheckShopApp
{
    /// <summary>
    /// valorant-api.com (topluluk, herkese açık) üzerinden statik oyun verisi:
    /// istemci sürümü ve skin adı/görseli. Kimlik bilgisi gerektirmez.
    /// </summary>
    public static class ValorantApi
    {
        /// <summary>Riot mağaza uç noktalarının istediği X-Riot-ClientVersion değerini üretir.</summary>
        public static string GetClientVersion()
        {
            var client = new RestClient("https://valorant-api.com/v1/version");
            string content = client.Execute(new RestRequest(Method.GET)).Content ?? "";
            JToken? data = JObject.Parse(content)["data"];
            string version = data?.Value<string>("version") ?? "";
            string tail = version.Length >= 6 ? version.Substring(version.Length - 6) : version;
            return $"{data?.Value<string>("branch")}-shipping-{data?.Value<string>("buildVersion")}-{tail}";
        }

        /// <summary>Bir skin-level UUID'sini görünen ad + ikon URL'sine çevirir.</summary>
        public static (string name, string image) GetSkinLevel(string uuid)
        {
            try
            {
                var client = new RestClient($"https://valorant-api.com/v1/weapons/skinlevels/{uuid}");
                string content = client.Execute(new RestRequest(Method.GET)).Content ?? "";
                JToken? data = JObject.Parse(content)["data"];
                return (
                    data?.Value<string>("displayName") ?? "Bilinmeyen Skin",
                    data?.Value<string>("displayIcon") ?? "");
            }
            catch
            {
                return ("Bilinmeyen Skin", "");
            }
        }
    }
}
