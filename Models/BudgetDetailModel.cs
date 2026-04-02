using System;
using System.Collections.Generic;

namespace SapGateway.Models
{
    public class BudgetDetailModel
    {
        public string AcctCode { get; set; }
        public string AcctName { get; set; }
        public string OcrCode { get; set; }
        public double BudgetTotal { get; set; }
        public string TransTypeName { get; set; }
        public string DocNumber { get; set; }
        public double DocTotal { get; set; }
    }
}
