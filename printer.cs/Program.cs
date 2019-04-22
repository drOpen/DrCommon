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
            dddn.Merge(ddn);

            using (XmlWriter writer = XmlWriter.Create(dpath))
            {
                dddn.Serialize(writer);
                Console.WriteLine("Serialized");
            }

            //using (XmlReader reader = XmlReader.Create(spath))
            //{
            //    dddn = DDNodeSne.Deserialize(reader);
            //    Console.WriteLine("Deserialized");
            //}

            


            Console.Read();
        }

        static private DDNode GetStockHierarhyWithArrayValue()
        {
            var root = new DDNode(Guid.Empty.ToString(), String.Empty);
            var a = new DDNode("a");
            a.Attributes.Add("value a->a", "string");
            a.Attributes.Add("value a->b", true);
            var a_b = a.Add("a.b");
            var a_c = a.Add("a.c");
            a_c.Attributes.Add("value a.c->a", "string");
            a_c.Attributes.Add("value a.c->b", true);
            var a_b_d = a_b.Add("a.b.d");
            var a_b_d_e = a_b_d.Add("a.b.d.e");
            a_b_d_e.Attributes.Add("value a.b.d.e->a", 1);
            a_b_d_e.Attributes.Add("value a.b.d.e->b", null);
            root.Add(a);
            return root;
        }
    }
}
