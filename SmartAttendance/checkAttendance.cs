using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FaceDetectionAndRecognition
{
    public partial class checkAttendance : Form
    {
        string str = @"Data Source=ZULKEFAL\SQLEXPRESS;Initial Catalog=SmartAttendance;Integrated Security=True";
        
        public checkAttendance()
        {
            InitializeComponent();
            panel4.Hide();
            panel5.Hide();
            button7.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel4.Visible= true;
            panel5.Visible= false;
            SqlConnection con = new SqlConnection(str);
            SqlCommand cmd= new SqlCommand("select * from absent ",con);
            con.Open();

            SqlDataAdapter sda= new SqlDataAdapter(cmd);
            DataTable dt = new DataTable(); 
            sda.Fill(dt);
            con.Close();

            dataGridView1.DataSource = dt;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel4.Visible= false;
            panel5.Visible = true;
            SqlConnection con = new SqlConnection(str);
            SqlCommand cmd = new SqlCommand("select * from cs5b", con);
            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            con.Close();

            dataGridView1.DataSource = dt;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(str);
            SqlCommand cmd = new SqlCommand("select * from attendance2", con);
            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            con.Close();

            dataGridView1.DataSource = dt;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();


            var form2 = new Main_Menu();
            form2.Closed += (s, args) => this.Close();
            form2.Show();

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

     

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var index= e.RowIndex;
            textBox1.Text = dataGridView1.Rows[index].Cells[0].Value.ToString();

            textBox2.Text = dataGridView1.Rows[index].Cells[0].Value.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int idd = int.Parse(textBox1.Text);

            SqlConnection conn = new SqlConnection(str);
            string query = "Delete from absent where registrationID=" + idd;
            conn.Open();
            SqlCommand command = new SqlCommand(query, conn);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            conn.Close();

            dataGridView1.DataSource = dt;

            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            int idd2 = int.Parse(textBox2.Text);
            SqlConnection conn = new SqlConnection(str);

            string query = "Delete from cs5b where registrationID=" + idd2;
            conn.Open();
            SqlCommand command = new SqlCommand(query, conn);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            int i = command.ExecuteNonQuery();
            conn.Close();
            dataGridView1.DataSource = dt;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)

            {

                SaveFileDialog save = new SaveFileDialog();

                save.Filter = "PDF (*.pdf)|*.pdf";

                save.FileName = "Result.pdf";

                bool ErrorMessage = false;

                if (save.ShowDialog() == DialogResult.OK)

                {

                    if (File.Exists(save.FileName))

                    {

                        try

                        {

                            File.Delete(save.FileName);

                        }

                        catch (Exception ex)

                        {

                            ErrorMessage = true;

                            MessageBox.Show("Unable to wride data in disk" + ex.Message);

                        }

                    }

                    if (!ErrorMessage)

                    {

                        try

                        {

                            PdfPTable pTable = new PdfPTable(dataGridView1.Columns.Count);

                            pTable.DefaultCell.Padding = 2;

                            pTable.WidthPercentage = 100;

                            pTable.HorizontalAlignment = Element.ALIGN_LEFT;

                            foreach (DataGridViewColumn col in dataGridView1.Columns)

                            {

                                PdfPCell pCell = new PdfPCell(new Phrase(col.HeaderText));

                                pTable.AddCell(pCell);

                            }

                            foreach (DataGridViewRow viewRow in dataGridView1.Rows)

                            {

                                foreach (DataGridViewCell dcell in viewRow.Cells)

                                {

                                    pTable.AddCell(dcell.Value.ToString());

                                }

                            }

                            using (FileStream fileStream = new FileStream(save.FileName, FileMode.Create))

                            {

                                Document document = new Document(PageSize.A4, 8f, 16f, 16f, 8f);

                                PdfWriter.GetInstance(document, fileStream);

                                document.Open();

                                document.Add(pTable);

                                document.Close();

                                fileStream.Close();

                            }

                            MessageBox.Show("Data Export Successfully", "info");

                        }

                        catch (Exception ex)

                        {

                            MessageBox.Show("Error while exporting Data" + ex.Message);

                        }

                    }

                }

            }

            else

            {

                MessageBox.Show("No Record Found", "Info");

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
           // string aa = "Absent";
            SqlConnection conn = new SqlConnection(str);
            // string query = "SELECT * from absent WHERE Value=" + aa;
            string query = "SELECT * from absent2";
            conn.Open();

            try
            {
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataAdapter sda = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                int i = command.ExecuteNonQuery();
                conn.Close();
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("None is absent");
            }
        }
    }
}
