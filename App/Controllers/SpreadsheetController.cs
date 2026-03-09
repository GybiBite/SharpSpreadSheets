using SharpSpreadSheets.Logic;
using SharpSpreadSheets.Model;
using SharpSpreadSheets.Model.Tokens;
using SharpSpreadSheets.View;

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

            _view.RequestCellValue = (row, col) =>
            {
                var cell = _model.GetCell(row, col);
                // Display empty string for 0 if the formula is also just "0" (untouched cell)
                if (cell.Value == 0 && cell.Formula == "0") return "";
                return cell.Value.ToString();
            };

            // Wire up View events to Controller actions
            _view.CellBeginEdit += OnCellBeginEdit;
            _view.CellEndEdit += OnCellEndEdit;
            _view.SelectionChanged += OnSelectionChanged;
            _view.AddressSubmitted += OnAddressSubmitted;
            _view.FormulaInputSubmitted += OnCellEndEdit;
            _view.ShowAboutRequested += OnShowAboutRequested;
            _view.SaveFileRequested += SaveFile;
            _view.OpenFileRequested += OpenFile;
            _view.DiscardSheetRequested += DiscardSpreadsheet;
        }

        private void OnAddressSubmitted(string address)
        {
            CellToken token = new();
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
                var (row, col) = _view.GetCurrentSelection(); // You'll need to expose this from View
                _view.UpdateAddressBar(row, col);
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

            // 2. Strip the '=' sign 
            if (newFormula.StartsWith('='))
            {
                newFormula = newFormula.Substring(1);
            }

            // 3. Update the model. Your EvaluateCells() logic handles 
            // updating the internal Values of all dependent cells here!
            _model.ChangeCellFormula(row, col, newFormula);

            // 4. Force the view to refresh. It will automatically ask the 
            // Controller for the new values of all visible cells.
            _view.RefreshGrid();

            // 5. Keep the formula bar perfectly in sync
            var cell = _model.GetCell(row, col);
            _view.SetInputText(cell.Formula);
        }

        private void OnShowAboutRequested()
        {
            // 'using' ensures the form is properly disposed of from memory when closed
            using var aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void SaveFile(string filePath)
        {
            _model.SaveToFile(filePath);
        }

        private void OpenFile(string filePath)
        {
            _model.LoadFromFile(filePath);
            _view.RefreshGrid();
        }

        private void DiscardSpreadsheet ()
        {
            _model.EraseSpreadsheet();
            _view.RefreshGrid();
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