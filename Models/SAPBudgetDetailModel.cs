using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapGateway.Models
{
    public class SAPBudgetDetailModel
    {
        public string AcctCode { get; set; }
        public string AcctName { get; set; }
        public string OcrCode { get; set; }
        public double BudgetTotal { get; set; }
        public string TransTypeName { get; set; }
        public string DocNumber { get; set; }
        public double SumTotal { get; set; }
    }
}