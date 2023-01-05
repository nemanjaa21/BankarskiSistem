using Contracts;
using Manager;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public string CardRequest()
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
            
            string certName = clientName + "_MasterCard";

            string path = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName + @"\Sertifikati";


            // .pvk and .cer for auth

            CertificateHelper.GenerateCertificateForAuth(path, clientName);

            // .pfx for auth

            CertificateHelper.GeneratePvk(path, clientName);


            // .pvk and .cer for digital signature

            CertificateHelper.GenerateCertificateForDS(path, clientName + "_sign");

            // .pfx for auth

            CertificateHelper.GeneratePvk(path, clientName + "_sign");

            // Generisanje PIN-a

            string pin = PinHelper.GeneratePin();

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

            // Kriptuj PIN sa RSA

            string secretKey = SecretKey.GenerateKey();
            string toEncrypt = secretKey + pin;

            X509Certificate2 certClient = null;

            try
            {
                certClient = CertManager.GetCertificateFromFile(clientName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
 
            string encrypted = Manager.RSA.Encrypt(toEncrypt, certClient.GetRSAPublicKey().ToXmlString(false));

            try
            {
                SecretKey.StoreKey(secretKey, clientName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return encrypted;
        }
    }
}
