using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace LedFinger
{
    public partial class Form1 : Form
    {
        bool ledState = false;

        SerialPort serialPort1 = new SerialPort();

        public Form1()
        {
            InitializeComponent();

            serialPort1.PortName = "COM3";
            serialPort1.BaudRate = 9600;
            serialPort1.Open(); 
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*VERIFICAR HUELLA*/
            serialPort1.Write("0");

            while (serialPort1.IsOpen)
            {
                string mensaje1 = serialPort1.ReadLine();
                if (mensaje1 == "Favor de colocar su huella\r")
                {
                    MessageBox.Show(mensaje1);
                }
                if (mensaje1 == "1\r")
                {
                    MessageBox.Show("Huella Verificada");
                    label2.Text = "ID: " + serialPort1.ReadLine();
                    serialPort1.Close();
                }
                if (mensaje1 == "0\r")
                {
                    MessageBox.Show("Huella no encontrada");
                    label2.Text = "No existe la huella";
                    serialPort1.Close();
                }
            }
            serialPort1.Open();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*DIGITALIZAR HUELLA*/
            bool estado = false;
            button1.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;

            label2.Text = "Registrando huella...";
            serialPort1.Write("1");

            while (serialPort1.IsOpen)
            {
                string mensaje = serialPort1.ReadLine();
                if ((mensaje == "Ponga su huella digital\r") ||
                    (mensaje == "Remueva el dedo\r") ||
                    (mensaje == "Volver a poner su huella\r") ||
                    (mensaje == "Huella grabada satiscatoriamente\r") ||
                    (mensaje == "Fallo la digitalizacion\r") ||
                    (mensaje == "Imposible capturar por tercera vez\r") ||
                    (mensaje == "Imposible capturar por segunda vez\r") ||
                    (mensaje == "Imposible capturar\r"))
                {
                    MessageBox.Show(mensaje);
                }

                /**
                 * ESTADOS DE LECTURA
                 * 0 = FALLO
                 * 1 = LECTURA CORRECTA
                 **/
                    
                if (mensaje == "1\r")
                {
                    label2.Text = "Huella Registrada!";
                    serialPort1.Close();
                }
                else if (mensaje == "0\r")
                {
                    label2.Text = "Falló la Digitalizacion!";
                    serialPort1.Close();
                    estado = true;
                }

            }
            serialPort1.Open();
            if (!estado)
            {
                MessageBox.Show(serialPort1.ReadLine());
            }
            button1.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /*NUMERO DE HUELLAS*/
            serialPort1.Write("2");
            label2.Text = "Numero de huellas digitalizadas: " + serialPort1.ReadLine();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            /*BORRAR HUELLAS*/
            serialPort1.Write("3");
            label2.Text = "Huellas borradas";
        }
    }
}
