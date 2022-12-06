using System.Data;
using System.Data.SQLite;

namespace FichasHonesko
{
    public partial class Form1 : Form
    {
        string textoDoPastel;
        int counter = 0;
        public class DalHelper
        {
            private static SQLiteConnection sqliteConnection;
            public DalHelper()
            { }
            private static SQLiteConnection DbConnection()
            {
                sqliteConnection = new SQLiteConnection("Data Source=Fichas.sqlite; Version=3;");
                sqliteConnection.Open();
                return sqliteConnection;
            }
            public static void CriarBancoSQLite()
            {
                try
                {
                    SQLiteConnection.CreateFile(@"Fichas.sqlite");
                }
                catch
                {
                    throw;
                }
            }
            public static void CriarTabelaSQlite()
            {
                try
                {
                    using (var cmd = DbConnection().CreateCommand())
                    {
                        cmd.CommandText = "CREATE TABLE IF NOT EXISTS Fichas(id INTEGER PRIMARY KEY AUTOINCREMENT, Nome Varchar(50), Cod VarChar(10), Data VarChar(50))";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "CREATE TABLE IF NOT EXISTS Impressora(id INTEGER PRIMARY KEY AUTOINCREMENT, Nome Varchar(100))";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "CREATE TABLE IF NOT EXISTS Contador(id INTEGER PRIMARY KEY AUTOINCREMENT, Valor int)";
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static DataTable GetFicha(int cod)
            {
                SQLiteDataAdapter da = null;
                DataTable dt = new DataTable();
                try
                {
                    using (var cmd = DbConnection().CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Fichas Where Cod=" + cod;
                        da = new SQLiteDataAdapter(cmd.CommandText, DbConnection());
                        da.Fill(dt);
                        return dt;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static void Add(string pastel, int cod, string data)
            {
                try
                {
                    using (var cmd = DbConnection().CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO Fichas(Nome, cod, data) values (@nome, @cod, @data)";
                        cmd.Parameters.AddWithValue("@nome", pastel);
                        cmd.Parameters.AddWithValue("@cod", cod);
                        cmd.Parameters.AddWithValue("@data", data);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            CarregarListaDeImpressoras();
        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void CarregarListaDeImpressoras()
        {
            impressoraComboBox.Items.Clear();

            foreach (var printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                impressoraComboBox.Items.Add(printer);
            }
        }

        private void btnClick(object sender, EventArgs e)
        {
            textoDoPastel = ((Button)sender).Text;
            chamarImpressao();
        }

        private void chamarImpressao()
        {
            using (var printDocument = new System.Drawing.Printing.PrintDocument())
            {
                printDocument.PrintPage += printDocument_PrintPage;
#pragma warning disable CS8601 // Possível atribuição de referência nula.
                printDocument.PrinterSettings.PrinterName = impressoraComboBox.SelectedItem.ToString();
#pragma warning restore CS8601 // Possível atribuição de referência nula.
                printDocument.Print();
            }
        }

        void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            var printDocument = sender as System.Drawing.Printing.PrintDocument;
            int offset = 125;
            counter++;
            counterText.Text = counter.ToString();
            if (printDocument != null)
            {
                using (var font = new Font("Times New Roman", 18))
                using (var minFont = new Font("Times New Roman", 12))
                using (var brush = new SolidBrush(Color.Black))
                {
                    e.Graphics.DrawLine(Pens.Black, 5, 5, 310, 5);
                    e.Graphics.DrawString(
                        textoDoPastel,
                        font,
                        brush,
                        new RectangleF(5, 20, printDocument.DefaultPageSettings.PrintableArea.Width, printDocument.DefaultPageSettings.PrintableArea.Height));
                    e.Graphics.DrawString(
                        "Data:" + DateTime.Now,
                        font,
                        brush,
                        new RectangleF(5, 50, printDocument.DefaultPageSettings.PrintableArea.Width, printDocument.DefaultPageSettings.PrintableArea.Height));
                    e.Graphics.DrawString(
                        "Honesko Massas - Pedido n: " + counter,
                        minFont,
                        brush,
                        new RectangleF(5, 90, printDocument.DefaultPageSettings.PrintableArea.Width, printDocument.DefaultPageSettings.PrintableArea.Height));

                    e.Graphics.DrawLine(Pens.Black, 20, offset, 310, offset);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (!System.IO.File.Exists(@"Fichas.sqlite"))
                {
                    DalHelper.CriarBancoSQLite();
                    DalHelper.CriarTabelaSQlite();
                } else
                {
                    loadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro : " + ex.Message);
            }
        }

        private void loadData()
        {

        }
    }
}