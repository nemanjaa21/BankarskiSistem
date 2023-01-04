using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Cert : ICert
    {
        public void TestCommunication()
        {
            Console.WriteLine("[CERT] Communication established.");
        }
    }
}
