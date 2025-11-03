using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapGateway.Models
{
    public class SAPExchangeRateModel
    {
        public string CompanyName { get; set; }
        public DateTime Period { get; set; }
        public double Rate { get; set; }
        public string CurrencyId { get; set; }
    }
}
