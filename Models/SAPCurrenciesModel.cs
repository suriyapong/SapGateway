namespace SapGateway.Models
{
    public class SAPCurrenciesModel
    {
        public List<SAPCurrencyModel>? value { get; set; }

        public class SAPCurrencyModel
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string? DocumentsCode { get; set; }
        }
    }
}
