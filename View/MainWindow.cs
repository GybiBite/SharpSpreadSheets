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
            spreadsheetView.Columns.Clear(); // Remove designer columns

            // Create columns A through H (8 columns)
            for (int i = 0; i < 8; i++)
            {
                string colName = GetColumnName(i); // Your C# version of printCellToken
                spreadsheetView.Columns.Add(colName, colName);
            }

            // Set initial row count
            spreadsheetView.RowCount = 8;
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
    }
}
