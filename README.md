# TicketFlow

TicketFlow, ASP.NET Core Web API ile geliştirilmiş ticket yönetim API projesidir.

Bu projede kullanıcı kayıt/giriş işlemleri, JWT tabanlı kimlik doğrulama ve kullanıcıya ait ticket işlemleri bulunmaktadır.

## Kullanılan Teknolojiler

- ASP.NET Core Web API
- C#
- Entity Framework Core
- MSSQL
- JWT Authentication
- Swagger

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
- Katmanlı mimari
- Global exception handling

## Proje Yapısı

```text
TicketFlow
├── TicketFlow.API
├── TicketFlow.Application
├── TicketFlow.Domain
└── TicketFlow.Infrastructure
```

## API Endpointleri

### Auth

```http
POST /api/Auth/register
POST /api/Auth/login
```

### Tickets

```http
GET    /api/Tickets
GET    /api/Tickets/{id}
POST   /api/Tickets
PUT    /api/Tickets/{id}
DELETE /api/Tickets/{id}
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

Veritabanı bağlantısı yapıldıktan sonra migration işlemleri uygulanarak veritabanı oluşturulabilir:

```bash
dotnet ef database update
```

## Geliştirici

Ferhat Samet Kalkan
