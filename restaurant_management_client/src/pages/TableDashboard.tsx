/* eslint-disable react-hooks/exhaustive-deps */
/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthProvider";
import { TableService } from "../services/TableService";
import { OrderService } from "../services/OrderService";
import { MenuItemService } from "../services/MenuItemService";
import { PaymentService } from "../services/PaymentService";
import { InvoiceService } from "../services/InvoiceService";
import { RestaurantService } from "../services/RestaurantService";
import type { TableResponse } from "../types/Table/TableResponse";
import type { MenuItemResponse } from "../types/MenuItem/MenuItemResponse";
import type { OrderResponse } from "../types/Order/OrderResponse";
import type { CreateOrderFullRequest } from "../types/Order/CreateOrderFullRequest";
import type { CreateOrderItemRequest } from "../types/Order/CreateOrderItemRequest";
import type { UpdateTableStatusRequest } from "../types/Table/UpdateTableStatusRequest";
import "./../styles/TableDashboard.css";
import { HubConnectionBuilder, HubConnection } from "@microsoft/signalr";
import {
  FaSpinner,
  FaTimes,
  FaCheck,
  FaChair,
  FaUtensils,
  FaReceipt,
  FaMoneyBillWave,
  FaInfoCircle,
} from "react-icons/fa";

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
      {type === "error" && <FaTimes />}
      {type === "success" && <FaCheck />}
      {type === "info" && <FaInfoCircle />}
      <span>{message}</span>
    </div>
  );
};

export default function TableDashboard() {
  const { id: userId, roles } = useAuth();
  const [restaurantId, setRestaurantId] = useState<number | null>(null);
  const [tables, setTables] = useState<TableResponse[]>([]);
  const [menuItems, setMenuItems] = useState<MenuItemResponse[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [selectedTable, setSelectedTable] = useState<TableResponse | null>(
    null
  );
  const [currentOrders, setCurrentOrders] = useState<OrderResponse[]>([]);
  const [orderItems, setOrderItems] = useState<
    { menuItem: MenuItemResponse; quantity: number }[]
  >([]);
  const [selectedOrder, setSelectedOrder] = useState<OrderResponse | null>(
    null
  );
  const [paymentMethod, setPaymentMethod] = useState<string>("Cash");
  const [statusMessage, setStatusMessage] = useState<{
    message: string;
    type: "success" | "error" | "info";
  } | null>(null);
  const [, setHubConnection] = useState<HubConnection | null>(null);

  const showStatus = (message: string, type: "success" | "error" | "info") => {
    setStatusMessage({ message, type });
    setTimeout(() => setStatusMessage(null), 4000);
  };

  const fetchTablesAndOrders = async () => {
    setIsLoading(true);
    try {
      const [tableRes, orderRes, menuRes] = await Promise.all([
        TableService.getTablesByRestaurantId(),
        OrderService.getOrdersByRestaurantId(),
        MenuItemService.getMenuItemsByRestaurantId(),
      ]);

      if (tableRes.success && tableRes.data)
        setTables(tableRes.data.sort((a, b) => a.number - b.number));
      else showStatus(tableRes.message || "Masalar alınamadı.", "error");

      if (orderRes.success && orderRes.data) {
        // Siparişleri filtrele: sadece tamamlanmamış ve iptal edilmemiş olanları göster
        const activeOrders = orderRes.data.filter(
          (o) => o.status !== "Completed" && o.status !== "Cancelled"
        );
        setCurrentOrders(activeOrders);
      } else showStatus(orderRes.message || "Siparişler alınamadı.", "error");

      if (menuRes.success && menuRes.data) setMenuItems(menuRes.data);
      else showStatus(menuRes.message || "Menü öğeleri alınamadı.", "error");
    } catch (err: any) {
      showStatus("Veri alınırken bir hata oluştu.", "error");
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    const fetchRestaurantId = async () => {
      if (!userId) return;
      setIsLoading(true);
      try {
        const res = await RestaurantService.getRestaurantByUserId();
        if (res.success && res.data) {
          setRestaurantId(res.data.id);
          fetchTablesAndOrders();
        } else {
          showStatus(res.message || "Restoran bilgisi alınamadı.", "error");
        }
      } catch (err: any) {
        showStatus("Restoran bilgisi alınırken bir hata oluştu.", "error");
        console.error(err);
      } finally {
        setIsLoading(false);
      }
    };
    fetchRestaurantId();
  }, [userId]);

  const handleSelectTable = (table: TableResponse) => {
    if (table.status !== "Available") {
      showStatus("Bu masa zaten dolu veya rezerve.", "info");
      return;
    }
    setSelectedTable(table);
    setOrderItems([]);
  };

  const handleAddOrderItem = (menuItem: MenuItemResponse) => {
    setOrderItems((prev) => {
      const existing = prev.find((o) => o.menuItem.id === menuItem.id);
      if (existing) {
        return prev.map((o) =>
          o.menuItem.id === menuItem.id ? { ...o, quantity: o.quantity + 1 } : o
        );
      } else {
        return [...prev, { menuItem, quantity: 1 }];
      }
    });
  };

  const handleRemoveOrderItem = (menuItemId: number) => {
    setOrderItems((prev) => {
      const existing = prev.find((o) => o.menuItem.id === menuItemId);
      if (existing && existing.quantity > 1) {
        return prev.map((o) =>
          o.menuItem.id === menuItemId ? { ...o, quantity: o.quantity - 1 } : o
        );
      } else {
        return prev.filter((o) => o.menuItem.id !== menuItemId);
      }
    });
  };

  const handleCreateOrder = async () => {
    if (!restaurantId || !selectedTable || orderItems.length === 0 || !userId) {
      showStatus("Lütfen sipariş için en az bir ürün seçin.", "info");
      return;
    }

    const req: CreateOrderFullRequest = {
      order: {
        tableId: selectedTable.id,
        shippingAddress: "",
      },
      orderItems: orderItems.map(
        (o) =>
          ({
            menuItemId: o.menuItem.id,
            quantity: o.quantity,
          } as CreateOrderItemRequest)
      ),
    };

    setIsLoading(true);
    try {
      const res = await OrderService.createOrder(req);
      if (!res.success) {
        showStatus("Sipariş oluşturulamadı: " + res.message, "error");
        return;
      }

      await TableService.updateTableStatus({
        id: selectedTable.id,
        restaurantId,
        status: "Occupied",
      } as UpdateTableStatusRequest);

      showStatus(
        "Sipariş başarıyla oluşturuldu ve masa doluya alındı.",
        "success"
      );
      fetchTablesAndOrders();
      setSelectedTable(null);
      setOrderItems([]);
    } catch (err: any) {
      showStatus("Sipariş oluşturulurken bir hata oluştu.", "error");
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleOpenPaymentModal = (order: OrderResponse) => {
    setSelectedOrder(order);
    setPaymentMethod("Cash");
  };

  const handlePaymentAndInvoice = async () => {
    if (!selectedOrder || !restaurantId) return;

    setIsLoading(true);
    try {
      const paymentRes = await PaymentService.createPayment({
        orderId: selectedOrder.id,
        paymentMethod,
      });

      if (!paymentRes.success) {
        showStatus("Ödeme başarısız: " + paymentRes.message, "error");
        return;
      }

      const invoiceRes = await InvoiceService.createInvoice(selectedOrder.id);
      if (!invoiceRes.success) {
        showStatus("Fatura oluşturulamadı: " + invoiceRes.message, "error");
        return;
      }

      await TableService.updateTableStatus({
        id: selectedOrder.tableId,
        restaurantId,
        status: "Available",
      } as UpdateTableStatusRequest);

      showStatus(
        "Ödeme ve fatura başarıyla tamamlandı. Masa tekrar müsait.",
        "success"
      );
      setSelectedOrder(null);
      fetchTablesAndOrders();
    } catch (err: any) {
      showStatus("Ödeme veya fatura oluşturulurken bir hata oluştu.", "error");
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
          fetchTablesAndOrders();
        });
        connection.on("OrderChanged", () => {
          console.log("Sipariş güncellendi:");
          fetchTablesAndOrders();
        });
        connection.on("CreateInvoice", () => {
          fetchTablesAndOrders();
        });

        connection.on("MenuItemChanged", () => {
          fetchTablesAndOrders();
        });

        connection.on("CategoryChanged", () => {
          fetchTablesAndOrders();
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
        <h2 className="table-management-title">Masa Paneli</h2>
        <div className="status-container">
          {isLoading && <FaSpinner className="spinner loading-spinner" />}
          <StatusMessage
            message={statusMessage?.message || null}
            type={statusMessage?.type || null}
          />
        </div>
      </div>

      <div className="content-card">
        <div className="content-header">
          <h3>Masalar</h3>
        </div>
        <div className="table-grid">
          {tables.length > 0 ? (
            tables.map((table) => {
              // Yalnızca aktif siparişler arasında arama yap
              const tableOrder = currentOrders.find(
                (o) => o.tableId === table.id
              );
              return (
                <div
                  key={table.id}
                  className={`table-card ${getStatusClass(table.status)}`}
                >
                  <div className="card-header">
                    <h4>
                      <FaChair /> Masa {table.number}
                    </h4>
                    <span
                      className={`status-badge ${getStatusClass(table.status)}`}
                    >
                      {table.status === "Available"
                        ? "Müsait"
                        : table.status === "Occupied"
                        ? "Dolu"
                        : "Rezerve"}
                    </span>
                  </div>
                  <div className="card-body">
                    <p className="capacity-text">
                      Kapasite: {table.capacity} kişi
                    </p>
                  </div>

                  <div className="card-actions">
                    {roles.includes("Waiter") &&
                      table.status === "Available" && (
                        <button
                          className="table-button primary-btn"
                          onClick={() => handleSelectTable(table)}
                        >
                          <FaUtensils /> Sipariş Gir
                        </button>
                      )}

                    {roles.includes("Cashier") && tableOrder && (
                      <button
                        className="table-button secondary-btn"
                        onClick={() => handleOpenPaymentModal(tableOrder)}
                      >
                        <FaMoneyBillWave /> Öde & Fatura
                      </button>
                    )}
                  </div>
                </div>
              );
            })
          ) : (
            <p className="no-data-message">
              Henüz masa bulunamadı. Lütfen yöneticinizden masa eklemesini
              isteyin.
            </p>
          )}
        </div>
      </div>

      {/* Waiter Sipariş Modal */}
      {selectedTable && (
        <div className="modal-overlay">
          <div className="modal-content">
            <div className="modal-header">
              <h3>Masa {selectedTable.number} Sipariş Ekle</h3>
              <button
                className="modal-close-btn"
                onClick={() => setSelectedTable(null)}
              >
                <FaTimes />
              </button>
            </div>
            <div className="modal-body">
              <div className="menu-items-list">
                {menuItems.length > 0 ? (
                  menuItems.map((menu) => {
                    const orderItem = orderItems.find(
                      (o) => o.menuItem.id === menu.id
                    );
                    const quantity = orderItem?.quantity || 0;
                    return (
                      <div key={menu.id} className="menu-item-card">
                        <div className="item-info">
                          <span className="item-name">{menu.name}</span>
                          <span className="item-price">{menu.price}₺</span>
                        </div>
                        <div className="item-actions">
                          <button
                            className="quantity-btn danger-btn"
                            onClick={() => handleRemoveOrderItem(menu.id)}
                            disabled={quantity === 0}
                          >
                            -
                          </button>
                          <span className="item-quantity">{quantity}</span>
                          <button
                            className="quantity-btn primary-btn"
                            onClick={() => handleAddOrderItem(menu)}
                          >
                            +
                          </button>
                        </div>
                      </div>
                    );
                  })
                ) : (
                  <div className="no-data-message">Menü öğesi bulunamadı.</div>
                )}
              </div>
            </div>
            <div className="modal-actions">
              <button
                className="table-button primary-btn"
                onClick={handleCreateOrder}
                disabled={orderItems.length === 0}
              >
                <FaCheck /> Siparişi Onayla
              </button>
              <button
                type="button"
                className="table-button secondary-btn"
                onClick={() => setSelectedTable(null)}
              >
                <FaTimes /> İptal
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Cashier Ödeme Modal */}
      {selectedOrder && (
        <div className="modal-overlay">
          <div className="modal-content">
            <div className="modal-header">
              <h3>Sipariş #{selectedOrder.orderNumber} Ödeme</h3>
              <button
                className="modal-close-btn"
                onClick={() => setSelectedOrder(null)}
              >
                <FaTimes />
              </button>
            </div>
            <div className="modal-body">
              <div className="order-summary">
                {selectedOrder.orderItems.map((item, idx) => (
                  <div key={idx} className="summary-item">
                    <span>
                      {item.menuItemName} x {item.quantity}
                    </span>
                    <span>{item.unitPrice?.toFixed(2)}₺</span>
                  </div>
                ))}
                <div className="summary-total">
                  <span>Toplam:</span>
                  <span className="total-amount">
                    {selectedOrder.totalAmount?.toFixed(2)}₺
                  </span>
                </div>
              </div>

              <div className="payment-method-select form-group">
                <label>Ödeme Yöntemi:</label>
                <select
                  value={paymentMethod}
                  onChange={(e) => setPaymentMethod(e.target.value)}
                >
                  <option value="Cash">Nakit</option>
                  <option value="QR">QR Kod</option>
                  <option value="Card">Kart</option>
                </select>
              </div>
            </div>
            <div className="modal-actions">
              <button
                className="table-button primary-btn"
                onClick={handlePaymentAndInvoice}
              >
                <FaReceipt /> Öde & Fatura
              </button>
              <button
                type="button"
                className="table-button secondary-btn"
                onClick={() => setSelectedOrder(null)}
              >
                <FaTimes /> İptal
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
