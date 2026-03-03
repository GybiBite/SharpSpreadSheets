using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSpreadSheets.Model;
using SharpSpreadSheets.Logic;
using SharpSpreadSheets.Model.Tokens;
using Xunit;

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
            spreadsheet.ChangeCellForumla(0, 0, "5+3");
            Assert.Equal("5+3", spreadsheet.GetCell(0, 0).Formula);
        }

        // Test that ChangeCellFormula recalculates the cell value
        [Fact]
        public void Spreadsheet_ChangeCellFormula_RecalculatesValue()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            spreadsheet.ChangeCellForumla(0, 0, "5+3");
            Assert.Equal(8, spreadsheet.GetCell(0, 0).Value);
        }

        // Test that one cell can reference another cell's value
        [Fact]
        public void Spreadsheet_CellReference_EvaluatesCorrectly()
        {
            Spreadsheet spreadsheet = new Spreadsheet(5, 5);
            spreadsheet.ChangeCellForumla(1, 0, "5");    // A0 = 5
            spreadsheet.ChangeCellForumla(2, 0, "A1+2"); // A2 = A1 + 2 = 7
            Assert.Equal(7, spreadsheet.GetCell(2, 0).Value);
        }
    }
}
