using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CheckShopApp
{
    /// <summary>
    /// Riot mağaza (pd) uç noktalarını çağırır: cüzdan + günlük storefront.
    /// Tüm istekler yerel Riot istemcisinden alınan token'larla imzalanır.
    /// </summary>
    public static class RiotStore
    {
        // Para birimi UUID'leri (Riot sabitleri)
        private const string VpId = "85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741";
        private const string RadId = "e59aa87c-4cbf-517a-5983-6e81511be9b7";
        private const string KcId = "85ca954a-41f2-ce94-9b45-8ca3dd39a00d";

        // Base64 kodlu istemci platform bilgisi (Riot'un beklediği sabit başlık)
        private const string ClientPlatform =
            "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9";

        /// <summary>Standart Riot mağaza başlıklarını isteğe ekler.</summary>
        public static void AddHeaders(RestRequest request, RiotSession s)
        {
            request.AddHeader("Authorization", $"Bearer {s.AccessToken}");
            request.AddHeader("X-Riot-Entitlements-JWT", s.EntitlementToken);
            request.AddHeader("X-Riot-ClientPlatform", ClientPlatform);
            request.AddHeader("X-Riot-ClientVersion", s.Version);
        }

        private static RestClient Pd(RiotSession s, string path)
        {
            var client = new RestClient($"https://pd.{s.Region}.a.pvp.net{path}");
            client.RemoteCertificateValidationCallback = (a, b, c, d) => true;
            return client;
        }

        /// <summary>Cüzdan bakiyelerini getirir.</summary>
        public static Wallet GetWallet(RiotSession s)
        {
            var request = new RestRequest(Method.GET);
            AddHeaders(request, s);
            string content = Pd(s, $"/store/v1/wallet/{s.Subject}").Execute(request).Content ?? "";

            JToken? balances = ParseOrThrow(content, "Cüzdan")["Balances"];
            return new Wallet
            {
                ValorantPoints = balances?[VpId]?.Value<int>() ?? 0,
                RadianitePoints = balances?[RadId]?.Value<int>() ?? 0,
                KingdomCredits = balances?[KcId]?.Value<int>() ?? 0,
            };
        }

        /// <summary>Günlük mağazayı (skinler + fiyat + yenilenme süresi) ve cüzdanı çeker.</summary>
        public static ShopSnapshot GetShop(RiotSession s)
        {
            var snap = new ShopSnapshot { Region = s.Region, Wallet = GetWallet(s) };

            // storefront v3 -> POST (boş {} gövdesi)
            var request = new RestRequest(Method.POST);
            AddHeaders(request, s);
            request.AddJsonBody("{}");
            string content = Pd(s, $"/store/v3/storefront/{s.Subject}").Execute(request).Content ?? "";

            JToken? panel = ParseOrThrow(content, "Storefront")["SkinsPanelLayout"];
            if (panel == null)
                throw new Exception("Mağaza verisi beklenen biçimde değil (SkinsPanelLayout yok).");

            snap.SecondsRemaining = panel.Value<int?>("SingleItemOffersRemainingDurationInSeconds") ?? 0;

            // Fiyatları aynı yanıttaki SingleItemStoreOffers listesinden eşle
            var priceByOffer = new Dictionary<string, int>();
            if (panel["SingleItemStoreOffers"] is JArray storeOffers)
            {
                foreach (var offer in storeOffers)
                {
                    string? id = offer.Value<string>("OfferID");
                    if (id != null)
                        priceByOffer[id] = offer["Cost"]?[VpId]?.Value<int>() ?? 0;
                }
            }

            var uuids = panel["SingleItemOffers"] as JArray ?? new JArray();
            foreach (var token in uuids)
            {
                string uuid = token.Value<string>() ?? "";
                if (uuid == "") continue;

                var (name, image) = ValorantApi.GetSkinLevel(uuid);
                snap.Skins.Add(new SkinOffer
                {
                    Uuid = uuid,
                    Name = name,
                    ImageUrl = image,
                    Price = priceByOffer.TryGetValue(uuid, out int p) ? p : 0,
                });
            }
            return snap;
        }

        private static JObject ParseOrThrow(string content, string what)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new Exception($"{what} yanıtı boş geldi. Valorant açık ve giriş yapılı olmalı.");
            try { return JObject.Parse(content); }
            catch { throw new Exception($"{what} yanıtı çözümlenemedi: {content.Substring(0, Math.Min(200, content.Length))}"); }
        }
    }
}
