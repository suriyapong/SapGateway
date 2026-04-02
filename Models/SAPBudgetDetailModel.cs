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
        public double PRDraft { get; set; }
        public double PR { get; set; }
        public double PODraft { get; set; }
        public double PO { get; set; }
        public double GRPO { get; set; }
        public double AP { get; set; }
        public double Accounting { get; set; }
    }
}