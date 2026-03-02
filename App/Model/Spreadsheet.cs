using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSpreadSheets.Model
{
    public class Spreadsheet
    {
        private Cell[,] _cells;

        public Cell GetCell(int row, int col) { }

        void ChangeCellForumla(int row, int col, string newFormula) {
            GetCell(row, col).ChangeFormula(newFormula);
        }
    }
}
