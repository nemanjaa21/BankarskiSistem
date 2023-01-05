using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Service
{
    static class XMLHelper
    {
        static public void AddBankAccount(Racun racun)
        {
            List<Racun> racuni = ReadAllBankAccounts();
            racuni.Add(racun);

            XmlSerializer serializer = new XmlSerializer(typeof(List<Racun>));
            using (TextWriter textWriter = new StreamWriter("../../bankAccounts.xml"))
            {
                serializer.Serialize(textWriter, racuni);
            }
        }

        static public List<Racun> ReadAllBankAccounts()
        {
            List<Racun> racuni = new List<Racun>();
            XmlSerializer serializer = new XmlSerializer(typeof(List<Racun>));
            using (TextReader textReader = new StreamReader("../../bankAccounts.xml"))
            {
                racuni = (List<Racun>)serializer.Deserialize(textReader);
            }
            return racuni;
        }

    }
}
