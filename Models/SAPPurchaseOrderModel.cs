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
        public string? VendorContactFirstName { get; set; }
        public string? VendorContactLastName { get; set; }
        public string? VendorContactPhone1 { get; set; }
        public string? VendorContactE_Mail { get; set; }
        public string? VendorAddress { get; set; }
        public string? VendorBlock { get; set; }

        public string? NumAtCard { get; set; }

        public decimal? DocTotal { get; set; }
        public string? DocCurrency { get; set; }

        public decimal? VatSum { get; set; }

        public string? Comments { get; set; }

        public string? DocumentStatus { get; set; }

        public string? Confirmed { get; set; }

        public string? U_DERIVERY_TERM { get; set; }
        public string? FederalTaxID { get; set; }
        public string? U_Base_on_Purchase_Material_Requisition { get; set; }
        public int? DataVersion { get; set; }
        public string? U_CONDITION_OF_DELIVERY { get; set; }

        public string? U_GTSC_of_Materials { get; set; }
        public string? U_GTSC_of_Service { get; set; }
        public string? U_Payment_duw { get; set; }
        public string? U_Vender_Bank_Details { get; set; }
        public string? U_TERM_OF_PAYMENT { get; set; }

        public int? DocumentsOwner { get; set; }
        public string? DocOwnerFirstName { get; set; }
        public string? DocOwnerLastName { get; set; }
        public string? DocOwnerOfficePhone { get; set; }
        public string? DocOwnerOfficeExtension { get; set; }
        public string? DocOwnerMobilePhone { get; set; }
        public string? DocOwnereMail { get; set; }

        public int? PaymentGroupCode { get; set; }
        public string? PaymentTermsGroupName { get; set; }

        public List<PODocumentLine>? DocumentLines { get; set; }
        
        public AddressExtension AddressExtension { get; set; }
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

    public class AddressExtension
    {
        public string ShipToStreet { get; set; }
        public string ShipToGlobalLocationNumber { get; set; }
    }

    public class DocumentOwnerModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OfficePhone { get; set; }
        public string OfficeExtension { get; set; }
        public string MobilePhone { get; set; }
        public string eMail { get; set; }
    }

    public class ContactEmployeesModel
    {
        public string Address { get; set; }
        public string Block { get; set; }
        public List<ContactEmployees>? ContactEmployees { get; set; } 
    }
    
    public class ContactEmployees
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone1 { get; set; }
        public string E_Mail { get; set; }

    }

    public class PaymentGroupModel
    {
        public int GroupNumber { get; set; }
        public string PaymentTermsGroupName { get; set; }
    }
}
