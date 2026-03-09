using SharpSpreadSheets.Logic;
using SharpSpreadSheets.Model;
using SharpSpreadSheets.Model.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Testing
{
    public class ExpressionTreeTests
    {
        // Test that a new ExpressionTree has a null root
        [Fact]
        public void ExpressionTree_DefaultConstructor_RootIsNull()
        {
            ExpressionTree tree = new ExpressionTree();
            Assert.Null(tree.Root);
        }

        // Test that building a tree from a single literal sets the root correctly
        [Fact]
        public void ExpressionTree_BuildFromSingleLiteral_RootIsLiteralToken()
        {
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("5");
            tree.BuildExpressionTree(stack);

            Assert.NotNull(tree.Root);
            Assert.IsType<LiteralToken>(tree.Root.Token);
            Assert.Equal(5, ((LiteralToken)tree.Root.Token).getValue());
        }

        // Test that building a tree from an addition formula has an operator at the root
        [Fact]
        public void ExpressionTree_BuildFromAddition_RootIsOperatorToken()
        {
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("3+4");
            tree.BuildExpressionTree(stack);

            Assert.NotNull(tree.Root);
            Assert.IsType<OperatorToken>(tree.Root.Token);
            Assert.Equal('+', ((OperatorToken)tree.Root.Token).getOperatorToken());
        }

        // Test that Evaluate correctly computes addition
        [Fact]
        public void ExpressionTree_Evaluate_Addition_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("3+4");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(7, result);
        }

        // Test that Evaluate correctly computes subtraction
        [Fact]
        public void ExpressionTree_Evaluate_Subtraction_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("10-3");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(7, result);
        }

        // Test that Evaluate correctly computes multiplication
        [Fact]
        public void ExpressionTree_Evaluate_Multiplication_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("3*4");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(12, result);
        }

        // Test that Evaluate correctly computes division
        [Fact]
        public void ExpressionTree_Evaluate_Division_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("12/4");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(3, result);
        }

        // Test that dividing by zero returns 0 instead of crashing
        [Fact]
        public void ExpressionTree_Evaluate_DivisionByZero_ReturnsZero()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("5/0");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(0, result);
        }

        // Test that Evaluate on a null node returns 0
        [Fact]
        public void ExpressionTree_Evaluate_NullNode_ReturnsZero()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            int result = ExpressionTree.Evaluate(null, spreadsheet);
            Assert.Equal(0, result);
        }

        // Test a more complex formula with operator precedence
        [Fact]
        public void ExpressionTree_Evaluate_ComplexFormula_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("5+10*2");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(25, result);
        }

        [Fact]
        public void ExpressionTree_Evaluate_Exponentiation_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("2^3");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(8, result); // 2^3 = 8
        }

        // Test that a negative literal is parsed and evaluated correctly
        [Fact]
        public void ExpressionTree_Evaluate_NegativeLiteral_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("-5+3");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(-2, result); // -5 + 3 = -2
        }

        // Test subtraction where the right operand is a negative number
        [Fact]
        public void ExpressionTree_Evaluate_SubtractNegativeNumber_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("3 - -5");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(8, result); // 3 - (-5) = 8
        }

        // Test the original bug case: cell reference minus a negative number
        [Fact]
        public void Spreadsheet_CellMinusNegativeNumber_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            spreadsheet.ChangeCellFormula(1, 0, "-1");  // A1 = -1
            spreadsheet.ChangeCellFormula(2, 0, "A1 - -5"); // A2 = -1 - (-5) = 4
            Assert.Equal(4, spreadsheet.GetCell(2, 0).Value);
        }

        // Test negative number multiplied by a positive number
        [Fact]
        public void ExpressionTree_Evaluate_NegativeTimesPositive_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("-3 * 4");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(-12, result); // -3 * 4 = -12
        }

        // Test cell reference times a negative literal
        [Fact]
        public void ExpressionTree_Evaluate_CellTimesNegativeLiteral_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            spreadsheet.ChangeCellFormula(1, 0, "3"); // A1 = 3
            spreadsheet.ChangeCellFormula(2, 0, "A1 * -5"); // A2 = 3 * -5 = -15
            Assert.Equal(-15, spreadsheet.GetCell(2, 0).Value);
        }

        // Test two negative literals multiplied
        [Fact]
        public void ExpressionTree_Evaluate_NegativeTimesNegative_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("-3 * -5");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(15, result); // -3 * -5 = 15
        }

        // Test negative cell reference times a literal
        [Fact]
        public void ExpressionTree_Evaluate_NegativeCellTimesLiteral_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            spreadsheet.ChangeCellFormula(1, 0, "3"); // A1 = 3
            spreadsheet.ChangeCellFormula(2, 0, "-A1 * 5"); // A2 = -3 * 5 = -15
            Assert.Equal(-15, spreadsheet.GetCell(2, 0).Value);
        }

        // Test dividing a positive number by a negative literal
        [Fact]
        public void ExpressionTree_Evaluate_DivideByNegative_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("10 / -2");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(-5, result); // 10 / -2 = -5
        }

        // Test dividing a negative number by a positive literal
        [Fact]
        public void ExpressionTree_Evaluate_NegativeDividedByPositive_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("-10 / 2");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(-5, result); // -10 / 2 = -5
        }

        // Test dividing a negative number by a negative number
        [Fact]
        public void ExpressionTree_Evaluate_NegativeDividedByNegative_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("-10 / -2");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(5, result); // -10 / -2 = 5
        }

        // Test squaring a negative number
        [Fact]
        public void ExpressionTree_Evaluate_NegativeSquared_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("-3 ^ 2");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(9, result); // -3 ^ 2 = 9
        }

        // Test a positive number raised to a negative exponent
        // Note: since we use int, 2^-1 = 0 (integer truncation of 0.5)
        [Fact]
        public void ExpressionTree_Evaluate_NegativeExponent_ReturnsZero()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("2 ^ -1");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(0, result); // 2^-1 = 0.5, truncated to 0 as int
        }

        // Test a negative number raised to a negative exponent
        [Fact]
        public void ExpressionTree_Evaluate_NegativeBaseNegativeExponent_ReturnsZero()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("-2 ^ -3");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(0, result); // -2^-3 = -0.125, truncated to 0 as int
        }

        // Test a negative number raised to an odd power
        [Fact]
        public void ExpressionTree_Evaluate_NegativeBaseOddExponent_ReturnsNegativeResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            ExpressionTree tree = new ExpressionTree();
            Stack<IToken> stack = Util.GetFormula("-2 ^ 3");
            tree.BuildExpressionTree(stack);

            int result = ExpressionTree.Evaluate(tree.Root, spreadsheet);
            Assert.Equal(-8, result); // -2^3 = -8
        }

        // Test a cell reference squared where the cell holds a negative value
        [Fact]
        public void Spreadsheet_NegativeCellSquared_ReturnsCorrectResult()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            spreadsheet.ChangeCellFormula(1, 0, "-3"); // A1 = -3
            spreadsheet.ChangeCellFormula(2, 0, "A1 ^ 2"); // A2 = -3^2 = 9
            Assert.Equal(9, spreadsheet.GetCell(2, 0).Value);
        }
    }
}
