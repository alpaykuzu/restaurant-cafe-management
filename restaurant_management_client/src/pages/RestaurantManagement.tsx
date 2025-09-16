/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useState } from "react";
import { RestaurantService } from "../services/RestaurantService";
import { RoleService } from "../services/RoleService";
import "./../styles/RestaurantManagement.css";
import type { RestaurantResponse } from "../types/Restaurant/RestaurantResponse";
import type { UserResponse } from "../types/User/UserResponse";
import { EmployeeService } from "../services/EmployeeService";
import { UserService } from "../services/UserService";
import type { CreateUserRequest } from "../types/User/CreateUserRequest";
import Select from "react-select";
import {
  FaPlus,
  FaTrashAlt,
  FaEye,
  FaTimes,
  FaBuilding,
  FaUserPlus,
} from "react-icons/fa";
import React from "react";
import type { EmployeeResponse } from "../types/Employee/EmployeeResponse";

export default function RestaurantManagement() {
  const [restaurants, setRestaurants] = useState<RestaurantResponse[]>([]);
  const [expanded, setExpanded] = useState<number | null>(null);
  const [employees, setEmployees] = useState<
    (EmployeeResponse & UserResponse)[]
  >([]);
  const [showEmployeeModal, setShowEmployeeModal] = useState(false);
  const [selectedRestaurant, setSelectedRestaurant] =
    useState<RestaurantResponse | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [newRestaurant, setNewRestaurant] = useState({
    name: "",
    address: "",
    phone: "",
    email: "",
  });

  const [newEmployee, setNewEmployee] = useState({
    email: "",
    firstName: "",
    lastName: "",
    password: "",
    roles: ["Waiter"],
    salary: 0,
    hireDate: "",
  });

  const AVAILABLE_ROLES = [
    { label: "User", value: "User" },
    { label: "Waiter", value: "Waiter" },
    { label: "Kitchen", value: "Kitchen" },
    { label: "Cashier", value: "Cashier" },
    { label: "Manager", value: "Manager" },
    { label: "Admin", value: "Admin" },
  ];

  const fetchRestaurants = async () => {
    setIsLoading(true);
    try {
      const res = await RestaurantService.getRestaurants();
      setRestaurants(res.data);
      setError(null);
    } catch (err: any) {
      setError("Restoranlar yüklenirken bir hata oluştu.");
      console.error(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  const fetchEmployees = async (restaurantId: number) => {
    setIsLoading(true);
    try {
      const res = await EmployeeService.getEmployeesByRestaurantId(
        restaurantId
      );
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
    fetchRestaurants();
  }, []);

  const handleAddRestaurant = async () => {
    setIsLoading(true);
    try {
      await RestaurantService.createRestaurant(newRestaurant);
      setNewRestaurant({ name: "", address: "", phone: "", email: "" });
      fetchRestaurants();
      setError(null);
    } catch (err: any) {
      setError("Restoran eklenirken bir hata oluştu.");
      console.error(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteRestaurant = async (id: number) => {
    setIsLoading(true);
    try {
      await RestaurantService.deleteRestaurant(id);
      fetchRestaurants();
      setError(null);
    } catch (err: any) {
      setError("Restoran silinirken bir hata oluştu.");
      console.error(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  const handleOpenEmployees = (restaurant: RestaurantResponse) => {
    const newExpandedId = restaurant.id === expanded ? null : restaurant.id;
    setExpanded(newExpandedId);
    setSelectedRestaurant(restaurant);
    if (newExpandedId) {
      fetchEmployees(restaurant.id);
    }
  };

  const handleAddEmployee = async () => {
    if (!selectedRestaurant) return;
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
          restaurantId: selectedRestaurant.id,
          salary: newEmployee.salary,
          hireDate: new Date(newEmployee.hireDate),
        });
        fetchEmployees(selectedRestaurant.id);
        setShowEmployeeModal(false);
        setNewEmployee({
          email: "",
          firstName: "",
          lastName: "",
          password: "",
          roles: ["Waiter"],
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
        for (const r of user.roles) {
          await RoleService.deleteRole({ userId, role: r });
        }
      }
      fetchEmployees(selectedRestaurant!.id);
      setError(null);
    } catch (err: any) {
      setError("Çalışan silinirken bir hata oluştu.");
      console.error(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  const handleRolesChange = async (userId: number, newRoles: string[]) => {
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

      fetchEmployees(selectedRestaurant!.id);
      setError(null);
    } catch (err: any) {
      setError("Roller güncellenirken bir hata oluştu.");
      console.error(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="res-management-container">
      <div className="res-management-header">
        <h2 className="res-management-title">Restoran Yönetimi</h2>
        {isLoading && (
          <p className="res-management-loading">İşlem yapılıyor...</p>
        )}
        {error && <p className="res-management-error">{error}</p>}
      </div>

      {/* Restoran Ekleme Formu */}
      <div className="res-management-form-card">
        <h3>
          <FaBuilding /> Yeni Restoran Ekle
        </h3>
        <div className="res-management-form-grid">
          <input
            type="text"
            placeholder="Ad"
            value={newRestaurant.name}
            onChange={(e) =>
              setNewRestaurant({ ...newRestaurant, name: e.target.value })
            }
          />
          <input
            type="text"
            placeholder="Adres"
            value={newRestaurant.address}
            onChange={(e) =>
              setNewRestaurant({ ...newRestaurant, address: e.target.value })
            }
          />
          <input
            type="text"
            placeholder="Telefon"
            value={newRestaurant.phone}
            onChange={(e) =>
              setNewRestaurant({ ...newRestaurant, phone: e.target.value })
            }
          />
          <input
            type="email"
            placeholder="Email"
            value={newRestaurant.email}
            onChange={(e) =>
              setNewRestaurant({ ...newRestaurant, email: e.target.value })
            }
          />
          <button onClick={handleAddRestaurant}>
            <FaPlus /> Ekle
          </button>
        </div>
      </div>

      {/* Restoran Listesi */}
      <div className="res-management-list">
        <h3>Restoranlar</h3>
        <div className="res-management-table-wrapper">
          <table className="res-management-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Ad</th>
                <th>Adres</th>
                <th>Telefon</th>
                <th>Email</th>
                <th>Çalışanlar</th>
                <th>İşlemler</th>
              </tr>
            </thead>
            <tbody>
              {restaurants.map((r) => (
                <React.Fragment key={r.id}>
                  <tr className="res-row">
                    <td data-label="ID">{r.id}</td>
                    <td data-label="Ad">{r.name}</td>
                    <td data-label="Adres">{r.address}</td>
                    <td data-label="Telefon">{r.phone}</td>
                    <td data-label="Email">{r.email}</td>
                    <td data-label="Çalışanlar">
                      <button
                        className="res-table-button primary-btn"
                        onClick={() => handleOpenEmployees(r)}
                      >
                        <FaEye /> {expanded === r.id ? "Kapat" : "Görüntüle"}
                      </button>
                    </td>
                    <td data-label="İşlemler">
                      <button
                        className="res-table-button danger-btn"
                        onClick={() => handleDeleteRestaurant(r.id)}
                      >
                        <FaTrashAlt /> Sil
                      </button>
                    </td>
                  </tr>
                  {expanded === r.id && (
                    <tr className="expanded-row">
                      <td colSpan={7}>
                        <div className="employee-details-card">
                          <div className="employee-details-header">
                            <h4>{r.name} Çalışanları</h4>
                            <button
                              className="res-table-button success-btn"
                              onClick={() => setShowEmployeeModal(true)}
                            >
                              <FaUserPlus /> Çalışan Ekle
                            </button>
                          </div>
                          {employees.length > 0 ? (
                            <div className="employee-table-wrapper">
                              <table className="employee-table">
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
                                  {employees.map((e) => (
                                    <tr key={e.id}>
                                      <td data-label="ID">{e.id}</td>
                                      <td data-label="Ad Soyad">{`${e.firstName} ${e.lastName}`}</td>
                                      <td data-label="Email">{e.email}</td>
                                      <td data-label="Roller">
                                        <Select
                                          isMulti
                                          value={e.roles.map((r) => ({
                                            label: r,
                                            value: r,
                                          }))}
                                          options={AVAILABLE_ROLES}
                                          onChange={(selected) =>
                                            handleRolesChange(
                                              e.userId,
                                              selected.map((s) => s.value)
                                            )
                                          }
                                        />
                                      </td>
                                      <td data-label="Maaş">{e.salary}</td>
                                      <td data-label="İşe Giriş">
                                        {new Date(
                                          e.hireDate
                                        ).toLocaleDateString()}
                                      </td>
                                      <td data-label="İşlem">
                                        <button
                                          className="res-table-button danger-btn"
                                          onClick={() =>
                                            handleDeleteEmployee(e.userId, e.id)
                                          }
                                        >
                                          <FaTrashAlt /> Sil
                                        </button>
                                      </td>
                                    </tr>
                                  ))}
                                </tbody>
                              </table>
                            </div>
                          ) : (
                            <p className="no-employees-message">
                              Bu restorana ait çalışan bulunamadı.{" "}
                              <FaUserPlus /> Yeni çalışan ekleyebilirsiniz.
                            </p>
                          )}
                        </div>
                      </td>
                    </tr>
                  )}
                </React.Fragment>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Çalışan Ekleme Modalı */}
      {showEmployeeModal && (
        <div className="res-modal-overlay">
          <div className="res-modal">
            <div className="res-modal-header">
              <h3>Çalışan Ekle - {selectedRestaurant?.name}</h3>
              <button
                className="res-modal-close"
                onClick={() => setShowEmployeeModal(false)}
              >
                <FaTimes />
              </button>
            </div>
            <div className="res-modal-body">
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
                placeholder="Rolleri Seç"
                value={newEmployee.roles.map((r) => ({ label: r, value: r }))}
                options={AVAILABLE_ROLES}
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
            <div className="res-modal-actions">
              <button
                className="res-button primary-btn"
                onClick={handleAddEmployee}
              >
                Kaydet
              </button>
              <button
                className="res-button secondary-btn"
                onClick={() => setShowEmployeeModal(false)}
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
