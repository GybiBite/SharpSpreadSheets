using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSpreadSheets.Model.Tokens
{
    public class LiteralToken(int val) : IToken
    {
        private readonly int _value = val;

        // Stub for getValue()
        public int GetValue() => _value;
    }
}
