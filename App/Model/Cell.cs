using SharpSpreadSheets.Model;
using SharpSpreadSheets.Model.Tokens;
using System.Collections.Generic;

namespace SharpSpreadSheets.Model
{
    public class Cell
    {
        public string Formula { get; set; }
        public int Value { get; set; }
        public ExpressionTree? ExpressionTree { get; set; }
        
        public List<Cell> Dependents { get; private set; } = [];
        public List<Cell> DependentOn { get; private set; } = [];
        
        public int SortStatus = 0; // 0 = unsorted, temp mark == 1, permanent mark == 2

        public Cell()
        {
            Formula = "0";
            Value = 0;
            ExpressionTree = null;
        }

        public void Evaluate(Spreadsheet spreadsheet)
        {
            if (ExpressionTree != null && ExpressionTree.Root != null)
            {
                Value = ExpressionTree.Evaluate(ExpressionTree.Root, spreadsheet);
            }
        }
    }
}