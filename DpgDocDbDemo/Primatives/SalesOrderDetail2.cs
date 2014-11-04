namespace DpgDocDbDemo
{
    public class SalesOrderDetail2
    {
        public int OrderQty { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyCode { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
