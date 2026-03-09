using System;
using System.Windows.Forms;
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
                if (cell.Value == 0 && cell.Formula == "0") return "";
                return cell.Value.ToString();
            };

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

        [STAThread]
        public static void Main()
        {
            ApplicationConfiguration.Initialize();

            var mainWin = new MainWindow();
            var sheet = new Spreadsheet(255, 255);
            _ = new SpreadsheetController(mainWin, sheet);

            Application.Run(mainWin);
        }

        /// <summary>
        /// Parses an address string (e.g., "A1") and updates the view's selected cell if valid.
        /// </summary>
        private void OnAddressSubmitted(string address)
        {
            CellToken token = new();
            Util.GetCellToken(address.ToUpper(), 0, token);

            if (token.GetRow() != Util.BadCell && token.GetColumn() != Util.BadCell)
            {
                if (token.GetRow() < _model.GetNumRows() && token.GetColumn() < _model.GetNumCols())
                {
                    _view.SelectCell(token.GetRow(), token.GetColumn());
                }
                else
                {
                    MessageBox.Show("Cell out of range.", "Invalid Address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                var (row, col) = _view.GetCurrentSelection();
                _view.UpdateAddressBar(row, col);
            }
        }

        private void OnCellBeginEdit(int row, int col)
        {
            var cell = _model.GetCell(row, col);
            _view.SetInputText(cell.Formula);
        }

        private void OnSelectionChanged(int row, int col)
        {
            _view.UpdateAddressBar(row, col);

            var cell = _model.GetCell(row, col);
            _view.SetInputText(cell.Formula);
        }

        /// <summary>
        /// Commits an edited formula to the model, recalculates dependencies, and forces a visual refresh.
        /// </summary>
        private void OnCellEndEdit(int row, int col, string newFormula)
        {
            if (string.IsNullOrWhiteSpace(newFormula))
            {
                newFormula = "0";
            }

            if (newFormula.StartsWith('='))
            {
                newFormula = newFormula[1..];
            }

            _model.ChangeCellFormula(row, col, newFormula);

            _view.RefreshGrid();

            var cell = _model.GetCell(row, col);
            _view.SetInputText(cell.Formula);
        }

        private void OnShowAboutRequested()
        {
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

        private void DiscardSpreadsheet()
        {
            _model.EraseSpreadsheet();
            _view.RefreshGrid();
        }
    }
}