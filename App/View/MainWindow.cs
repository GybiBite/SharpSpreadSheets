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

            //spreadsheetView.RowHeadersWidth = 10;
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
