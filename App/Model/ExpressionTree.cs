using SharpSpreadSheets.Model.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSpreadSheets.Model
{
    internal class ExpressionTree
    {

        public static int Evaluate(ExpressionTreeNode node)
        {
            if (node.Left == null && node.Right == null)
            {
                if (node.Token is LiteralToken literal)
                {
                    return literal.getValue(); //
                }
                if (node.Token is CellToken cell)
                {
                    // For now, return 0 or a placeholder. 
                    return 0;
                }
            }

            int leftVal = Evaluate(node.Left);
            int rightVal = Evaluate(node.Right);

            if (node.Token is OperatorToken opToken)
            {
                char op = opToken.getOperatorToken(); //
                return op switch
                {
                    '+' => leftVal + rightVal,
                    '-' => leftVal - rightVal,
                    '*' => leftVal * rightVal,
                    '/' => rightVal != 0 ? leftVal / rightVal : 0,
                    _ => 0
                };
            }

            return 0;

        }
    }
}