using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SharpSpreadSheets.Logic;
using SharpSpreadSheets.Model.Tokens;

namespace SharpSpreadSheets.Model
{
    public class Spreadsheet
    {
        // 2D array of cells representing the spreadsheet grid
        private Cell[,] _cells;

        // Constructor: initializes the spreadsheet with the specified number of rows and columns
        // and fills every spot with a default Cell object
        public Spreadsheet(int rows, int cols) 
        {
            _cells = new Cell[rows, cols];
            for (int i = 0; i < rows; i++)          // loop through each row
                for (int j = 0; j < cols; j++)      // loop through each column
                    _cells[i, j] = new Cell();      // create a new Cell at this position
        }

        // Returns the cell at the specified row and column indices
        public Cell GetCell(int row, int col) { 
            return _cells[row, col];
        }

        // Returns the total number of rows in the spreadsheet
        public int GetNumRows() { return _cells.GetLength(0); }

        // Returns the total number of columns in the spreadsheet
        public int GetNumCols() { return _cells.GetLength(1); }

        // Changes the formula of a cell, rebuilds its expression tree, and recalculates its value
        public void ChangeCellForumla(int row, int col, string newFormula) {
            Cell cell = GetCell(row, col);      // get the cell we want to update
            cell.ChangeFormula(newFormula);     // store the new formula string on the cell

            // convert the formula string from infix to a postfix stack of tokens  
            Stack<IToken> tokenStack = Util.GetFormula(newFormula);

            // create a new expression tree and build it from the postfix token stack
            cell.ExpressionTree = new ExpressionTree();
            cell.ExpressionTree.BuildExpressionTree(tokenStack);

            // recalculate the cell's numeric value by using the new expression tree
            cell.Evaluate(this);    // "this" passes the spreadsheet so cell references can be looked up 
        }
    }
}
