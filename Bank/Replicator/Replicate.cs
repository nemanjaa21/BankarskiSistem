using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Replicator
{
    public class Replicate : IReplicate
    {
        public void AddAccount(string username, string pin, string secretKey)
        {
            Racun racun = new Racun()
            {
                Username = username,
                Pin = pin,
                Balance = 0
            };

            try
            {
                XMLHelper.AddBankAccount(racun);
                SecretKey.StoreKey(secretKey, username);

                Console.WriteLine("Korisnik {0} repliciran.", racun.Username);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void RevokeCertificateUpdate(string serialNumber)
        {
            try
            {
                TXTHelper.SaveSerialNumber(serialNumber);

                Console.WriteLine("Serijski broj {0} repliciran", serialNumber);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void TestCommunication()
        {
            Console.WriteLine("[REPLICATOR] Communication established.");
        }

        public void UpdateAccountBalance(string username, float amount)
        {
            try
            {
                XMLHelper.UpdateBankAccountBalance(username, amount);

                Console.WriteLine("Korisniku {0} replicirana novo stanje racuna.", username);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void UpdateAccountPin(string username, string newPin)
        {
            try
            {
                XMLHelper.UpdateBankAccount(username, newPin);

                Console.WriteLine("Korisniku {0} replicirana novi pin.", username);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
