import React, { createContext, useContext, useEffect, useState } from "react";
import Cookies from "js-cookie";
import { AuthService } from "../services/AuthService";
import type { LoginResponse } from "../types/Auth/LoginResponse";

interface AuthContextType {
  accessToken: string | null;
  refreshToken: string | null;
  id: number | null;
  roles: string[];
  login: (data: LoginResponse) => void;
  logout: () => void;
  loading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [accessToken, setAccessToken] = useState(Cookies.get("accessToken"));
  const [refreshToken, setRefreshToken] = useState(Cookies.get("refreshToken"));
  const [id, setId] = useState<number | null>(null);
  const [roles, setRoles] = useState<string[]>([]);
  const [loading, setLoading] = useState(true);

  const login = (data: LoginResponse) => {
    setAccessToken(data.accessToken.accessToken);
    setRefreshToken(data.refreshToken.refreshToken);
    setId(data.userInfo.id);
    setRoles(data.userInfo.roles);

    const accessExp = new Date(data.accessToken.accessTokenExpTime);
    const refreshExp = new Date(data.refreshToken.refreshTokenExpTime);

    Cookies.set("accessToken", data.accessToken.accessToken, {
      expires: accessExp,
    });
    Cookies.set("refreshToken", data.refreshToken.refreshToken, {
      expires: refreshExp,
    });
  };

  const logout = () => {
    setAccessToken("");
    setRefreshToken("");
    setRoles([]);
    setId(null);
    Object.keys(Cookies.get()).forEach((cookie) => Cookies.remove(cookie));
    window.location.href = "/";
  };

  // Sayfa yenilenince me() çağrısı
  useEffect(() => {
    const initAuth = async () => {
      const access = Cookies.get("accessToken");
      if (!access) {
        setLoading(false);
        return;
      }

      try {
        const response = await AuthService.me();
        if (response.success && response.data) {
          setId(response.data.id);
          setRoles(response.data.roles);
        } else {
          logout();
        }
      } catch {
        logout();
      } finally {
        setLoading(false);
      }
    };

    initAuth();
  }, []);

  // Refresh token interval
  useEffect(() => {
    const id = setInterval(async () => {
      const access = Cookies.get("accessToken");
      const refresh = Cookies.get("refreshToken");
      const currentPath = window.location.pathname;
      if (!refresh) {
        if (currentPath !== "/login") logout();
        return;
      }
      if (!access && currentPath === "/login") return;
      if (!access) {
        try {
          const response = await AuthService.refreshToken({
            refreshToken: refresh,
          });
          const data = response.data;
          setAccessToken(data.accessToken.accessToken);
          setRefreshToken(data.refreshToken.refreshToken);
          setRoles(data.roles);
          const accessExp = new Date(data.accessToken.accessTokenExpTime);
          const refreshExp = new Date(data.refreshToken.refreshTokenExpTime);
          Cookies.set("accessToken", data.accessToken.accessToken, {
            expires: accessExp,
          });
          Cookies.set("refreshToken", data.refreshToken.refreshToken, {
            expires: refreshExp,
          });
        } catch {
          logout();
        }
      }
    }, 2000);
    return () => clearInterval(id);
  }, []);

  if (loading) return <div>Loading...</div>;

  return (
    <AuthContext.Provider
      value={{
        accessToken: accessToken ?? null,
        refreshToken: refreshToken ?? null,
        id: id ?? null,
        roles,
        login,
        logout,
        loading,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

// eslint-disable-next-line react-refresh/only-export-components
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within an AuthProvider");
  return context;
};
