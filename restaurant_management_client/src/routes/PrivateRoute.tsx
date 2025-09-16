import React from "react";
import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../context/AuthProvider";

interface PrivateRouteProps {
  allowedRoles: string[];
}

const PrivateRoute: React.FC<PrivateRouteProps> = ({ allowedRoles }) => {
  const { accessToken, roles, loading } = useAuth();

  if (loading) return <div>Loading...</div>; // me() bitene kadar bekle

  if (!accessToken) {
    return <Navigate to="/login" replace />;
  }

  const hasPermission = roles.some((role) => allowedRoles.includes(role));
  if (!hasPermission) {
    return <Navigate to="/login" replace />; // yetkisiz
  }

  return <Outlet />;
};

export default PrivateRoute;
