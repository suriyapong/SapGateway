using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapGateway.Models
{
    public class SAPPurchaseRequestModel
    {
        public string DocumentType {  get; set; }
        public string DocObjectCode { get; set; }
        public string DocType { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string Comments { get; set; }
        public string RequesterName { get; set; }
        public string? RequesterEmail { get; set; }
        public DateTime RequriedDate { get; set; }
        public List<DocumentLine> DocumentLines { get; set; }

        public static implicit operator SAPPurchaseRequestModel(SAPPurchaseOrderModel v)
        {
            throw new NotImplementedException();
        }

        public class DocumentLine
        {
            public string? ItemCode { get; set; } //item
            public string? ItemDescription { get; set; } //item, Service
            public DateTime? RequiredDate { get; set; } //item, Service
            public double? Quantity { get; set; } //item, Service
            public double? UnitPrice { get; set; } //item, Service
            public string? VatGroup { get; set; } //item
            public string? AccountCode { get; set; } //item, Service
            public string? CostingCode { get; set; } //item
            public int? UoMEntry { get; set; } //item
            public string? Currency { get; set; } //item, Service
        }
    }
}