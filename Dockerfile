# ----------------------------------------
# 1. Base Runtime Image (สำหรับรันแอพ)
# ----------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
# .NET 8/9/10 ใน Docker จะฟังพอร์ต 8080 เป็นค่าเริ่มต้น
EXPOSE 8080

# ----------------------------------------
# 2. Build Stage (สำหรับ compile โค้ด)
# ----------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# คัดลอกเฉพาะ .csproj มา restore cache ก่อน เพื่อความรวดเร็วในการ build
COPY ["stock-api.csproj", "./"]
RUN dotnet restore "stock-api.csproj"

# คัดลอกโค้ดทั้งหมดแล้ว build
COPY . .
RUN dotnet build "stock-api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ----------------------------------------
# 3. Publish Stage
# ----------------------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "stock-api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ----------------------------------------
# 4. Final Stage (Image จริงที่นำไปรัน)
# ----------------------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "stock-api.dll"]