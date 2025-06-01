#######################################################################
# 1️⃣ Build aşaması – resmi .NET 8 SDK imajı
#######################################################################
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Çözüm ve proje dosyalarını kopyala
COPY ["HalisahaOtomasyonBackend.sln", "./"]         
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["ServiceContracts/ServiceContracts.csproj", "ServiceContracts/"]
COPY ["Service/Service.csproj", "Service/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["LoggerService/LoggerService.csproj", "LoggerService/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
COPY ["HalisahaOtomasyon/HalisahaOtomasyon.csproj", "HalisahaOtomasyon/"]
COPY ["HalisahaOtomasyonPresentation/HalisahaOtomasyonPresentation.csproj", "HalisahaOtomasyonPresentation/"]

# NuGet bağımlılıklarını geri yükle
RUN dotnet restore "HalisahaOtomasyonBackend.sln"

# Kaynak kodunun tamamını kopyala
COPY . .

# Web API projesinin dizinine geç
WORKDIR "/src/HalisahaOtomasyon"

# Yayınlama (framework-dependent, runtimeconfig dâhil)
RUN dotnet publish "HalisahaOtomasyon.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Derlenmiş çıktıyı kopyala
COPY --from=build /app/publish .

# Debug amaçlı listeleyebilirsin (isteğe bağlı)
# RUN ls -la /app

# Port ayarları
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Uygulamayı çalıştır
ENTRYPOINT ["dotnet", "HalisahaOtomasyon.dll"]
