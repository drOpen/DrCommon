using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DrOpen.DrCommon.DrData;

namespace DrDataSample
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                var s = new string[] {"a\0bc", "def"};

                var b2 = Encoding.UTF8.GetBytes(s[0] + "\0\0" + s[1]);

                var attr = new DDValue();
                attr.SetValue("ddddd+ыыыы");


            }
            catch (Exception)
            {
                
                
            }



            var a = new DDNode("A");

            var b = a.Add("B");
            var c = b.Add("C");
            var z = b.Add(@".\.");
            c.Add();
            c.Add();
            c.Add();
            c.Add();
            c.Add();
            c.Add();

            c.Attributes.Add(new DDValue("value"));
            c.Attributes.Add(new DDValue("value"));
            c.Attributes.Add(new DDValue("value"));

            foreach (var node in c)
            {
                Debug.Print(node.Value.Name);
            }

            var d = a.Clone(true);
            //var ff = d.Attributes["fff"];
            // c.Add(b);
           
            d.Attributes.Add("ff", 123);
            
            var f = d.Attributes["ff"];
            var  t = d.Attributes.GetValue("ff", 56);
            var  r = d.Attributes.GetValue("fff", 56);
            d.Attributes.Add("Value");


            var ddNode = new DDNode("test");
            var ddNodeVars = ddNode.Add("Vars");
            var ddNodeActions = ddNode.Add("Actions");
            ddNodeVars.Attributes.Add("LogonName", "UserName");
            ddNodeVars.Attributes.Add("ExpectedResult", "false");
            ddNodeVars.Attributes.Add("ExpectedResult", "true", ResolveConflict.OVERWRITE);
            ddNodeVars.Attributes.GetValue("ExpectedResult", false).GetValueAsBool();
            ddNode.GetNode("/Vars").Attributes.GetValue("ExpectedResult", true).GetValueAsBool();

        }
    }
}
