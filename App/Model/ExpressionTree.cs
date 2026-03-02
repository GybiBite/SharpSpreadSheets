using SharpSpreadSheets.Model.Tokens;
using System;
using System.Collections.Generic;

namespace SharpSpreadSheets.Model
{
    public class ExpressionTree
    {
        public ExpressionTreeNode Root { get; set; }

        public ExpressionTree()
        {
            Root = null;
        }

        public void BuildExpressionTree(Stack<IToken> stack)
        {
            Root = GetExpressionTree(stack);
        }

        private ExpressionTreeNode GetExpressionTree(Stack<IToken> stack)
        {
            if (stack.Count == 0) return null; // [cite: 150-151]

            IToken token = stack.Pop();

            if (token is LiteralToken || token is CellToken)
            {
                return new ExpressionTreeNode(token); // Leaf nodes [cite: 153-155]
            }
            else if (token is OperatorToken)
            {
                ExpressionTreeNode right = GetExpressionTree(stack);
                ExpressionTreeNode left = GetExpressionTree(stack);
                return new ExpressionTreeNode(token, left, right);
            }
            return null;
        }

        public static int Evaluate(ExpressionTreeNode node, Spreadsheet spreadsheet)
        {
            if (node == null) return 0;

            if (node.Left == null && node.Right == null)
            {
                if (node.Token is LiteralToken literal)
                {
                    return literal.getValue(); //
                }
                if (node.Token is CellToken cell)
                {
                    return spreadsheet.GetCell(cell.getRow(), cell.getColumn()).Value;
                }
            }

            int leftVal = Evaluate(node.Left, spreadsheet);
            int rightVal = Evaluate(node.Right, spreadsheet);

            if (node.Token is OperatorToken opToken)
            {
                char op = opToken.getOperatorToken(); //
                return op switch
                {
                    '+' => leftVal + rightVal,
                    '-' => leftVal - rightVal,
                    '*' => leftVal * rightVal,
                    '/' => rightVal != 0 ? leftVal / rightVal : 0, // Div by zero guard
                    _ => 0
                };
            }
            return 0;
        }
    }
}