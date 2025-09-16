/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useState, useCallback } from "react";
import { useAuth } from "../context/AuthProvider";
import { PaymentService } from "../services/PaymentService";
import { OrderService } from "../services/OrderService";
import type { PaymentResponse } from "../types/Payment/PaymentResponse";
import "./../styles/PaymentManagement.css";
import { RestaurantService } from "../services/RestaurantService";
import { Bar } from "react-chartjs-2";
import { HubConnectionBuilder, HubConnection } from "@microsoft/signalr";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
} from "chart.js";
import type { OrderResponse } from "../types/Order/OrderResponse";
import type { RestaurantResponse } from "../types/Restaurant/RestaurantResponse";
import {
  FaSpinner,
  FaTimes,
  FaCheckCircle,
  FaInfoCircle,
} from "react-icons/fa";

ChartJS.register(
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend
);

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
      {type === "success" && <FaCheckCircle />}
      {type === "info" && <FaInfoCircle />}
      {message}
    </div>
  );
};

export default function PaymentManagement() {
  const { id: userId } = useAuth();
  const [restaurant, setRestaurant] = useState<RestaurantResponse | null>(null);
  const [payments, setPayments] = useState<PaymentResponse[]>([]);
  const [orders, setOrders] = useState<OrderResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [statusMessage, setStatusMessage] = useState<{
    message: string;
    type: "success" | "error" | "info";
  } | null>(null);
  const [, setHubConnection] = useState<HubConnection | null>(null);

  const showStatus = (message: string, type: "success" | "error" | "info") => {
    setStatusMessage({ message, type });
    setTimeout(() => setStatusMessage(null), 3000);
  };

  const fetchData = useCallback(async () => {
    if (!userId) {
      showStatus("Kullanıcı kimliği bulunamadı.", "error");
      return;
    }
    setLoading(true);

    try {
      const resRestaurant = await RestaurantService.getRestaurantByUserId();
      if (!resRestaurant.success || !resRestaurant.data) {
        showStatus(
          resRestaurant.message || "Restoran bilgisi alınamadı.",
          "info"
        );
        return;
      }
      setRestaurant(resRestaurant.data);

      const [resPayments, resOrders] = await Promise.all([
        PaymentService.getPaymentsByRestaurantId(),
        OrderService.getOrdersByRestaurantId(),
      ]);

      if (resPayments.success && resPayments.data) {
        setPayments(resPayments.data);
      } else {
        showStatus(resPayments.message || "Ödemeler yüklenemedi.", "error");
      }

      if (resOrders.success && resOrders.data) {
        setOrders(resOrders.data);
      } else {
        showStatus(resOrders.message || "Siparişler yüklenemedi.", "error");
      }
    } catch (err: any) {
      showStatus("Veriler yüklenirken bir hata oluştu.", "error");
      console.error("Veri çekme hatası:", err);
    } finally {
      setLoading(false);
    }
  }, [userId]);

  // ilk veri çekme
  useEffect(() => {
    fetchData();
  }, [fetchData]);

  // Hub bağlantısı
  useEffect(() => {
    if (!restaurant?.id) return;

    const connection = new HubConnectionBuilder()
      .withUrl("http://localhost:5164/hubs", {
        withCredentials: true,
      })
      .withAutomaticReconnect()
      .build();

    connection
      .start()
      .then(() => {
        connection.invoke("JoinRestaurantGroup", restaurant.id);
        connection.on("MakePayment", () => {
          fetchData();
        });
      })
      .catch((err) => console.error("SignalR connection error:", err));

    setHubConnection(connection);

    return () => {
      connection.stop();
    };
  }, [restaurant?.id, fetchData]);

  const dailyData = payments.reduce((acc: Record<string, number>, p) => {
    const date = new Date(p.paymentDate).toLocaleDateString("tr-TR");
    acc[date] = (acc[date] || 0) + p.amount;
    return acc;
  }, {});

  const monthlyData = payments.reduce((acc: Record<string, number>, p) => {
    const month = new Date(p.paymentDate).toLocaleString("tr-TR", {
      month: "long",
      year: "numeric",
    });
    acc[month] = (acc[month] || 0) + p.amount;
    return acc;
  }, {});

  const dailyChart = {
    labels: Object.keys(dailyData),
    datasets: [
      {
        label: "Günlük Ödemeler (₺)",
        data: Object.values(dailyData),
        backgroundColor: "rgba(75, 192, 192, 0.8)",
        borderColor: "rgba(75, 192, 192, 1)",
        borderWidth: 1,
      },
    ],
  };

  const monthlyChart = {
    labels: Object.keys(monthlyData),
    datasets: [
      {
        label: "Aylık Ödemeler (₺)",
        data: Object.values(monthlyData),
        backgroundColor: "rgba(153, 102, 255, 0.8)",
        borderColor: "rgba(153, 102, 255, 1)",
        borderWidth: 1,
      },
    ],
  };

  const getOrderById = (orderId: number) => {
    return orders.find((o) => o.id === orderId) || null;
  };

  return (
    <div className="payment-management-container">
      <div className="page-header">
        <h2 className="page-title">
          {restaurant
            ? `${restaurant.name} - Ödeme Yönetimi`
            : "Ödeme Yönetimi"}
        </h2>
        <div className="status-container">
          {loading && <FaSpinner className="spinner loading-spinner" />}
          <StatusMessage
            message={statusMessage?.message || null}
            type={statusMessage?.type || null}
          />
        </div>
      </div>

      {/* Grafikler */}
      <div className="chart-container">
        <div className="content-card chart-section">
          <div className="card-header">
            <h3 className="card-title">Günlük Ödemeler</h3>
          </div>
          <div className="card-body">
            <Bar data={dailyChart} />
          </div>
        </div>

        <div className="content-card chart-section">
          <div className="card-header">
            <h3 className="card-title">Aylık Ödemeler</h3>
          </div>
          <div className="card-body">
            <Bar data={monthlyChart} />
          </div>
        </div>
      </div>

      <div className="content-card">
        <div className="card-header">
          <h3 className="card-title">Tüm Ödemeler</h3>
        </div>
        <div className="table-wrapper">
          {payments.length === 0 && !loading ? (
            <div className="info-message">
              <FaInfoCircle /> Henüz ödeme kaydı bulunmamaktadır.
            </div>
          ) : (
            <table className="data-table">
              <thead>
                <tr>
                  <th>Ödeme ID</th>
                  <th>Sipariş No</th>
                  <th>Sipariş Tutarı (₺)</th>
                  <th>Ödeme Tutarı (₺)</th>
                  <th>Ödeme Yöntemi</th>
                  <th>Durum</th>
                  <th>Tarih</th>
                  <th>Detaylar</th>
                </tr>
              </thead>
              <tbody>
                {payments.map((p) => {
                  const order = getOrderById(p.orderId);
                  return (
                    <tr key={p.id}>
                      <td data-label="Ödeme ID">{p.id}</td>
                      <td data-label="Sipariş No">
                        {order?.orderNumber || "-"}
                      </td>
                      <td data-label="Sipariş Tutarı">
                        {order?.totalAmount?.toFixed(2) ?? "-"} ₺
                      </td>
                      <td data-label="Ödeme Tutarı">
                        {p.amount?.toFixed(2) ?? "-"} ₺
                      </td>
                      <td data-label="Ödeme Yöntemi">{p.paymentMethod}</td>
                      <td data-label="Durum">{p.status}</td>
                      <td data-label="Tarih">
                        {new Date(p.paymentDate).toLocaleString("tr-TR")}
                      </td>
                      <td data-label="Detaylar">
                        {order?.orderItems?.length ? (
                          <ul className="order-items-list">
                            {order.orderItems.map((item, i) => (
                              <li key={i}>
                                {item.menuItemName} x{item.quantity} (
                                {item.unitPrice?.toFixed(2)}₺)
                              </li>
                            ))}
                          </ul>
                        ) : (
                          "-"
                        )}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </div>
  );
}
