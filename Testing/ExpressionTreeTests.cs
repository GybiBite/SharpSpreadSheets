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
    }
}
