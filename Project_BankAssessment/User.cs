using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Project_BankAssessment
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }
   
        public string Password { get; set; }

        public virtual Account Account { get; set; }
    }
}
