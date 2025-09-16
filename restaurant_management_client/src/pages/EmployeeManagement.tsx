/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable react-hooks/exhaustive-deps */
import { useEffect, useState } from "react";
import { RestaurantService } from "../services/RestaurantService";
import { EmployeeService } from "../services/EmployeeService";
import { UserService } from "../services/UserService";
import { RoleService } from "../services/RoleService";
import Select from "react-select";
import makeAnimated from "react-select/animated";
import "./../styles/EmployeeManagement.css";
import { HubConnectionBuilder, HubConnection } from "@microsoft/signalr";
import type { RestaurantResponse } from "../types/Restaurant/RestaurantResponse";
import type { EmployeeResponse } from "../types/Employee/EmployeeResponse";
import type { UserResponse } from "../types/User/UserResponse";
import type { CreateUserRequest } from "../types/User/CreateUserRequest";
import { useAuth } from "../context/AuthProvider";

import { FaPlus, FaTrashAlt, FaTimes } from "react-icons/fa";

const roleOptions = [
  { value: "Manager", label: "Manager" },
  { value: "Waiter", label: "Waiter" },
  { value: "Kitchen", label: "Kitchen" },
  { value: "Cashier", label: "Cashier" },
];

export default function EmployeeManagement() {
  const { id: userId } = useAuth();
  const [restaurant, setRestaurant] = useState<RestaurantResponse | null>(null);
  const [employees, setEmployees] = useState<
    (EmployeeResponse & UserResponse)[]
  >([]);
  const [showModal, setShowModal] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [newEmployee, setNewEmployee] = useState({
    email: "",
    firstName: "",
    lastName: "",
    password: "",
    roles: [] as string[],
    salary: 0,
    hireDate: "",
  });
  const [, setHubConnection] = useState<HubConnection | null>(null);

  const fetchRestaurant = async () => {
    if (!userId) return;
    setIsLoading(true);
    try {
      const res = await RestaurantService.getRestaurantByUserId();
      setRestaurant(res.data);
      if (res.data) {
        await fetchEmployees();
      }
      setError(null);
    } catch (err: any) {
      setError("Restoran bilgileri yüklenirken bir hata oluştu.");
      console.error(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  const fetchEmployees = async () => {
    setIsLoading(true);
    try {
      const res = await EmployeeService.getEmployeesOwnRestaurant();
      const users = await UserService.getUsers();

      const merged = res.data.map((emp) => {
        const user = users.data.find((u) => u.id === emp.userId);
        return {
          ...emp,
          firstName: user?.firstName || "Bilinmiyor",
          lastName: user?.lastName || "",
          email: user?.email || "",
          roles: user?.roles || [],
        };
      });
      setEmployees(merged);
      setError(null);
    } catch (err: any) {
      setError("Çalışanlar yüklenirken bir hata oluştu.");
      console.error(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchRestaurant();
  }, [userId]);

  const handleAddEmployee = async () => {
    if (!restaurant) return;
    setIsLoading(true);
    try {
      const userReq: CreateUserRequest = {
        email: newEmployee.email,
        firstName: newEmployee.firstName,
        lastName: newEmployee.lastName,
        password: newEmployee.password,
      };
      const userRes = await UserService.createUser(userReq);

      if (userRes.success && userRes.data) {
        for (const role of newEmployee.roles) {
          await RoleService.createRole({ userId: userRes.data.id, name: role });
        }
        await EmployeeService.createEmployee({
          userId: userRes.data.id,
          restaurantId: restaurant.id,
          salary: newEmployee.salary,
          hireDate: new Date(newEmployee.hireDate),
        });

        await fetchEmployees();
        setShowModal(false);
        setNewEmployee({
          email: "",
          firstName: "",
          lastName: "",
          password: "",
          roles: [],
          salary: 0,
          hireDate: "",
        });
        setError(null);
      }
    } catch (err: any) {
      setError("Çalışan eklenirken bir hata oluştu.");
      console.error(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteEmployee = async (userId: number, employeeId: number) => {
    setIsLoading(true);
    try {
      await EmployeeService.deleteEmployee(employeeId);
      const user = employees.find((e) => e.userId === userId);
      if (user) {
        await Promise.all(
          user.roles.map((role) => RoleService.deleteRole({ userId, role }))
        );
      }
      if (restaurant) await fetchEmployees();
      setError(null);
    } catch (err: any) {
      setError("Çalışan silinirken bir hata oluştu.");
      console.error(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  const handleRoleChange = async (userId: number, newRoles: string[]) => {
    setIsLoading(true);
    const user = employees.find((e) => e.userId === userId);
    if (!user) {
      setIsLoading(false);
      return;
    }

    try {
      const rolesToAdd = newRoles.filter((r) => !user.roles.includes(r));
      const rolesToRemove = user.roles.filter((r) => !newRoles.includes(r));

      await Promise.all(
        rolesToAdd.map((r) => RoleService.createRole({ userId, name: r }))
      );
      await Promise.all(
        rolesToRemove.map((r) => RoleService.deleteRole({ userId, role: r }))
      );

      if (restaurant) await fetchEmployees();
      setError(null);
    } catch (err: any) {
      setError("Roller güncellenirken bir hata oluştu.");
      console.error(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  //hub
  useEffect(() => {
    if (!restaurant?.id) return;

    // Hub bağlantısı
    const connection = new HubConnectionBuilder()
      .withUrl("http://localhost:5164/hubs", { withCredentials: true })
      .withAutomaticReconnect()
      .build();

    connection
      .start()
      .then(() => {
        console.log("SignalR connected");
        connection.invoke("JoinRestaurantGroup", restaurant?.id);
        connection.on("EmployeeChanged", () => {
          fetchEmployees();
        });
      })
      .catch((err) => console.error("SignalR connection error:", err));
    setHubConnection(connection);
    return () => {
      connection.stop();
    };
  }, [restaurant?.id]);

  return (
    <div className="emp-management-container">
      <div className="emp-management-header">
        <h2 className="emp-management-title">
          {restaurant
            ? `${restaurant.name} - Çalışan Yönetimi`
            : "Çalışan Yönetimi"}
        </h2>
        {isLoading && (
          <p className="emp-management-loading">İşlem yapılıyor...</p>
        )}
        {error && <p className="emp-management-error">{error}</p>}
      </div>

      <div className="emp-list-section">
        <div className="emp-list-header">
          <h3>Çalışanlar</h3>
          <button className="emp-add-btn" onClick={() => setShowModal(true)}>
            <FaPlus /> Çalışan Ekle
          </button>
        </div>

        <div className="emp-table-wrapper">
          <table className="emp-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Ad Soyad</th>
                <th>Email</th>
                <th>Roller</th>
                <th>Maaş</th>
                <th>İşe Giriş</th>
                <th>İşlem</th>
              </tr>
            </thead>
            <tbody>
              {employees.length > 0 ? (
                employees.map((e) => (
                  <tr key={e.id}>
                    <td data-label="ID">{e.id}</td>
                    <td data-label="Ad Soyad">
                      {e.firstName} {e.lastName}
                    </td>
                    <td data-label="Email">{e.email}</td>
                    <td data-label="Roller">
                      <Select
                        isMulti
                        closeMenuOnSelect={false}
                        components={makeAnimated()}
                        options={roleOptions}
                        value={e.roles.map((r) => ({ value: r, label: r }))}
                        onChange={(selected) =>
                          handleRoleChange(
                            e.userId,
                            selected.map((s) => s.value)
                          )
                        }
                      />
                    </td>
                    <td data-label="Maaş">{e.salary}</td>
                    <td data-label="İşe Giriş">
                      {new Date(e.hireDate).toLocaleDateString()}
                    </td>
                    <td data-label="İşlem">
                      <button
                        className="emp-delete-btn"
                        onClick={() => handleDeleteEmployee(e.userId, e.id)}
                      >
                        <FaTrashAlt /> Sil
                      </button>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={7} className="no-employees-message">
                    Henüz çalışan bulunmuyor. <FaPlus /> Yeni bir çalışan
                    ekleyerek başlayın.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Çalışan Ekleme Modalı */}
      {showModal && (
        <div className="emp-modal-overlay">
          <div className="emp-modal">
            <div className="emp-modal-header">
              <h3>Çalışan Ekle</h3>
              <button
                className="emp-modal-close"
                onClick={() => setShowModal(false)}
              >
                <FaTimes />
              </button>
            </div>
            <div className="emp-modal-body">
              <input
                type="text"
                placeholder="Ad"
                value={newEmployee.firstName}
                onChange={(e) =>
                  setNewEmployee({ ...newEmployee, firstName: e.target.value })
                }
              />
              <input
                type="text"
                placeholder="Soyad"
                value={newEmployee.lastName}
                onChange={(e) =>
                  setNewEmployee({ ...newEmployee, lastName: e.target.value })
                }
              />
              <input
                type="email"
                placeholder="Email"
                value={newEmployee.email}
                onChange={(e) =>
                  setNewEmployee({ ...newEmployee, email: e.target.value })
                }
              />
              <input
                type="password"
                placeholder="Şifre"
                value={newEmployee.password}
                onChange={(e) =>
                  setNewEmployee({ ...newEmployee, password: e.target.value })
                }
              />
              <Select
                isMulti
                closeMenuOnSelect={false}
                components={makeAnimated()}
                options={roleOptions}
                placeholder="Rolleri Seç"
                value={newEmployee.roles.map((r) => ({ value: r, label: r }))}
                onChange={(selected) =>
                  setNewEmployee({
                    ...newEmployee,
                    roles: selected ? selected.map((s) => s.value) : [],
                  })
                }
              />
              <input
                type="number"
                placeholder="Maaş"
                value={newEmployee.salary}
                onChange={(e) =>
                  setNewEmployee({
                    ...newEmployee,
                    salary: Number(e.target.value),
                  })
                }
              />
              <input
                type="date"
                value={newEmployee.hireDate}
                onChange={(e) =>
                  setNewEmployee({ ...newEmployee, hireDate: e.target.value })
                }
              />
            </div>
            <div className="emp-modal-actions">
              <button
                className="emp-button primary-btn"
                onClick={handleAddEmployee}
              >
                Kaydet
              </button>
              <button
                className="emp-button secondary-btn"
                onClick={() => setShowModal(false)}
              >
                Kapat
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
