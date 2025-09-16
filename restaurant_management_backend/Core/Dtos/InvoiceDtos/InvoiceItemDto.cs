namespace Core.Dtos.InvoiceDtos
{
    public class InvoiceItemDto
    {
        public string ItemName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }
}
