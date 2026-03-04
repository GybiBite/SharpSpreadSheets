using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSpreadSheets.Model
{
    public class Spreadsheet
    {
        private Dictionary<(int, int), Cell> _cells = new Dictionary<(int, int), Cell>();

        public Spreadsheet() { }

        public Cell GetCell(int row, int col)
        {
            if (!_cells.TryGetValue((row, col), out Cell cell))
            {
                cell = new Cell();
                _cells[(row, col)] = cell;
            }
            return cell;
        }

        void ChangeCellForumla(int row, int col, string newFormula) {
            GetCell(row, col).ChangeFormula(newFormula);
        }
    }
}
