🏦 Banking Website API
A secure, scalable, and feature-rich backend API for managing banking operations including accounts, transactions, payments, and user settings. Built with ASP.NET Core, Onion Architecture, JWT Authentication, 2FA, and modern security standards.

🚀 Features

✅ JWT Authentication & Role Management (Admin, User)✅ Two-Factor Authentication (2FA) Support✅ Clean Onion Architecture (Domain, Application, Infrastructure, Persistence, API)✅ Repository Pattern with Read/Write Separation✅ Full CRUD for Accounts, Transactions, and Payments✅ Audit Log System with IP Address, Browser Info, and Endpoint Tracking✅ Suspicious Transaction Detection (AI Rules)✅ AES-256 Encryption for Sensitive Data (e.g., Transaction Notes)✅ FluentValidation for Robust Input Validation✅ Rate Limiting for DDoS Prevention✅ Swagger UI with JWT Authorization Support✅ AutoMapper for DTO Mapping✅ Seed Data for Roles and Admin User

📁 Project Structure

BankingWebsite/
├── API/               -> ASP.NET Core Web API (Controllers, Middleware, Swagger)
├── Application/       -> DTOs, Interfaces, Validators, Services, Utilities
├── Domain/            -> Entity Models and Enums
├── Infrastructure/    -> JWT, AES, External Integrations
├── Persistence/       -> EF Core, DbContext, Migrations, Repositories
└── README.md          -> Project Documentation

⚙️ Getting Started

🔧 1. Clone the Repository

git clone https://github.com/JaFidAn/BankingWebsite.git
cd API

🛠️ 2. Configure appsettings.json

"ConnectionStrings": {
  "DefaultConnection": "Your SQL Server connection string"
},
"JwtSettings": {
  "Key": "YourSuperSecretKey",
  "Issuer": "BankingAuthAPI",
  "Audience": "BankingUser",
  "DurationInMinutes": 60
}

🛋️ 3. Apply Migrations & Seed Data

dotnet ef database update --project Persistence --startup-project API

▶️ 4. Run the Application

dotnet run --project API

🌐 5. Open Swagger UIhttps://localhost:5001/swagger

🔐 Authentication & Roles

🔸 JWT Bearer Authentication with Refresh Token Support🔸 Two-Factor Authentication (SMS or Email)🔸 Role-based access control for Admin/User🔸 Default Admin Credentials:

{
  "email": "r.alagezov@gmail.com",
  "password": "R@sim1984"
}

💰 Accounts API

POST /api/accounts

GET /api/accounts

GET /api/accounts/{id}

PUT /api/accounts/{id}

DELETE /api/accounts/{id}

💸 Transactions API

POST /api/transactions (Transfer, Deposit, Withdraw)

GET /api/transactions/user

GET /api/transactions/account/{id}

GET /api/transactions/{id}

GET /api/transactions/suspicious (Admin only)

📄 Audit Logs API

GET /api/auditlogs (Admin only)

GET /api/auditlogs/{id}Logs include: UserId, TableName, Action, IPAddress, BrowserInfo, Endpoint, Timestamps

💳 Payments API

POST /api/payments (Simulated or Stripe-based)

🧾 Settings API

PUT /api/users/change-password

PUT /api/users/update-profile

POST /api/users/enable-2fa

📦 Security Features

AES-256 Encryption for sensitive data like notes

Rate Limiting to prevent abuse

Suspicious Transaction detection with rule-based AI

Audit trail of all changes with full traceability

📜 License
This project is open-source and free to use for educational or commercial purposes. Licensed under the MIT License.

👨‍💻 Author
Created with ❤️ by Rasim Alagezov📧 Email: r.alagezov@gmail.com💻 GitHub: JaFidAn🔗 LinkedIn: rasim-alagezov

✨ Have questions or want to collaborate? Feel free to reach out!
