Product Inventory Management Monorepo

This repository contains both the API (ASP.NET Core Web API) and the UI (React) for the Product Inventory System with Stock Management.

🗂️ Project Overview
A robust, full-stack web application for managing products with variants (like size, color), stock levels, users, and activity logging. Designed for clarity and maintainability, everything you need—API, UI, and DB scripts—is included and ready to run.

Features include:

Secure user registration/login with token-based (JWT) authentication and authorization

Flexible product creation, including variants and variant options (e.g. "size: M, color: Red")

Stock management at the variant-combination level,

Automatic generation of variant combinations

Clean, modern React UI (with Zustand state and Axios API calls)

All key API calls and errors are logged to the database

Complete database table scripts included (api/DB/)

Pagination for product listing

🚀 Quick Start
1️⃣ API (.NET Core Backend)
Setup the database:

Run the SQL scripts in api/DB/ to create all required tables:

Users, Products, Variants, VariantOptions, ProductVariantCombinations, ProductVariantCombinationOptions, Logs, etc.

Start the API:

text
cd api
dotnet restore
dotnet build
dotnet run
API runs at https://ocalhost:7146 (see launchSettings.json for actual port).

Configuration:

Set your DB details in api/appsettings.json as needed.

API Documentation:

Test all authorized endpoints (and view models) via Swagger at /swagger.

Token required: Register and log in (using Swagger or UI) to obtain a JWT for protected endpoints.

2️⃣ UI (React Frontend)
Install dependencies and start:

text
cd ui
npm install
npm start
UI is available at http://localhost:5173 by default.

API URL:

If your backend runs on a non-default location, set REACT_APP_API_URL in ui/.env.

🛡️ Authentication & Authorization
All sensitive API endpoints require a valid JWT passed as a Bearer token.

To test: register a user via UI/login page or API /register route, then use those credentials to log in and get your token.

The UI handles JWT automatically in requests (via Axios).

📃 Table Scripts
ALL required SQL table scripts are under api/DB/—run these before using the app/API.

Some sample/test data can be added manually or via UI registration.

🔍 Logging
All significant actions (user register/log in, product/stock changes, errors) are logged in the Logs table.

Log entries include a unique Guid, DateTime, and the LogMessage.

🧩 Technology Stack
Backend:

ASP.NET Core Web API

Dapper (SQL access)

JWT Authentication/Authorization

Logging (custom service, logs to DB)

Frontend:

React, Axios

Zustand (global state management)

Minimal, fast UI

🗒️ User/Flow Demo
Register a new user:

Use UI login page or API /register to create a user.

Log in:

Gets a JWT token.

Manage Products:

Create products with variants (size/color/etc.), images, and full details.

Stock Operations:

Add or remove stock for any variant combination.

All actions require authentication, and are logged automatically.

