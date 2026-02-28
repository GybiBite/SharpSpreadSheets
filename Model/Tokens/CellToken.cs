using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSpreadSheets.Model.Tokens
{
    public class CellToken : IToken
    {
        // The professor specifically mentions a "BadCell" constant for errors
        public const int BadCell = -1;

        public int Column { get; set; } = BadCell; // A=0, B=1, etc. [cite: 72]
        public int Row { get; set; } = BadCell;
    }
}
