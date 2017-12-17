using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Project_BankAssessment
{
    public class ApplicationMenu
    {
        private readonly string Cooperative = "admin";

        // Stores the transactions info a user makes in each session.
        private List<string> CurrentSession { get; set; }

        // Used to call methods for database access.
        private BankAccountRepository BankRepository { get; set; }

        // Stores menu options and an action to be called 
        // for each option. 
        private readonly Dictionary<string, Action> MainMenu;


        public ApplicationMenu(string user)
        {
            BankRepository = new BankAccountRepository();
            CurrentSession = new List<string>();

            if (user == Cooperative)
            {
                MainMenu = new Dictionary<string, Action>
                {
                    { "View Internal Account Info",     () => PrintAccountInfo(Cooperative) },
                    { "View All Members Account Info",  () => PrintAllAccountInfo() },
                    { "Deposit to Member's Account",    () => Transfer(TransactionType.Deposit, Cooperative) },
                    { "Withdraw from Member's Account", () => Transfer(TransactionType.Withdraw, toAccount: Cooperative) },
                    { "Send Today's Statement",         () => SendStatement(Cooperative) },
                    { "Exit Application",               () => EXIT() } 
            
                };
            }
            else
            {
                MainMenu= new Dictionary<string, Action>
                {
                    { "View Internal Account Info",         () => PrintAccountInfo(user) },
                    { "Deposit to Cooperative's Account",   () => Transfer(TransactionType.Deposit, user, Cooperative) },
                    { "Deposit to Member's Account",        () => Transfer(TransactionType.Deposit, user) },
                    { "Send Today's Statement",             () => SendStatement(user) },
                    { "Exit Application",                   () => EXIT() }           
                };
            }
    
            ShowMainMenu(MainMenu);
        }


        private void ShowMainMenu(Dictionary<string,Action> mainMenu)
        {
            List<string> entries = mainMenu.Keys.ToList();
            int menuRange = mainMenu.Count();
            int choice;

            // Main loop.
            while (true)
            {
                // Show Menu...
                PrintUserMenu(entries);

                // Get input...
                choice = ConfirmInputRange(menuRange);

                // Execute appropriate action...
                Console.Clear();
                mainMenu[entries[choice - 1]].Invoke();

                // Wait for user to continue...
                WaitMenuReturn();
            }   
        }


        /// <summary>
        ///  Functions that match menu options in MainMenu Dictionary.
        ///  By asking the user for appropriate input, a method from
        ///  BankAccountRepository is called to access the database and
        ///  return info.
        /// </summary>
        
        private void PrintAccountInfo(string user)
        {
            Console.Clear();
            Account account = BankRepository.GetAccount(user);

            if (account != null)
            {
                Console.WriteLine("User\tTransaction Date\tAmount");
                Console.WriteLine("----\t----------------\t-------\n");
                Console.WriteLine(account);
            }
            else
            {
                Console.WriteLine("\n ERROR: Account Not Found.");
            }
        }

        private void PrintAllAccountInfo()
        {
            Console.Clear();
            List<Account> accounts = BankRepository.GetAllAccounts();

            if(accounts != null)
            {
                Console.WriteLine("User\tTransaction Date\tAmount");
                Console.WriteLine("----\t----------------\t-------\n");

                foreach (var acc in accounts)
                {
                    Console.WriteLine(acc);
                }
            }
            else
            {
                Console.WriteLine("\n No registered users yet...");
            }
        }

        private void Transfer(TransactionType action, string fromAccount = "", string toAccount = "")
        {
            bool keep_asking = (action == TransactionType.Withdraw || fromAccount == Cooperative) ? true : false;

            string fromUser;
            string toUser;
            decimal amount;
            string logTime;

            do
            {
                Console.WriteLine($">> {action} <<");

                // Get the users involved in the 
                // transaction if not passed as arguements.
                fromUser = (fromAccount == string.Empty) ? GetUser() : fromAccount;

                toUser = (toAccount == string.Empty) ? GetUser() : toAccount;

                // Ask the current user for
                // an appropriate amount.
                amount = GetAmount();

                // If amount and user values given are valid...
                if (amount != 0 && fromUser != toUser)
                {
                    // Execute transaction in database 
                    // and get the time it was completed.
                    logTime = BankRepository.Transfer(fromUser, amount, toUser);

                    // If executed time was valid add new transaction info
                    // to current session list.
                    if (logTime != string.Empty)
                    {
                        // If the transaction was a withdrawal log the the user whose account
                        // the money was taken from, else the user whose account the money was
                        // deposited into.
                        string logUser = (action == TransactionType.Withdraw) ? fromUser : toUser;

                        AddToSession(action.ToString(), amount, logUser, logTime);

                        Console.WriteLine("\n$ Transaction was successful $");
                    }
                }
                else
                {
                    Console.WriteLine("\n ERROR: Invalid User or Amount input.");
                    Console.WriteLine(" Please try again...");
                }

                // Ask user if they want to 
                // make another transaction...
                if (keep_asking)
                {             
                    keep_asking = TryAgain();
                    Console.Clear();
                }
            }
            while (keep_asking);
        }

        private void SendStatement(string accessLevel)
        {
            string filename;
            string userInfo;
           

            userInfo = (accessLevel == Cooperative) ? Cooperative : $"user_{accessLevel}";
           
            filename = $"statement_{userInfo}_" + DateTime.Today.ToString("dd_MM_yyyy") + ".txt";


            new BankLog().AddToLog(accessLevel, filename, CurrentSession);

            Console.WriteLine("\n$ Statement Sent Successfully $");
        }

        private void EXIT()
        {
            Console.Clear();
            Console.WriteLine("\n$ Thanks for using the Console Bank! $\n");
            Console.WriteLine("Press any key to terminate application...");

            Console.ReadKey();
            Environment.Exit(0);
        }


        // Add session info in string format to CurrentSession List.
        private void AddToSession(string action, decimal amount, string onUser, string dateInfo)
        {
            var culture = new CultureInfo("el-GR");

            string money = amount.ToString("C", culture);

            string session = $"{action} \t|{dateInfo}| \t{onUser}\t{money}";

            CurrentSession.Add(session);
        }


        /// <summary>
        /// Basic ApplicationMenu methods
        /// to ensure correct user input
        /// and help with menu flow
        /// in the application.
        /// </summary>
      
        private bool TryAgain()
        {
            string answer;

            Console.WriteLine("\n ## Do you want to make another transaction? [Y/N] ##");
            Console.Write("-> ");
            answer = Console.ReadLine();

            switch (answer)
            {
                case "y":
                case "Y":
                case "yes":
                case "Yes":
                case "YES":
                    return true;
                default:
                    return false;
            }
        }

        private string GetUser()
        {
            Console.WriteLine("____________________");
            Console.Write($"Please enter a user. -> ");
            string answer = Console.ReadLine();

            return answer;
        }

        private decimal GetAmount()
        {
            bool pass;
            int tries = 3;
            decimal amount;

            // While the user does not give a positive number or a number at all
            // and can still try again, prompt him for input. 
            do
            {
                Console.WriteLine($"\n__[Tries left: {tries}]____________________________");
                Console.Write("Please enter an appropriate(positive) amount. -> ");
                pass = decimal.TryParse(Console.ReadLine(), out amount);
                tries--;

            } while ((amount <= 0 || !pass) && tries > 0);

            return (tries > 0) ? amount : 0;
        }

        private int ConfirmInputRange(int limit)
        {
            int choice;
            bool pass;

            do
            {
                Console.Write("\nPlease choose a Number from the above list: ");
                pass = int.TryParse(Console.ReadLine(), out choice);

            } while ((choice <= 0 || choice > limit) || !pass);

            return choice;
        }


        private void WaitMenuReturn()
        {
            Console.WriteLine("\n----------------------------");
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
            Console.Clear();
        }

        private void PrintUserMenu(List<string> menuList)
        {
            Console.WriteLine("+---------------------------------------------------+\n");

            for (int i = 0; i < menuList.Count; i++)
            {
                Console.WriteLine($" [{i + 1}] {menuList[i]}.\n");
            }

            Console.WriteLine("+---------------------------------------------------+");
        }
    }
}
