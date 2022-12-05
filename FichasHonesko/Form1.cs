using System.Diagnostics;

namespace FichasHonesko
{
    public partial class Form1 : Form
    {
        String textoDoPastel;
        public Form1()
        {
            InitializeComponent();
            CarregarListaDeImpressoras();
        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

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

            if (printDocument != null)
            {
                using (var font = new Font("Times New Roman", 18))
                using (var brush = new SolidBrush(Color.Black))
                {
                    e.Graphics.DrawString(
                        textoDoPastel,
                        font,
                        brush,
                        new RectangleF(0, 0, printDocument.DefaultPageSettings.PrintableArea.Width, printDocument.DefaultPageSettings.PrintableArea.Height));
                }
            }
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
            chamarImpressao();
        }
    }
}