import type { ApiResponse } from "../types/ApiResponse";
import type { CreateOrderFullRequest } from "../types/Order/CreateOrderFullRequest";
import type { OrderResponse } from "../types/Order/OrderResponse";
import { BaseService } from "./BaseService";

export class OrderService {
  private static readonly APIURL = "http://localhost:5164/api";

  static async getOrdersByRestaurantId(): Promise<
    ApiResponse<OrderResponse[]>
  > {
    return BaseService.requestWithToken<OrderResponse[]>(
      `${this.APIURL}/Order/get-all-orders-by-restaurant-id`
    );
  }

  static async getOrdersByDaily(
    startDate: Date
  ): Promise<ApiResponse<OrderResponse[]>> {
    const formattedDate = startDate.toISOString().split("T")[0];
    return BaseService.requestWithToken<OrderResponse[]>(
      `${this.APIURL}/Order/get-all-orders-by-daily?startDate=${formattedDate}`
    );
  }

  static async getOrdersByRestaurantIdAndStatusAsync(
    status: string
  ): Promise<ApiResponse<OrderResponse[]>> {
    return BaseService.requestWithToken<OrderResponse[]>(
      `${this.APIURL}/Order/get-orders-by-status/${status}`
    );
  }

  static async getOrderById(id: number): Promise<ApiResponse<OrderResponse>> {
    return BaseService.requestWithToken<OrderResponse>(
      `${this.APIURL}/Order/get-order-by-id/${id}`
    );
  }

  static async createOrder(
    order: CreateOrderFullRequest
  ): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/Order/create-order`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(order),
      }
    );
  }

  static async updateOrderStatus(
    orderId: number,
    newStatus: string
  ): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/Order/update-order-status/${orderId}/${newStatus}`,
      {
        method: "PUT",
      }
    );
  }
}
