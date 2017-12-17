using System;
using System.Text;

namespace Project_BankAssessment
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            // Show a login screen and get username.
            string username = new LoginScreen().ShowLoginScreen();

            // Show main application menu depending on username.
            new ApplicationMenu(username);


            Console.ReadKey();
        }
    }
}
