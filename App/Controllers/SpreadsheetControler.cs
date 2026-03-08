using SharpSpreadSheets.Logic;
using SharpSpreadSheets.Model;
using SharpSpreadSheets.Model.Tokens;

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
            _view.AddressSubmitted += OnAddressSubmitted;
            _view.FormulaInputSubmitted += OnCellEndEdit;
        }

        private void OnAddressSubmitted(string address)
        {
            CellToken token = new CellToken();
            // Utilize your existing logic in Util.cs
            Util.GetCellToken(address.ToUpper(), 0, token);

            if (token.getRow() != Util.BadCell && token.getColumn() != Util.BadCell)
            {
                // Validate against model boundaries
                if (token.getRow() < _model.GetNumRows() && token.getColumn() < _model.GetNumCols())
                {
                    _view.SelectCell(token.getRow(), token.getColumn());
                }
                else
                {
                    MessageBox.Show("Cell out of range.");
                }
            }
            else
            {
                // Revert to current selection if invalid input
                var current = _view.GetCurrentSelection(); // You'll need to expose this from View
                _view.UpdateAddressBar(current.row, current.col);
            }
        }

        private void OnCellBeginEdit(int row, int col)
        {
            // When the user clicks a cell, show the FORMULA in the input box
            var cell = _model.GetCell(row, col);
            _view.SetInputText(cell.Formula);
        }

        private void OnSelectionChanged(int row, int col)
        {
            // Update the Address box
            _view.UpdateAddressBar(row, col);

            // ADD THIS: Fetch the cell and update the formula bar immediately when selected
            var cell = _model.GetCell(row, col);
            _view.SetInputText(cell.Formula);
        }

        private void OnCellEndEdit(int row, int col, string newFormula)
        {
            // 1. Prevent parser crashes on empty cells
            if (string.IsNullOrWhiteSpace(newFormula))
            {
                newFormula = "0";
            }

            // 2. Strip the '=' sign because Util.GetFormula does not support it
            if (newFormula.StartsWith("="))
            {
                newFormula = newFormula.Substring(1);
            }

            // 3. Update the model
            _model.ChangeCellFormula(row, col, newFormula);

            // 4. Get the evaluated result and update the grid
            var cell = _model.GetCell(row, col);
            _view.UpdateCellDisplay(row, col, cell.Value);

            // 5. Keep the formula bar perfectly in sync with the backend
            _view.SetInputText(cell.Formula);
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