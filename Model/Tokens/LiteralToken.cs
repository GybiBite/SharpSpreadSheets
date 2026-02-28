using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSpreadSheets.Model.Tokens
{
    public class LiteralToken : IToken
    {
        private int _value;
        public LiteralToken(int val) => _value = val;

        // Stub for getValue()
        public int getValue() => _value;
    }
}
