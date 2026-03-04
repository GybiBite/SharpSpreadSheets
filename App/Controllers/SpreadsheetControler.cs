using SharpSpreadSheets.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSpreadSheets.Controllers
{
    public class SpreadsheetControler
    {
        private readonly static MainWindow View = new();
        private readonly static Spreadsheet Spreadsheet = new();

        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(View);
        }


    }
}
