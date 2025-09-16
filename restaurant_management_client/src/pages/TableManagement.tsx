/* eslint-disable react-hooks/exhaustive-deps */
/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useState } from "react";
import Select from "react-select";
import { TableService } from "../services/TableService";
import { useAuth } from "../context/AuthProvider";
import { RestaurantService } from "../services/RestaurantService";
import type { TableResponse } from "../types/Table/TableResponse";
import type { CreateTableRequest } from "../types/Table/CreateTableRequest";
import type { UpdateTableRequest } from "../types/Table/UpdateTableRequest";
import "./../styles/TableManagement.css";
import { HubConnectionBuilder, HubConnection } from "@microsoft/signalr";
import {
  FaChair,
  FaTimes,
  FaPlus,
  FaRegEdit,
  FaTrashAlt,
  FaSpinner,
} from "react-icons/fa";

const statusOptions = [
  { value: "Available", label: "Müsait" },
  { value: "Occupied", label: "Dolu" },
  { value: "Reserved", label: "Rezerve" },
];

export default function TableManagement() {
  const { id } = useAuth();
  const [restaurantId, setRestaurantId] = useState<number | null>(null);
  const [tables, setTables] = useState<TableResponse[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Sayılar
  const [totalCount, setTotalCount] = useState<number>(0);
  const [availableCount, setAvailableCount] = useState<number>(0);
  const [occupiedCount, setOccupiedCount] = useState<number>(0);
  const [reservedCount, setReservedCount] = useState<number>(0);
  const [, setHubConnection] = useState<HubConnection | null>(null);

  // Ekleme formu
  const [newTable, setNewTable] = useState<{
    number: number;
    capacity: number;
    status: string;
  }>({
    number: 1,
    capacity: 2,
    status: "Available",
  });

  // Düzenleme popup state
  const [editingTable, setEditingTable] = useState<TableResponse | null>(null);

  const fetchTables = async () => {
    setIsLoading(true);
    try {
      const res = await TableService.getTablesByRestaurantId();
      if (!res.success) {
        setError(res.message);
        return;
      }
      setTables(res.data.sort((a, b) => a.number - b.number));
      fetchCounts();
      setError(null);
    } catch (err: any) {
      setError("Masa verileri alınamadı: " + err.message);
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  const fetchCounts = async () => {
    try {
      const totalRes = await TableService.getTableCountByRestaurantId();
      if (totalRes.success) setTotalCount(Number(totalRes.data));

      const availRes = await TableService.getTableCountByRestaurantIdAndStatus(
        "Available"
      );
      if (availRes.success) setAvailableCount(Number(availRes.data));

      const occRes = await TableService.getTableCountByRestaurantIdAndStatus(
        "Occupied"
      );
      if (occRes.success) setOccupiedCount(Number(occRes.data));

      const resRes = await TableService.getTableCountByRestaurantIdAndStatus(
        "Reserved"
      );
      if (resRes.success) setReservedCount(Number(resRes.data));
    } catch (err: any) {
      console.error("Masa sayıları alınamadı: " + err.message);
    }
  };

  const fetchRestaurant = async () => {
    if (!id) return;
    setIsLoading(true);
    try {
      const res = await RestaurantService.getRestaurantByUserId();
      if (!res.success) {
        setError(res.message);
        return;
      }
      setRestaurantId(res.data.id);
      fetchTables();
      setError(null);
    } catch (err: any) {
      setError("Restoran bulunamadı: " + err.message);
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchRestaurant();
  }, [id]);

  const handleAdd = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!restaurantId) return;

    setIsLoading(true);
    try {
      const req: CreateTableRequest = {
        number: newTable.number,
        capacity: newTable.capacity,
        status: newTable.status,
      };
      const res = await TableService.createTable(req);
      if (!res.success) {
        setError(res.message);
        return;
      }
      await fetchTables();
      setNewTable({ number: 1, capacity: 2, status: "Available" });
      setError(null);
    } catch (err: any) {
      setError("Masa eklenemedi: " + err.message);
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleEditSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!restaurantId || !editingTable) return;
    setIsLoading(true);
    try {
      const req: UpdateTableRequest = {
        id: editingTable.id,
        number: editingTable.number,
        capacity: editingTable.capacity,
        status: editingTable.status,
      };
      const res = await TableService.updateTable(req);
      if (!res.success) {
        setError(res.message);
        return;
      }
      await fetchTables();
      setEditingTable(null);
      setError(null);
    } catch (err: any) {
      setError("Masa güncellenemedi: " + err.message);
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!restaurantId) return;
    if (!window.confirm("Bu masayı silmek istediğinize emin misiniz?")) return;
    setIsLoading(true);
    try {
      const res = await TableService.deleteTable(id);
      if (!res.success) {
        setError(res.message);
        return;
      }
      await fetchTables();
      setError(null);
    } catch (err: any) {
      setError("Masa silinemedi: " + err.message);
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleStatusChange = async (table: TableResponse, status: string) => {
    if (!restaurantId) return;
    setIsLoading(true);
    try {
      const res = await TableService.updateTableStatus({
        id: table.id,
        status,
      });
      if (!res.success) {
        setError(res.message);
        return;
      }
      await fetchTables();
      setError(null);
    } catch (err: any) {
      setError("Durum güncellenemedi: " + err.message);
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  const getStatusClass = (status: string) => {
    switch (status) {
      case "Available":
        return "status-available";
      case "Occupied":
        return "status-occupied";
      case "Reserved":
        return "status-reserved";
      default:
        return "";
    }
  };

  //hub
  useEffect(() => {
    if (!restaurantId) return;

    // Hub bağlantısı
    const connection = new HubConnectionBuilder()
      .withUrl("http://localhost:5164/hubs", { withCredentials: true })
      .withAutomaticReconnect()
      .build();

    connection
      .start()
      .then(() => {
        console.log("SignalR connected");
        connection.invoke("JoinRestaurantGroup", restaurantId);
        connection.on("TableChanged", () => {
          fetchTables();
        });
      })
      .catch((err) => console.error("SignalR connection error:", err));
    setHubConnection(connection);
    return () => {
      connection.stop();
    };
  }, [restaurantId]);

  return (
    <div className="table-management-container">
      <div className="table-management-header">
        <h2 className="table-management-title">Masa Yönetimi</h2>
        {isLoading && (
          <p className="table-management-loading">
            <FaSpinner className="spinner" /> İşlem yapılıyor...
          </p>
        )}
        {error && <p className="table-management-error">{error}</p>}
      </div>

      <div className="table-summary-cards">
        <div className="summary-card total">
          <h3>Toplam Masa</h3>
          <p>{totalCount}</p>
        </div>
        <div className="summary-card available">
          <h3>Müsait</h3>
          <p>{availableCount}</p>
        </div>
        <div className="summary-card occupied">
          <h3>Dolu</h3>
          <p>{occupiedCount}</p>
        </div>
        <div className="summary-card reserved">
          <h3>Rezerve</h3>
          <p>{reservedCount}</p>
        </div>
      </div>

      <div className="content-card">
        <div className="content-header">
          <h3>Yeni Masa Ekle</h3>
        </div>
        <form className="table-add-form" onSubmit={handleAdd}>
          <div className="form-group">
            <label htmlFor="table-number">Masa No.</label>
            <input
              type="number"
              id="table-number"
              value={newTable.number}
              onChange={(e) =>
                setNewTable((prev) => ({
                  ...prev,
                  number: Number(e.target.value),
                }))
              }
              required
            />
          </div>
          <div className="form-group">
            <label htmlFor="capacity">Kapasite</label>
            <input
              type="number"
              id="capacity"
              value={newTable.capacity}
              onChange={(e) =>
                setNewTable((prev) => ({
                  ...prev,
                  capacity: Number(e.target.value),
                }))
              }
              required
            />
          </div>
          <div className="form-group">
            <label htmlFor="status">Durum</label>
            <Select
              options={statusOptions}
              value={statusOptions.find((s) => s.value === newTable.status)}
              onChange={(opt) =>
                setNewTable((prev) => ({
                  ...prev,
                  status: opt?.value || "Available",
                }))
              }
              classNamePrefix="react-select"
            />
          </div>
          <button type="submit" className="table-button primary-btn">
            <FaPlus /> Masa Ekle
          </button>
        </form>
      </div>

      <div className="content-card">
        <div className="content-header">
          <h3>Masalar</h3>
        </div>
        {isLoading ? (
          <p className="loading-message">
            <FaSpinner className="spinner" /> Masalar yükleniyor...
          </p>
        ) : (
          <div className="table-grid">
            {tables.length > 0 ? (
              tables.map((t) => (
                <div
                  key={t.id}
                  className={`table-card ${getStatusClass(t.status)}`}
                >
                  <div className="card-header">
                    <h4>
                      <FaChair /> Masa {t.number}
                    </h4>
                    <span
                      className={`status-badge ${getStatusClass(t.status)}`}
                    >
                      {statusOptions.find((s) => s.value === t.status)?.label}
                    </span>
                  </div>
                  <div className="card-body">
                    <p className="capacity-text">Kapasite: {t.capacity} kişi</p>
                    <div className="inline-select">
                      <Select
                        options={statusOptions}
                        value={statusOptions.find((s) => s.value === t.status)}
                        onChange={(opt) =>
                          opt && handleStatusChange(t, opt.value)
                        }
                        classNamePrefix="react-select-inline"
                      />
                    </div>
                  </div>
                  <div className="card-actions">
                    <button
                      className="table-button secondary-btn"
                      onClick={() => setEditingTable(t)}
                    >
                      <FaRegEdit /> Düzenle
                    </button>
                    <button
                      className="table-button danger-btn"
                      onClick={() => handleDelete(t.id)}
                    >
                      <FaTrashAlt /> Sil
                    </button>
                  </div>
                </div>
              ))
            ) : (
              <p className="no-data-message">
                Henüz masa eklenmedi. <FaPlus /> İlk masayı ekleyerek başlayın.
              </p>
            )}
          </div>
        )}
      </div>

      {/* Düzenleme Modal */}
      {editingTable && (
        <div className="modal-overlay">
          <div className="modal-content">
            <div className="modal-header">
              <h3>Masa Düzenle</h3>
              <button
                className="modal-close-btn"
                onClick={() => setEditingTable(null)}
              >
                <FaTimes />
              </button>
            </div>
            <form onSubmit={handleEditSubmit} className="modal-form">
              <div className="form-group">
                <label>Masa Numarası</label>
                <input
                  type="number"
                  value={editingTable.number}
                  onChange={(e) =>
                    setEditingTable((prev) =>
                      prev ? { ...prev, number: Number(e.target.value) } : prev
                    )
                  }
                  required
                />
              </div>
              <div className="form-group">
                <label>Kapasite</label>
                <input
                  type="number"
                  value={editingTable.capacity}
                  onChange={(e) =>
                    setEditingTable((prev) =>
                      prev
                        ? { ...prev, capacity: Number(e.target.value) }
                        : prev
                    )
                  }
                  required
                />
              </div>
              <div className="form-group">
                <label>Durum</label>
                <Select
                  options={statusOptions}
                  value={statusOptions.find(
                    (s) => s.value === editingTable.status
                  )}
                  onChange={(opt) =>
                    setEditingTable((prev) =>
                      prev
                        ? { ...prev, status: opt?.value || "Available" }
                        : prev
                    )
                  }
                  classNamePrefix="react-select"
                />
              </div>
              <div className="modal-actions">
                <button type="submit" className="table-button primary-btn">
                  <FaRegEdit /> Kaydet
                </button>
                <button
                  type="button"
                  className="table-button secondary-btn"
                  onClick={() => setEditingTable(null)}
                >
                  <FaTimes /> İptal
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
