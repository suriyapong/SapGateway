using System;
using System.Collections.Generic;

namespace SapGateway.Models
{
    public class BudgetModel
    {
        public string Department { get; set; } = "";
        public string BudgetCode { get; set; } = "";
        public decimal BudgetFY25 { get; set; }
        public decimal ActualFY25 { get; set; }
        public decimal UnderOver { get; set; }
    }   
}
