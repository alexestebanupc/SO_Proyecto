using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace EjGuia
{
    public partial class Inicio : Form
    {
        Socket server;
        string ip;
        int puerto;
        public Inicio()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void SetPuerto(string ip, int puerto)
        {
            this.ip = ip;
            this.puerto = puerto;
        }

        private void conectar_Click(object sender, EventArgs e)
        {
            if (nombreTxt.Text != "")
            {

                // IPEndPoint: server ip & port
                IPAddress direc = IPAddress.Parse(ip);
                IPEndPoint ipep = new IPEndPoint(direc, puerto);

                // Socket
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    server.Connect(ipep);
                    this.BackColor = Color.Green;


                    if (posicionMedia.Checked)
                    {
                        if (nombreTxt.Text != "")
                        {
                            string mensaje = "2/1/" + nombreTxt.Text;
                            // Enviamos el nombre al servidor
                            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                            server.Send(msg);

                            //Recibimos la respuesta del servidor
                            byte[] msg2 = new byte[80];
                            server.Receive(msg2);
                            mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0]; // mensaje viene con basura => nos quedamos con [0]
                            if (mensaje != "0")
                                MessageBox.Show("La posición media de " + nombreTxt.Text + " es: " + mensaje);
                            else
                                MessageBox.Show("Jugador no encontrado en la base de datos");
                        }
                        else
                            MessageBox.Show("Escribe un nombre antes.");
                    }
                    else if (jugMasVeces.Checked)
                    {
                        string mensaje = "2/2/Cualquier";
                        byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);

                        byte[] msg2 = new byte[80];
                        server.Receive(msg2);
                        mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                        string nom = mensaje.Split('/')[0];
                        string vegades = mensaje.Split('/')[1];
                        if (nom != "")
                            MessageBox.Show("El jugador que más veces ha ganado es " + nom + " y ha ganado " + vegades + " veces.");
                        else
                            MessageBox.Show("No hay datos");
                    }
                    else if (numVictorias.Checked)
                    {
                        if (nombreTxt.Text != "")
                        {
                            string mensaje = "2/3/" + nombreTxt.Text;
                            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                            server.Send(msg);

                            byte[] msg2 = new byte[80];
                            server.Receive(msg2);
                            mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                            if (mensaje == "0")
                                MessageBox.Show(nombreTxt.Text + " no ha ganado nunca.");
                            else
                                MessageBox.Show(nombreTxt.Text + " ha ganado " + mensaje + " veces.");
                        }
                        else
                            MessageBox.Show("Escribe un nombre antes.");
                    }
                    // Fin del servicio => nos desconectamos
                    this.BackColor = Color.Gray;
                    server.Shutdown(SocketShutdown.Both);
                    server.Close();
                }
                catch (SocketException)
                {
                    // Si hay excepción salimos del programa con return 
                    MessageBox.Show("No he podido conectar con el servidor");
                    return;
                }
            }
            else
                MessageBox.Show("Escribe un nombre antes.");
        }
    }
}
