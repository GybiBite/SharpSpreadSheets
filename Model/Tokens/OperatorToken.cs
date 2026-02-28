using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSpreadSheets.Model.Tokens
{
    public class OperatorToken : IToken
    {
        // Priorities defined in the provided utility code
        public char Operator { get; }

        public OperatorToken(char op) => Operator = op;

        public int Priority => Operator switch
        {
            '+' or '-' => 0,
            '*' or '/' => 1,
            '(' => 2,
            _ => -1
        };
    }
}
