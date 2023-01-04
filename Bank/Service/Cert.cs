using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
    public class Cert : ICert
    {
        private static string workingDirectory = Environment.CurrentDirectory;

        public void TestCommunication()
        {
            Console.WriteLine("[CERT] Communication established.");
        }

        public void CardRequest(string pin)
        {
            string clientName = Formatter.ParseName(Thread.CurrentPrincipal.Identity.Name);
            Console.WriteLine("[CERT] {0} je poslao zahtev za kreiranje racuna.", clientName);

            List<Racun> racuni = XMLHelper.ReadAllBankAccounts();

            var racun = racuni.Find(x => x.Username.Equals(clientName));

            if (racun != null)
            {
                throw new FaultException<CertException>(
                    new CertException("Imate otvoren racun u banci!"));
            }

            string certCN = clientName.ToLower() + "_MasterCard";
            string certName = clientName + "_MasterCard";

            string path = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName + @"\Sertifikati\makecert.exe";
           

            // .pvk and .cer

            Process p1 = new Process();

            string arguments = string.Format("-sv {0}.pvk -iv BankCA.pvk -n \"CN={1}\" -pe -ic BankCA.cer {0}.cer -sr localmachine -ss My -sky signature", certName , certCN);

            ProcessStartInfo info = new ProcessStartInfo(path, arguments);
            info.WorkingDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName + @"\Sertifikati";

            p1.StartInfo = info;

            try
            {
                p1.Start();
            }
            catch (Exception e)
            {
                throw new FaultException<CertException>(
                      new CertException(e.Message));
            }
    
            p1.WaitForExit();
            p1.Dispose();

            // .pfx

            Process p2 = new Process();
            path = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName + @"\Sertifikati\pvk2pfx.exe";
      
            arguments = string.Format("/pvk {0}.pvk /pi 123 /spc {0}.cer /pfx {0}.pfx", certName);

            info = new ProcessStartInfo(path, arguments);
            info.WorkingDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName + @"\Sertifikati";

            p2.StartInfo = info;

            try
            {
                p2.Start();
            }
            catch (Exception e)
            {
                throw new FaultException<CertException>(
                      new CertException(e.Message));
            }

            p2.WaitForExit();
            p2.Dispose();

            // Snimi racun u xml fajl

            Racun noviRacun = new Racun()
            {
                Username = clientName,
                Pin = HashHelper.HashPassword(pin),
                Balance = 0
            };

            try
            {
                XMLHelper.AddBankAccount(noviRacun);
            }
            catch(Exception e)
            {
                throw new FaultException<CertException>(
                      new CertException(e.Message));
            }
        }
    }
}
