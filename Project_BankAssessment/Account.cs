using System;
using System.Globalization;

namespace Project_BankAssessment
{
    public class Account
    {
      
        public int Id { get; set; }
  
        public DateTime TransactionDate { get; set; }
        
        public decimal Amount { get; set; }


        public int UserId { get; set; }

        public virtual User User { get; set; }


        public override string ToString()
        {
            string money = Amount.ToString("C", new CultureInfo("el-GR"));

            return $"{User.Username}\t{TransactionDate}\t" + money;
        }
    }
}
