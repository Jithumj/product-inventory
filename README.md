# Product Inventory Monorepo

This repository contains both the **API** (ASP.NET Core Web API) and the **UI** (React) for the Product Inventory System.

---

## About the Project

The **Product Inventory System** is a full-stack web application designed to help online businesses manage their products, variations (such as size and color), and real-time stock levels efficiently. 

### Key Features

- **Centralized Product Catalog:** Manage products with images, codes, HSN, and essential details.
- **Variants & Sub-Variants:** Assign attributes like size, color, or custom options; generate all valid combinations automatically.
- **Stock Management:** Purchase (add) and sell (remove) inventory at the variant-combination level.
- **User Management:** Secure user registration and authentication for system access.
- **Clear RESTful API:** Robust, well-documented backend suitable for frontend/UI, integrations, or reporting.
- **Modern UI:** Fast, user-friendly React app for admins or operators.
- **Pagination & Searching:** Supports efficient product listing with filtering options (scalable to thousands of products).

This monorepo pattern keeps backend and frontend tightly integrated for ease of development and deployment.
---

## Getting Started

### 1. API (.NET Backend)

1. Open a terminal and navigate to the `api` folder:
    ```
    cd api
    ```
2. Restore packages and build the solution:
    ```
    dotnet restore
    dotnet build
    ```
3. Apply any necessary database migrations (if using EF/Dapper tools).
4. Run the API:
    ```
    dotnet run
    ```
    The API will typically run at `https://localhost:5001` or similar.

---

### 2. UI (React Frontend)

1. Open another terminal and navigate to the `ui` folder:
    ```
    cd ui
    ```
2. Install dependencies:
    ```
    npm install
    ```
3. Start the development server:
    ```
    npm start
    ```
    The UI will typically be available at `http://localhost:3000`.

---

## Configuration

- **API:** Update connection strings and other settings in `api/appsettings.json`.
- **UI:** If your API base URL is not the default, set `REACT_APP_API_URL` in `ui/.env` or where used.

---

## Notes

- The backend (API) and frontend (UI) code live in separate folders for clarity.
- Pull requests and branches should be made from the repo root.
- You may add a `docs/` folder or other top-level directories for shared assets as needed.

---

