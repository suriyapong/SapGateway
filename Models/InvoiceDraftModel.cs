using System.Text.Json.Serialization;
using static SapGateway.Models.SAPPurchaseRequestModel;

namespace SapGateway.Models
{
    public class InvoiceDraftModel
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string DocType { get; set; }

        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }

        public string CardCode { get; set; }
        public string CardName { get; set; }

        public double DocTotal { get; set; }
        public string DocCurrency { get; set; }

        public string Comments { get; set; }

        public List<DocumentLine> DocumentLines { get; set; }

        public List<DocumentInstallment> DocumentInstallments { get; set; }
    }

    public class DocumentLine
    {
        public int LineNum { get; set; }
        public string ItemDescription { get; set; }
        public double Price { get; set; }
        public double LineTotal { get; set; }
        public string AccountCode { get; set; }
        public string ProjectCode { get; set; }
    }

    public class DocumentInstallment
    {
        public DateTime DueDate { get; set; }
        public double Percentage { get; set; }
        public double Total { get; set; }
    }
}
