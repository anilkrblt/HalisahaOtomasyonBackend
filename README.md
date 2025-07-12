<h1 align="center" id="title">Halısaha Otomasyon Backend</h1>

<p align="center">
  <img src="https://socialify.git.ci/anilkrblt/HalisahaOtomasyonBackend/image?font=Inter&amp;forks=1&amp;language=1&amp;name=1&amp;owner=1&amp;stargazers=1&amp;theme=Light" alt="project-image">
</p>

---

## 📝 Proje Hakkında

Bu proje, halı saha işletmelerinin rezervasyon süreçlerini yönetebileceği bir otomasyon sisteminin backend altyapısını içerir. Kullanıcılar saatlik rezervasyon yapabilir, geçmiş rezervasyonlarını görüntüleyebilir ve işletmeler duyuru/paylaşım gibi işlevleri kullanabilir.

## 🚀 Özellikler

- Kullanıcı ve yönetici rolleri
- Rezervasyon oluşturma ve listeleme
- Geçmiş rezervasyonların takibi
- Duyuru yönetimi
- JWT tabanlı kimlik doğrulama
- Swagger ile API dokümantasyonu

## 🛠️ Kullanılan Teknolojiler

- ASP.NET Core Web API
- Entity Framework Core
- MySQL
- JWT Authentication
- Layered Architecture
- Repository Pattern
- Swagger

## 📂 Proje Yapısı
HalisahaOtomasyonBackend/  
├── Contracts/  
├── Entities/  
├── HalisahaOtomasyon/  
├── HalisahaOtomasyonBackend.Tests/  
├── HalisahaOtomasyonPresentation/  
├── LoggerService/  
├── Repository/  
├── Service/  
├── ServiceContracts/   
├── Shared/   



## 🧪 API Dokümantasyonu

> Swagger UI üzerinden test edilebilir:  
> `https://halisaha.up.railway.app/swagger/index.html`

## ⚙️ Kurulum

```bash
git clone https://github.com/anilkrblt/HalisahaOtomasyonBackend.git
cd HalisahaOtomasyonBackend
dotnet restore
dotnet run
