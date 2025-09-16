import type { OrderItemResponse } from "./OrderItemResponse";

export interface OrderResponse {
  id: number;
  restaurantId: number;
  tableId: number;
  tableNumber: number;
  employeeId: number;
  orderNumber: number;
  status: string; //"Pending", "Preparing", "Ready", "Served", "Completed", "Cancelled"
  orderDate: Date;
  totalAmount: number;
  shippingAddress: string;
  orderItems: OrderItemResponse[];
}
