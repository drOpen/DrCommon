using System;
using System.Collections.Generic;
using System.Text;

namespace DrOpen.DrCommon.DrVar.Resolver
{
    internal class DrVarTokenMaster
    {
        public DrVarTokenMaster()
        {
            this.TokenSign = DrVarSign.varSign.ToString();
            this.TokenEscape = DrVarSign.escapeVarSign;
        }

        public DrVarTokenMaster(string tokenSign, string tokenEscape)
        {
            this.TokenEscape = tokenSign;
            this.TokenEscape = tokenEscape; 
        }

        public string TokenSign { get; private set; }
        public string TokenEscape { get; private set; }


        public Token.DrVarTokenStack GetTokenStack(string value)
        {
            var tl = new Token.DrVarTokenStack(this.TokenSign, this.TokenEscape);
            tl.Parse(value);
            return tl;
        }

        public string Resolve(string value, Token.DrVarTokenStack tokenStack, IVarManager vValue)
        {
            var lstStart = value.Length;
            var s = String.Empty;

            do {
                var t = tokenStack.Pop();
                if (lstStart > t.EndIndex) s = value.Substring(t.EndIndex, lstStart - t.EndIndex)  + s;
                s = vValue.GetValue(t.Name) + s;
                lstStart = t.StartIndex;

            } while (tokenStack.Count() != 0);

            if (lstStart > 0 ) s = value.Substring(0, lstStart)  + s;
            return s;
        }
        public string Resolve(string value, IVarManager vValue)
        {
            var tokenStack = GetTokenStack(value); 
            return Resolve(value, tokenStack, vValue);
        }

    }
}
