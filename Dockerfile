#######################################################################
# 1️⃣ Build aşaması – resmi .NET 8 SDK imajı
#######################################################################
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ▸ Yalnızca .csproj'leri kopyala → restore cache'i koru
COPY ["HalisahaOtomasyonBackend.sln", "./"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["ServiceContracts/ServiceContracts.csproj", "ServiceContracts/"]
COPY ["Service/Service.csproj", "Service/"]
COPY ["Repository/Repository.csproj",          "Repository/"]
COPY ["LoggerService/LoggerService.csproj", "LoggerService/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["Contracts/Contracts.csproj",            "Contracts/"]
COPY ["HalisahaOtomasyon/HalisahaOtomasyon.csproj", "HalisahaOtomasyon/"]
COPY ["HalisahaOtomasyonPresentation/HalisahaOtomasyonPresentation.csproj","HalisahaOtomasyonPresentation/"]
COPY ["HalisahaOtomasyonBackend.Tests/HalisahaOtomasyonBackend.Tests.csproj","HalisahaOtomasyonBackend.Tests/"]

RUN dotnet restore "HalisahaOtomasyonBackend.sln"

# ▸ Tüm kaynak kodunu kopyala
COPY . .

# ▸ Web API projesini publish et (self-contained değil)
WORKDIR /src/HalisahaOtomasyon
RUN dotnet publish "HalisahaOtomasyon.csproj" -c Release -o /app/publish


#######################################################################
# 2️⃣ Runtime aşaması – hafif ASP.NET 8 runtime imajı
#######################################################################
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# ▸ Yayınlanan çıktıyı ekle
COPY --from=build /app/publish .

# ▸ wwwroot klasörünü garantiye al (Railway Volume → /app/wwwroot)
RUN mkdir -p /app/wwwroot

# ▸ İsteğe bağlı: Volume bildir (bilgi amaçlı)
VOLUME ["/app/wwwroot"]

# ▸ ASP.NET portu
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "HalisahaOtomasyon.dll"]
