using System.Xml.Serialization;

namespace SharpSpreadSheets.Model
{
    // A lightweight representation of a cell for safe saving/loading
    public class CellDto
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Formula { get; set; } = "";
    }
}