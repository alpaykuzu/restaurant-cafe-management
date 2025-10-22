# 🍽️ Restaurant/Cafe Management System 

A full-stack web application designed to streamline and automate daily restaurant operations. The system provides a robust role-based access control (RBAC) interface, enabling different staff members—from administrators to kitchen staff—to manage specific tasks efficiently.

---

## ✨ Features

### Frontend

* **Role-Based Access Control (RBAC):** Users only see features relevant to their role (Admin, Manager, Waiter, Cashier, Kitchen).
* **User & Restaurant Management:** Admin dashboards for user accounts and restaurant details.
* **Employee Management:** Managers can add, update, and remove employees with real-time updates via SignalR.
* **Table & Menu Management:** Manage tables and menu items (CRUD).
* **Order Management:** Full order lifecycle (waiter → kitchen → cashier).
* **Financial Modules:** Handle invoices, payments, and generate sales reports.
* **Real-Time Dashboards:** Live updates for tables, orders, and kitchen tasks.
* **Secure Authentication:** Token-based authentication with refresh handling.
* **PDF Report Generation:** Export reports with `jspdf` and `jspdf-autotable`.
* **Data Visualization:** Interactive sales charts with Chart.js.
* **Responsive Design:** Optimized UI for both desktop and mobile.

### Backend

* **Authentication & Authorization:** JWT authentication with refresh tokens. RBAC across all user roles.
* **Restaurant Management:** Full CRUD for restaurants.
* **Employee Management:** Manage employee roles and profiles.
* **Menu & Category Management:** CRUD operations with role-based permissions.
* **Table Management:** Manage tables and update statuses (Available, Occupied, Cleaning).
* **Order & Kitchen Management:** Track orders through all statuses (Pending, Preparing, Ready, Served).
* **Payment & Invoicing:** Cashiers can handle payments and invoices.
* **Sales Reporting:** Managers generate detailed reports by date range.
* **Real-Time Communication:** SignalR hub for instant updates (orders, tables).

---

## 💻 Technologies Used

### Frontend

* **React** (with TypeScript)
* **React Router DOM** – Routing
* **@microsoft/signalr** – Real-time communication
* **Chart.js & react-chartjs-2** – Charts & analytics
* **js-cookie** – Cookie/session handling
* **jspdf & jspdf-autotable** – PDF generation
* **React Icons & lucide-react** – Icons
* **React Select** – Advanced select component

### Backend

* **ASP.NET Core 8.0** – Web API
* **Entity Framework Core** – ORM
* **SQL Server** – Database
* **JWT Bearer Authentication** – Secure tokens
* **Microsoft SignalR** – Real-time updates
* **AutoMapper** – DTO mapping
* **FluentValidation** – Request validation

---

## ⚙️ Installation & Setup

### Prerequisites

* Node.js (LTS recommended)
* npm or yarn
* .NET 8.0 SDK
* SQL Server

### Frontend Setup

```bash
git clone https://github.com/alpaykuzu/restaurant-cafe-management
cd restaurant-management-client

# Install dependencies
npm install
# or
yarn install

# Run development server
npm run dev
# or
yarn dev
```

The frontend runs at: [http://localhost:5173](http://localhost:5173)

> Ensure `AuthService` and `SignalR` hub URLs point to your backend API (default: `http://localhost:5164/hubs`).

### Backend Setup

```bash
git clone https://github.com/alpaykuzu/restaurant-cafe-management
cd restaurant-management-api

# Update connection string in appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=RestaurantDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
}

# Run migrations
dotnet ef database update

# Start API
dotnet run
```

The backend runs at: [https://localhost:5164](https://localhost:5164)

---

## 🗺️ Project Structure

### Frontend (`restaurant-management-client`)

```
src/
├── components/     # Reusable UI components (Layout, Sidebar, etc.)
├── context/        # Context providers (e.g., AuthProvider)
├── pages/          # Page-level components (dashboards, forms)
├── routes/         # Routing & guards (PrivateRoute)
├── services/       # API services (AuthService, EmployeeService)
├── styles/         # Global & scoped CSS
├── types/          # TypeScript models & types
└── App.tsx         # Root component & router
```

### Backend (`restaurant-management-api`)

```
API/
 ├── Controllers
 ├── Filters
 ├── Middleware

Core/
 ├── Dtos
 ├── Extensions
 ├── Hubs
 ├── Interfaces
 ├── Models
 ├── Responses
 └── Validators

Data/
 ├── Configurations
 ├── Migration
 ├── Repositories
 └── UnitOfWork

Service/
 └── Services
```

---

## 📖 API Documentation

Once backend is running, Swagger is available at:
👉 [https://localhost:5164/swagger/index.html](https://localhost:5164/swagger/index.html)

### Example Endpoints

* **Auth**

  * `POST /api/Auth/register` – Register new user
  * `POST /api/Auth/login` – Login & get JWT
  * `POST /api/Auth/refresh-token` – Refresh token
* **Restaurant**

  * `POST /api/Restaurant/create-restaurant` – Create restaurant (Admin)
  * `GET /api/Restaurant/get-all-restaurants` – Get all (Admin)
* **Order**

  * `POST /api/Order/create-order` – Create order
  * `PUT /api/Order/update-order-status/{orderId}/{newStatus}` – Update status
* **Table**

  * `POST /api/Table/create-table` – Create table
  * `PUT /api/Table/update-table-status` – Change table status
* **Payment & Invoice**

  * `POST /api/Payment/make-payment` – Process payment
  * `POST /api/Invoice/create-invoice/{orderId}` – Generate invoice

---

## 🔌 Real-Time Communication

* SignalR Hub: `https://localhost:5164/hubs`
* Clients should call: `JoinRestaurantGroup(int restaurantId)` to subscribe to events.
* Events include:

  * `OrderCreated`
  * `TableStatusUpdated`

---

## 👤 Default Admin Account

The system auto-generates a default admin on first run:

```
Email: admin@admin.com
Password: 123456
```

---

## 📊 Reports & Visualization

* Managers can generate sales reports via API.
* Reports can be exported to PDF or viewed with dynamic charts.

---

## 📜 License

This project is licensed under the MIT License.
