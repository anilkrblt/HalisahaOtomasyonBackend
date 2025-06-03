#######################################################################
# 1️⃣ Build aşaması – .NET 8 SDK
#######################################################################
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Çözüm & proje dosyaları → restore cache'i korur
COPY ["HalisahaOtomasyonBackend.sln", "./"]
COPY ["Shared/Shared.csproj",                               "Shared/"]
COPY ["ServiceContracts/ServiceContracts.csproj",          "ServiceContracts/"]
COPY ["Service/Service.csproj",                            "Service/"]
COPY ["Repository/Repository.csproj",                      "Repository/"]
COPY ["LoggerService/LoggerService.csproj",                "LoggerService/"]
COPY ["Entities/Entities.csproj",                          "Entities/"]
COPY ["Contracts/Contracts.csproj",                        "Contracts/"]
COPY ["HalisahaOtomasyon/HalisahaOtomasyon.csproj",        "HalisahaOtomasyon/"]
COPY ["HalisahaOtomasyonPresentation/HalisahaOtomasyonPresentation.csproj", "HalisahaOtomasyonPresentation/"]
COPY ["HalisahaOtomasyonBackend.Tests/HalisahaOtomasyonBackend.Tests.csproj", "HalisahaOtomasyonBackend.Tests/"]

RUN dotnet restore "HalisahaOtomasyonBackend.sln"

# 2. Tüm kaynak kodu
COPY . .

# 3. Publish
WORKDIR /src/HalisahaOtomasyon
RUN dotnet publish "HalisahaOtomasyon.csproj" -c Release -o /app/publish


#######################################################################
# 2️⃣ Runtime aşaması – hafif ASP.NET 8
#######################################################################
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Yayınlanan çıktıyı kopyala
COPY --from=build /app/publish .

# wwwroot (Railway Volume mount noktası) – eksikse oluştur & izin ver
RUN mkdir -p /app/wwwroot \
    && chmod -R 775 /app/wwwroot

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "HalisahaOtomasyon.dll"]
