import type { InvoiceItemResponse } from "./InvoiceItemResponse";

export interface InvoiceResponse {
  id: number;
  orderNumber: number;
  issuedAt: Date;
  totalAmount: number;
  items: InvoiceItemResponse[];
}
