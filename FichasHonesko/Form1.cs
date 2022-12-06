using System.Data;
using System.Data.SQLite;
using System.Diagnostics;

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
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static String[] GetFicha(string cod)
            {
                try
                {
                    using (var cmd = DbConnection().CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Fichas Where Cod=" + cod;
                        using var get = new SQLiteCommand(cmd.CommandText, DbConnection());
                        using SQLiteDataReader rdr = get.ExecuteReader();
                        string[] data = new string[3];
                        while (rdr.Read())
                        {
                            data[0] = rdr.GetString(1);
                            data[1] = rdr.GetString(2);
                            data[2] = rdr.GetString(3);
                        }
                        return data;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static int GetQtdFicha()
            {
                try
                {
                    int qtd = 0;
                    using (var cmd = DbConnection().CreateCommand())
                    {
                        cmd.CommandText = "SELECT COUNT(id) as qtd FROM Fichas";
                        using var get = new SQLiteCommand(cmd.CommandText, DbConnection());
                        using SQLiteDataReader rdr = get.ExecuteReader();
                        while (rdr.Read())
                        {
                            qtd = rdr.GetInt32(0);
                        }
                        return qtd;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static void Add(string pastel, string cod, string data)
            {
                try
                {
                    using (var cmd = DbConnection().CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO Fichas(Nome, Cod, Data) values (@nome, @cod, @data)";
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

            public static void AddImpressora(string impressora)
            {
                try
                {
                    using (var cmd = DbConnection().CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM Impressora WHERE 1";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "INSERT INTO Impressora(Nome) values (@nome)";
                        cmd.Parameters.AddWithValue("@nome", impressora);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static string GetImpressora()
            {
                try
                {
                    string impressora = "";
                    using (var cmd = DbConnection().CreateCommand())
                    {
                        cmd.CommandText = "SELECT Nome FROM Impressora ORDER BY Nome DESC LIMIT 1";
                        using var get = new SQLiteCommand(cmd.CommandText, DbConnection());
                        using SQLiteDataReader rdr = get.ExecuteReader();
                        while (rdr.Read())
                        {
                            impressora = rdr.GetString(0);
                        }
                        return impressora;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static Boolean codigoExiste(string cod)
            {
                try
                {
                    using (var cmd = DbConnection().CreateCommand())
                    {
                        cmd.CommandText = "SELECT Nome FROM Fichas WHERE Cod=" + cod;
                        var existe = false;
                        using var get = new SQLiteCommand(cmd.CommandText, DbConnection());
                        using SQLiteDataReader rdr = get.ExecuteReader();
                        while (rdr.Read())
                        {
                            if(rdr.GetString(0) != "" && rdr.GetString(0) != null)
                            {
                                existe = true;
                            }
                        }
                        return existe;
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
            if (impressoraComboBox.SelectedIndex == -1)
            {
                string menssagem = "Por favor, Selecione uma impressora.";
                MessageBox.Show(menssagem);
            } 
            else
            {
                chamarImpressao();
            }
        }

        private void chamarImpressao()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("Erro : " + ex.Message);
            }            
        }

        void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            var printDocument = sender as System.Drawing.Printing.PrintDocument;
            int offset = 139;
            counter++;
            counterText.Text = counter.ToString();
            if (printDocument != null)
            {
                var data = DateTime.Now;
                var cod = "";
                Random randNum = new Random();
                for (int i = 0; i <= 6; i++)
                {
                    cod += randNum.Next(9);
                }

                while (DalHelper.codigoExiste(cod))
                {
                    cod = "";
                    for (int i = 0; i <= 6; i++)
                    {
                        cod += randNum.Next(9);
                    }
                }

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
                        "Data:" + data,
                        font,
                        brush,
                        new RectangleF(5, 50, printDocument.DefaultPageSettings.PrintableArea.Width, printDocument.DefaultPageSettings.PrintableArea.Height));
                    e.Graphics.DrawString(
                        "Honesko Massas - Pedido n: " + counter,
                        minFont,
                        brush,
                        new RectangleF(7, 84, printDocument.DefaultPageSettings.PrintableArea.Width, printDocument.DefaultPageSettings.PrintableArea.Height));
                    e.Graphics.DrawString(
                        "Código: " + cod,
                        minFont,
                        brush,
                        new RectangleF(7, 104, printDocument.DefaultPageSettings.PrintableArea.Width, printDocument.DefaultPageSettings.PrintableArea.Height));

                    e.Graphics.DrawLine(Pens.Black, 5, offset, 310, offset);
                    Image i = Image.FromFile(@"honesko.png");
                    e.Graphics.DrawImage(i, 260, 88, 50, 48);
                }
                DalHelper.Add(textoDoPastel, cod, data.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                CarregarListaDeImpressoras();
                if (!System.IO.File.Exists(@"Fichas.sqlite"))
                {
                    DalHelper.CriarBancoSQLite();
                    DalHelper.CriarTabelaSQlite();
                } 
                else
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
            string impressora = DalHelper.GetImpressora();
            impressoraComboBox.SelectedIndex = impressoraComboBox.FindStringExact(impressora);
            counter = DalHelper.GetQtdFicha();
            counterText.Text = DalHelper.GetQtdFicha().ToString();
        }

        private void impressoraComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            DalHelper.AddImpressora(((ComboBox)sender).SelectedItem.ToString());
        }

        private void btnVerifica_Click(object sender, EventArgs e)
        {
            if (txtCodigo.Text != "" && txtCodigo.Text != null)
            {
                var data = DalHelper.GetFicha(txtCodigo.Text);
                if (data[0] != "" && data[0] != null)
                {
                    lbValido.Text = "Status: VÁLIDO";
                    lbValido.ForeColor = Color.Green;
                    lbName.Text = "Pastel: " + data[0];
                    lbVenda.Text = "Venda: " + data[2];
                }
                else
                {
                    lbValido.Text = "Status: INVÁLIDO";
                    lbValido.ForeColor = Color.Red;
                    lbName.Text = "Pastel: ";
                    lbVenda.Text = "Venda: ";
                }
            } else
            {
                MessageBox.Show("Por Favor, Insira um Código.");
            }            
        }
    }
}