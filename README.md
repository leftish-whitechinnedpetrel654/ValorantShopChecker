# Valorant Shop Checker

**Valorant hesabınıza şifre girmeden, mağazadaki silah skinlerini çeker.**
Oyuna girmeden; günlük 4 skin, fiyatları, cüzdan bakiyeniz (VP / Radianite) ve mağazanın yenilenmesine kalan süreyi tek pencerede gösterir.

> **Şifre istemez.** Uygulama, bilgisayarınızda **zaten açık olan** Riot İstemcisi'nden
> yerel olarak (`127.0.0.1`) kimlik bilgisi okur. Riot kullanıcı adı/şifreniz hiçbir yere gönderilmez.

<!-- Ekran görüntüsü eklemek isterseniz: repo'ya screenshots/shop.png koyup alttaki satırı açın -->

<!-- ![Ekran görüntüsü](screenshots/shop.png) -->

---

## Özellikler

- Günlük mağazadaki 4 skin — görsel + isim + VP fiyatı
- Cüzdan bakiyesi (Valorant Points ve Radianite Points)
- Mağazanın yenilenmesine kalan süre (canlı geri sayım)
- Tek tıkla yenileme
- Bölge (EU / NA / AP / KR) **otomatik** tespit edilir
- Şifresiz, tamamen yerel çalışır — üçüncü tarafa veri gitmez
- Responsive, koyu, Valorant temalı arayüz

---

## Nasıl çalışır? (Güvenlik)

Riot İstemcisi çalışırken bilgisayarınızda yerel bir yönetim API'si açar. Uygulama şu adımları izler:

1. `%LOCALAPPDATA%\Riot Games\Riot Client\Config\lockfile` dosyasından yerel port ve tek kullanımlık parolayı okur.
2. `https://127.0.0.1:<port>/entitlements/v1/token` adresinden geçici erişim (access) ve entitlement token'ını alır.
3. Bu token'larla Riot'un mağaza uç noktalarına (`pd.<bölge>.a.pvp.net`) istek atar.
4. Skin görselleri/isimleri herkese açık [valorant-api.com](https://valorant-api.com) üzerinden çekilir.

**Riot kullanıcı adınız veya şifreniz hiçbir adımda kullanılmaz, saklanmaz, gönderilmez.**
Bu, üçüncü taraf araçlar için Riot'un da onayladığı yerel token yöntemidir.

---

## Gereksinimler

- Windows 10 / 11
- Çalışan ve **giriş yapılmış** Valorant / Riot İstemcisi (uygulamayı açmadan önce oyunu/istemciyi açın)
- Sadece kaynaktan derleyecekseniz: [.NET SDK 9+](https://dotnet.microsoft.com/download)

> Hazır `.exe` self-contained üretildiğinde son kullanıcıda .NET kurulu olması **gerekmez**.

---

## Kullanım

1. Valorant'ı (veya en azından Riot İstemcisi'ni) açıp hesabınıza giriş yapın.
2. `ValorantShopChecker.exe`'yi çalıştırın.
3. Mağaza otomatik yüklenir. Gerekirse **Yenile**'ye basın.

---

## Kaynaktan çalıştırma (geliştirici)

```bash
git clone <repo-url>
cd ValorantShopChecker
dotnet run -c Release
```

## Tek `.exe` oluşturma

En kolayı: **`build-exe.bat`** dosyasına çift tıklayın. Ya da elle:

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

Çıktı:

```
bin\Release\net9.0-windows\win-x64\publish\ValorantShopChecker.exe
```

Bu **tek dosyayı** dilediğiniz bilgisayara kopyalayıp çalıştırabilirsiniz (bağımlılık gerekmez).

---

## 🧩 Proje yapısı

```
├─ App.xaml / App.xaml.cs        Tema (renkler, buton stilleri)
├─ MainWindow.xaml / .cs         Arayüz + durum yönetimi (yükleniyor/hata/mağaza)
├─ src/
│  ├─ Models.cs                  Veri modelleri (oturum, cüzdan, skin)
│  ├─ RiotAuth.cs                Lockfile ile şifresiz yerel giriş + bölge tespiti
│  ├─ RiotStore.cs               Mağaza + cüzdan uç noktaları
│  └─ ValorantApi.cs             Skin adı/görseli + istemci sürümü
├─ build-exe.bat                 Tek exe üretme kısayolu
└─ 26.ico                        Uygulama ikonu
```

---

## ❓ Sorun giderme

| Mesaj                                             | Sebep / Çözüm                                                                                     |
| ------------------------------------------------- | ------------------------------------------------------------------------------------------------- |
| "Riot İstemcisi çalışmıyor (lockfile bulunamadı)" | Valorant/Riot İstemcisi'ni açıp giriş yapın, sonra tekrar deneyin.                                |
| "Yerel token alınamadı"                           | İstemci açık ama giriş yapılmamış olabilir; hesabınıza girin.                                     |
| "Bölge otomatik tespit edilemedi"                 | Oturum kapanmış olabilir; oyunu yeniden açın. Parantez içindeki durum kodlarını issue'ya ekleyin. |
| Mağaza boş/hatalı                                 | Riot bir güncelleme yapmış olabilir; issue açın.                                                  |

---

## ⚠️ Yasal uyarı

Bu proje **resmî değildir** ve Riot Games ile bir bağlantısı yoktur. "Valorant" ve ilgili tüm
varlıklar Riot Games, Inc.'e aittir. Uygulama yalnızca hesabınıza ait mağaza verisini **okur**;
hiçbir değişiklik yapmaz. Kendi sorumluluğunuzda kullanın.
