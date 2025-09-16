/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState, useCallback } from "react";
import { InvoiceService } from "../services/InvoiceService";
import { RestaurantService } from "../services/RestaurantService";
import type { InvoiceResponse } from "../types/Invoice/InvoiceResponse";
import type { RestaurantResponse } from "../types/Restaurant/RestaurantResponse";
import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";
import "./../styles/InvoiceManagement.css";
import { useAuth } from "../context/AuthProvider";
import { HubConnectionBuilder, HubConnection } from "@microsoft/signalr";
import {
  FaDownload,
  FaSpinner,
  FaTimes,
  FaCheckCircle,
  FaInfoCircle,
  FaCalendarAlt,
  FaFilePdf,
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
      {type === "success" && <FaCheckCircle />}
      {type === "info" && <FaInfoCircle />}
      {message}
    </div>
  );
};

const InvoiceManagement: React.FC = () => {
  const { id: userId } = useAuth();
  const [invoices, setInvoices] = useState<InvoiceResponse[]>([]);
  const [restaurant, setRestaurant] = useState<RestaurantResponse | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [statusMessage, setStatusMessage] = useState<{
    message: string;
    type: "success" | "error" | "info";
  } | null>(null);
  const [selectedDate, setSelectedDate] = useState<string>(
    new Date().toISOString().split("T")[0]
  );
  const [, setHubConnection] = useState<HubConnection | null>(null);

  const showStatus = (message: string, type: "success" | "error" | "info") => {
    setStatusMessage({ message, type });
    setTimeout(() => setStatusMessage(null), 3000);
  };

  //Fatura + restoran verilerini çeken fonksiyon
  const fetchData = useCallback(async () => {
    if (!userId) {
      showStatus("Kullanıcı kimliği bulunamadı.", "error");
      return;
    }
    setLoading(true);

    try {
      const restaurantRes = await RestaurantService.getRestaurantByUserId();
      if (!restaurantRes.success || !restaurantRes.data) {
        showStatus(
          restaurantRes.message || "Restoran bilgisi alınamadı.",
          "info"
        );
        setLoading(false);
        return;
      }
      setRestaurant(restaurantRes.data);

      let invoiceRes;
      if (selectedDate) {
        invoiceRes = await InvoiceService.getInvoicesByDaily(
          new Date(selectedDate)
        );
      } else {
        invoiceRes = await InvoiceService.getInvoicesByRestaurantId();
      }

      if (invoiceRes.success && invoiceRes.data) {
        setInvoices(invoiceRes.data);
      } else {
        showStatus(invoiceRes.message || "Faturalar yüklenemedi.", "error");
      }
    } catch (err: any) {
      showStatus("Fatura verileri alınırken bir hata oluştu.", "error");
      console.error("Fatura verileri alınırken hata:", err.message);
    } finally {
      setLoading(false);
    }
  }, [userId, selectedDate]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  //hub
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
        connection.on("CreateInvoice", () => {
          fetchData();
        });
      })
      .catch((err) => console.error("SignalR connection error:", err));

    setHubConnection(connection);

    return () => {
      connection.stop();
    };
  }, [restaurant?.id, fetchData]);

  // PDF indirme
  const handleDownloadPDF = (invoice: InvoiceResponse) => {
    if (!restaurant) {
      showStatus("Restoran bilgisi bulunamadı!", "error");
      return;
    }

    const doc = new jsPDF();

    doc.setFontSize(18);
    doc.text(`Fatura #${invoice.id}`, 14, 20);
    doc.setFontSize(10);
    doc.text(restaurant.name, 14, 30);
    doc.text(restaurant.address, 14, 36);
    doc.text(`Tel: ${restaurant.phone}`, 14, 42);
    doc.text(
      `Tarih: ${new Date(invoice.issuedAt).toLocaleDateString("tr-TR")}`,
      150,
      30
    );
    doc.text(`Sipariş No: ${invoice.orderNumber}`, 150, 36);

    const tableData = invoice.items.map((item) => [
      item.itemName,
      item.quantity,
      `${item.unitPrice.toFixed(2)} ₺`,
      `${item.lineTotal.toFixed(2)} ₺`,
    ]);

    autoTable(doc, {
      head: [["Ürün", "Adet", "Birim Fiyat", "Toplam"]],
      body: tableData,
      startY: 55,
      theme: "striped",
      styles: { cellPadding: 2, fontSize: 10 },
      headStyles: { fillColor: [52, 73, 94] },
    });

    const finalY = (doc as any).lastAutoTable.finalY;
    doc.setFontSize(12);
    doc.text(
      `Genel Toplam: ${invoice.totalAmount.toFixed(2)} ₺`,
      14,
      finalY + 15
    );

    doc.save(`Fatura_${invoice.id}.pdf`);
  };

  // ✅ Tüm Faturaları PDF Olarak İndirme
  const handleDownloadAllPDFs = () => {
    if (invoices.length === 0) {
      showStatus("İndirilecek fatura bulunamadı.", "info");
      return;
    }
    if (!restaurant) {
      showStatus("Restoran bilgisi bulunamadı!", "error");
      return;
    }

    const doc = new jsPDF();
    let yOffset = 20;

    invoices.forEach((invoice, index) => {
      // Her fatura için yeni sayfa
      if (index > 0) {
        doc.addPage();
        yOffset = 20;
      }

      doc.setFontSize(18);
      doc.text(`Fatura #${invoice.id}`, 14, yOffset);
      yOffset += 10;
      doc.setFontSize(10);
      doc.text(restaurant.name, 14, yOffset);
      doc.text(restaurant.address, 14, yOffset + 6);
      doc.text(`Tel: ${restaurant.phone}`, 14, yOffset + 12);
      doc.text(
        `Tarih: ${new Date(invoice.issuedAt).toLocaleDateString("tr-TR")}`,
        150,
        yOffset
      );
      doc.text(`Sipariş No: ${invoice.orderNumber}`, 150, yOffset + 6);
      yOffset += 30;

      const tableData = invoice.items.map((item) => [
        item.itemName,
        item.quantity,
        `${item.unitPrice.toFixed(2)} ₺`,
        `${item.lineTotal.toFixed(2)} ₺`,
      ]);

      autoTable(doc, {
        head: [["Ürün", "Adet", "Birim Fiyat", "Toplam"]],
        body: tableData,
        startY: yOffset,
        theme: "striped",
        styles: { cellPadding: 2, fontSize: 10 },
        headStyles: { fillColor: [52, 73, 94] },
      });

      yOffset = (doc as any).lastAutoTable.finalY;
      doc.setFontSize(12);
      doc.text(
        `Genel Toplam: ${invoice.totalAmount.toFixed(2)} ₺`,
        14,
        yOffset + 15
      );
    });

    const formattedDate = new Date(selectedDate).toLocaleDateString("tr-TR");
    doc.save(`Tüm_Faturalar_${formattedDate}.pdf`);
    showStatus("Faturalar başarıyla indirildi.", "success");
  };

  return (
    <div className="invoice-management-container">
      <div className="page-header">
        <h2 className="page-title">Fatura Yönetimi</h2>
        <div className="status-container">
          {loading && <FaSpinner className="spinner loading-spinner" />}
          <StatusMessage
            message={statusMessage?.message || null}
            type={statusMessage?.type || null}
          />
        </div>
      </div>

      <div className="filter-area">
        <div className="date-filter">
          <FaCalendarAlt />
          <input
            type="date"
            value={selectedDate}
            onChange={(e) => setSelectedDate(e.target.value)}
          />
        </div>
        <button
          className="download-all-btn"
          onClick={handleDownloadAllPDFs}
          disabled={invoices.length === 0}
        >
          <FaFilePdf /> Tümünü İndir
        </button>
      </div>

      <div className="invoice-list-container">
        {invoices.length === 0 && !loading ? (
          <div className="info-message">
            <FaInfoCircle /> Gösterilecek fatura bulunamadı.
          </div>
        ) : (
          <div className="invoice-grid">
            {invoices.map((invoice) => (
              <div key={invoice.id} className="invoice-card">
                <div className="card-header">
                  <h3 className="card-title">Fatura #{invoice.id}</h3>
                </div>
                <div className="card-body">
                  <p>
                    <strong>Tarih:</strong>{" "}
                    {new Date(invoice.issuedAt).toLocaleDateString("tr-TR")}
                  </p>
                  <p>
                    <strong>Sipariş No:</strong> {invoice.orderNumber}
                  </p>
                  <p>
                    <strong>Toplam Tutar:</strong>{" "}
                    {invoice.totalAmount.toFixed(2)} ₺
                  </p>
                </div>
                <div className="card-footer">
                  <button
                    className="download-btn"
                    onClick={() => handleDownloadPDF(invoice)}
                  >
                    <FaDownload /> PDF İndir
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default InvoiceManagement;
