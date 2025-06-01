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

# Restore nuget packages
RUN dotnet restore

# Şimdi tüm kaynakları kopyala
COPY . .

# HalisahaOtomasyonPresentation API katmanını publish et
WORKDIR /src/HalisahaOtomasyonPresentation
RUN dotnet publish -c Release -o /app/publish

# 2️⃣ Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Publish edilen çıktıyı al
COPY --from=build /app/publish .

# Port ayarla (Render ve Railway default 8080 dinler)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Uygulama başlat
ENTRYPOINT ["dotnet", "HalisahaOtomasyonPresentation.dll"]
