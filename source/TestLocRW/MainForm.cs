using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestLocRW
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        private void MnuOpenLocFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = ".loc files (*.loc)|*.loc";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.Rows.Clear();
                List<LocRW.TriggerClass> list = LocRW.LocFile.ReadLocFile(dialog.FileName);

                foreach(var item in list)
                {
                    dataGridView1.Rows.Add(item.Value, item.Message);
                }
            }
        }

        private void MnuOpenCsvFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = ".csv files (*.csv)|*.csv";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.Rows.Clear();
                List<LocRW.TriggerClass> list = LocRW.CsvFile.ReadCSV(dialog.FileName);
                foreach (var item in list)
                {
                    dataGridView1.Rows.Add(item.Value, item.Message) ;
                }
            }
        }

        private void MnuSaveLocFile_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = ".loc files (*.loc)|*.loc";
                dialog.DefaultExt = "loc";
                dialog.AddExtension = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    List<LocRW.TriggerClass> list = new List<LocRW.TriggerClass>();
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        LocRW.TriggerClass item = new LocRW.TriggerClass()
                        {
                            Value = row.Cells[0].Value.ToString(),
                            Message = row.Cells[1].Value.ToString()
                        };
                        list.Add(item);
                    }
                    LocRW.LocFile.WriteLocFile(dialog.FileName, list);
                }
            }
        }

        private void MnuSaveCsvFile_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "CSV file (*.csv)|*.csv";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    List<LocRW.TriggerClass> triggers = new List<LocRW.TriggerClass>();
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        LocRW.TriggerClass item = new LocRW.TriggerClass();
                        item.Value = row.Cells[0].Value.ToString().Trim();
                        item.Message = row.Cells[1].Value.ToString().Trim();
                        triggers.Add(item);
                    }

                    LocRW.CsvFile.WriteCSV(dialog.FileName, triggers);
                }
            }
        }

        private void MnuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
