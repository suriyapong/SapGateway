using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapGateway.Models
{
    public class PurchaseRequestDraftModel
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string DocType { get; set; }
        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public string DocCurrency { get; set; }
        public double DocRate { get; set; }
        public string Comments { get; set; }
        public string JournalMemo { get; set; }
        public string Confirmed { get; set; }
        public int Series { get; set; }
        public string TaxDate { get; set; }
        public string DocumentStatus { get; set; }
        public double VatSum { get; set; }
        public double DocTotalSys { get; set; }
        public string Requester { get; set; }
        public string RequesterName { get; set; }
        public string RequesterEmail { get; set; }
        public string CreationDate { get; set; }
        public string UpdateDate { get; set; }

        public List<PurchaseRequestLineModel> DocumentLines { get; set; }
        public TaxExtensionModel TaxExtension { get; set; }
        public AddressExtensionModel AddressExtension { get; set; }
    }

    public class PurchaseRequestLineModel
    {
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double PriceAfterVAT { get; set; }
        public string Currency { get; set; }
        public string WarehouseCode { get; set; }
        public string AccountCode { get; set; }
        public string ProjectCode { get; set; }
        public string VatGroup { get; set; }
        public double LineTotal { get; set; }
        public double TaxTotal { get; set; }
        public double TaxPercentagePerRow { get; set; }
        public string UoMCode { get; set; }
        public double InventoryQuantity { get; set; }
        public double OpenAmount { get; set; }
        public string LineStatus { get; set; }
    }

    public class TaxExtensionModel
    {
        public int DocEntry { get; set; }
        public double NetWeight { get; set; }
        public double GrossWeight { get; set; }
    }

    public class AddressExtensionModel
    {
        public int DocEntry { get; set; }

        public string ShipToCity { get; set; }
        public string ShipToCountry { get; set; }

        public string BillToCity { get; set; }
        public string BillToCountry { get; set; }
    }
}