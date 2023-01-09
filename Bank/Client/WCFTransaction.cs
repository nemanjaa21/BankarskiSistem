using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class WCFTransaction : ChannelFactory<IBank>, IBank, IDisposable
	{
		IBank factory;

		public WCFTransaction(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
			/// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
			string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
			this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
			this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
			this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

			/// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
			this.Credentials.ClientCertificate.Certificate =
				CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

			factory = this.CreateChannel();
		}

		public void TestCommunication()
		{
            try
            {
                factory.TestCommunication();
                Console.WriteLine("[TestCommunication] Uspesno ste se autentifikovali uz pomoc sertifikata.\n");
            }
            catch (Exception)
            {
                Console.WriteLine("[TestCommunication] Ne posedujete sertifikat ili vam je sertifikat povucen.\n");

                throw new Exception();
            }
        }

		public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
		}

        public void Deposit(byte[] message)
        {
            try
            {
                factory.Deposit(message);

                Console.WriteLine("Uspesno uplacen depozit.");
            }
            catch (FaultException<BankException> exp)
            {
                Console.WriteLine("[Deposit] " + exp.Detail.Reason);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Deposit] " + e.Message);
            }
        }

        public void Withdraw(byte[] message)
        {
            try
            {
                factory.Withdraw(message);

                Console.WriteLine("Uspesno isplacen depozit.");
            }
            catch (FaultException<BankException> exp)
            {
                Console.WriteLine("[Withdraw] " + exp.Detail.Reason);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Withdraw] " + e.Message);
            }
        }

        public byte[] ResetPin(byte[] message)
        {
            byte[] newPin = null;

            string clientName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            string secretKey = SecretKey.LoadKey(clientName);

            try
            {
                byte[] encrypted = factory.ResetPin(message);

                byte[] decrypted = TripleDES.Decrypt(encrypted, secretKey);

                X509Certificate2 signBank =
                    CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, "bank_sign");

                byte[] sign = new byte[256];
                newPin = new byte[decrypted.Length - 256];

                Buffer.BlockCopy(decrypted, 0, sign, 0, 256);
                Buffer.BlockCopy(decrypted, 256, newPin, 0, decrypted.Length - 256);

                string newPinStr = System.Text.Encoding.UTF8.GetString(newPin);

                if (DigitalSignature.Verify(newPinStr, sign, signBank))
                {
                    Console.WriteLine("\nUspesno resetovan PIN. Novi PIN: " + newPinStr);
                }
                else
                {
                    Console.WriteLine("Neuspesna verifikacija.");
                }
            }
            catch (FaultException<BankException> exp)
            {
                Console.WriteLine("[ResetPin] " + exp.Detail.Reason);
            }
            catch (Exception e)
            {
                Console.WriteLine("[ResetPin] " + e.Message);
            }

            return newPin;
        }
    }
}
