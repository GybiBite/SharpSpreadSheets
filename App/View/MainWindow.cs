using SharpSpreadSheets.Logic;
using SharpSpreadSheets.Model.Tokens;

namespace SharpSpreadSheets
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

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

        private void SetupEventForwarding()
        {
            // 1. Ask the Controller for the value when the grid needs to draw a cell
            spreadsheetView.CellValueNeeded += (s, e) =>
            {
                if (RequestCellValue != null)
                {
                    e.Value = RequestCellValue(e.RowIndex, e.ColumnIndex);
                }
            };

            // 2. Tell the Controller when the user types directly into a cell
            spreadsheetView.CellValuePushed += (s, e) =>
            {
                string newValue = e.Value?.ToString() ?? "";
                CellEndEdit?.Invoke(e.RowIndex, e.ColumnIndex, newValue);
            };

            // Keep your existing selection and begin-edit events
            spreadsheetView.CellBeginEdit += (s, e) =>
                CellBeginEdit?.Invoke(e.RowIndex, e.ColumnIndex);

            spreadsheetView.SelectionChanged += (s, e) =>
            {
                if (spreadsheetView.CurrentCell != null)
                    SelectionChanged?.Invoke(spreadsheetView.CurrentCell.RowIndex, spreadsheetView.CurrentCell.ColumnIndex);
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeGrid(255, 255);
        }

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

            //spreadsheetView.RowHeadersWidth = 10;
        }

        public void SetInputText(string Forumla)
        {
            FormulaInputBox.Text = Forumla;
        }

        public void RefreshGrid()
        {
            // This tells the WinForms rendering engine "the data changed, redraw the grid."
            // Because of Virtual Mode, it will instantly fire CellValueNeeded 
            // ONLY for the cells currently visible on screen. Super efficient!
            spreadsheetView.Invalidate();
        }

        public void UpdateCellDisplay(int row, int col, int value)
        {
            spreadsheetView.InvalidateCell(col, row);
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

        public void SelectCell(int row, int col)
        {
            if (row >= 0 && row < spreadsheetView.RowCount && col >= 0 && col < spreadsheetView.ColumnCount)
            {
                spreadsheetView.CurrentCell = spreadsheetView.Rows[row].Cells[col];

                spreadsheetView.FirstDisplayedScrollingRowIndex = Math.Max(0, row - 2);
            }

            UpdateAddressBar(row, col);
        }

        private void JumpCellBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddressSubmitted?.Invoke(JumpCellBox.Text);
                e.SuppressKeyPress = true; // Prevents the 'ding' sound
            }
        }

        public (int row, int col) GetCurrentSelection()
        {
            // Safety check in case nothing is selected
            if (spreadsheetView.CurrentCell != null)
            {
                return (spreadsheetView.CurrentCell.RowIndex, spreadsheetView.CurrentCell.ColumnIndex);
            }

            return (0, 0);
        }

        private void FormulaInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var (row, col) = GetCurrentSelection();
                FormulaInputSubmitted?.Invoke(row, col, FormulaInputBox.Text);

                e.SuppressKeyPress = true;
                spreadsheetView.Focus(); // Return focus to the grid
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Restrict the user to opening specific file types
                openFileDialog.Filter = "SharpSpreadSheets file (*.sss)|*.sss";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Pass the selected file path to the Controller
                    OpenFileRequested?.Invoke(openFileDialog.FileName);
                }
            }
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowAboutRequested?.Invoke();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                // Ensure it saves with your custom extension by default
                saveFileDialog.Filter = "SharpSpreadSheets file (*.sss)|*.sss";
                saveFileDialog.DefaultExt = "sss";
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Pass the chosen file path to the Controller
                    SaveFileRequested?.Invoke(saveFileDialog.FileName);
                }
            }
        }
    }
}
