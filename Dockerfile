# 1️⃣ Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Solution ve projeleri kopyala
COPY *.sln ./
COPY Contracts/*.csproj ./Contracts/
COPY Entities/*.csproj ./Entities/
COPY HalisahaOtomasyon/*.csproj ./HalisahaOtomasyon/
COPY HalisahaOtomasyonPresentation/*.csproj ./HalisahaOtomasyonPresentation/
COPY LoggerService/*.csproj ./LoggerService/
COPY Repository/*.csproj ./Repository/
COPY Service/*.csproj ./Service/
COPY ServiceContracts/*.csproj ./ServiceContracts/
COPY Shared/*.csproj ./Shared/

# NuGet paketlerini indir
RUN dotnet restore

# Tüm kaynakları kopyala
COPY . .

# API projesini publish et (framework bağımlı ve Linux için)
WORKDIR /src/HalisahaOtomasyonPresentation
RUN dotnet publish -c Release -o /app/publish --runtime linux-x64 --self-contained false

# 2️⃣ Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Yayınlanan çıktı dosyalarını al
COPY --from=build /app/publish .

# Port ayarı (Railway 8080 kullanır)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "HalisahaOtomasyonPresentation.dll"]
