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

        private void SetupEventForwarding()
        {
            spreadsheetView.CellBeginEdit += (s, e) =>
                CellBeginEdit?.Invoke(e.RowIndex, e.ColumnIndex);

            spreadsheetView.CellEndEdit += (s, e) => {
                var newValue = spreadsheetView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";
                CellEndEdit?.Invoke(e.RowIndex, e.ColumnIndex, newValue);
            };

            spreadsheetView.SelectionChanged += (s, e) => {
                if (spreadsheetView.CurrentCell != null)
                    SelectionChanged?.Invoke(spreadsheetView.CurrentCell.RowIndex, spreadsheetView.CurrentCell.ColumnIndex);
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeGrid(255,255);
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

        public void UpdateCellDisplay(int row, int col, int value)
        {
            spreadsheetView.Rows[row].Cells[col].Value = value;

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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
