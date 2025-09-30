# Ucak Rezervasyon Sistemi

Bu proje, C# ve ASP.NET Core kullanılarak geliştirilmiş çok katmanlı bir uçak rezervasyon sistemidir.

## Proje Katmanları

1. **Entities**

   * Veritabanı modelleri (User, Flight, Reservation vb.)
2. **DataAccess**

   * Repository deseni ile veri erişimi
   * Entity Framework Core kullanımı
3. **Business**

   * İş mantığı ve servisler (AuthService, UserService)
4. **Api**

   * ASP.NET Core Web API
   * Controller’lar üzerinden HTTP isteklerini yönlendirir

## Gereksinimler

* .NET 8 SDK
* Visual Studio 2022 veya 2023
* SQLite veya tercih edilen başka bir veritabanı

## Kurulum

1. Projeyi klonlayın veya ZIP dosyasını açın:

   ```bash
   git clone <repo-link>
   ```
2. Visual Studio ile çözümü açın: `UcakRezervasyon.sln`
3. NuGet paketlerini yükleyin:

   * `Microsoft.EntityFrameworkCore.Sqlite`
   * `Microsoft.EntityFrameworkCore.Tools`
   * `System.IdentityModel.Tokens.Jwt`
   * `Microsoft.IdentityModel.Tokens`
   * `Microsoft.AspNetCore.DataProtection`
4. Solution'u **Clean** ve **Rebuild** yapın.
5. `Api` projesini başlatın.

## Kullanım

* API üzerinden kullanıcı kaydı, giriş, uçuş listeleme ve rezervasyon işlemleri yapılabilir.
* JWT token sistemi ile güvenli kimlik doğrulama sağlanır.

## Katmanlı Mimari

```
Entities <-- DataAccess <-- Business <-- Api
```

* **Entities**: Veri modelleri
* **DataAccess**: Veritabanı ve repository işlemleri
* **Business**: İş mantığı ve servisler
* **Api**: Kullanıcıya servis sağlayan Web API
