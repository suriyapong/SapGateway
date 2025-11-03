using System;
using System.Collections.Generic;

namespace SapGateway.Models
{
    public class CurrencyModel
    {
        public string CompanyName { get; set; }
        public DateTime Period { get; set; }
        public bool IsBuyingRate { get; set; }
        public string CurrencyId { get; set; }
        public string SAPCurrencyId { get; set; }
    }   
}
