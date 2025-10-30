using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Emgu.CV.CvEnum;

namespace FaceDetectionAndRecognition
{
    public partial class Attendance : Form
    {
        MCvFont font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 0.6d, 0.6d);
        HaarCascade faceDetected;
        Image<Bgr, Byte> Frame;
        Capture camera;
        Image<Gray, byte> result;
        Image<Gray, byte> TrainedFace = null;
        Image<Gray, byte> grayFace = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> Users = new List<string>();
        int Count, NumLables, t;
        string name, names = null;
        int g_count = 0;

        //store the present studnts
      

        string str = @"Data Source=ZULKEFAL\SQLEXPRESS;Initial Catalog=SmartAttendance;Integrated Security=True";

        //passing name to functoin for attendance
        String namess;

        bool flag=true;


        HashSet<string> stdnts = new HashSet<string>();




        public Attendance()
        {
            InitializeComponent();
        }



        private void start_Click(object sender, EventArgs e)
        {
            //HaarCascade is for face detection
            faceDetected = new HaarCascade("haarcascade_frontalface_default.xml");
            camera = new Capture();
            camera.QueryFrame();
            Application.Idle += new EventHandler(FrameProcedure);

            try
            {
                string Labelsinf = File.ReadAllText(Application.StartupPath + "/Faces/Faces.txt");
                string[] Labels = Labelsinf.Split(',');

                //The first label before , will be the number of faces saved.
                NumLables = Convert.ToInt16(Labels[0]);
                Count = NumLables;

                string FacesLoad;

                for (int i = 1; i < NumLables + 1; i++)
                {
                    //Load of previus trainned faces and labels for each image
                    FacesLoad = "face" + i + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + $"/Faces/{FacesLoad}"));
                    labels.Add(Labels[i]);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("None is registered");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new Main_Menu();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void FrameProcedure(object sender, EventArgs e)
        {
            try
            {
                Users.Add("");
                Frame = camera.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                grayFace = Frame.Convert<Gray, Byte>();
                MCvAvgComp[][] facesDetectedNow = grayFace.DetectHaarCascade(faceDetected, 1.2, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));
                foreach (MCvAvgComp f in facesDetectedNow[0])
                {
                    result = Frame.Copy(f.rect).Convert<Gray, Byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                    Frame.Draw(f.rect, new Bgr(Color.Green), 2);
                    if (trainingImages.ToArray().Length != 0)
                    {
                        MCvTermCriteria termCriterias = new MCvTermCriteria(Count, 0.001);

                        EigenObjectRecognizer recognizer = new EigenObjectRecognizer(trainingImages.ToArray(), labels.ToArray(), 1000, ref termCriterias);
                        name = recognizer.Recognize(result);

                        Frame.Draw(name, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.Red));

                        //  namess = name;

                        //pass the detected person to mark the attendance
                        TimeSpan TodayTime = DateTime.Now.TimeOfDay;
                        namess = name;

                        if(name=="")
                        {

                        }
                        else
                        {
                            stdnts.Add(namess);                           
                        }
                       

                        // atten(namess);


                    }

                    Users.Add("");
                }
                cameraBox.Image = Frame;
                names = "";
                Users.Clear();
            }

            catch (Exception ex)
            {
                //MessageBox.Show("User Not Found");
            }
        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
           // button1.BackColor=Color.AliceBlue;
        }

        private void notatten()
        {
            MessageBox.Show(Program.g_count.ToString());
            for (int axz=0; axz<Program.g_count; axz++)
            {
                
                SqlConnection connn = new SqlConnection(str);

                SqlCommand cmd1 = new SqlCommand("SELECT registrationID,name from cs5b");
                var dateTime = DateTime.Now;
                var date = dateTime.ToShortDateString();

                cmd1.CommandType = CommandType.Text;
                cmd1.Connection = connn;
                connn.Open();
                using (SqlDataReader sdr = cmd1.ExecuteReader())
                {

                    sdr.Read();

                    long i_d = sdr.GetInt64(0);
                    string n = sdr["name"].ToString();
                    //MessageBox.Show(n);

                    connn.Close();

                    SqlConnection connn1 = new SqlConnection(str);

                    string query1 = "insert into absent (registrationID,Name,Department,Semester,Value,date) VALUES ('" + i_d + "','" + n + "','" + "Computer Science" + "','" + "5" + "','" + "Absent " + "','" + date + "')";

                    connn1.Open();

                    SqlCommand command = new SqlCommand(query1, connn1);
                    flag = false;
                    int i1 = command.ExecuteNonQuery();
                    if (i1 > 0)
                    {
                        MessageBox.Show("Attendance done Successfully");

                    }
                    else
                    {
                        MessageBox.Show("Attendance Not done Successfully");
                    }
                    //MessageBox.Show("Attendance Done Successfully");

                    connn1.Close();
                }
                flag= false;
            }
        }
        private void atten(string a)
        {
            SqlConnection conn = new SqlConnection(str);
            SqlCommand cmd = new SqlCommand("SELECT (registrationID) FROM cs5b WHERE name = '" + a + "'");

            {
                var dateTime = DateTime.Now;
                var date1 = dateTime.ToShortDateString();
                TimeSpan TodayTime = DateTime.Now.TimeOfDay;
                string nm = "Present";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                conn.Open();
                   using (SqlDataReader sdr = cmd.ExecuteReader())
                    {

                        sdr.Read();


                    try
                    {
                        long i_d = sdr.GetInt64(0);
                        //string n = sdr["name"].ToString();

                        //MessageBox.Show(n);

                        conn.Close();

                        SqlConnection conn1 = new SqlConnection(str);

                        string query = "insert into absent (registrationID,Name,Department,Semester,Value,date) VALUES ('" + i_d + "','" + a + "','" + "Computer Science" + "','" + "5" + "','" + "Present" + "','" + date1 + "')";

                        conn1.Open();

                        SqlCommand command = new SqlCommand(query, conn1);
                        flag = false;
                        int i1 = command.ExecuteNonQuery();
                        conn1.Close();
                    }

                    catch (Exception ex)
                    {

                    }   
               }

            }

        }
      /*  private void allabsent()
        {
            SqlConnection conn = new SqlConnection(str);
            string qu= ("insert into absent2 select * from cs5b ");
            conn.Open();
            SqlCommand command = new SqlCommand(qu, conn);
            flag = false;
            int i1 = command.ExecuteNonQuery();
            conn.Close();
            flag= false;
            
        }*/

      /*  private void makePresent(string mak)
        {
            SqlConnection conn = new SqlConnection(str);
            string qu = ("Delete from absent2  Where Name=" + mak);
            conn.Open();
            SqlCommand command = new SqlCommand(qu, conn);
            flag = false;
            int i1 = command.ExecuteNonQuery();
            conn.Close();
            flag = false;

        }*/

        private void button1_Click(object sender, EventArgs e)
        {
            int count = stdnts.Count;

            try
            {
                if (count == 0)
                {
                    //  MessageBox.Show("you are not registered.");
                }
                else
                {
                    foreach (var s in stdnts)
                    {
                        atten(s);
                       /* if (flag==true)
                        {

                            //notatten();
                            allabsent();
                            
                            makePresent(s);

                        }
                       else
                        {
                            atten(s);
                            makePresent(s);
                        }*/

                    }

                    /* int rem = g_count - count;

                     for(int abs=0; abs<rem; abs++)

                     {
                         MessageBox.Show(marked[abs].ToString());
                         SqlConnection connn = new SqlConnection(str);
                         bool flag1 = true;

                         if(flag1==true)
                         {
                             for (int abz = 0; abz < marked.Count(); abz++)
                             {
                                 SqlCommand cmd1 = new SqlCommand("SELECT registrationID,name FROM cs5b WHERE registrationID != '" + marked[abs] + "'");
                                 var dateTime = DateTime.Now;
                                 var date = dateTime.ToShortDateString();

                                 cmd1.CommandType = CommandType.Text;
                                 cmd1.Connection = connn;
                                 connn.Open();
                                 using (SqlDataReader sdr = cmd1.ExecuteReader())
                                 {

                                     sdr.Read();
                                     long i_d = sdr.GetInt64(0);
                                     string n = sdr["name"].ToString();

                                     //MessageBox.Show(n);

                                     connn.Close();

                                     SqlConnection connn1 = new SqlConnection(str);

                                     string query1 = "insert into absent (registrationID,Name,Department,Semester,Value,date) VALUES ('" + i_d + "','" + n + "','" + "Computer Science" + "','" + "5" + "','" + "Absent " + "','" + date + "')";

                                     connn1.Open();

                                     SqlCommand command = new SqlCommand(query1, connn1);
                                     connn1.Close();

                                 }
                             }
                             flag1 = false;
                         }
                         else
                         {

                         }

                     }*/


                }

            }
            catch(Exception ex) { }
 
        }
    }
}
