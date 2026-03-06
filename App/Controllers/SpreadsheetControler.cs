using SharpSpreadSheets.Model;
using SharpSpreadSheets.Logic;

namespace SharpSpreadSheets.Controllers
{
    public class SpreadsheetController
    {
        private readonly MainWindow _view;
        private readonly Spreadsheet _model;

        public SpreadsheetController(MainWindow view, Spreadsheet model)
        {
            _view = view;
            _model = model;

            // Wire up View events to Controller actions
            _view.CellBeginEdit += OnCellBeginEdit;
            _view.CellEndEdit += OnCellEndEdit;
            _view.SelectionChanged += OnSelectionChanged;
        }

        private void OnCellBeginEdit(int row, int col)
        {
            // When the user clicks a cell, show the FORMULA in the input box
            var cell = _model.GetCell(row, col);
            _view.SetInputText(cell.Formula);
        }

        private void OnCellEndEdit(int row, int col, string newFormula)
        {
            // Update the model
            _model.GetCell(row, col).ChangeFormula(newFormula);

            // In a true Observer pattern, the Model would fire an event 
            // that the Controller hears to update the View's display value.
            var cell = _model.GetCell(row, col);
            _view.UpdateCellDisplay(row, col, cell.Value);
        }

        private void OnSelectionChanged(int row, int col)
        {
            _view.UpdateAddressBar(row, col);
        }

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var mainWin = new MainWindow();
            var sheet = new Spreadsheet(255,255);

            // Initialize the controller to bridge them
            var controller = new SpreadsheetController(mainWin, sheet);

            Application.Run(mainWin);
        }
    }
}