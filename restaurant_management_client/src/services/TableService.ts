import type { ApiResponse } from "../types/ApiResponse";
import type { CreateTableRequest } from "../types/Table/CreateTableRequest";
import type { TableResponse } from "../types/Table/TableResponse";
import type { UpdateTableRequest } from "../types/Table/UpdateTableRequest";
import type { UpdateTableStatusRequest } from "../types/Table/UpdateTableStatusRequest";
import { BaseService } from "./BaseService";

export class TableService {
  private static readonly APIURL = "http://localhost:5164/api";

  static async getTablesByRestaurantId(): Promise<
    ApiResponse<TableResponse[]>
  > {
    return BaseService.requestWithToken<TableResponse[]>(
      `${this.APIURL}/Table/get-all-tables-by-restaurant-id`
    );
  }

  static async getTablesByRestaurantIdAndStatusAsync(
    status: string
  ): Promise<ApiResponse<TableResponse[]>> {
    return BaseService.requestWithToken<TableResponse[]>(
      `${this.APIURL}/Table/get-tables-by-restaurant-id-and-status/${status}`
    );
  }

  static async getTableById(id: number): Promise<ApiResponse<TableResponse>> {
    return BaseService.requestWithToken<TableResponse>(
      `${this.APIURL}/Table/get-table-by-id/${id}`
    );
  }

  static async getTableCountByRestaurantId(): Promise<ApiResponse<string>> {
    return BaseService.requestWithToken<string>(
      `${this.APIURL}/Table/get-table-count-by-restaurant-id`
    );
  }

  static async getTableCountByRestaurantIdAndStatus(
    status: string
  ): Promise<ApiResponse<string>> {
    return BaseService.requestWithToken<string>(
      `${this.APIURL}/Table/get-table-count-by-restaurant-id-and-status/${status}`
    );
  }

  static async createTable(
    table: CreateTableRequest
  ): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/Table/create-table`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(table),
      }
    );
  }

  static async updateTable(
    employee: UpdateTableRequest
  ): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/Table/update-table`,
      {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(employee),
      }
    );
  }

  static async updateTableStatus(
    tableStatus: UpdateTableStatusRequest
  ): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/Table/update-table-status`,
      {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(tableStatus),
      }
    );
  }

  static async deleteTable(id: number): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/Table/delete-table/${id}`,
      {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
      }
    );
  }
}
