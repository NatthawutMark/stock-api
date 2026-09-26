# การเชื่อมต่อฐานข้อมูลและสร้าง Model (Database First) สำหรับโปรเจกต์ .NET Core

เอกสารนี้จะอธิบายขั้นตอนที่คุณสามารถทำได้ด้วยตนเอง หากต้องการเชื่อมต่อฐานข้อมูล PostgreSQL และดึงตารางต่างๆ มาสร้างเป็น Class (Model) อัตโนมัติ (ที่เราเรียกว่า Scaffold หรือ Reverse Engineering)

## สิ่งที่ต้องเตรียม

1. โปรเจกต์ต้องติดตั้งแพ็กเกจที่จำเป็น ได้แก่:
   - `Microsoft.EntityFrameworkCore.Design` (สำหรับใช้งานคำสั่ง ef)
   - `Npgsql.EntityFrameworkCore.PostgreSQL` (สำหรับเชื่อมต่อ PostgreSQL)
2. ติดตั้งเครื่องมือ `dotnet-ef` สำหรับรันคำสั่ง

---

## ขั้นตอนการทำ (Step-by-Step)

### 1. ติดตั้งแพ็กเกจในโปรเจกต์
เปิด Terminal ที่โฟลเดอร์โปรเจกต์ (ตำแหน่งเดียวกับไฟล์ `.csproj`) แล้วรันคำสั่ง:
```bash
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

### 2. ติดตั้งเครื่องมือ dotnet-ef
หากยังไม่เคยติดตั้งในเครื่อง ให้ติดตั้งโดยใช้คำสั่ง:
```bash
# สร้างไฟล์เก็บประวัติ tool ของโปรเจกต์
dotnet new tool-manifest

# ติดตั้ง dotnet-ef ลงในโปรเจกต์
dotnet tool install dotnet-ef
```

### 3. รันคำสั่ง Scaffold (ดึงฐานข้อมูลมาเป็น Model)
เมื่อเตรียมทุกอย่างพร้อมแล้ว ให้ใช้คำสั่งดึงโครงสร้างตารางจากฐานข้อมูลมาสร้างเป็น Model และ DbContext ในโฟลเดอร์ `Models`:

```bash
dotnet tool run dotnet-ef dbcontext scaffold "Server=localhost;port=5432;Database=Stock_Management;User Id=sa;Password=P@ssP@STGRESQLw0rd;" Npgsql.EntityFrameworkCore.PostgreSQL -o Models -c DbContexts --force
``````
dotnet ef dbcontext scaffold "Server=localhost;port=5432;Database=Stock_Management;User Id=postgres;Password=P@ssP@STGRESQLw0rd;" Npgsql.EntityFrameworkCore.PostgreSQL -o Models -c DbContexts --force
```

**คำอธิบายคำสั่ง:**
- `dbcontext scaffold` : คำสั่งสำหรับทำ Reverse Engineering
- `"Server=localhost;..."` : Connection String ของฐานข้อมูลเป้าหมาย
- `Npgsql.EntityFrameworkCore.PostgreSQL` : Database Provider ที่จะใช้ (เราใช้ Postgres)
- `-o Models` : กำหนดโฟลเดอร์ปลายทางที่ใช้เก็บ Model คลาสที่สร้างขึ้นมา (จะสร้างโฟลเดอร์ให้อัตโนมัติถ้าไม่มี)
- `-c ApplicationDbContext` : ตั้งชื่อคลาสสำหรับจัดการ Context (ค่าเริ่มต้นจะเป็นชื่อตามฐานข้อมูล)
- `--force` : บังคับให้เขียนทับไฟล์เก่าหากมีไฟล์ Model เดิมอยู่แล้ว

### 4. การนำไปใช้งานใน Program.cs
หลังจากได้ไฟล์ Model และ DbContext แล้ว คุณสามารถนำ `ApplicationDbContext` ไปใช้ใน `Program.cs` เพื่อเชื่อมต่อกับ Database ในโปรเจกต์ได้ โดยเพิ่มคำสั่งนี้ก่อน `builder.Build()`:

```csharp
using Microsoft.EntityFrameworkCore;
using stock_api.Models; // เปลี่ยน namespace ให้ตรงกับโปรเจกต์ของคุณ

// เชื่อมต่อฐานข้อมูล
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```
*อย่าลืมตรวจสอบว่าใน `appsettings.Development.json` มี ConnectionString ที่ชื่อ `DefaultConnection` ถูกต้องแล้ว*

