using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSpreadSheets.Model
{
    internal class Cell
    {
        // The raw formula string as entered by the user (e.g. "5 + B3 * 8")
        public string Formula { get; set; }

        // The computed integer result of evaluating the formula
        public int Value { get; set; }

        // The expression tree built from the formula, used to evaluate the cell's value
        // TODO: This gets assigned in Spreadsheet.ChangeCellFormulaAndRecalculate()
        // when a user changes a cell's formula
        public ExpressionTree ExpressionTree { get; set; }

        // Constructor initializes a cell to a default state
        // Per the assignment, empty cells are treated as having formula "0" and value 0
        public Cell() 
        {
            Formula = "0";
            Value = 0;
            ExpressionTree = null;
        }

        // Evaluates this cell's expression tree and stores the result in Value
        // Requires the spreadsheet so that any referenced cells (e.g. A3) can be looked up
        // This will be called during the topological sort in Spreadsheet.cs
        // TODO: Will not work until Spreadsheet.GetCellValue() is implemented
        public void Evaluate(Spreadsheet spreadsheet)
        {
            // Only evaluate if a formula has been assigned
            // Cells with no formula stay at their default value of 0
            if (ExpressionTree != null)
            {
                Value = ExpressionTree.Evaluate(ExpressionTree.Root, spreadsheet);
            }
            
        }
    }
}
