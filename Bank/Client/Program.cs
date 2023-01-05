using Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static WCFCert bankCert = null;
        private static WCFTransaction bankTransaction = null;

        static void Main(string[] args)
        {
            // Proxy za sertifikate

            CertProxy();

            Console.WriteLine("Da li zelite da kreirate racun u banci? [Y/N]");
            string answer = Console.ReadLine();

            if(answer.Equals("Y") || answer.Equals("y"))
            {
                string pin = bankCert.CardRequest();
            }

            BankProxy();

            if (bankTransaction != null)
            {
                Menu();

                bankCert.Close();
                bankTransaction.Close();
            }

            Console.WriteLine("\nPress <enter> to stop ...");
            Console.ReadLine();
        }

        private static void CertProxy()
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:17002/Cert";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;


            EndpointAddress endpointAddress = new EndpointAddress(new Uri(address));

            bankCert = new WCFCert(binding, endpointAddress);

            bankCert.TestCommunication();
        }

        private static void BankProxy()
        {
            string srvCertCN = "bankservice";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, srvCertCN);

            EndpointAddress endpointAddress = new EndpointAddress(new Uri("net.tcp://localhost:17001/BankTransaction"),
                                      new X509CertificateEndpointIdentity(srvCert));

            bankTransaction = new WCFTransaction(binding, endpointAddress);

            bankTransaction.TestCommunication();
        }

        private static void Menu()
        {
            bool end = false;

            do {

                Console.WriteLine("1. Zahtev za novim MasterCard sertifikatom");
                Console.WriteLine("2. Uplata");
                Console.WriteLine("3. Isplata");
                Console.WriteLine("4. Promena PIN-a");
                Console.WriteLine("5. Izlazak");

                Console.WriteLine("-> ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        // TO DO
                        break;
                    case "2":
                        // TO DO
                        break;
                    case "3":
                        // TO DO
                        break;
                    case "4":
                        // TO DO
                        break;
                    case "5":
                        end = true;
                        break;
                    default:
                        Console.WriteLine("Nepoznata komanda!\n");
                        break;
                }
            } while (!end);
        }
    }
}
