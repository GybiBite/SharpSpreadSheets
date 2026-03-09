using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSpreadSheets.Model.Tokens
{
    public class OperatorToken(char op) : IToken
    {
        private readonly char _op = op;

        // Stub for getOperatorToken()
        public char GetOperatorToken() => _op;

        // Stub for priority() 
        public int Priority()
        {
            return _op switch
            {
                '+' or '-' => 0,
                '*' or '/' => 1,
                '(' => 2,
                '^' => 3,
                _ => -1
            };
        }
    }
}
