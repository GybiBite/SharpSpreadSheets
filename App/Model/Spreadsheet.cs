using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

        // Changes the formula of a cell and evaluates effected cells. Reverts changes if a cycle is detected.
        public void ChangeCellFormula(int row, int col, string newFormula) {
            Cell thisCell = GetCell(row, col); 
            string oldFormula = thisCell.Formula; // Save previous formula in case of cell evaluation failure
            
            thisCell.ChangeFormula(newFormula);
            
            Stack<IToken> newFormulaTokens = Util.GetFormula(newFormula); // convert formula string to a stack of tokens  
            
            UpdateDependencies(thisCell, newFormulaTokens); // stack is preserved
            UpdateExpressionTree(thisCell, newFormulaTokens);
            
            // if cell evaluation fails, revert to previous formula
            if (!EvaluateCells(thisCell))
            {
                ChangeCellFormula(row, col, oldFormula);
            }
        }

        // Updates dependencies for this cell and other effected cells
        private void UpdateDependencies(Cell thisCell, Stack<IToken> formulaTokens)
        {
            // Remove old dependencies
            foreach (var otherCell in thisCell.DependentOn)
            {
                otherCell.Dependents.Remove(thisCell);
            }
            
            thisCell.DependentOn.Clear();

            // Add new dependencies 
            foreach (var token in formulaTokens)
            {
                if (token is CellToken otherCellToken)
                {
                    Cell otherCell = GetCell(otherCellToken.Row, otherCellToken.Column);
                    
                    thisCell.DependentOn.Add(otherCell);
                    otherCell.Dependents.Add(thisCell);
                }
            }
        }
        
        // create a new expression tree using the new formula
        private void UpdateExpressionTree(Cell thisCell, Stack<IToken> formulaTokens)
        {
            thisCell.ExpressionTree = new ExpressionTree();
            thisCell.ExpressionTree.BuildExpressionTree(formulaTokens);
        }
        
        // Calculate required order and evaluate cells, returns 0 if a cycle is found
        private bool EvaluateCells(Cell thisCell)
        {
            Stack<Cell> sortedCells = new Stack<Cell>();
            bool cyclicError = false;
            
            VisitCell(thisCell);

            if (cyclicError)
            {
                foreach (var cell in sortedCells)
                {
                    cell.SortStatus = 0;
                }
            } 
            else
            {
                foreach (var cell in sortedCells)
                {
                    cell.SortStatus = 0;
                    cell.Evaluate(this);
                }
            }
            
            return !cyclicError;
            
            void VisitCell(Cell currentCell)
            {
                switch (currentCell.SortStatus)
                {
                    case 2:
                        return;
                    case 1:
                        cyclicError = true; 
                        return;
                }

                currentCell.SortStatus = 1;

                foreach (var dependantCell in currentCell.Dependents)
                {
                    VisitCell(dependantCell);
                }

                currentCell.SortStatus = 2;
                
                sortedCells.Push(currentCell);
            }
        }
    }
    
    
}
