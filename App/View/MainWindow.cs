using SharpSpreadSheets.Logic;
using SharpSpreadSheets.Model.Tokens;

namespace SharpSpreadSheets
{
    public partial class MainWindow : Form
    {
        // --- Public Events & Callbacks ---

        public event Action<int, int>? CellBeginEdit;
        public event Action<int, int, string>? CellEndEdit;
        public event Action<int, int>? SelectionChanged;
        public event Action<string>? AddressSubmitted;
        public event Action<int, int, string>? FormulaInputSubmitted;
        public event Action? ShowAboutRequested;
        public event Action? DiscardSheetRequested;
        public event Action<string>? OpenFileRequested;
        public event Action<string>? SaveFileRequested;

        public Func<int, int, string>? RequestCellValue;

        // --- Constructor ---

        public MainWindow()
        {
            InitializeComponent();
        }

        // --- Public Methods ---

        /// <summary>
        /// Sets up the visual grid with the specified number of rows and columns, including alphabetical column headers.
        /// </summary>
        public void InitializeGrid(int rows, int cols)
        {
            spreadsheetView.Rows.Clear();
            spreadsheetView.Columns.Clear();

            for (int i = 0; i < cols; i++)
            {
                string fullCoord = Util.PrintCellToken(new CellToken { Column = i, Row = 0 });
                string colName = new([.. fullCoord.TakeWhile(char.IsLetter)]);

                spreadsheetView.Columns.Add(colName, colName);
                spreadsheetView.Columns[i].Width = 60;
            }

            spreadsheetView.Rows.Add(rows);

            for (int j = 0; j < rows; j++)
            {
                spreadsheetView.Rows[j].HeaderCell.Value = j.ToString();
            }

            SetupEventForwarding();
        }

        public void SetInputText(string formula)
        {
            FormulaInputBox.Text = formula;
        }

        /// <summary>
        /// Forces the WinForms rendering engine to redraw the grid, instantly firing CellValueNeeded for visible cells.
        /// </summary>
        public void RefreshGrid()
        {
            spreadsheetView.Invalidate();
        }

        public void UpdateAddressBar(int row, int col)
        {
            CellToken dispToken = new()
            {
                Column = col,
                Row = row
            };

            JumpCellBox.Text = Util.PrintCellToken(dispToken);
        }

        /// <summary>
        /// Navigates the grid to a specific cell, scrolling it into the user's view if necessary.
        /// </summary>
        public void SelectCell(int row, int col)
        {
            if (row >= 0 && row < spreadsheetView.RowCount && col >= 0 && col < spreadsheetView.ColumnCount)
            {
                spreadsheetView.CurrentCell = spreadsheetView.Rows[row].Cells[col];
                spreadsheetView.FirstDisplayedScrollingRowIndex = Math.Max(0, row - 2);
            }

            UpdateAddressBar(row, col);
        }

        public (int row, int col) GetCurrentSelection()
        {
            if (spreadsheetView.CurrentCell != null)
            {
                return (spreadsheetView.CurrentCell.RowIndex, spreadsheetView.CurrentCell.ColumnIndex);
            }

            return (0, 0);
        }

        // --- Private Methods (Event Handlers & Helpers) ---

        private void SetupEventForwarding()
        {
            spreadsheetView.CellValueNeeded += (s, e) =>
            {
                if (RequestCellValue != null)
                {
                    e.Value = RequestCellValue(e.RowIndex, e.ColumnIndex);
                }
            };

            spreadsheetView.CellValuePushed += (s, e) =>
            {
                string newValue = e.Value?.ToString() ?? "";
                CellEndEdit?.Invoke(e.RowIndex, e.ColumnIndex, newValue);
            };

            spreadsheetView.CellBeginEdit += (s, e) =>
                CellBeginEdit?.Invoke(e.RowIndex, e.ColumnIndex);

            spreadsheetView.SelectionChanged += (s, e) =>
            {
                if (spreadsheetView.CurrentCell != null)
                {
                    SelectionChanged?.Invoke(spreadsheetView.CurrentCell.RowIndex, spreadsheetView.CurrentCell.ColumnIndex);
                }
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeGrid(255, 255);
        }

        private void JumpCellBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddressSubmitted?.Invoke(JumpCellBox.Text);
                e.SuppressKeyPress = true;
            }
        }

        private void FormulaInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var (row, col) = GetCurrentSelection();
                FormulaInputSubmitted?.Invoke(row, col, FormulaInputBox.Text);

                e.SuppressKeyPress = true;
                spreadsheetView.Focus();
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "SharpSpreadSheets file (*.sss)|*.sss";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenFileRequested?.Invoke(openFileDialog.FileName);
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "SharpSpreadSheets file (*.sss)|*.sss";
            saveFileDialog.DefaultExt = "sss";
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveFileRequested?.Invoke(saveFileDialog.FileName);
            }
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult confirmResult = MessageBox.Show(
                "Are you sure you want to clear this spreadsheet?",
                "New Spreadsheet",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                DiscardSheetRequested?.Invoke();
            }
        }

        private void AboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowAboutRequested?.Invoke();
        }
    }
}