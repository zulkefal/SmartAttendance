using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace FaceDetectionAndRecognition
{
    public partial class Registration : Form
    {
        //Declare Variables to use them in all this project
        MCvFont font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 0.6d, 0.6d);
        HaarCascade faceDetected;

        Image<Bgr, Byte> Frame;
        Capture camera;

        Image<Gray, byte> result;

        Image<Gray, byte> TrainedFace = null;
        Image<Gray, byte> grayFace = null;

        // taking images whenever camera opens
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();

        List<string> labels = new List<string>();
        List<string> Users = new List<string>();
        int Count, NumLables, t;
        string name, names = null;

        string str = @"Data Source=ZULKEFAL\SQLEXPRESS;Initial Catalog=SmartAttendance;Integrated Security=True";
       

        public Registration()
        {
            InitializeComponent();

            //HaarCascade is for face detection
            faceDetected = new HaarCascade("haarcascade_frontalface_default.xml");
            textBox3.Text = "Computer Science";
            textBox4.Text = "5";
            textBox3.Enabled = false;
            textBox4.Enabled = false;
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
                Program.g_count = NumLables;
                string FacesLoad;

                for (int i=1;i<NumLables + 1; i++)
                {
                    //Load of previus trainned faces and labels for each image
                    FacesLoad = "face" + i + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + $"/Faces/{FacesLoad}"));
                    labels.Add(Labels[i]);                 
                }

            }
            catch(Exception ex)
            {
               // MessageBox.Show("No One is Registered yet");
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new Main_Menu();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
            
        }

        private void start_Click(object sender, EventArgs e)
        {
            /*camera = new Capture();
            camera.QueryFrame();
            Application.Idle += new EventHandler(FrameProcedure);*/
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
           
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            string idd = textBox1.Text;
            string Name = textName.Text;
            string dept = textBox3.Text;
            string semes = textBox4.Text;

            Program.g_count++;
            Count = Count + 1;
            grayFace = camera.QueryGrayFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

            //Face Detector
            MCvAvgComp[][] DetectedFaces = grayFace.DetectHaarCascade(faceDetected, 1.2, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));


            //Action for each element detected
            foreach (MCvAvgComp f in DetectedFaces[0])
            {
                TrainedFace = Frame.Copy(f.rect).Convert<Gray, byte>();
                break;
            }
            //resize face detected image for force to compare the same size with the 
            //test image with cubic interpolation type method

            TrainedFace = result.Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
            trainingImages.Add(TrainedFace);
            labels.Add(textName.Text);

            //Write the number of trained faces in a file text for further load
            File.WriteAllText(Application.StartupPath + "/Faces/Faces.txt", trainingImages.ToArray().Length.ToString() + ",");

            //Write the labels of trained faces in a file text for further load
            for (int i =1;i<trainingImages.ToArray().Length + 1;i++)
            {
                trainingImages.ToArray()[i - 1].Save(Application.StartupPath + "/Faces/face" + i + ".bmp");
                File.AppendAllText(Application.StartupPath + "/Faces/Faces.txt", labels.ToArray()[i - 1] + ",");
            }

            MessageBox.Show(textName.Text + " saved Successfully");

            //adding to database

            try
            {
                string query = "insert into cs5b (registrationID,name,department,semester) VALUES ('" + idd + "','" + Name + "','" + "Computer Science" + "','" + "5" + "')";

                SqlCommand command = new SqlCommand(query, conn);
                Program.g_count += 1;

                int i1 = command.ExecuteNonQuery();
             /*   if (i1 > 0)
                {
                    MessageBox.Show("Student Added Successfully");

                }
                else
                {
                    MessageBox.Show("Student  Not Added ");
                }*/
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("User Already Registered");
            }
            textBox1.Text = null;
               textName.Text = null;
               textBox3.Text=null;
               textBox4.Text=null;

        }

        private void FrameProcedure(object sender, EventArgs e)
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

                    }

                    Users.Add("");
                }

                cameraBox.Image = Frame;
                names = "";

                Users.Clear();
        }
    }
}
