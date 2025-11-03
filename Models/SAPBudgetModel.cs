using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRWSapGatewayebAPI.Models
{
    public class SAPBudgetModel
    {
        public string AcctCode { get; set; }
        public string AcctName { get; set; }
        public string OcrCode { get; set; }
        public string Total { get; set; }
        public string BudgetTotal { get; set; }
    }
}