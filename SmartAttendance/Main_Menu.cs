using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetectionAndRecognition
{
    public partial class Main_Menu : Form
    {
        public Main_Menu()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            login lo=new login();   
            lo.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            

            var form2 = new Registration();
            form2.Closed += (s, args) => this.Close();
            form2.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();

            var form2 = new Attendance();
            form2.Closed += (s, args) => this.Close();
            form2.Show();

      /*      Attendance at = new Attendance();
            at.ShowDialog();*/
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new checkAttendance();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
          /*  checkAttendance ch= new checkAttendance();
            ch.ShowDialog();
            this.Hide();*/
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new checkAttendance();
            form2.Closed += (s, args) => this.Close();
            form2.Show();

           /* checkAttendance check1 = new checkAttendance();
            check1.ShowDialog();
            this.Hide();*/
        }
    }
}
