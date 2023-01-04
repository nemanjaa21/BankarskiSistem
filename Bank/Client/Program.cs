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

            if (bankCert != null)
            {
                // Proxy za transakcije
               
                BankProxy();

                if (bankTransaction != null)
                {
                    Menu();

                    bankCert.Close();
                    bankTransaction.Close();
                }

                bankCert.Close();
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

                Console.WriteLine("1. Kreiraj racun");
                Console.WriteLine("2. Povuci MasterCard sertifikat");
                Console.WriteLine("3. Zahtev za novim MasterCard sertifikatom");
                Console.WriteLine("4. Uplata");
                Console.WriteLine("5. Isplata");
                Console.WriteLine("6. Promena PIN-a");
                Console.WriteLine("7. Izlazak\n");

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
                        // TO DO
                        break;
                    case "6":
                        // TO DO
                        break;
                    case "7":
                        // TO DO
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
