using SharpSpreadSheets.Model;
using SharpSpreadSheets.Model.Tokens;
using System.Collections.Generic;

namespace SharpSpreadSheets.Model
{
    public class Cell
    {
        public string Formula { get; set; } = "0";
        public int Value { get; set; }
        public ExpressionTree ExpressionTree { get; set; }
        public List<CellToken> Dependencies { get; private set; } = new List<CellToken>();

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

        public void ChangeFormula(string newFormula)
        {
            this.Formula = newFormula;
        }
    }
}