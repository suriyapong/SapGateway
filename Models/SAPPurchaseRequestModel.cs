using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapGateway.Models
{
    public class SAPPurchaseRequestModel
    {
        public string CompanyName { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string Comments { get; set; }
        public string RequesterName { get; set; }
        public string RequesterEmail { get; set; }
        public DateTime RequriedDate { get; set; }
        public List<DocumentLine> DocumentLines { get; set; }

        public class DocumentLine
        {
            public string ItemCode { get; set; }
            public string ItemDescription { get; set; }
            public double Quantity { get; set; }
            public double UnitPrice { get; set; }
            public string Currency { get; set; }
            public string VatGroup { get; set; }
            public DateTime RequiredDate { get; set; }
            public string AccountCode { get; set; }
            public string CostingCode { get; set; }
        }
    }
}