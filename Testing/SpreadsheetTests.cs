
using SharpSpreadSheets.Model;

namespace Testing
{
    public class SpreadsheetTests
    {
        // Test that a new spreadsheet has the correct number of rows and columns
        [Fact]
        public void Spreadsheet_Constructor_SetsCorrectSize()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 10);
            Assert.Equal(5, spreadsheet.GetNumRows());
            Assert.Equal(10, spreadsheet.GetNumCols());
        }

        // Test that all cells start with default values
        [Fact]
        public void Spreadsheet_Constructor_AllCellsInitialized()
        {
            Spreadsheet spreadsheet = new Spreadsheet(3, 3);
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Assert.NotNull(spreadsheet.GetCell(i, j));
        }

        // Test that GetCell returns the correct cell
        [Fact]
        public void Spreadsheet_GetCell_ReturnsCorrectCell()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            Cell cell = spreadsheet.GetCell(2, 3);
            Assert.NotNull(cell);
            Assert.Equal("0", cell.Formula);
        }

        // Test that all cells start with value 0
        [Fact]
        public void Spreadsheet_AllCells_StartWithValueZero()
        {
            Spreadsheet spreadsheet = new Spreadsheet(3, 3);
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Assert.Equal(0, spreadsheet.GetCell(i, j).Value);
        }

        // Test that ChangeCellFormula updates the formula on the correct cell
        [Fact]
        public void Spreadsheet_ChangeCellFormula_UpdatesFormulal()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            spreadsheet.ChangeCellFormula(0, 0, "5+3");
            Assert.Equal("5+3", spreadsheet.GetCell(0, 0).Formula);
        }

        // Test that ChangeCellFormula recalculates the cell value
        [Fact]
        public void Spreadsheet_ChangeCellFormula_RecalculatesValue()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            spreadsheet.ChangeCellFormula(0, 0, "5+3");
            Assert.Equal(8, spreadsheet.GetCell(0, 0).Value);
        }

        // Test that one cell can reference another cell's value
        [Fact]
        public void Spreadsheet_CellReference_EvaluatesCorrectly()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            spreadsheet.ChangeCellFormula(1, 0, "5");    // A1 = 5
            spreadsheet.ChangeCellFormula(2, 0, "A1+2"); // A2 = A1 + 2 = 7
            Assert.Equal(7, spreadsheet.GetCell(2, 0).Value);
        }
        
        // Test that dependencies are updated when formula changes
        [Fact]
        public void Spreadsheet_ChangeCellFormula_UpdateDependencies()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            spreadsheet.ChangeCellFormula(1, 0, "5"); // A1 = 5
            spreadsheet.ChangeCellFormula(2, 1, "A1"); // B2 = A1

            // B2 is dependantOn A1
            Assert.Contains(spreadsheet.GetCell(1, 0), spreadsheet.GetCell(2, 1).DependentOn);
            // B2 is a dependent of A1
            Assert.Contains(spreadsheet.GetCell(2, 1), spreadsheet.GetCell(1, 0).Dependents);

            spreadsheet.ChangeCellFormula(2, 1, "10"); // B2 = 10 

            Assert.Empty(spreadsheet.GetCell(2, 1).DependentOn);
            Assert.Empty(spreadsheet.GetCell(1, 0).Dependents);
        }
        
        // Test that a complex formula evaluates correctly
        [Fact]
        public void Spreadsheet_ChangeCellFormula_ComplexFormula()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            spreadsheet.ChangeCellFormula(1, 3, "24");      // D1 = 24
            spreadsheet.ChangeCellFormula(2, 1, "D1 + 6");  // B2 = D1(24) + 6  = 30
            spreadsheet.ChangeCellFormula(3, 2, "B2 + B2"); // C3 = B2(30) + B2(30) = 60 
            spreadsheet.ChangeCellFormula(1, 0, "C3 + D1"); // A1 = C3(60) + D1(24) = 84

            Assert.Equal(84, spreadsheet.GetCell(1, 0).Value);
        }
        
        // Test that formula is reset when a cycle is detected
        [Fact]
        public void Spreadsheet_ChangeCellFormula_DetectCycle()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            spreadsheet.ChangeCellFormula(1, 0, "5"); // A1 = 5
            spreadsheet.ChangeCellFormula(2, 1, "A1"); // B2 = A1
            spreadsheet.ChangeCellFormula(1, 0, "B2"); // A1 = B2

            Assert.Equal("5", spreadsheet.GetCell(1, 0).Formula);
        }
    }
}
