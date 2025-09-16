import type { ApiResponse } from "../types/ApiResponse";
import type { InvoiceResponse } from "../types/Invoice/InvoiceResponse";
import { BaseService } from "./BaseService";

export class InvoiceService {
  private static readonly APIURL = "http://localhost:5164/api";

  static async getInvoicesByRestaurantId(): Promise<
    ApiResponse<InvoiceResponse[]>
  > {
    return BaseService.requestWithToken<InvoiceResponse[]>(
      `${this.APIURL}/Invoice/get-all-invoice-by-restaurant-id`
    );
  }

  static async getInvoicesByDaily(
    startDate: Date
  ): Promise<ApiResponse<InvoiceResponse[]>> {
    const formattedDate = startDate.toISOString().split("T")[0];
    return BaseService.requestWithToken<InvoiceResponse[]>(
      `${this.APIURL}/Invoice/get-all-invoices-by-daily?startDate=${formattedDate}`
    );
  }

  static async getInvoiceById(
    invoiceId: number
  ): Promise<ApiResponse<InvoiceResponse>> {
    return BaseService.requestWithToken<InvoiceResponse>(
      `${this.APIURL}/Invoice/get-invoice-by-id/${invoiceId}`
    );
  }

  static async createInvoice(
    orderId: number
  ): Promise<ApiResponse<InvoiceResponse>> {
    return BaseService.requestWithToken<InvoiceResponse>(
      `${this.APIURL}/Invoice/create-invoice/${orderId}`,
      {
        method: "POST",
      }
    );
  }
}
