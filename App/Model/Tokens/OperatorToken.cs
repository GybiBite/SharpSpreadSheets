using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSpreadSheets.Model.Tokens
{
    public class OperatorToken : IToken
    {
        private char _op;
        public OperatorToken(char op) => _op = op;

        // Stub for getOperatorToken()
        public char getOperatorToken() => _op;

        // Stub for priority() 
        public int priority()
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
