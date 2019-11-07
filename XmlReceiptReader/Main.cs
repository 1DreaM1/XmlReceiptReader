using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XmlReceiptReader
{
    public partial class Main : Form
    {
        private int rowIndex;
        private bool stopProcess = false;

        public static double vyberyAmount;
        public static double vkladyAmount;
        public static double invoicesAmount;
        public static double obratAmount;

        public static double sumPaycash = 0;
        public static double sumPaycard = 0;
        public static double sumPaystr = 0;
        public static double sumPayvmp = 0;

        public static double sumPaycashInvoice = 0;
        public static double sumPaycardInvoice = 0;

        public static double sumTaxBaseBasic = 0;
        public static double sumTaxBaseReduced = 0;
        public static double sumBasicVatAmount = 0;
        public static double sumReducedVatAmount = 0;
        public static double sumTaxFreeAmount = 0;

        public static string firstDate;
        public static string lastDate;

        public Main(string loadMethodName = "")
        {
            InitializeComponent();

            CultureInfo nonInvariantCulture = new CultureInfo("en-US");
            nonInvariantCulture.NumberFormat.NumberDecimalSeparator = ".";
            nonInvariantCulture.NumberFormat.NumberGroupSeparator = " ";
            Thread.CurrentThread.CurrentCulture = nonInvariantCulture;

        }

        private void LoadGridData()
        {
            SqlDriver driver = new SqlDriver();
            string[,] items = driver.GetAllReceipts();
            dataGridView.Rows.Clear();

            int receiptsCount = 0;
            double priceSum = 0;
            vyberyAmount = 0;
            vkladyAmount = 0;
            invoicesAmount = 0;
            sumTaxBaseBasic = 0;
            sumTaxBaseReduced = 0;
            sumBasicVatAmount = 0;
            sumReducedVatAmount = 0;
            sumTaxFreeAmount = 0;
            sumPaycard = 0;
            sumPaycash = 0;
            sumPaystr = 0;
            sumPayvmp = 0;
            sumPaycardInvoice = 0;
            sumPaycashInvoice = 0;

            for (int index = 0; index < items.GetLength(0); index++)
            {
                dataGridView.Rows.Add();
                dataGridView.Rows[index].Cells[0].Value = items[index, 0];
                dataGridView.Rows[index].Cells[1].Value = items[index, 5];
                dataGridView.Rows[index].Cells[2].Value = items[index, 1];
                dataGridView.Rows[index].Cells[3].Value = items[index, 2];
                dataGridView.Rows[index].Cells[4].Value = items[index, 3];
                dataGridView.Rows[index].Cells[5].Value = items[index, 4];
                dataGridView.Rows[index].Cells[6].Value = items[index, 6];
                dataGridView.Rows[index].Cells[7].Value = items[index, 8];
                dataGridView.Rows[index].Cells[8].Value = items[index, 7];
                dataGridView.Rows[index].Cells[9].Value = items[index, 9];
                dataGridView.Rows[index].Cells[10].Value = items[index, 10];
                dataGridView.Rows[index].Cells[11].Value = items[index, 11];
                dataGridView.Rows[index].Cells[12].Value = items[index, 13];
                dataGridView.Rows[index].Cells[13].Value = items[index, 15];
                dataGridView.Rows[index].Cells[14].Value = items[index, 14];
                dataGridView.Rows[index].Cells[15].Value = items[index, 16];
                dataGridView.Rows[index].Cells[16].Value = items[index, 17];
                dataGridView.Rows[index].Cells[17].Value = items[index, 12];

                if (items[index, 6].Equals("ONLINE") || items[index, 6].Equals("OFFLINE"))
                {
                    if (items[index, 11].Equals("UF"))
                    {
                        invoicesAmount += double.Parse(items[index, 2], CultureInfo.InvariantCulture);
                        sumPaycardInvoice += items[index, 7] == String.Empty ? 0 : double.Parse(items[index, 7], CultureInfo.InvariantCulture);
                        sumPaycashInvoice += items[index, 8] == String.Empty ? 0 : double.Parse(items[index, 8], CultureInfo.InvariantCulture);
                    }
                    if (items[index, 11].Equals("VY"))
                    {
                        vyberyAmount += double.Parse(items[index, 2], CultureInfo.InvariantCulture);
                    }
                    if (items[index, 11].Equals("VK"))
                    {
                        vkladyAmount += double.Parse(items[index, 2], CultureInfo.InvariantCulture);
                    }
                    if (items[index, 11].Equals("PD"))
                    {
                        priceSum += double.Parse(items[index, 2], CultureInfo.InvariantCulture);
                        sumPaycard += items[index, 7] == String.Empty ? 0 : double.Parse(items[index, 7], CultureInfo.InvariantCulture);
                        sumPaycash += items[index, 8] == String.Empty ? 0 : double.Parse(items[index, 8], CultureInfo.InvariantCulture);
                        sumPaystr += items[index, 9] == String.Empty ? 0 : double.Parse(items[index, 9], CultureInfo.InvariantCulture);
                        sumPayvmp += items[index, 10] == String.Empty ? 0 : double.Parse(items[index, 10], CultureInfo.InvariantCulture);
                    }

                    receiptsCount++;

                    sumTaxBaseBasic += items[index, 13] == String.Empty ? 0 : double.Parse(items[index, 13], CultureInfo.InvariantCulture);
                    sumTaxBaseReduced += items[index, 14] == String.Empty ? 0 : double.Parse(items[index, 14], CultureInfo.InvariantCulture);
                    sumBasicVatAmount += items[index, 15] == String.Empty ? 0 : double.Parse(items[index, 15], CultureInfo.InvariantCulture);
                    sumReducedVatAmount += items[index, 16] == String.Empty ? 0 : double.Parse(items[index, 16], CultureInfo.InvariantCulture);
                    sumTaxFreeAmount += items[index, 17] == String.Empty ? 0 : double.Parse(items[index, 17], CultureInfo.InvariantCulture);
                }
            }

            if (dataGridView.Rows.Count > 0)
            {
                firstDate = dataGridView.Rows[0].Cells[1].Value.ToString();
                lastDate = dataGridView.Rows[dataGridView.Rows.Count - 1].Cells[1].Value.ToString();
            }
            else
            {
                var now = DateTime.Now;
                firstDate = now.ToString("dd.MM.yyyy HH.mm.ss");
                lastDate = now.ToString("dd.MM.yyyy HH.mm.ss");
            }

            toolStripPD.Text = receiptsCount.ToString();
            toolStripZisk.Text = priceSum.ToString() + " €";
            obratAmount = priceSum;

            toolTaxBaseBasic.Text = sumTaxBaseBasic.ToString("N2") + " €";
            toolVatBaseBasic.Text = sumBasicVatAmount.ToString("N2") + " €";
            toolTaxBaseReduced.Text = sumTaxBaseReduced.ToString("N2") + " €";
            toolVarBaseReduced.Text = sumReducedVatAmount.ToString("N2") + " €";
            toolTaxFree.Text = sumTaxFreeAmount.ToString("N2") + " €";
            toolPayCash.Text = sumPaycash.ToString("N2") + " €";
            toolPayCard.Text = sumPaycard.ToString("N2") + " €";
        }

        private void LoadGridDataToItemsTable(string receiptid)
        {
            SqlDriver driver = new SqlDriver();
            string[,] items = driver.GetItems(receiptid);
            dataGridItems.Rows.Clear();

            for (int index = 0; index < items.GetLength(0); index++)
            {
                dataGridItems.Rows.Add();
                dataGridItems.Rows[index].Cells[0].Value = items[index, 0];
                dataGridItems.Rows[index].Cells[1].Value = items[index, 1];
                dataGridItems.Rows[index].Cells[2].Value = items[index, 7];
                dataGridItems.Rows[index].Cells[3].Value = items[index, 2];
                dataGridItems.Rows[index].Cells[4].Value = items[index, 3];
                dataGridItems.Rows[index].Cells[5].Value = items[index, 4];
                dataGridItems.Rows[index].Cells[6].Value = items[index, 5];
                dataGridItems.Rows[index].Cells[7].Value = items[index, 6];
            }
        }

        private void Receipts_Load(object sender, EventArgs e)
        {
            if (Program.LOAD_DATA == true)
                this.LoadGridData();
        }

        private void ToolStripButton4_Click(object sender, EventArgs e)
        {
            dataGridView.Visible = true;
            dataGridItems.Visible = false;
            backToReceipts.Visible = false;
            toolStripShowItems.Visible = true;
        }

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            if (dataGridView.Rows.Count > 0 && Program.LOAD_DATA == true)
            {
                int index = dataGridView.SelectedRows[0].Index;

                dataGridView.Visible = false;
                dataGridItems.Visible = true;
                backToReceipts.Visible = true;
                toolStripShowItems.Visible = false;

                string id = dataGridView.Rows[index].Cells[0].Value.ToString();
                this.LoadGridDataToItemsTable(id);
            }
        }

        public static int GetMonthFromDateString(string date)
        {
            string[] array = date.Split('.');
            string month = array[1];
            return Convert.ToInt32(month);
        }

        public static int GetYearFromDateString(string date)
        {
            string[] array = date.Split('.');
            string year = array[2];
            string formatedYear = year.Substring(0, 4);
            return Convert.ToInt32(formatedYear);
        }

        private void DataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView.Visible = false;
            dataGridItems.Visible = true;
            backToReceipts.Visible = true;
            toolStripShowItems.Visible = false;

            int index = e.RowIndex;
            string id = dataGridView.Rows[index].Cells[0].Value.ToString();
            this.LoadGridDataToItemsTable(id);
        }

        private void DataGridView_CellContentClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.ColumnIndex != -1 && e.RowIndex != -1)
            {
                contextMenuStrip.Show(Cursor.Position.X, Cursor.Position.Y);
                this.rowIndex = e.RowIndex;
            }
        }

        private void ZobraziťTovarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView.Visible = false;
            dataGridItems.Visible = true;
            backToReceipts.Visible = true;
            toolStripShowItems.Visible = false;

            string id = dataGridView.Rows[rowIndex].Cells[0].Value.ToString();
            this.LoadGridDataToItemsTable(id);
        }

            async public void CreateDatabase() {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                SqlDriver sqlDriver = new SqlDriver();
                sqlDriver.CreateDatabase();

                stopProcess = false;
                panel.Visible = true;
                textBoxLog.Text = "Nová databáza vytvorená: /n " + Program.DB_PATH;
                string path = folderBrowserDialog.SelectedPath;

                string[] files = Directory.GetFiles(path, "*sent.xml");
                double totalFiles = files.Length;
                double filesDone = 0;

                labelDone.Text = "0";
                labelLeft.Text = "0";
                labelPer.Text = "0%";
                progressBarProcess.Value = 0;

                labelAll.Text = totalFiles.ToString();

                foreach (string file in Directory.EnumerateFiles(path, "*sent.xml"))
                {
                    if (stopProcess)
                        break;

                    if (file.Contains("sent"))
                    {
                        await Task.Delay(10);
                        filesDone++;
                        labelDone.Text = filesDone.ToString();
                        labelLeft.Text = (totalFiles - filesDone).ToString();
                        labelPer.Text = ((filesDone / totalFiles) * 100).ToString("N2") + "%";
                        progressBarProcess.Value = Int32.Parse(Math.Ceiling((filesDone / totalFiles) * 100).ToString());
                        Console.WriteLine(file);
                        textBoxLog.AppendText("\nSpracovanie: " + file);
                        textBoxLog.ScrollToCaret();
                        string xml = File.ReadAllText(file);
                        XmlHandler xmlHandler = new XmlHandler(xml);
                        if (xmlHandler.ReadXML())
                        {
                            textBoxLog.SelectionColor = Color.Green;
                            textBoxLog.AppendText("\nOK");
                            textBoxLog.SelectionColor = Color.Black;
                        }
                        else {
                            textBoxLog.SelectionColor = Color.Red;
                            textBoxLog.AppendText("\nERROR:");
                            textBoxLog.SelectionColor = Color.Black;
                        }

                        Dictionary<string, string> data = new Dictionary<string, string>();
                        data.Add("uid", XmlHandler.OKPValue);
                        data.Add("date", XmlHandler.DateValue);
                        data.Add("amount", XmlHandler.AmountValue);
                        data.Add("receiptnumber", String.Empty);
                        data.Add("intreceiptnumber", XmlHandler.ReceiptNumberValue);
                        data.Add("mod", "ONLINE");
                        data.Add("paycard", XmlHandler.payment[1]);
                        data.Add("paycash", XmlHandler.payment[0]);
                        data.Add("paystr", XmlHandler.payment[2]);
                        data.Add("payvmp", XmlHandler.payment[3]);
                        data.Add("xmldata", xml);
                        data.Add("receipttype", XmlHandler.ReceiptTypeValue);
                        data.Add("invoicenumber", XmlHandler.InvoiceNumberValue);
                        data.Add("taxbasebasic", XmlHandler.TaxBaseBasicValue);
                        data.Add("taxbasereduced", XmlHandler.TaxBaseReducedValue);
                        data.Add("basicvatamount", XmlHandler.BasicVatAmountValue);
                        data.Add("reducedvatamount", XmlHandler.ReducedVatAmountValue);
                        data.Add("taxfreeamount", XmlHandler.TaxFreeAmountValue);

                        SqlDriver driver = new SqlDriver();
                        string lastid = driver.InsertReceipt(data);
                        driver.InsertReceiptItems(XmlHandler.items, lastid);
                    }
                }

                if (totalFiles == filesDone) {
                    textBoxLog.SelectionColor = Color.Green;
                    textBoxLog.AppendText("\nDOKONČENÉ");
                    textBoxLog.SelectionColor = Color.Black;
                    textBoxLog.ScrollToCaret();
                }

                LoadGridData();
                buttonOk.Enabled = true;
            }
        }

        private void toolbuttonLoadFolder_Click(object sender, EventArgs e)
        {
            CreateDatabase();
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            if (!File.Exists(Program.DB_PATH))
            {
                var result = MessageBox.Show("Generovať novú databázu ?", "Databáza", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    Program.LOAD_DATA = false;
                    CreateDatabase();
                    Program.LOAD_DATA = true;
                }
                else
                {
                    Program.LOAD_DATA = true;

                    OpenFileDialog fileDialog = new OpenFileDialog
                    {
                        InitialDirectory = @"C:\",
                        Title = "Vyberte databázu na načítanie",

                        CheckFileExists = true,
                        CheckPathExists = true,

                        DefaultExt = "db",

                        FilterIndex = 2,
                        RestoreDirectory = true,

                        ReadOnlyChecked = true,
                        ShowReadOnly = true
                    };

                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Program.DB_PATH = fileDialog.FileName;
                        LoadGridData();
                    }
                    else
                    {
                        Environment.Exit(1);
                    }
                }
            }
            else {
                LoadGridData();
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            panel.Visible = false;
        }

        async private void button2_Click(object sender, EventArgs e)
        {
            stopProcess = true;
            await Task.Delay(250);
            textBoxLog.SelectionColor = Color.Red;
            textBoxLog.AppendText("\nZRUŠENÉ");
            textBoxLog.SelectionColor = Color.Black;
            textBoxLog.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            stopProcess = true;
            panel.Visible = false;
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Vyberte databázu na načítanie",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "db",

                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                Program.DB_PATH = fileDialog.FileName;
                LoadGridData();
            }
            else
            {
                MessageBox.Show("Musite zvoliť platný a čítateľný súbor !", "Načítanie databázy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
