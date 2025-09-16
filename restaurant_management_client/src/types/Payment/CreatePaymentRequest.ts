export interface CreatePaymentRequest {
  orderId: number;
  paymentMethod: string; //Card, Cash, QR
}
