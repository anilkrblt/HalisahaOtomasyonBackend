# 1️⃣ Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

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

RUN dotnet restore
COPY . .

WORKDIR /src/HalisahaOtomasyonPresentation
RUN dotnet publish "HalisahaOtomasyonPresentation.csproj" -c Release -r linux-x64 --self-contained false -o /app/publish

# 2️⃣ Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "HalisahaOtomasyonPresentation.dll"]
