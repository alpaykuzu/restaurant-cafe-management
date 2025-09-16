/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable react-hooks/exhaustive-deps */
import { useEffect, useState, type JSX } from "react";
import "./../styles/OrderManagement.css";
import { useAuth } from "../context/AuthProvider";
import { OrderService } from "../services/OrderService";
import { RestaurantService } from "../services/RestaurantService";
import { PaymentService } from "../services/PaymentService";
import { InvoiceService } from "../services/InvoiceService";
import type { OrderResponse } from "../types/Order/OrderResponse";
import type { CreatePaymentRequest } from "../types/Payment/CreatePaymentRequest";
import { HubConnectionBuilder, HubConnection } from "@microsoft/signalr";
import {
  FaSpinner,
  FaCheckCircle,
  FaUtensils,
  FaClock,
  FaTruck,
  FaClipboardList,
  FaInfoCircle,
  FaBan,
  FaRegPaperPlane,
  FaTimes,
  FaCalendarAlt,
} from "react-icons/fa";

const STATUS_OPTIONS = [
  "Pending",
  "Preparing",
  "Ready",
  "Served",
  "Completed",
  "Cancelled",
];

const STATUS_ICONS: { [key: string]: JSX.Element } = {
  Pending: <FaClock />,
  Preparing: <FaUtensils />,
  Ready: <FaTruck />,
  Served: <FaCheckCircle />,
  Completed: <FaClipboardList />,
  Cancelled: <FaBan />,
};

export default function OrderManagement() {
  const { id } = useAuth();
  const [restaurantId, setRestaurantId] = useState<number | null>(null);
  const [orders, setOrders] = useState<OrderResponse[]>([]);
  const [filterStatus, setFilterStatus] = useState<string>("");
  const [selectedDate, setSelectedDate] = useState<string>(
    new Date().toISOString().split("T")[0]
  );
  const [isLoading, setIsLoading] = useState(false);
  const [statusMessage, setStatusMessage] = useState<{
    message: string;
    type: "success" | "error" | "info";
  } | null>(null);
  const [, setHubConnection] = useState<HubConnection | null>(null);

  useEffect(() => {
    if (!id) return;
    fetchRestaurant();
  }, [id, filterStatus, selectedDate]);

  const showStatusMessage = (
    message: string,
    type: "success" | "error" | "info"
  ) => {
    setStatusMessage({ message, type });
    setTimeout(() => setStatusMessage(null), 3000);
  };

  const fetchRestaurant = async () => {
    setIsLoading(true);
    try {
      const res = await RestaurantService.getRestaurantByUserId();
      if (!res.success) {
        showStatusMessage(res.message || "Restoran bulunamadı.", "error");
        return;
      }
      setRestaurantId(res.data.id);
      fetchOrders();
    } catch (err: any) {
      showStatusMessage("Restoran çağrılırken hata: " + err.message, "error");
    } finally {
      setIsLoading(false);
    }
  };

  const fetchOrders = async () => {
    setIsLoading(true);
    try {
      let res;
      if (selectedDate) {
        res = await OrderService.getOrdersByDaily(new Date(selectedDate));
      } else if (filterStatus) {
        res = await OrderService.getOrdersByRestaurantIdAndStatusAsync(
          filterStatus
        );
      } else {
        res = await OrderService.getOrdersByRestaurantId();
      }

      if (res.success && res.data) {
        setOrders(res.data);
      } else {
        setOrders([]);
        showStatusMessage(res.message || "Sipariş bulunamadı.", "info");
      }
    } catch (err: any) {
      showStatusMessage("Siparişler alınamadı: " + err.message, "error");
    } finally {
      setIsLoading(false);
    }
  };

  const handleStatusChange = async (
    order: OrderResponse,
    newStatus: string
  ) => {
    if (order.status === "Completed" || order.status === "Cancelled") {
      showStatusMessage(
        "Tamamlanmış veya iptal edilmiş siparişler değiştirilemez.",
        "info"
      );
      return;
    }

    if (
      !window.confirm(`Sipariş durumu '${newStatus}' olarak güncellensin mi?`)
    )
      return;

    setIsLoading(true);
    try {
      const res = await OrderService.updateOrderStatus(order.id, newStatus);
      if (!res.success) {
        showStatusMessage(res.message || "Durum güncellenemedi.", "error");
        return;
      }

      if (newStatus === "Completed") {
        const paymentRequest: CreatePaymentRequest = {
          orderId: order.id,
          paymentMethod: "Cash",
        };
        const paymentRes = await PaymentService.createPayment(paymentRequest);
        if (!paymentRes.success) {
          showStatusMessage(
            paymentRes.message || "Ödeme oluşturulamadı.",
            "error"
          );
          return;
        }

        const invoiceRes = await InvoiceService.createInvoice(order.id);
        if (!invoiceRes.success) {
          showStatusMessage(
            invoiceRes.message || "Fatura oluşturulamadı.",
            "error"
          );
          return;
        }

        showStatusMessage(
          "Sipariş tamamlandı, ödeme ve fatura oluşturuldu.",
          "success"
        );
      } else {
        showStatusMessage("Sipariş durumu başarıyla güncellendi.", "success");
      }

      if (restaurantId) fetchOrders();
    } catch (err: any) {
      showStatusMessage("Durum güncellenemedi: " + err.message, "error");
    } finally {
      setIsLoading(false);
    }
  };

  const handleCancelOrder = async (order: OrderResponse) => {
    if (order.status === "Completed" || order.status === "Cancelled") {
      showStatusMessage(
        "Sipariş zaten tamamlanmış veya iptal edilmiş.",
        "info"
      );
      return;
    }

    if (!window.confirm("Siparişi iptal etmek istediğinizden emin misiniz?"))
      return;

    setIsLoading(true);
    try {
      const res = await OrderService.updateOrderStatus(order.id, "Cancelled");
      if (res.success) {
        showStatusMessage("Sipariş başarıyla iptal edildi.", "success");
        if (restaurantId) fetchOrders();
      } else {
        showStatusMessage(res.message || "Sipariş iptal edilemedi.", "error");
      }
    } catch (err: any) {
      showStatusMessage("Sipariş iptal edilemedi: " + err.message, "error");
    } finally {
      setIsLoading(false);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case "Pending":
        return "#f39c12";
      case "Preparing":
        return "#3498db";
      case "Ready":
        return "#2ecc71";
      case "Served":
        return "#1abc9c";
      case "Completed":
        return "#2c3e50";
      case "Cancelled":
        return "#e74c3c";
      default:
        return "#bdc3c7";
    }
  };

  const renderStatusButton = (order: OrderResponse) => {
    const currentIndex = STATUS_OPTIONS.indexOf(order.status);
    const nextIndex = currentIndex + 1;
    const nextStatus = STATUS_OPTIONS[nextIndex];

    if (order.status === "Completed" || order.status === "Cancelled") {
      return null;
    }

    return (
      <button
        className="update-status-btn"
        onClick={() => handleStatusChange(order, nextStatus)}
        disabled={!nextStatus}
      >
        <FaRegPaperPlane /> {nextStatus ? `${nextStatus}` : "Güncelle"}
      </button>
    );
  };

  const renderCancelButton = (order: OrderResponse) => {
    if (order.status === "Completed" || order.status === "Cancelled") {
      return null;
    }
    return (
      <button
        className="cancel-order-btn"
        onClick={() => handleCancelOrder(order)}
      >
        <FaTimes /> İptal Et
      </button>
    );
  };

  //hub
  useEffect(() => {
    if (!restaurantId) return;
    const connection = new HubConnectionBuilder()
      .withUrl("http://localhost:5164/hubs", { withCredentials: true })
      .withAutomaticReconnect()
      .build();

    connection
      .start()
      .then(() => {
        console.log("SignalR connected");
        connection.invoke("JoinRestaurantGroup", restaurantId);
        connection.on("OrderChanged", () => {
          fetchOrders();
        });
        connection.on("MakePayment", () => {
          fetchOrders();
        });
      })
      .catch((err) => console.error("SignalR connection error:", err));
    setHubConnection(connection);
    return () => {
      connection.stop();
    };
  }, [restaurantId]);

  return (
    <div className="order-management-container">
      <div className="page-header">
        <h2 className="page-title">Sipariş Yönetimi</h2>
        <div className="status-container">
          {isLoading && <FaSpinner className="spinner loading-spinner" />}
          {statusMessage && (
            <div className={`status-message ${statusMessage.type}`}>
              {statusMessage.message}
            </div>
          )}
        </div>
      </div>

      <div className="filter-area">
        <div className="filter-buttons">
          <button
            className={`filter-btn ${
              filterStatus === "" && !selectedDate ? "active" : ""
            }`}
            onClick={() => {
              setFilterStatus("");
              setSelectedDate("");
            }}
          >
            Tümü
          </button>
          {STATUS_OPTIONS.map((status) => (
            <button
              key={status}
              className={`filter-btn ${
                filterStatus === status ? "active" : ""
              }`}
              onClick={() => {
                setFilterStatus(status);
                setSelectedDate("");
              }}
            >
              {STATUS_ICONS[status]} {status}
            </button>
          ))}
        </div>
        <div className="date-filter">
          <FaCalendarAlt />
          <input
            type="date"
            value={selectedDate}
            onChange={(e) => {
              setSelectedDate(e.target.value);
              setFilterStatus("");
            }}
          />
        </div>
      </div>

      <div className="order-list">
        {orders.length === 0 && !isLoading ? (
          <div className="info-message">
            <FaInfoCircle /> Gösterilecek sipariş bulunamadı.
          </div>
        ) : (
          orders.map((o) => (
            <div key={o.id} className="order-card">
              <div className="card-header">
                <span className="order-number">
                  Sipariş No: #{o.orderNumber}
                </span>
                <div
                  className="order-status-badge"
                  style={{ backgroundColor: getStatusColor(o.status) }}
                >
                  {STATUS_ICONS[o.status]} {o.status}
                </div>
              </div>
              <div className="card-body">
                <p>
                  <strong>Masa No:</strong> {o.tableId || "Yok"}
                </p>
                <p>
                  <strong>Çalışan ID:</strong> {o.employeeId || "Yok"}
                </p>
                <p>
                  <strong>Toplam:</strong> {o.totalAmount.toFixed(2)} ₺
                </p>
                <p>
                  <strong>Tarih:</strong>{" "}
                  {new Date(o.orderDate).toLocaleString()}
                </p>
                <p>
                  <strong>Adres:</strong> {o.shippingAddress || "Restoran içi"}
                </p>
              </div>
              <div className="card-footer">
                {renderStatusButton(o)}
                {renderCancelButton(o)}
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}
