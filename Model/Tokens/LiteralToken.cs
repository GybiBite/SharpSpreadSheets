using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSpreadSheets.Model.Tokens
{
    public class LiteralToken : IToken
    {
        public int Value { get; set; }
        public LiteralToken(int value) => Value = value;
    }
}
