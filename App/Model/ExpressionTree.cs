using SharpSpreadSheets.Model.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SharpSpreadSheets.Model
{
    internal class ExpressionTree
    {
        // The root node of the expression tree
        public ExpressionTreeNode Root { get; set; }

        // Constructor initializes an empty tree
        // Call BuildExpressionTree() to populate it
        public ExpressionTree()
        {
            Root = null;
        }

        // Builds the expression tree from a postfix token stack
        // The stack comes from Util.GetFormula() which converts an infix formula string to postfix
        // TODO: This will be called in Spreadsheet.ChangeCellFormulaAndRecalculate()
        public void BuildExpressionTree(Stack<IToken> stack)
        {
            Root = GetExpressionTree(stack);
        }

        // Recursively builds the expression tree from the postfix stack
        // Leaves (literals and cell references) become nodes with no children
        // Operators become nodes whose left and right children are subtrees
        private ExpressionTreeNode GetExpressionTree(Stack<IToken> stack)
        {
            if (stack.Count == 0) return null;

            IToken token = stack.Pop();

            // Literals (numbers) and CellTokens (e.g. A3) are leaf nodes
            if (token is LiteralToken || token is CellToken)
            {
                return new ExpressionTreeNode(token);
            }
            // Operators (+ - * /) are internal nodes with a left and right subtree
            else if (token is OperatorToken)
            {
                // Right subtree is built first because the stack is LIFO
                ExpressionTreeNode right = GetExpressionTree(stack);
                ExpressionTreeNode left = GetExpressionTree(stack);
                return new ExpressionTreeNode(token, left, right);
            }
            return null;
        }

        // Recursively evaluates the expression tree and returns an integer result
        // Requires the spreadsheet to look up values of referenced cells (e.g. A3)
        // TODO: spreadsheet.GetCellValue() needs to be implemented in Spreadsheet.cs
        public static int Evaluate(ExpressionTreeNode node, Spreadsheet spreadsheet)
        {
            // Base case: empty node returns 0
            if (node == null) return 0;

            // Leaf node: either a number or a cell reference
            if (node.Left == null && node.Right == null)
            {
                // If it's a number, return its value directly
                if (node.Token is LiteralToken literal)
                {
                    return literal.getValue(); 
                }
                // If it's a cell reference, look up its value in the spreadsheet
                // TODO: Spreadsheet.GetCellValue() must be implemented for this to work
                if (node.Token is CellToken cell)
                {  
                    return spreadsheet.GetCellValue(cell.getRow(), cell.getColumn());
                }
            }

            // Internal node: recursively evaluate left and right subtrees first
            int leftVal = Evaluate(node.Left, spreadsheet);
            int rightVal = Evaluate(node.Right, spreadsheet);

            // Apply the operator to the two subtree results
            if (node.Token is OperatorToken opToken)
            {
                char op = opToken.getOperatorToken(); //
                return op switch
                {
                    '+' => leftVal + rightVal,
                    '-' => leftVal - rightVal,
                    '*' => leftVal * rightVal,
                    // Guard against division by zero, returns 0 if divisor is 0
                    '/' => rightVal != 0 ? leftVal / rightVal : 0,
                    _ => 0
                };
            }

            return 0;

        }
    }
}