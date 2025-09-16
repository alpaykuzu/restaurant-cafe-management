import type { ApiResponse } from "../types/ApiResponse";
import type { CreatePaymentRequest } from "../types/Payment/CreatePaymentRequest";
import type { PaymentResponse } from "../types/Payment/PaymentResponse";
import { BaseService } from "./BaseService";

export class PaymentService {
  private static readonly APIURL = "http://localhost:5164/api";

  static async getPaymentsByRestaurantId(): Promise<
    ApiResponse<PaymentResponse[]>
  > {
    return BaseService.requestWithToken<PaymentResponse[]>(
      `${this.APIURL}/Payment/get-payments-by-restaurant-id`
    );
  }

  static async getPaymentByOrderId(
    orderId: number
  ): Promise<ApiResponse<PaymentResponse>> {
    return BaseService.requestWithToken<PaymentResponse>(
      `${this.APIURL}/Payment/get-payment-by-order-id/${orderId}`
    );
  }

  static async getPaymentById(
    paymentId: number
  ): Promise<ApiResponse<PaymentResponse>> {
    return BaseService.requestWithToken<PaymentResponse>(
      `${this.APIURL}/Payment/get-payment-by-id/${paymentId}`
    );
  }

  static async createPayment(
    payment: CreatePaymentRequest
  ): Promise<ApiResponse<PaymentResponse>> {
    return BaseService.requestWithToken<PaymentResponse>(
      `${this.APIURL}/Payment/make-payment`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payment),
      }
    );
  }
}
