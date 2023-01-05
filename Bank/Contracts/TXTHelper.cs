using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class TXTHelper
    {
        public static List<string> ReadSerialNumbers()
        {
            string path = "povuceniSertifikati.txt";
            List<string> retList = new List<string>();

            using (StreamReader file = new StreamReader(path))
            {
                string line;

                while ((line = file.ReadLine()) != null)
                {
                    retList.Add(line);
                }
            }

            return retList;
        }

        public static void SaveSerialNumber(string serialNumber)
        {
            string path = "povuceniSertifikati.txt";

            using (StreamWriter file = new StreamWriter(path, true))
            {
                file.WriteLine(serialNumber);
            }
        }
    }
}
