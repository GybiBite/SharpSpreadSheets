using SharpSpreadSheets.Model;
using SharpSpreadSheets.Model.Tokens;
using SharpSpreadSheets.Logic;

namespace Testing
{
    public class CellTests
    {
        // Test that a new cell has the correct default values
        [Fact]
        public void Cell_DefaultConstructor_HasCorrectDefaults()
        {
            Cell cell = new Cell();
            Assert.Equal("0", cell.Formula);
            Assert.Equal(0, cell.Value);
            Assert.Null(cell.ExpressionTree);
        }

        // Test that Evaluate correctly computes a simple literal formula
        [Fact]
        public void Cell_Evaluate_SimpleLiteralFormula_SetsCorrectValue()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            Cell cell = new Cell();
            cell.Formula = "7";

            Stack<IToken> stack = Util.GetFormula("7");
            cell.ExpressionTree = new ExpressionTree();
            cell.ExpressionTree.BuildExpressionTree(stack);
            cell.Evaluate(spreadsheet);

            Assert.Equal(7, cell.Value);
        }

        // Test the Evaluate correctly computes an addition formula
        [Fact]
        public void Cell_Evaluate_AdditionFormula_SetsCorrectValue()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            Cell cell = new Cell();
            cell.Formula = "3+4";

            Stack<IToken> stack = Util.GetFormula("3+4");
            cell.ExpressionTree = new ExpressionTree();
            cell.ExpressionTree.BuildExpressionTree(stack);
            cell.Evaluate(spreadsheet);

            Assert.Equal(7, cell.Value);
        }

        // Test that Evaluate does nothing if ExpressionTree is null
        [Fact]
        public void Cell_Evaluate_NullExpressionTree_ValueRemainsZero()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            Cell cell = new Cell();
            cell.Evaluate(spreadsheet);
            Assert.Equal(0, cell.Value);
        }

        // Test that Dependencies list starts empty
        [Fact]
        public void Cell_Dependencies_StartsEmpty()
        {
            Cell cell = new Cell();
            Assert.Empty(cell.Dependents);
        }
    }
}
