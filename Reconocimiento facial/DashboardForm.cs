using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Reconocimiento_facial
{
    public partial class DashboardForm : Form
    {
        #region Variable de datos estáticos
        List<string> nombresEstudiantes = new List<string>()
        {
            "Maxwell Rafael Tavares",
            "Carlos Yael De Los Santos Zorilla",
            "Pedro", 
            "Keira Rodri",
            "Moises Nuñes",
            "Daniel Alexander Pereyra Beltran",
            "Ronny Díaz",
            "Peter Ferreiras",
            "Jozmar Reyes Alcantara",
            "Gustavo",
            "Carlos E.",
            "Erick Moises",
            "Cedric B. Castillo",
            "Gabriel de jesus Hernandéz",
            "Cesar Angel Polanco Pérez",
            "Javier Luciano"
        };
        List<string> nombresProfesores = new List<string>()
        {
            "Juan Matinez",
            "Willis Polanco",
            "Raydelto Hernandez",
            "Ivis Pérez",
            "Ramón Mena",
            "viela Reyez",
            "Maximiliano",
            "Maxwell Tavares",
        };

        #endregion

        public DashboardForm()
        { InitializeComponent(); }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            guna2ShadowForm1.SetShadowForm(this);

            Size = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            Location = new Point(0, 0);

            #region Datos estáticos
            #region Alumnos
            for (int i = 0; i < nombresEstudiantes.Count; i++)
            {
                Random random = new Random();
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewAlumno);

                //Matrícula al azar
                row.Cells[0].Value = random.Next(2016, 2021).ToString() + "-" + random.Next(1000, 9999);

                //Nombre al azar
                row.Cells[1].Value = nombresEstudiantes[random.Next(0, nombresEstudiantes.Count)];

                ////Asistencia al azar
                //int intrandom = 0;
                //intrandom = random.Next(0, 3);
                //switch (intrandom)
                //{
                //    case 0:
                //        row.Cells[2].Value = "Ausente";
                //        break;
                //    case 1:
                //        row.Cells[2].Value = "Ausente";
                //        break;
                //    case 2:
                //        row.Cells[2].Value = "Presente";
                //        break;
                //    case 3:
                //        row.Cells[2].Value = "Presente";
                //        break;

                //    default:
                //        row.Cells[2].Value = "Presente";
                //        break;
                //}

                //Añadiendo esa columna al DGV
                dataGridViewAlumno.Rows.Add(row);
            }
            #endregion

            #endregion
        }

        private void Exit_Button_Click(object sender, EventArgs e)
        {
            DialogResult d = MessageBox.Show("¿Está seguro que desea salir de la aplicación?", "¡Está saliendo!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (d == DialogResult.Yes)
                Application.Exit();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            Registrar registrar = new Registrar();
            registrar.Show();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage1;
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            Reconocimiento reconocimiento = new Reconocimiento();
            reconocimiento.Show();
        }
    }
}
