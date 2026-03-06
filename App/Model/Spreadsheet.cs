using SharpSpreadSheets.Logic;
using SharpSpreadSheets.Model.Tokens;

namespace SharpSpreadSheets.Model
{
    public class Spreadsheet
    {
        // Use a tuple (row, col) as the key for O(1) lookups in a sparse grid
        private readonly Dictionary<(int, int), Cell> _cells = new();
        private readonly int _maxRows;
        private readonly int _maxCols;

        public Spreadsheet(int rows, int cols)
        {
            _maxRows = rows;
            _maxCols = cols;
            // No need to loop and fill! The dictionary starts empty to save memory.
        }

        // Returns the cell at the specified indices; creates it if it doesn't exist
        public Cell GetCell(int row, int col)
        {
            if (row < 0 || row >= _maxRows || col < 0 || col >= _maxCols)
                throw new IndexOutOfRangeException("Cell coordinates are out of bounds.");

            if (!_cells.ContainsKey((row, col)))
            {
                _cells[(row, col)] = new Cell();
            }
            return _cells[(row, col)];
        }

        public int GetNumRows() => _maxRows;
        public int GetNumCols() => _maxCols;

        public void ChangeCellFormula(int row, int col, string newFormula)
        {
            Cell thisCell = GetCell(row, col);
            string oldFormula = thisCell.Formula;

            thisCell.ChangeFormula(newFormula);

            // Note: Ensure your Util.GetFormula doesn't destroy the stack if you iterate it
            Stack<IToken> newFormulaTokens = Util.GetFormula(newFormula);

            UpdateDependencies(thisCell, newFormulaTokens);
            UpdateExpressionTree(thisCell, newFormulaTokens);

            if (!EvaluateCells(thisCell))
            {
                // Recursive call to revert; ensure ChangeFormula triggers a re-calc
                ChangeCellFormula(row, col, oldFormula);
            }
        }

        private void UpdateDependencies(Cell thisCell, Stack<IToken> formulaTokens)
        {
            foreach (var otherCell in thisCell.DependentOn)
            {
                otherCell.Dependents.Remove(thisCell);
            }
            thisCell.DependentOn.Clear();

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

        private void UpdateExpressionTree(Cell thisCell, Stack<IToken> formulaTokens)
        {
            thisCell.ExpressionTree = new ExpressionTree();
            // Passing a copy of the stack is safer if BuildExpressionTree consumes it
            thisCell.ExpressionTree.BuildExpressionTree(new Stack<IToken>(formulaTokens.Reverse()));
        }

        private bool EvaluateCells(Cell thisCell)
        {
            Stack<Cell> sortedCells = new Stack<Cell>();
            bool cyclicError = false;

            VisitCell(thisCell);

            if (cyclicError)
            {
                foreach (var cell in sortedCells) cell.SortStatus = 0;
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
                if (currentCell.SortStatus == 2) return;
                if (currentCell.SortStatus == 1)
                {
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