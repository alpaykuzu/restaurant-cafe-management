import React from "react";
import { Outlet } from "react-router-dom";
import Sidebar from "./Sidebar";
import "./../styles/Layout.css";

const Layout: React.FC = () => {
  return (
    <div className="res-layout-container">
      <Sidebar />
      <main className="res-layout-main">
        <Outlet />
      </main>
    </div>
  );
};

export default Layout;
