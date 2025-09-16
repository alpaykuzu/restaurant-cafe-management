export interface PaymentResponse {
  id: number;
  orderId: number;
  amount: number;
  paymentMethod: string;
  paymentDate: Date;
  status: string; //Success
}
