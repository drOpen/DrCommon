using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrDataSn;

namespace printer.cs
{
    class Program
    {
        static void Main(string[] args)
        {
            String spath = "C:\\WorkNew\\DrCommon\\DrData\\UTests\\UTestDrDataSn\\XML\\1.xml";
            String dpath = "C:\\WorkNew\\DrCommon\\DrData\\UTests\\UTestDrDataSn\\XML\\2.xml";

            DDNode ddn = GetStockHierarhyWithArrayValue();

            using (XmlWriter writer = XmlWriter.Create(spath))
            {
                ddn.Serialize(writer);
                Console.WriteLine("Serialized");
            }

            DDNode dddn = new DDNode();


            using (XmlReader reader = XmlReader.Create(spath))
            {
                dddn = DDNodeSne.Deserialize(reader);
                Console.WriteLine("Deserialized");
            }

            using (XmlWriter writer = XmlWriter.Create(dpath))
            {
                dddn.Serialize(writer);
                Console.WriteLine("Serialized");
            }






            Console.Read();
        }

        static private DDNode GetStockHierarhyWithArrayValue()
        {
            var root = new DDNode(Guid.Empty.ToString(), String.Empty);
            var n = new DDNode();
            var a = n.Attributes;
            a.Add(null);
            a.Add(new DDValue());
            a.Add(new DDValue(null));
            a.Add(String.Empty);
            root.Add(n);
            return root;
        }
    }
}
