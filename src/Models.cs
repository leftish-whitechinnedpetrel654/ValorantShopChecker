using System.Collections.Generic;

namespace CheckShopApp
{
    /// <summary>Riot istemcisinden yerel olarak alınan oturum bilgileri (şifre içermez).</summary>
    public class RiotSession
    {
        public string AccessToken { get; set; } = "";
        public string EntitlementToken { get; set; } = "";
        public string Subject { get; set; } = "";   // puuid
        public string Region { get; set; } = "";     // shard: eu / na / ap / kr
        public string Version { get; set; } = "";
    }

    /// <summary>Cüzdan bakiyeleri.</summary>
    public class Wallet
    {
        public int ValorantPoints { get; set; }
        public int RadianitePoints { get; set; }
        public int KingdomCredits { get; set; }
    }

    /// <summary>Günlük mağazadaki tek bir skin teklifi.</summary>
    public class SkinOffer
    {
        public string Uuid { get; set; } = "";
        public string Name { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public int Price { get; set; }

        public string PriceText => Price > 0 ? Price.ToString("N0") : "—";
    }

    /// <summary>Mağazanın tek çekimlik anlık görüntüsü.</summary>
    public class ShopSnapshot
    {
        public List<SkinOffer> Skins { get; set; } = new List<SkinOffer>();
        public int SecondsRemaining { get; set; }
        public Wallet Wallet { get; set; } = new Wallet();
        public string Region { get; set; } = "";
    }
}
