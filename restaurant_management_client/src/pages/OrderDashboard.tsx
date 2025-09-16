/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable react-hooks/exhaustive-deps */
import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthProvider";
import { OrderService } from "../services/OrderService";
import { RestaurantService } from "../services/RestaurantService";
import type { OrderResponse } from "../types/Order/OrderResponse";
import "./../styles/OrderDashboard.css";
import { HubConnectionBuilder, HubConnection } from "@microsoft/signalr";
import {
  FaSpinner,
  FaUtensils,
  FaCheck,
  FaCheckDouble,
  FaBicycle,
  FaInfoCircle,
  FaTimes,
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
      {type === "error" && <FaInfoCircle />}
      {type === "success" && <FaCheck />}
      {type === "info" && <FaInfoCircle />}
      <span>{message}</span>
    </div>
  );
};

export default function OrderDashboard() {
  const { id: userId, roles } = useAuth();
  const [restaurantId, setRestaurantId] = useState<number | null>(null);
  const [orders, setOrders] = useState<OrderResponse[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [statusMessage, setStatusMessage] = useState<{
    message: string;
    type: "success" | "error" | "info";
  } | null>(null);
  const [, setHubConnection] = useState<HubConnection | null>(null);

  const showStatus = (message: string, type: "success" | "error" | "info") => {
    setStatusMessage({ message, type });
    setTimeout(() => setStatusMessage(null), 4000);
  };

  const fetchOrders = async () => {
    setIsLoading(true);
    try {
      const res = await OrderService.getOrdersByRestaurantId();
      if (!res.success) {
        showStatus(res.message || "Siparişler alınamadı.", "error");
        return;
      }

      let filtered: OrderResponse[] = [];
      if (roles.includes("Kitchen")) {
        filtered = res.data.filter(
          (o) => o.status === "Pending" || o.status === "Preparing"
        );
      } else if (roles.includes("Waiter")) {
        filtered = res.data.filter((o) => o.status === "Ready");
      }
      setOrders(filtered);
    } catch (err: any) {
      showStatus("Sipariş verileri alınırken bir hata oluştu.", "error");
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    const fetchRestaurantIdAndOrders = async () => {
      if (!userId) return;
      setIsLoading(true);
      try {
        const res = await RestaurantService.getRestaurantByUserId();
        if (res.success && res.data) {
          setRestaurantId(res.data.id);
          fetchOrders();
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
    fetchRestaurantIdAndOrders();

    const interval = setInterval(() => {
      if (restaurantId) fetchOrders();
    }, 5000);

    return () => clearInterval(interval);
  }, [userId, restaurantId]);

  const handleUpdateStatus = async (order: OrderResponse) => {
    let newStatus = "";
    if (roles.includes("Kitchen")) {
      if (order.status === "Pending") newStatus = "Preparing";
      else if (order.status === "Preparing") newStatus = "Ready";
    } else if (roles.includes("Waiter")) {
      if (order.status === "Ready") newStatus = "Served";
    }

    if (!newStatus) {
      showStatus("Güncellenecek durum bulunamadı.", "info");
      return;
    }

    setIsLoading(true);
    try {
      const res = await OrderService.updateOrderStatus(order.id, newStatus);
      if (!res.success) {
        showStatus("Güncelleme başarısız: " + res.message, "error");
        return;
      }
      showStatus("Sipariş durumu başarıyla güncellendi.", "success");
      if (restaurantId) fetchOrders();
    } catch (err: any) {
      showStatus(
        "Durum güncellenirken bir hata oluştu: " + err.message,
        "error"
      );
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleCancelOrder = async (order: OrderResponse) => {
    if (!window.confirm("Siparişi iptal etmek istediğinizden emin misiniz?")) {
      return;
    }
    setIsLoading(true);
    try {
      const res = await OrderService.updateOrderStatus(order.id, "Cancelled");
      if (res.success) {
        showStatus("Sipariş başarıyla iptal edildi.", "success");
        if (restaurantId) fetchOrders();
      } else {
        showStatus(res.message || "Sipariş iptal edilemedi.", "error");
      }
    } catch (err: any) {
      showStatus(
        "Sipariş iptal edilirken bir hata oluştu: " + err.message,
        "error"
      );
    } finally {
      setIsLoading(false);
    }
  };

  const getStatusClass = (status: string) => {
    switch (status) {
      case "Pending":
        return "status-pending";
      case "Preparing":
        return "status-preparing";
      case "Ready":
        return "status-ready";
      case "Served":
        return "status-served";
      case "Cancelled":
        return "status-cancelled";
      default:
        return "";
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case "Pending":
        return "Bekliyor";
      case "Preparing":
        return "Hazırlanıyor";
      case "Ready":
        return "Hazır";
      case "Served":
        return "Servis Edildi";
      case "Cancelled":
        return "İptal Edildi";
      default:
        return status;
    }
  };

  const getActionButton = (order: OrderResponse) => {
    const buttons = [];
    if (roles.includes("Kitchen")) {
      if (order.status === "Pending") {
        buttons.push(
          <button
            key="start-btn"
            className="order-button primary-btn"
            onClick={() => handleUpdateStatus(order)}
          >
            <FaUtensils /> Hazırlamaya Başla
          </button>
        );
      } else if (order.status === "Preparing") {
        buttons.push(
          <button
            key="complete-btn"
            className="order-button success-btn"
            onClick={() => handleUpdateStatus(order)}
          >
            <FaCheck /> Tamamlandı
          </button>
        );
      }
    } else if (roles.includes("Waiter")) {
      if (order.status === "Ready") {
        buttons.push(
          <button
            key="serve-btn"
            className="order-button success-btn"
            onClick={() => handleUpdateStatus(order)}
          >
            <FaBicycle /> Servis Et
          </button>
        );
      }
    }

    if (
      order.status !== "Cancelled" &&
      order.status !== "Served" &&
      order.status !== "Completed"
    ) {
      buttons.push(
        <button
          key="cancel-btn"
          className="order-button cancel-btn"
          onClick={() => handleCancelOrder(order)}
        >
          <FaTimes /> İptal Et
        </button>
      );
    }

    return buttons;
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
    <div className="order-dashboard-container">
      <div className="dashboard-header">
        <h2 className="dashboard-title">
          {roles.includes("Kitchen")
            ? "Mutfak Paneli"
            : roles.includes("Waiter")
            ? "Servis Paneli"
            : "Sipariş Paneli"}
        </h2>
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
          <h3>Aktif Siparişler</h3>
        </div>
        {orders.length > 0 ? (
          <div className="orders-grid">
            {orders.map((order) => (
              <div
                key={order.id}
                className={`order-card ${getStatusClass(order.status)}`}
              >
                <div className="card-header">
                  <h4>Sipariş #{order.orderNumber}</h4>
                  <span
                    className={`status-badge ${getStatusClass(order.status)}`}
                  >
                    {getStatusText(order.status)}
                  </span>
                </div>
                <div className="card-body">
                  <p>Masa: {order.tableNumber}</p>
                  <div className="order-items">
                    <p className="items-title">Ürünler:</p>
                    <ul>
                      {order.orderItems.map((item, idx) => (
                        <li key={idx}>
                          {item.menuItemName}{" "}
                          <span className="item-quantity">
                            x {item.quantity}
                          </span>
                        </li>
                      ))}
                    </ul>
                  </div>
                </div>
                <div className="card-actions">{getActionButton(order)}</div>
              </div>
            ))}
          </div>
        ) : (
          <div className="no-data-message">
            <FaCheckDouble size={48} />
            <p>
              {roles.includes("Kitchen")
                ? "Şu anda hazırlık bekleyen sipariş yok."
                : "Şu anda servis bekleyen sipariş yok."}
            </p>
          </div>
        )}
      </div>
    </div>
  );
}
