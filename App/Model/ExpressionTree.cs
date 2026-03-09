using SharpSpreadSheets.Model.Tokens;

namespace SharpSpreadSheets.Model
{
    public class ExpressionTree
    {
        public ExpressionTreeNode? Root { get; set; }

        public ExpressionTree()
        {
            Root = null;
        }

        public void BuildExpressionTree(Stack<IToken> stack)
        {
            Root = ConstructTree(stack);
        }

        public static int Evaluate(ExpressionTreeNode? node, Spreadsheet spreadsheet)
        {
            if (node == null)
            {
                return 0;
            }

            if (node.Left == null && node.Right == null)
            {
                if (node.Token is LiteralToken literal)
                {
                    return literal.GetValue();
                }

                if (node.Token is CellToken cell)
                {
                    return spreadsheet.GetCell(cell.GetRow(), cell.GetColumn()).Value;
                }
            }

            int leftVal = Evaluate(node.Left, spreadsheet);
            int rightVal = Evaluate(node.Right, spreadsheet);

            if (node.Token is OperatorToken opToken)
            {
                char op = opToken.GetOperatorToken();

                return op switch
                {
                    '+' => leftVal + rightVal,
                    '-' => leftVal - rightVal,
                    '*' => leftVal * rightVal,
                    '/' => rightVal != 0 ? leftVal / rightVal : 0,
                    '^' => (int)Math.Pow(leftVal, rightVal),
                    _ => 0
                };
            }

            return 0;
        }

        private static ExpressionTreeNode? ConstructTree(Stack<IToken> stack)
        {
            if (stack.Count == 0)
            {
                return null;
            }

            IToken token = stack.Pop();

            if (token is LiteralToken || token is CellToken)
            {
                return new ExpressionTreeNode(token);
            }
            else if (token is OperatorToken)
            {
                ExpressionTreeNode? right = ConstructTree(stack);
                ExpressionTreeNode? left = ConstructTree(stack);

                return new ExpressionTreeNode(token, left, right);
            }

            return null;
        }
    }
}