using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Bank : IBank
    {
        public void TestCommunication()
        {
            Console.WriteLine("[TRANSACTION] Communication established.");
        }
    }
}
