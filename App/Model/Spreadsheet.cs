using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using SharpSpreadSheets.Logic;
using SharpSpreadSheets.Model.Tokens;

namespace SharpSpreadSheets.Model
{
    public class Spreadsheet(int rows, int cols)
    {
        private readonly Dictionary<(int, int), Cell> _cells = [];
        private readonly int _maxRows = rows;
        private readonly int _maxCols = cols;

        public Cell GetCell(int row, int col)
        {
            if (row < 0 || row >= _maxRows || col < 0 || col >= _maxCols)
            {
                throw new IndexOutOfRangeException("Cell coordinates are out of bounds.");
            }

            if (!_cells.ContainsKey((row, col)))
            {
                _cells[(row, col)] = new Cell();
            }

            return _cells[(row, col)];
        }

        public int GetNumRows() => _maxRows;

        public int GetNumCols() => _maxCols;

        /// <summary>
        /// Updates the formula for a cell and recalculates dependencies. 
        /// If a circular dependency is detected, the change is reverted.
        /// </summary>
        public void ChangeCellFormula(int row, int col, string newFormula)
        {
            Cell thisCell = GetCell(row, col);
            string oldFormula = thisCell.Formula;

            thisCell.Formula = newFormula;

            Stack<IToken> newFormulaTokens = Util.GetFormula(newFormula);

            UpdateDependencies(thisCell, newFormulaTokens);
            UpdateExpressionTree(thisCell, newFormulaTokens);

            if (!EvaluateCells(thisCell))
            {
                ChangeCellFormula(row, col, oldFormula);
            }
        }

        public void EraseSpreadsheet()
        {
            _cells.Clear();
        }

        public void SaveToFile(string filePath)
        {
            var dataToSave = _cells
                .Where(kvp => kvp.Value.Formula != "0" && !string.IsNullOrWhiteSpace(kvp.Value.Formula))
                .Select(kvp => new CellDto
                {
                    Row = kvp.Key.Item1,
                    Column = kvp.Key.Item2,
                    Formula = kvp.Value.Formula
                }).ToList();

            var serializer = new XmlSerializer(typeof(List<CellDto>));
            using var stream = new FileStream(filePath, FileMode.Create);
            serializer.Serialize(stream, dataToSave);
        }

        /// <summary>
        /// Loads spreadsheet data and simulates re-entering formulas to safely rebuild ExpressionTrees and dependency maps.
        /// </summary>
        public void LoadFromFile(string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<CellDto>));
            List<CellDto>? loadedData;

            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                loadedData = serializer.Deserialize(stream) as List<CellDto>;
            }

            EraseSpreadsheet();

            if (loadedData != null)
            {
                foreach (var cellData in loadedData)
                {
                    ChangeCellFormula(cellData.Row, cellData.Column, cellData.Formula);
                }
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

        private static void UpdateExpressionTree(Cell thisCell, Stack<IToken> formulaTokens)
        {
            thisCell.ExpressionTree = new ExpressionTree();
            thisCell.ExpressionTree.BuildExpressionTree(new Stack<IToken>(formulaTokens.Reverse()));
        }

        /// <summary>
        /// Performs a topological sort to detect cyclic errors and evaluate affected cells in the correct order.
        /// </summary>
        private bool EvaluateCells(Cell thisCell)
        {
            Stack<Cell> sortedCells = new();
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