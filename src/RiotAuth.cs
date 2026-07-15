using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace CheckShopApp
{
    /// <summary>
    /// Şifre GİRMEDEN kimlik doğrulama: çalışan Riot istemcisinin lockfile'ından
    /// yerel token'ları okur. Hiçbir kimlik bilgisi üçüncü tarafa gönderilmez;
    /// istekler yalnızca 127.0.0.1'e ve Riot'un kendi uç noktalarına gider.
    /// </summary>
    public static class RiotAuth
    {
        private static readonly string[] Shards = { "eu", "na", "ap", "kr" };

        public static RiotSession Login()
        {
            var session = new RiotSession();
            var (port, password) = ReadLockfile();

            // Yerel istemciden access + entitlement token + puuid
            var local = new RestClient($"https://127.0.0.1:{port}");
            local.RemoteCertificateValidationCallback = (a, b, c, d) => true; // yerel self-signed sertifika
            local.Authenticator = new HttpBasicAuthenticator("riot", password);

            var response = local.Execute(new RestRequest("/entitlements/v1/token", Method.GET));
            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
                throw new Exception("Yerel token alınamadı. Valorant'a giriş yapılı ve istemci açık olmalı.");

            JObject json = JObject.Parse(response.Content);
            session.AccessToken = json.Value<string>("accessToken") ?? "";
            session.EntitlementToken = json.Value<string>("token") ?? "";
            session.Subject = json.Value<string>("subject") ?? "";
            if (string.IsNullOrEmpty(session.Subject))
                session.Subject = new JsonWebToken(session.AccessToken).Subject;

            session.Version = ValorantApi.GetClientVersion();
            session.Region = DetectRegion(session);
            return session;
        }

        private static (string port, string password) ReadLockfile()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Riot Games", "Riot Client", "Config", "lockfile");

            if (!File.Exists(path))
                throw new Exception("Riot İstemcisi çalışmıyor (lockfile bulunamadı). Valorant/Riot İstemcisi'ni açıp giriş yapın.");

            string content;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs))
                content = sr.ReadToEnd();

            // biçim:  name:pid:port:password:protocol
            string[] parts = content.Split(':');
            if (parts.Length < 5)
                throw new Exception("lockfile beklenmedik biçimde. Riot İstemcisi'ni yeniden başlatın.");

            return (parts[2], parts[3]);
        }

        // Her aday shard'ın cüzdan uç noktasını yoklar; 200 dönen shard doğru bölgedir.
        // (PAS/riot-geo yerine bu yöntem daha güvenilir ve tek adımda token'ı da doğrular.)
        private static string DetectRegion(RiotSession session)
        {
            var diagnostics = new List<string>();
            foreach (string shard in Shards)
            {
                session.Region = shard;
                var request = new RestRequest(Method.GET);
                RiotStore.AddHeaders(request, session);

                var client = new RestClient($"https://pd.{shard}.a.pvp.net/store/v1/wallet/{session.Subject}");
                client.RemoteCertificateValidationCallback = (a, b, c, d) => true;

                var response = client.Execute(request);
                if ((int)response.StatusCode == 200)
                    return shard;
                diagnostics.Add($"{shard}:{(int)response.StatusCode}");
            }
            throw new Exception("Bölge otomatik tespit edilemedi (" + string.Join(", ", diagnostics) +
                                "). Valorant açık ve giriş yapılı olduğundan emin olun.");
        }
    }
}
