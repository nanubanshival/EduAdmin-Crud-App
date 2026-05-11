# EduAdmin — School Management System

A full-stack **ASP.NET Core MVC (.NET 8)** application for managing a school or institute: students, teachers, courses, attendance, and leave workflows — all behind a role-based login (Admin / Teacher / Student).

The UI is built on a custom **Slate Academy** design system (deep navy + warm gold, Fraunces + DM Sans typography) — no Bootstrap, no UI framework.

---

## Features

### Core modules
- **Students** — full CRUD with semester/course/fees auto-fill via AJAX
- **Teachers** — directory, edit profile, active/inactive status
- **Courses** — catalog with semester + fees, teacher-course mapping
- **Attendance** — bulk daily attendance with Present / Absent / Half-Day toggles
- **Leave** — leave types, leave applications, leave balances with approval status

### Auth & access control
- ASP.NET Core Identity (`Microsoft.AspNetCore.Identity.EntityFrameworkCore`) wired on a custom `Users : IdentityUser`
- Session-based login with role-aware navigation (Admin / Teacher / Student see different actions)
- Register, login, change password, and "verify email" recovery flow
- 30-minute idle timeout, HttpOnly session cookies

### UI / design system
- Custom CSS design tokens (no Bootstrap) — see [wwwroot/css/site.css](wwwroot/css/site.css)
- Component classes: `edu-card`, `edu-table`, `btn-edu-primary`, `badge-edu`, `edu-input`, `edu-details-grid`, …
- Bootstrap Icons via CDN for iconography
- Responsive down to 375 px

---

## Tech stack

| Layer            | Technology                                                |
| ---------------- | --------------------------------------------------------- |
| Framework        | ASP.NET Core 8.0 MVC                                      |
| Language         | C# 12 (nullable + implicit usings enabled)                |
| ORM              | Entity Framework Core 9.0 (SQL Server provider)           |
| Database         | SQL Server (LocalDB / SQLEXPRESS)                         |
| Auth             | ASP.NET Core Identity 8.0                                 |
| Frontend         | Razor views + custom CSS + jQuery (AJAX for cascading dropdowns) |
| Fonts            | Fraunces, DM Sans (Google Fonts)                          |
| Icons            | Bootstrap Icons 1.11 (CDN)                                |

---

## Project structure

```
CrudProject/
├── Controllers/           # MVC controllers (Account, Students, Teachers, Courses, Attendance, Leave, Home)
├── Models/                # EF entities: Student, Teachers, CourseManagement, Attendance, LeaveApplications, LeaveBalances, LeaveTypeMaster, Users
├── ViewModel/             # Login, Register, ChangePassword, VerifyEmail
├── Data/
│   └── ApplicationDbContext.cs   # IdentityDbContext<Users> + DbSets
├── Migrations/            # EF Core migrations
├── Views/
│   ├── Shared/            # _Layout, _AccountLayout, Error, partials
│   ├── Account/           # Login, Register, ChangePassword, VerifyEmail
│   ├── Home/              # Index (dashboard), Privacy
│   ├── Students/          # Index, Create, Edit, Details, Delete
│   ├── Teachers/          # + MapCourses
│   ├── CourseManagement/
│   ├── Attendance/        # BulkAttendance
│   ├── Leave/             # LeaveTypes, LeaveApplications, LeaveBalances, + CRUD for types
│   └── LeaveApplications/
├── wwwroot/
│   ├── css/site.css       # Slate Academy design system
│   ├── js/site.js
│   └── lib/               # jQuery, jQuery Validation
├── Program.cs             # DI, Identity, session, EF setup
├── appsettings.json       # Connection string (not committed — see Setup)
└── CrudProject.csproj
```

---

## Getting started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, SQL Server Express, or full SQL Server)
- (Optional) [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code

### 1. Clone

```bash
git clone https://github.com/<your-username>/CrudProject.git
cd CrudProject
```

### 2. Configure the database connection

Create or edit `appsettings.json` in the project root:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=CrudProjectDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "AllowedHosts": "*"
}
```

> Adjust `Server=...` to point at your SQL Server instance (e.g. `(localdb)\\MSSQLLocalDB`).

### 3. Apply migrations

```bash
dotnet tool install --global dotnet-ef     # one-time, if not installed
dotnet ef database update
```

### 4. Run

```bash
dotnet run
```

App will start at `https://localhost:5001` (or the port shown in the console).

### 5. First-time setup

1. Go to `/Account/Register` and create an account with role **Admin**.
2. Sign in — the dashboard, navbar, and module access unlock based on role.

---

## Role permissions

| Module             | Admin | Teacher | Student |
| ------------------ | :---: | :-----: | :-----: |
| Students — view    |  ✅   |   ✅    |   ✅    |
| Students — edit    |  ✅   |   ✅    |   —     |
| Students — delete  |  ✅   |   —     |   —     |
| Teachers — view    |  ✅   |   ✅    |   —     |
| Teachers — edit self|  ✅  |   ✅    |   —     |
| Teachers — delete  |  ✅   |   —     |   —     |
| Courses — manage   |  ✅   |   —     |   —     |
| Attendance         |  ✅   |   —     |   —     |
| Leave — apply      |  ✅   |   ✅    |   —     |
| Leave — types      |  ✅   |   —     |   —     |

---

## Screenshots

<img width="1918" height="885" alt="e1" src="https://github.com/user-attachments/assets/dbe20c3f-81f7-4784-a6d6-c954bca489a1" />
<img width="1917" height="875" alt="e2" src="https://github.com/user-attachments/assets/d7d1823e-3cb8-4ced-bab0-7fb75ab17c19" />
<img width="1901" height="882" alt="e3" src="https://github.com/user-attachments/assets/5a325d9d-35c8-4917-a87e-453a65475a77" />
<img width="1920" height="882" alt="e4" src="https://github.com/user-attachments/assets/6386177d-96bc-46d6-bcc3-c7f08e271da9" />
<img width="1900" height="881" alt="e5" src="https://github.com/user-attachments/assets/2f8d2301-b324-4ed0-83bd-0b7d2cf49dd8" />
<img width="1905" height="885" alt="e6" src="https://github.com/user-attachments/assets/42aadae0-4a8d-41b3-b24c-6a2301625c47" />
<img width="1902" height="880" alt="eS" src="https://github.com/user-attachments/assets/4fb975ca-27ea-4d91-ba0d-adb9df199cc2" />
<img width="1891" height="880" alt="e8" src="https://github.com/user-attachments/assets/a1ebf14b-5b4b-49bf-ac8b-ecbf6b5478ac" />

---

## Build & test

```bash
dotnet build              # compile (should produce 0 errors)
dotnet ef migrations list # confirm migration state
dotnet run                # start the dev server
```

---




## Notes & known issues

- `Controllers/AccountController.cs` currently Base64-encodes passwords for the legacy `StudentsUsers` table — this is **not secure** and should be migrated to ASP.NET Core Identity's password hasher (Identity is already wired up; the legacy path bypasses it).
- `Models/Users - Copy.cs` is a leftover and can be deleted.
- The seed block in `Program.cs` is commented out — uncomment to seed two demo courses on first run.

---

## License

This project is for educational / demo purposes. Feel free to fork and adapt.
