/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthProvider";
import { SalesReportService } from "../services/SalesReportService";
import { RestaurantService } from "../services/RestaurantService";
import type { SalesReportResponse } from "../types/SalesReport/SalesReportResponse";
import type { SalesReportRequest } from "../types/SalesReport/SalesReportRequest";
import type { RestaurantResponse } from "../types/Restaurant/RestaurantResponse";
import "./../styles/SalesReportManagement.css";

import { Bar } from "react-chartjs-2";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
} from "chart.js";
import {
  FaSpinner,
  FaChartBar,
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

export default function SalesReportManagement() {
  const { id: userId } = useAuth();
  const [restaurant, setRestaurant] = useState<RestaurantResponse | null>(null);
  const [startDate, setStartDate] = useState<string>("");
  const [endDate, setEndDate] = useState<string>("");
  const [report, setReport] = useState<SalesReportResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [statusMessage, setStatusMessage] = useState<{
    message: string;
    type: "success" | "error" | "info";
  } | null>(null);

  const showStatus = (message: string, type: "success" | "error" | "info") => {
    setStatusMessage({ message, type });
    setTimeout(() => setStatusMessage(null), 3000);
  };

  useEffect(() => {
    const fetchRestaurant = async () => {
      if (!userId) return;
      try {
        const res = await RestaurantService.getRestaurantByUserId();
        if (!res.success || !res.data) {
          showStatus(res.message || "Restoran bilgisi alınamadı.", "info");
          return;
        }
        setRestaurant(res.data);
      } catch (err: any) {
        showStatus("Restoran bilgisi alınırken bir hata oluştu.", "error");
        console.error(err);
      }
    };
    fetchRestaurant();
  }, [userId]);

  const handleGenerateReport = async () => {
    if (!restaurant) {
      showStatus("Restoran bilgisi mevcut değil.", "error");
      return;
    }
    if (!startDate || !endDate) {
      showStatus("Lütfen başlangıç ve bitiş tarihlerini seçin.", "info");
      return;
    }

    setLoading(true);
    setReport(null);
    try {
      const payload: SalesReportRequest = {
        startDate: new Date(startDate),
        endDate: new Date(endDate),
      };

      const res = await SalesReportService.generateSalesReport(payload);
      if (!res.success || !res.data) {
        showStatus(res.message || "Rapor alınamadı.", "error");
        return;
      }

      setReport(res.data);
      showStatus("Rapor başarıyla oluşturuldu.", "success");
    } catch (err: any) {
      showStatus("Rapor alınırken bir hata oluştu: " + err.message, "error");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const chartData = report
    ? {
        labels: [new Date(report.reportDate).toLocaleDateString("tr-TR")],
        datasets: [
          {
            label: "Toplam Satış (₺)",
            data: [report.totalSales],
            backgroundColor: "rgba(75, 192, 192, 0.8)",
          },
          {
            label: "Ortalama Sipariş Değeri (₺)",
            data: [report.averageOrderValue],
            backgroundColor: "rgba(153, 102, 255, 0.8)",
          },
        ],
      }
    : { labels: [], datasets: [] };

  return (
    <div className="sales-report-management-container">
      <div className="page-header">
        <h2 className="page-title">
          {restaurant ? `${restaurant.name} - Satış Raporu` : "Satış Raporu"}
        </h2>
        <div className="status-container">
          {loading && <FaSpinner className="spinner loading-spinner" />}
          <StatusMessage
            message={statusMessage?.message || null}
            type={statusMessage?.type || null}
          />
        </div>
      </div>

      <div className="content-card">
        <div className="card-header">
          <h3 className="card-title">Rapor Oluştur</h3>
        </div>
        <div className="card-body">
          <div className="date-filters">
            <label className="filter-label">
              Başlangıç Tarihi
              <input
                type="date"
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
                className="filter-input"
              />
            </label>
            <label className="filter-label">
              Bitiş Tarihi
              <input
                type="date"
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
                className="filter-input"
              />
            </label>
            <button
              onClick={handleGenerateReport}
              disabled={loading}
              className="generate-report-btn"
            >
              {loading ? <FaSpinner className="btn-spinner" /> : <FaChartBar />}
              <span className="btn-text">
                {loading ? "Rapor Oluşturuluyor..." : "Raporu Göster"}
              </span>
            </button>
          </div>
        </div>
      </div>

      {report && (
        <>
          <div className="content-card report-card">
            <div className="card-header">
              <h3 className="card-title">Rapor Özeti</h3>
            </div>
            <div className="card-body">
              <div className="summary-grid">
                <div className="summary-item">
                  <h4>Toplam Satış</h4>
                  <p className="summary-value">
                    {report.totalSales.toFixed(2)} ₺
                  </p>
                </div>
                <div className="summary-item">
                  <h4>Toplam Sipariş</h4>
                  <p className="summary-value">{report.totalOrders}</p>
                </div>
                <div className="summary-item">
                  <h4>Ortalama Sipariş Değeri</h4>
                  <p className="summary-value">
                    {report.averageOrderValue.toFixed(2)} ₺
                  </p>
                </div>
              </div>
            </div>
          </div>

          <div className="content-card">
            <div className="card-header">
              <h3 className="card-title">Satış Grafiği</h3>
            </div>
            <div className="card-body">
              <Bar data={chartData} />
            </div>
          </div>
        </>
      )}

      {!loading && !report && (
        <div className="info-message">
          <FaInfoCircle /> Rapor oluşturmak için tarih aralığı seçin.
        </div>
      )}
    </div>
  );
}
