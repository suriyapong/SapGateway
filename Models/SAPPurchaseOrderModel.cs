using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapGateway.Models
{
    public class SAPResponse<T>
    {
        public List<T>? value { get; set; }
    }

    public class SAPPurchaseOrderModel
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }

        public string? DocType { get; set; }

        public DateTime? DocDate { get; set; }
        public DateTime? DocDueDate { get; set; }

        public string? CardCode { get; set; }
        public string? CardName { get; set; }

        public string? NumAtCard { get; set; }

        public decimal? DocTotal { get; set; }
        public string? DocCurrency { get; set; }

        public decimal? VatSum { get; set; }

        public string? Comments { get; set; }

        public string? DocumentStatus { get; set; }

        // SAP = tYES/tNO
        public string? Confirmed { get; set; }

        // 🔥 สำคัญ
        public List<PODocumentLine>? DocumentLines { get; set; }
    }

    public class PODocumentLine
    {
        public int LineNum { get; set; }
        public string ItemDescription { get; set; }
        public DateTime ShipDate { get; set; }
        public double Price { get; set; }
        public double PriceAfterVAT { get; set; }
        public string Currency { get; set; }
        public int SalesPersonCode { get; set; }
        public string TreeType { get; set; }
        public string AccountCode { get; set; }
        public string CostingCode { get; set; }
        public string ProjectCode { get; set; }
        public string VatGroup { get; set; }
        public int BaseType { get; set; }
        public int BaseEntry { get; set; }
        public int BaseLine { get; set; }
        public string Address { get; set; }
        public string TaxType { get; set; }
        public string TaxLiable { get; set; }
        public string CorrectionInvoiceItem { get; set; }
        public double NetTaxAmount { get; set; }
        public double NetTaxAmountSC { get; set; }
        public double LineTotal { get; set; }
        public double TaxPercentagePerRow { get; set; }
        public double TaxTotal { get; set; }
        public string DistributeExpense { get; set; }
        public double RowTotalSC { get; set; }
        public string PickStatusEx { get; set; }
        public int VisualOrder { get; set; }
        public double UnitPrice { get; set; }
        public string LineStatus { get; set; }
        public string LineType { get; set; }
        public double OpenAmount { get; set; }
        public double OpenAmountSC { get; set; }
        public DateTime RequiredDate { get; set; }
        public int DocEntry { get; set; }
        public string PriceSource { get; set; }
        public int UoMEntry { get; set; }
        public string ItemType { get; set; }
        public double GrossPrice { get; set; }
        public double GrossTotal { get; set; }
        public double GrossTotalSC { get; set; }
        public int NCMCode { get; set; }
        public double U_HMC_Minute { get; set; }
        public DateTime U_Date_of_payment_plan { get; set; }
    }
}
