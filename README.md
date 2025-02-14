# # E-Commerce System for a Welding Store (Bachelor thesois)

## 📌 Project Overview
This project is a **web-based e-commerce platform** developed using **ASP.NET Core**. The application allows users to browse, search, and purchase welding equipment online. 
It includes an authentication and authorization system, shopping cart functionality, and payment processing integration using Stripe API**.

## 🚀 Features
- 🔐 **User Authentication & Authorization** (Registration, Login, Role-based Access)
- 🛍️ **Product Management** (Add, Edit, Delete, View Products)
- 🔍 **Product Search & Filtering**
- 🛒 **Shopping Cart & Checkout Process**
- 💳 **Stripe API Integration for Payments**
- 📜 **API Documentation with Swagger**

## 🛠️ Technologies Used
- **Backend:** ASP.NET Core 6.0
- **Frontend:** Blazor WebAssembly
- **Database:** SQL Server + Entity Framework Core
- **Payment Processing:** Stripe API
- **Security:** ASP.NET Identity for Authentication & Authorization
- **API Documentation:** Swagger & Postman

## 📁 Project Structure
```
ECommerceWeldingStore/
│── Server/          # Backend API (ASP.NET Core)
│── Client/       # Frontend (Blazor WASM)
│── SharedLibrary/         # Entity Framework Models & Migrations
│── Server.Repository/         # Business Logic & Services
│── README.md               # Project Documentation
```

## 🏗️ Installation & Setup
1. **Clone the repository**
   ```sh
   git clone https://github.com/yourusername/DiplomskiRad.git
   cd DiplomskiRad
   ```
2. **Setup the database** (SQL Server required)
   - Update `appsettings.json` with your database connection string
   - Run migrations
     ```sh
     dotnet ef database update
     ```
3. **Run the API**
   ```sh
   cd Server
   dotnet run
   ```
4. **Run the Client**
   ```sh
   cd Client
   dotnet run
   ```
5. **Access the application** in the browser:
   - API: `https://localhost:7151/swagger`
   - Frontend: `https://localhost:7193`


## 📞 Contact
For any questions or support, feel free to reach out:
- 🔗 GitHub: [github.com/devhawkz](https://github.com/devhawkz)
- 📫 How to reach me **pavlejovanovic34@gmail.com** 
- 📫 How to reach me **skydevhawkz@gmail.com**

<h3 align="left">Connect with me:</h3>
<p align="left">
<a href="https://linkedin.com/in/pavlejovanovic34" target="blank"><img align="center" src="https://raw.githubusercontent.com/rahuldkjain/github-profile-readme-generator/master/src/images/icons/Social/linked-in-alt.svg" alt="pavlejovanovic34" height="30" width="40" /></a>
</p>
