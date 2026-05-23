# TicketFlow

TicketFlow, ASP.NET Core Web API ile geliştirilmiş bir ticket yönetim API projesidir.

Bu projede kullanıcı kayıt/giriş işlemleri, JWT tabanlı kimlik doğrulama, kullanıcıya ait ticket yönetimi, katmanlı mimari, global exception handling, unit test ve integration test yapıları uygulanmıştır.

## Kullanılan Teknolojiler

- ASP.NET Core Web API
- C#
- Entity Framework Core
- MSSQL
- JWT Authentication
- Swagger
- AutoMapper
- xUnit
- Moq
- FluentAssertions
- InMemory Database

## Özellikler

- Kullanıcı kayıt işlemi
- Kullanıcı giriş işlemi
- JWT token üretimi
- Yetkilendirme gerektiren ticket işlemleri
- Ticket oluşturma
- Kullanıcının kendi ticketlarını listeleme
- Ticket detayını görüntüleme
- Ticket güncelleme
- Ticket silme
- Kullanıcının yalnızca kendi ticketlarına erişebilmesi
- Katmanlı mimari
- Global exception handling
- Unit testler
- Integration testler

## Proje Yapısı

```text
TicketFlow
├── TicketFlow.API
├── TicketFlow.Application
├── TicketFlow.Domain
├── TicketFlow.Infrastructure
└── TicketFlow.Tests
```

## API Endpointleri

### Auth

```http
POST /api/auth/register
POST /api/auth/login
```

### Tickets

```http
GET    /api/tickets
GET    /api/tickets/{id}
POST   /api/tickets
PUT    /api/tickets/{id}
DELETE /api/tickets/{id}
```

## Kimlik Doğrulama

Login işlemi başarılı olduğunda API bir JWT token döner.

Yetkilendirme isteyen endpointlere istek atarken bu token `Authorization` header içinde gönderilmelidir.

```http
Authorization: Bearer your-token-here
```

## Projeyi Çalıştırmak İçin Gerekli Ayarlar

Projeyi çalıştırmadan önce `TicketFlow.API` projesi altında bulunan `appsettings.json` dosyasında veritabanı bağlantısı ve JWT ayarları yapılandırılmalıdır.

Örnek yapı:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=TicketFlowDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_HERE",
    "Issuer": "TicketFlowAPI",
    "Audience": "TicketFlowClient",
    "ExpireMinutes": 60
  }
}
```

## Testler

Projede unit test ve integration test yapıları bulunmaktadır.

Unit testlerde servis katmanındaki business logic test edilmiştir.

Integration testlerde API endpointleri gerçek HTTP istekleri gibi test edilmiştir. Testlerde gerçek veritabanı yerine InMemory Database kullanılmıştır.

Testleri çalıştırmak için:

```bash
dotnet test
```

## Veritabanı Kurulumu

Veritabanı bağlantısı yapıldıktan sonra migration işlemleri uygulanarak veritabanı oluşturulabilir:

```bash
dotnet ef database update
```

## Geliştirici

Ferhat Samet Kalkan
