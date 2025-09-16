/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable react-hooks/exhaustive-deps */
import { useEffect, useState } from "react";
import "./../styles/UserManagement.css";
import { UserService } from "../services/UserService";
import { RoleService } from "../services/RoleService";
import type { UserResponse } from "../types/User/UserResponse";
import {
  FaPlus,
  FaTrashAlt,
  FaSpinner,
  FaCheck,
  FaInfoCircle,
} from "react-icons/fa";

const AVAILABLE_ROLES = [
  "User",
  "Waiter",
  "Kitchen",
  "Cashier",
  "Manager",
  "Admin",
];

const StatusMessage = ({
  message,
  type,
}: {
  message: string | null;
  type: "error" | "info" | "success" | null;
}) => {
  if (!message) return null;
  return (
    <div className={`status-message ${type}`}>
      {type === "error" && <FaInfoCircle />}
      {type === "success" && <FaCheck />}
      {type === "info" && <FaInfoCircle />}
      <span>{message}</span>
    </div>
  );
};

export default function UserManagement() {
  const [users, setUsers] = useState<UserResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [statusMessage, setStatusMessage] = useState<{
    message: string;
    type: "success" | "error" | "info";
  } | null>(null);
  const [selectedRolePerUser, setSelectedRolePerUser] = useState<{
    [key: number]: string;
  }>({});

  const showStatus = (message: string, type: "success" | "error" | "info") => {
    setStatusMessage({ message, type });
    setTimeout(() => setStatusMessage(null), 4000);
  };

  const fetchUsers = async () => {
    setLoading(true);
    try {
      const response = await UserService.getUsers();
      if (response.success) {
        setUsers(response.data);
      } else {
        showStatus(response.message || "Kullanıcılar alınamadı.", "error");
      }
    } catch (err: any) {
      showStatus("Kullanıcı verileri çekilirken hata oluştu.", "error");
      console.error(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleRoleChange = (userId: number, role: string) => {
    setSelectedRolePerUser((prev) => ({
      ...prev,
      [userId]: role,
    }));
  };

  const handleAddRole = async (userId: number) => {
    const selectedRole = selectedRolePerUser[userId];
    if (!selectedRole) {
      showStatus("Lütfen eklemek için bir rol seçin.", "info");
      return;
    }
    setLoading(true);
    try {
      const response = await RoleService.createRole({
        name: selectedRole,
        userId,
      });
      if (response.success) {
        showStatus("Rol başarıyla eklendi.", "success");
        await fetchUsers();
      } else {
        showStatus(response.message || "Rol eklenemedi.", "error");
      }
    } catch (err: any) {
      showStatus("Rol eklenirken bir hata oluştu.", "error");
      console.error(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteRole = async (userId: number, role: string) => {
    setLoading(true);
    try {
      const response = await RoleService.deleteRole({
        role: role,
        userId: userId,
      });
      if (response.success) {
        showStatus("Rol başarıyla silindi.", "success");
        await fetchUsers();
      } else {
        showStatus(response.message || "Rol silinemedi.", "error");
      }
    } catch (err: any) {
      showStatus("Rol silinirken bir hata oluştu.", "error");
      console.error(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  return (
    <div className="main-container">
      <div className="page-header">
        <h2 className="page-title">
          <span className="title-icon" /> Kullanıcı Yönetimi
        </h2>
        <div className="status-container">
          {loading && <FaSpinner className="loading-spinner" />}
          <StatusMessage
            message={statusMessage?.message || null}
            type={statusMessage?.type || null}
          />
        </div>
      </div>
      <div className="content-card">
        {users.length > 0 ? (
          <div className="table-responsive-wrapper">
            <table className="data-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Mail</th>
                  <th>Ad Soyad</th>
                  <th>Roller</th>
                  <th>İşlemler</th>
                </tr>
              </thead>
              <tbody>
                {users.map((user) => (
                  <tr key={user.id}>
                    <td data-label="ID">{user.id}</td>
                    <td data-label="Mail">{user.email}</td>
                    <td data-label="Ad Soyad">{`${user.firstName} ${user.lastName}`}</td>
                    <td data-label="Roller">
                      <div className="role-tags-container">
                        {user.roles.map((role: string) => (
                          <span key={role} className="role-tag">
                            {role}
                            <button
                              className="remove-role-btn"
                              onClick={() => handleDeleteRole(user.id, role)}
                            >
                              <FaTrashAlt />
                            </button>
                          </span>
                        ))}
                      </div>
                    </td>
                    <td data-label="İşlemler">
                      <div className="action-row">
                        <select
                          className="role-select"
                          value={selectedRolePerUser[user.id] || ""}
                          onChange={(e) =>
                            handleRoleChange(user.id, e.target.value)
                          }
                        >
                          <option value="" disabled>
                            Rol seç
                          </option>
                          {AVAILABLE_ROLES.filter(
                            (role) => !user.roles.includes(role)
                          ).map((role) => (
                            <option key={role} value={role}>
                              {role}
                            </option>
                          ))}
                        </select>
                        <button
                          className="action-button add-role-btn"
                          onClick={() => handleAddRole(user.id)}
                        >
                          <FaPlus />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : (
          <div className="no-data-message">
            <p>Sistemde kayıtlı kullanıcı bulunamadı.</p>
          </div>
        )}
      </div>
    </div>
  );
}
