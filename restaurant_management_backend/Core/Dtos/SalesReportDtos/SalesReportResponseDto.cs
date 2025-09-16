namespace Core.Dtos.SalesReportDtos
{
    public class SalesReportResponseDto
    {
        public DateTime ReportDate { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
    }
}
