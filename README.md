# 🛍️ İkinci El Websitesi

Kullanıcıların ikinci el ürün ilan verebildiği, mesajlaşabildiği ve ürün arayabildiği tam kapsamlı bir marketplace platformu.

## 🚀 Özellikler
- Ürün ilanı oluşturma, düzenleme, silme
- Kategori bazlı listeleme ve arama
- Kullanıcılar arası mesajlaşma
- Fiyat ve lokasyon bilgisi
- Responsive tasarım (Bootstrap 5)

## 🛠️ Kullanılan Teknolojiler
- **Backend:** ASP.NET Core MVC (.NET 8)
- **ORM:** Entity Framework Core
- **Veritabanı:** SQL Server LocalDB
- **Frontend:** Bootstrap 5, Bootstrap Icons

## ⚙️ Kurulum

### Gereksinimler
- .NET 8 SDK
- SQL Server LocalDB

### Adımlar
```bash
# Repoyu klonla
git clone https://github.com/taklaci59/ikinci-el-websitesi.git
cd ikinci-el-websitesi

# Veritabanını oluştur
dotnet ef database update

# Uygulamayı çalıştır
dotnet run
```

Tarayıcıda `https://localhost:5001` adresini aç.

## 🗄️ Veritabanı
Bağlantı dizesi `appsettings.json` içinde tanımlıdır:
```
Server=(localdb)\mssqllocaldb;Database=MarketPlaceProDb;Trusted_Connection=True
```

## 👤 Geliştirici
**Kıvanç** — [GitHub](https://github.com/taklaci59)
