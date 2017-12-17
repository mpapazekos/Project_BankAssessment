using System;

namespace Project_BankAssessment
{
    public class LoginScreen
    {
        private string Username { get; set; }

        /// <summary>
        /// Shows the user a simple login menu
        /// when the application starts and 
        /// checks if user exists in database. 
        /// </summary>
        public string ShowLoginScreen()
        {
            BankAccountRepository bankRepository = new BankAccountRepository();
            bool passed = false;
            int tries = 3;

            MessageMenu(@"
            ╔═╗┌─┐┌┐┌┌─┐┌─┐┬  ┌─┐  ╔╗ ┌─┐┌┐┌┬┌─
            ║  │ ││││└─┐│ ││  ├┤   ╠╩╗├─┤│││├┴┐
            ╚═╝└─┘┘└┘└─┘└─┘┴─┘└─┘  ╚═╝┴ ┴┘└┘┴ ┴
            ");

            do
            {
                Console.WriteLine(" Please enter Username and Password.");
                Console.WriteLine("=====================================\n");

                Console.Write("# Username: ");
                Username = Console.ReadLine();

                Console.Write("# Password: ");

                // Check if user exists in database
                passed = bankRepository.LoginCheck(Username, GetPassword());

                // If user was not found show appropriate message and
                // prompt them again.
                if (!passed)
                {
                    Console.Clear();
                    MessageMenu($"  ERROR: Wrong username or Password. [TRIES LEFT: {--tries}]");
                }
            }
            while (!passed && tries > 0);

            // If both checks are passed, clear console and show 
            // application menu depending on username,
            // else exit the application.   
            if (tries <= 0)
            {
                Environment.Exit(0);  
            }

            Console.Clear();
            return Username;
        }

        private void MessageMenu(string message)
        {
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine($"{message}");
            Console.WriteLine("+----------------------------------------------------------+\n");
        }

        private string GetPassword()
        {
            ConsoleKeyInfo key = Console.ReadKey(true);

            string input = "";

            while (key.Key != ConsoleKey.Enter)
            {
                if (key.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    input += key.KeyChar;
                }
                else
                {
                    if(!string.IsNullOrEmpty(input))
                    {
                        input = input.Substring(0, input.Length - 1);
                        Console.Write("\b \b");
                    }
                }

                key = Console.ReadKey(true);
            } 

            return input;
        }
    }
}
