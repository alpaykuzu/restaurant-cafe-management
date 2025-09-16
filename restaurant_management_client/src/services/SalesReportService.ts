import type { ApiResponse } from "../types/ApiResponse";
import type { SalesReportRequest } from "../types/SalesReport/SalesReportRequest";
import type { SalesReportResponse } from "../types/SalesReport/SalesReportResponse";
import { BaseService } from "./BaseService";

export class SalesReportService {
  private static readonly APIURL = "http://localhost:5164/api";

  static async generateSalesReport(
    salesReport: SalesReportRequest
  ): Promise<ApiResponse<SalesReportResponse>> {
    return BaseService.requestWithToken<SalesReportResponse>(
      `${this.APIURL}/SalesReport/generate`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(salesReport),
      }
    );
  }
}
