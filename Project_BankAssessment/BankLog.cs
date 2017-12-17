using System;
using System.IO;
using System.Collections.Generic;

namespace Project_BankAssessment
{
    public class BankLog
    {
        public void AddToLog(string username, string filename, List<string> logInfo)
        {
            if (!File.Exists(filename))
            {
                using(FileStream fs = File.Create(filename))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine($"--[{username}]------\n");
                    }
                }
            }

            if (logInfo != null)
            {
                File.AppendAllLines(filename, logInfo);
            }
        }
    }
}
