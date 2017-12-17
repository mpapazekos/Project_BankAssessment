using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project_BankAssessment
{
    // Provides a connection with the database afdemp_csharp_1
    public class BankAccountRepository
    {
        private readonly string Cooperative = "admin";

        // Checks if user and password match a row in the user table.
        public bool LoginCheck(string name, string pass)
        {
            using (var ctx = new BankAccountsContext())
            {
                try
                {
                    var User = ctx.Users.FromSql($"SELECT * FROM dbo.users WHERE username = {name} AND password = HASHBYTES('SHA2_256', CAST( {pass} as varchar(max)))").SingleOrDefault();
                        
                    return (User != null) ? true : false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }


        // Update (deposit or withdraw) amount and TransactionDate from two accounts. 
        public string Transfer(string fromUser, decimal amount, string toUser)
        {
            using (var ctx = new BankAccountsContext())
            {              
                try
                {
                    var fromAccount = ctx.Accounts.Single(acc => acc.User.Username == fromUser);

                    if (fromAccount.Amount >= amount)
                    {
                        var toAccount = ctx.Accounts.Single(acc => acc.User.Username == toUser);

                        fromAccount.Amount -= amount;
                        fromAccount.TransactionDate = DateTime.Now;

                        toAccount.Amount += amount;
                        toAccount.TransactionDate = DateTime.Now;

                        ctx.SaveChanges();

                        return toAccount.TransactionDate.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                    }
                    else
                    {
                        Console.WriteLine("\n ERROR: Insufficient funds in account.");
                    }                
                }
                catch(Exception ex)
                {
                    Console.WriteLine("\n ERROR: Account does not exist.");
                }

                return "";
            }
        }


        // Return a single bank account from database.
        public Account GetAccount(string accountUser)
        {
            using (var ctx = new BankAccountsContext())
            {
                try
                {
                    var toAccount = ctx.Accounts.Include(acc => acc.User)
                                                .Single(acc => acc.User.Username == accountUser);
                    return toAccount;
                }
                catch(Exception ex)
                {
                    return null;
                }        
            }
        }


        // Gets every account's info from the database and returns it as a list.
        public List<Account> GetAllAccounts()
        {
            using(var ctx = new BankAccountsContext())
            {
                var UserAcc = ctx.Accounts.Include(acc => acc.User)
                                          .Where(acc => acc.User.Username != Cooperative)
                                          .ToList();

                return UserAcc;      
            }
        }

    }
}
