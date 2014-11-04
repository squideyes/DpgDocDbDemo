using Microsoft.Azure.Documents;
using System;

namespace DpgDocDbDemo
{
    // use to get to internal properties
    public class SalesOrderDocument : Document
    {
        public string PurchaseOrderNumber
        {
            get { return GetValue<string>("PurchaseOrderNumber"); }
            set { SetValue("PurchaseOrderNumber", value); }
        }

        public DateTime OrderDate
        {
            get { return GetValue<DateTime>("OrderDate"); }
            set { SetValue("OrderDate", value); }
        }

        public DateTime ShipDate
        {
            get { return GetValue<DateTime>("ShipDate"); }
            set { SetValue("ShipDate", value); }
        }

        public string AccountNumber
        {
            get { return GetValue<string>("AccountNumber"); }
            set { SetValue("AccountNumber", value); }
        }

        public decimal SubTotal
        {
            get { return GetValue<decimal>("SubTotal"); }
            set { SetValue("SubTotal", value); }
        }

        public decimal TaxAmt
        {
            get { return GetValue<decimal>("TaxAmt"); }
            set { SetValue("TaxAmt", value); }
        }

        public decimal Freight
        {
            get { return GetValue<decimal>("Freight"); }
            set { SetValue("Freight", value); }
        }

        public decimal TotalDue
        {
            get { return GetValue<decimal>("TotalDue"); }
            set { SetValue("TotalDue", value); }
        }

        public SalesOrderDetail1[] Items
        {
            get { return GetValue<SalesOrderDetail1[]>("Item"); }
            set { SetValue("Item", value); }
        }
    }
}
