import { Routes, Route, Navigate, BrowserRouter } from "react-router-dom";
import { AuthProvider } from "./context/AuthProvider";
import Layout from "./components/Layout";
import LoginPage from "./pages/LoginPage";
import PrivateRoute from "./routes/PrivateRoute";
import UserManagement from "./pages/UserManagement";
import RestaurantManagement from "./pages/RestaurantManagement";
import EmployeeManagement from "./pages/EmployeeManagement";
import TableManagement from "./pages/TableManagement";
import MenuManagement from "./pages/MenuManagement";
import OrderManagement from "./pages/OrderManagement";
import InvoiceManagement from "./pages/InvoiceManagement";
import PaymentManagement from "./pages/PaymentManagement";
import SalesReportManagement from "./pages/SalesReportManagement";
import TableDashboard from "./pages/TableDashboard";
import OrderDashboard from "./pages/OrderDashboard";

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />

          {/* Admin paneli */}
          <Route element={<PrivateRoute allowedRoles={["Admin"]} />}>
            <Route element={<Layout />}>
              <Route path="/admin/users" element={<UserManagement />} />
            </Route>
            <Route element={<Layout />}>
              <Route
                path="/admin/restaurants"
                element={<RestaurantManagement />}
              />
            </Route>
          </Route>

          {/* Manager paneli */}
          <Route element={<PrivateRoute allowedRoles={["Manager"]} />}>
            <Route element={<Layout />}>
              <Route
                path="/manager/employees"
                element={<EmployeeManagement />}
              />
              <Route path="/manager/tables" element={<TableManagement />} />
              <Route path="/manager/menuItems" element={<MenuManagement />} />
              <Route path="/manager/orders" element={<OrderManagement />} />
              <Route path="/manager/invoices" element={<InvoiceManagement />} />
              <Route path="/manager/payments" element={<PaymentManagement />} />
              <Route
                path="/manager/salesReport"
                element={<SalesReportManagement />}
              />
            </Route>
          </Route>

          {/* Employee paneli */}
          <Route
            element={
              <PrivateRoute allowedRoles={["Waiter", "Cashier", "Kitchen"]} />
            }
          >
            <Route element={<Layout />}>
              <Route
                path="/employee/tableDashboard"
                element={<TableDashboard />}
              />
              <Route
                path="/employee/orderDashboard"
                element={<OrderDashboard />}
              />
            </Route>
          </Route>

          <Route path="/" element={<Navigate to="/login" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
