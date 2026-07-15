# Valorant Shop Checker

**Checks the weapon skins in your Valorant store — without entering your password.**
No game launch required; shows the 4 daily skins, their prices, your wallet balance (VP / Radianite) and the time left until the shop resets, all in one window.

> **No password needed.** The app reads credentials locally (`127.0.0.1`) from the Riot Client that is **already running** on your PC. Your Riot username/password is never sent anywhere.

<!-- Screenshot: put screenshots/shop.png in the repo and uncomment the line below -->
<!-- ![Screenshot](screenshots/shop.png) -->

🇹🇷 **Türkçe için [aşağı kaydırın](#valorant-shop-checker-türkçe).**

---

## Features

- 4 daily store skins — image + name + VP price
- Wallet balance (Valorant Points & Radianite Points)
- Live countdown to the next shop reset
- One-click refresh
- Region (EU / NA / AP / KR) detected **automatically**
- Passwordless, fully local — no data leaves your machine
- Responsive, dark, Valorant-themed UI

## How it works (security)

While the Riot Client runs, it exposes a local admin API on your PC. The app:

1. Reads the local port and a one-time password from `%LOCALAPPDATA%\Riot Games\Riot Client\Config\lockfile`.
2. Gets a temporary access token and entitlement token from `https://127.0.0.1:<port>/entitlements/v1/token`.
3. Uses those tokens to call Riot's store endpoints (`pd.<region>.a.pvp.net`).
4. Pulls skin images/names from the public [valorant-api.com](https://valorant-api.com).

**Your Riot username or password is never used, stored, or sent.** This is the local-token method that third-party tools are expected to use.

## Requirements

- Windows 10 / 11
- Valorant / Riot Client **running and logged in** (open it before launching the app)
- To build from source only: [.NET SDK 9+](https://dotnet.microsoft.com/download)

> When published as a self-contained `.exe`, end users do **not** need .NET installed.

## 🚀 Usage

1. Open Valorant (or at least the Riot Client) and log in.
2. Run `ValorantShopChecker.exe`.
3. The store loads automatically. Press **Refresh** if needed.

## Run from source (developers)

```bash
git clone https://github.com/k0laa/ValorantShopChecker.git
cd ValorantShopChecker
dotnet run -c Release
```

## Build a single `.exe`

Easiest: double-click **`build-exe.bat`**. Or manually:

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

Output: `bin\Release\net9.0-windows\win-x64\publish\ValorantShopChecker.exe` — copy this single file to any PC and run it (no dependencies).

## Troubleshooting

| Message | Cause / Fix |
|--------|-------------|
| "Riot Client not running (lockfile not found)" | Open Valorant/Riot Client, log in, try again. |
| "Could not get local token" | Client is open but not logged in — sign in to your account. |
| "Could not detect region" | Session may have expired; restart the game. Add the status codes in brackets to an issue. |
| Store empty / error | Riot may have changed something; open an issue. |

## Disclaimer

This project is **unofficial** and not affiliated with Riot Games. "Valorant" and all related assets belong to Riot Games, Inc. The app only **reads** your own store data and changes nothing. Use at your own risk.

---

<br>

# Valorant Shop Checker (Türkçe)

**Valorant hesabınıza şifre girmeden, mağazadaki silah skinlerini çeker.**
Oyuna girmeden; günlük 4 skin, fiyatları, cüzdan bakiyeniz (VP / Radianite) ve mağazanın yenilenmesine kalan süreyi tek pencerede gösterir.

> **Şifre istemez.** Uygulama, bilgisayarınızda **zaten açık olan** Riot İstemcisi'nden yerel olarak (`127.0.0.1`) kimlik bilgisi okur. Riot kullanıcı adı/şifreniz hiçbir yere gönderilmez.

🇬🇧 **For English, [scroll to top](#valorant-shop-checker).**

---

## Özellikler

- Günlük mağazadaki 4 skin — görsel + isim + VP fiyatı
- Cüzdan bakiyesi (Valorant Points ve Radianite Points)
- Mağazanın yenilenmesine kalan süre (canlı geri sayım)
- Tek tıkla yenileme
- Bölge (EU / NA / AP / KR) **otomatik** tespit edilir
- Şifresiz, tamamen yerel çalışır — üçüncü tarafa veri gitmez
- Responsive, koyu, Valorant temalı arayüz

## Nasıl çalışır? (Güvenlik)

Riot İstemcisi çalışırken bilgisayarınızda yerel bir yönetim API'si açar. Uygulama:

1. `%LOCALAPPDATA%\Riot Games\Riot Client\Config\lockfile` dosyasından yerel port ve tek kullanımlık parolayı okur.
2. `https://127.0.0.1:<port>/entitlements/v1/token` adresinden geçici erişim ve entitlement token'ı alır.
3. Bu token'larla Riot'un mağaza uç noktalarına (`pd.<bölge>.a.pvp.net`) istek atar.
4. Skin görselleri/isimleri herkese açık [valorant-api.com](https://valorant-api.com) üzerinden çekilir.

**Riot kullanıcı adınız veya şifreniz hiçbir adımda kullanılmaz, saklanmaz, gönderilmez.** Bu, üçüncü taraf araçlar için beklenen yerel token yöntemidir.

## Gereksinimler

- Windows 10 / 11
- Çalışan ve **giriş yapılmış** Valorant / Riot İstemcisi (uygulamayı açmadan önce oyunu/istemciyi açın)
- Sadece kaynaktan derleyecekseniz: [.NET SDK 9+](https://dotnet.microsoft.com/download)

> Self-contained `.exe` olarak üretildiğinde son kullanıcıda .NET kurulu olması **gerekmez**.

## Kullanım

1. Valorant'ı (veya en azından Riot İstemcisi'ni) açıp hesabınıza giriş yapın.
2. `ValorantShopChecker.exe`'yi çalıştırın.
3. Mağaza otomatik yüklenir. Gerekirse **Yenile**'ye basın.

## Kaynaktan çalıştırma (geliştirici)

```bash
git clone https://github.com/k0laa/ValorantShopChecker.git
cd ValorantShopChecker
dotnet run -c Release
```

## Tek `.exe` oluşturma

En kolayı: **`build-exe.bat`** dosyasına çift tıklayın. Ya da elle:

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

Çıktı: `bin\Release\net9.0-windows\win-x64\publish\ValorantShopChecker.exe` — bu tek dosyayı dilediğiniz bilgisayara kopyalayıp çalıştırabilirsiniz (bağımlılık gerekmez).


## Proje yapısı

```
├─ App.xaml / App.xaml.cs        Tema (renkler, buton stilleri)
├─ MainWindow.xaml / .cs         Arayüz + durum yönetimi (yükleniyor/hata/mağaza)
├─ src/
│  ├─ Models.cs                  Veri modelleri (oturum, cüzdan, skin)
│  ├─ RiotAuth.cs                Lockfile ile şifresiz yerel giriş + bölge tespiti
│  ├─ RiotStore.cs               Mağaza + cüzdan uç noktaları
│  └─ ValorantApi.cs             Skin adı/görseli + istemci sürümü
├─ build-exe.bat                 Tek exe üretme kısayolu
└─ valo-shop.ico                 Uygulama ikonu
```

## ❓ Sorun giderme

| Mesaj | Sebep / Çözüm |
|------|----------------|
| "Riot İstemcisi çalışmıyor (lockfile bulunamadı)" | Valorant/Riot İstemcisi'ni açıp giriş yapın, tekrar deneyin. |
| "Yerel token alınamadı" | İstemci açık ama giriş yapılmamış olabilir; hesabınıza girin. |
| "Bölge otomatik tespit edilemedi" | Oturum kapanmış olabilir; oyunu yeniden açın. Parantez içindeki durum kodlarını issue'ya ekleyin. |
| Mağaza boş/hatalı | Riot bir güncelleme yapmış olabilir; issue açın. |

## ⚠️ Yasal uyarı

Bu proje **resmî değildir** ve Riot Games ile bağlantısı yoktur. "Valorant" ve ilgili tüm varlıklar Riot Games, Inc.'e aittir. Uygulama yalnızca hesabınıza ait mağaza verisini **okur**, hiçbir değişiklik yapmaz. Kendi sorumluluğunuzda kullanın.
