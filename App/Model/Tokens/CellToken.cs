using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSpreadSheets.Model.Tokens
{
    public class CellToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }

        // Stubs used by getCellToken and printCellToken logic
        public void setRow(int row) => Row = row;
        public void setColumn(int col) => Column = col;
        public int getRow() => Row;
        public int getColumn() => Column;
    }
}
