using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Data.OleDb;

namespace Reconocimiento_facial
{
    public partial class Registrar : Form
    {
        //DECLARANDO TODAS LAS VARIABLES, vectores y  haarcascades
        Image<Bgr, Byte> currentFrame;
        Capture grabber;         
        HaarCascade face;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        Image<Gray, byte> result, TrainedFace = null;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> labels1 = new List<string>();
        List<string> NamePersons = new List<string>();
        int ContTrain, NumLabels, t;
        string name;
        
        public Registrar()
        {
            InitializeComponent();
            //GARGAMOS LA DETECCION DE LAS CARAS POR  haarcascades 
            face = new HaarCascade("haarcascade_frontalface_default.xml");
            try
            {
                //carga de caras previus trainned y etiquetas para cada imagen
                string Labelsinfo = File.ReadAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt");
                string[] Labels = Labelsinfo.Split('%');

                try
                { NumLabels = Convert.ToInt32(Labels[0]); }
                catch { }

                ContTrain = NumLabels;
                string LoadFaces;

                for (int tf = 1; tf < NumLabels + 1; tf++)//recorro el numero de nombres registrados
                {
                    LoadFaces = "face" + tf + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + "/TrainedFaces/" + LoadFaces));//cargo la foto con ese nombre
                    labels.Add(Labels[tf]);//cargo el nombre que se encuentre en la posicion del tf
                }               
            }
            catch (Exception e)
            {//Si la variable NumLabels es 0 me presenta el msj
                MessageBox.Show(e + " No hay ninguna cara en una Base de Datos, por favor añadir por lo menos una cara", "Cragar caras en tu Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btn_detectar_Click(object sender, EventArgs e)
        {
            //Inicia la Captura            
            grabber = new Capture();
            grabber.QueryFrame();

            //Inicia el evento FrameGraber
            Application.Idle += new EventHandler(FrameGrabber);
            this.button1.Enabled = true;
            btn_detectar.Enabled = false;
        }
        void FrameGrabber(object sender, EventArgs e)
        {
            lblNumeroDetect.Text = "0";
            NamePersons.Add("");

            //Obtenga el dispositivo de captura de la forma del marco de corriente
            currentFrame = grabber.QueryFrame().Resize(640, 480, INTER.CV_INTER_NN);

            //Convertir a escala de grises
            gray = currentFrame.Convert<Gray, Byte>();

            //Detector de rostros
            MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(face, 1.2, 10, HAAR_DETECTION_TYPE.FIND_BIGGEST_OBJECT, new Size(90, 90));

            //Accion para cada elemento detectado
            foreach (MCvAvgComp f in facesDetected[0])
            {
                t = t + 1;
                result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(173, 168, INTER.CV_INTER_CUBIC);

                //Dibujar el cuadro para el rostro detectado
                currentFrame.Draw(f.rect, new Bgr(Color.YellowGreen), 1);
                NamePersons[t - 1] = name;
                NamePersons.Add("");

                //Cambiar el numero de rostros detectados
                lblNumeroDetect.Text = facesDetected[0].Length.ToString();
            }
            t = 0;
            
            //Muestra los rostros prcocesados y reconocidos
            imageBoxFrameGrabber.Image = currentFrame;          
            NamePersons.Clear();
        }
        private void btn_agregar_Click(object sender, EventArgs e)
        {            
            try
            {
                //Contador para rostros capturados
                ContTrain = ContTrain + 1;

                //Obtenga un marco de gris de dispositivo de captura
                gray = grabber.QueryGrayFrame().Resize(640, 480, INTER.CV_INTER_CUBIC);

                //Detector de rostros
                MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(face, 8.0, 10, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(90, 90));

                //Acción para cada elemento detectado
                foreach (MCvAvgComp f in facesDetected[0])
                {
                    TrainedFace = currentFrame.Copy(f.rect).Convert<Gray, byte>();
                    break;
                }
                               
                //Redimencionar el rostro detectado para comparar el mismo tamaño con la imagen prueba
                //con el mètodo interpolation
                TrainedFace = result.Resize(173, 168, INTER.CV_INTER_CUBIC);

                //Guardamos el rostro redimencionado en la variable (trainingImages) de tipo lista
                trainingImages.Add(TrainedFace);
                labels.Add(txt_nombre.Text);

                //Mostramos el rostro capturado en escala de grises
                imbImagenGuradado.Image = TrainedFace;

                //Escribimos el numero del rostro agregado en el achivo de texto para poder ser usado luego
                File.WriteAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt", trainingImages.ToArray().Length.ToString() + "%");

                //Escribimos el nombre del rostro capturado en el arcivo de texto
                //y tambien asignamos el nùmero de rostro para luego ser usado
                for (int i = 1; i < trainingImages.ToArray().Length + 1; i++)
                {
                    trainingImages.ToArray()[i - 1].Save(Application.StartupPath + "/TrainedFaces/face" + i + ".bmp");
                    File.AppendAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt", labels.ToArray()[i - 1] + "%");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Primero debe Detectar un Rostro");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult d = MessageBox.Show("¿Está seguro que desea salir del registro?", "¡Está saliendo!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (d == DialogResult.Yes)
            {
                Application.Idle -= new EventHandler(FrameGrabber);
                grabber.Dispose();
                Close();
            }
                
        }

        private void button3_Click(object sender, EventArgs e)
        {
            imbImagenGuradado.Image=null;
            this.txt_codigo.Clear();
            this.txt_nombre.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Idle -= new EventHandler(FrameGrabber);//Detenemos el evento de captura
                grabber.Dispose();//Dejamos de usar la clase para capturar usar los dispositivos
                imageBoxFrameGrabber.ImageLocation = "img/1.png";//reiniciamos la imagen del control
                btn_detectar.Enabled = true;
                button1.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Resgistrar_Load(object sender, EventArgs e)
        {
            imageBoxFrameGrabber.ImageLocation = "img/1.png";
        }
    }
}
