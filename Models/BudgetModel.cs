using System;
using System.Collections.Generic;

namespace SapGateway.Models
{
    public class BudgetModel
    {
        public int Year { get; set; }
        public string Department { get; set; } = "";
        public string BudgetCode { get; set; } = "";
        public decimal BudgetAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal UnderOver { get; set; }
    }
}
