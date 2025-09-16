import React, { useState } from "react";
import { Link, useLocation } from "react-router-dom";
import { useAuth } from "../context/AuthProvider";
import "./../styles/Sidebar.css";
import {
  MdAdminPanelSettings,
  MdSupervisorAccount,
  MdPointOfSale,
  MdFastfood,
  MdKitchen,
  MdPeople,
  MdTableRestaurant,
  MdMenuBook,
  MdRestaurant,
  MdReceipt,
  MdPayments,
  MdBarChart,
  MdWork,
  MdMenu,
  MdClose,
} from "react-icons/md";

const Sidebar: React.FC = () => {
  const { roles, logout } = useAuth();
  const location = useLocation();
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  const getRoleIcon = (roleLabel: string) => {
    switch (roleLabel) {
      case "Admin Paneli":
        return <MdAdminPanelSettings />;
      case "Manager Paneli":
        return <MdSupervisorAccount />;
      case "Kasiyer Paneli":
        return <MdPointOfSale />;
      case "Garson Paneli":
        return <MdFastfood />;
      case "Mutfak Paneli":
        return <MdKitchen />;
      default:
        return null;
    }
  };

  const getChildIcon = (childLabel: string) => {
    switch (childLabel) {
      case "Kullanıcı Yönetimi":
        return <MdPeople />;
      case "Restoran Yönetimi":
        return <MdRestaurant />;
      case "Çalışan Yönetimi":
        return <MdWork />;
      case "Masa Yönetimi":
        return <MdTableRestaurant />;
      case "Menü Yönetimi":
        return <MdMenuBook />;
      case "Sipariş Yönetimi":
      case "Siparişler":
        return <MdFastfood />;
      case "Fatura Yönetimi":
        return <MdReceipt />;
      case "Ödeme Yönetimi":
        return <MdPayments />;
      case "Satış Raporu Yönetimi":
        return <MdBarChart />;
      case "Masa ve Sipariş":
        return <MdTableRestaurant />;
      default:
        return null;
    }
  };

  const menuItems = [
    {
      label: "Admin Paneli",
      roles: ["Admin"],
      children: [
        { label: "Kullanıcı Yönetimi", path: "/admin/users" },
        { label: "Restoran Yönetimi", path: "/admin/restaurants" },
      ],
    },
    {
      label: "Manager Paneli",
      roles: ["Manager"],
      children: [
        { label: "Çalışan Yönetimi", path: "/manager/employees" },
        { label: "Masa Yönetimi", path: "/manager/tables" },
        { label: "Menü Yönetimi", path: "/manager/menuItems" },
        { label: "Sipariş Yönetimi", path: "/manager/orders" },
        { label: "Fatura Yönetimi", path: "/manager/invoices" },
        { label: "Ödeme Yönetimi", path: "/manager/payments" },
        { label: "Satış Raporu Yönetimi", path: "/manager/salesReport" },
      ],
    },
    {
      label: "Kasiyer Paneli",
      roles: ["Cashier"],
      children: [
        { label: "Masa ve Sipariş", path: "/employee/tableDashboard" },
      ],
    },
    {
      label: "Garson Paneli",
      roles: ["Waiter"],
      children: [
        { label: "Masa ve Sipariş", path: "/employee/tableDashboard" },
        { label: "Siparişler", path: "/employee/orderDashboard" },
      ],
    },
    {
      label: "Mutfak Paneli",
      roles: ["Kitchen"],
      children: [{ label: "Siparişler", path: "/employee/orderDashboard" }],
    },
  ];

  return (
    <>
      <button
        className="mobile-menu-toggle"
        onClick={() => setIsMenuOpen(!isMenuOpen)}
      >
        {isMenuOpen ? <MdClose /> : <MdMenu />}
      </button>

      <div className={`res-sidebar-container ${isMenuOpen ? "is-open" : ""}`}>
        <div className="res-sidebar-header">
          <h2 className="res-sidebar-title">Restoran Yönetimi</h2>
        </div>
        <nav className="res-sidebar-nav">
          <ul className="res-sidebar-menu">
            {menuItems
              .filter((item) => item.roles.some((role) => roles.includes(role)))
              .map((item) => (
                <li key={item.label} className="res-menu-group">
                  <div className="res-menu-group-header">
                    {getRoleIcon(item.label)}
                    <span>{item.label}</span>
                  </div>
                  <ul className="res-submenu">
                    {item.children.map((child) => (
                      <li key={child.path} className="res-submenu-item">
                        <Link
                          to={child.path}
                          className={`res-submenu-link ${
                            location.pathname === child.path ? "is-active" : ""
                          }`}
                          onClick={() => setIsMenuOpen(false)}
                        >
                          {getChildIcon(child.label)}
                          <span>{child.label}</span>
                        </Link>
                      </li>
                    ))}
                  </ul>
                </li>
              ))}

            {/* Çıkış Yap Butonu */}
            <li className="res-menu-group">
              <button
                className="res-submenu-link"
                onClick={() => {
                  setIsMenuOpen(false);
                  logout();
                }}
                style={{
                  width: "100%",
                  textAlign: "left",
                  background: "none",
                  border: "none",
                  cursor: "pointer",
                  display: "flex",
                  alignItems: "center",
                  gap: "12px",
                  padding: "10px 16px",
                }}
              >
                <MdClose /> <span>Çıkış Yap</span>
              </button>
            </li>
          </ul>
        </nav>
      </div>
    </>
  );
};

export default Sidebar;
