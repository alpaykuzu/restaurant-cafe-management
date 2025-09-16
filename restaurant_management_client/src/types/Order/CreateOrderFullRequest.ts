import type { CreateOrderItemRequest } from "./CreateOrderItemRequest";
import type { CreateOrderRequest } from "./CreateOrderRequest";

export interface CreateOrderFullRequest {
  order: CreateOrderRequest;
  orderItems: CreateOrderItemRequest[];
}
