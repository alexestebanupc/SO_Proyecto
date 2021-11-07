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
        string ip = "192.168.56.102";
        int puerto = 9070;
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

        private void loginBtn_Click(object sender, EventArgs e)
        {
            string mensaje = "1/1/" + usuarioTxt.Text + "/" + contraseñaTxt.Text;
            // Enviamos el nombre al servidor
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            //Recibimos la respuesta del servidor
            byte[] msg2 = new byte[80];
            server.Receive(msg2);
            mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

            if (mensaje == "0")
            {
                MessageBox.Show("Se ha conectado");
            }
            else if (mensaje == "1")
            {
                MessageBox.Show("Constraseña incorrecta.");
            }
            else if (mensaje == "2")
            {
                MessageBox.Show("Usuario no registrado.");
            }
        }

        private void registrarseBtn_Click(object sender, EventArgs e)
        {
            string mensaje = "1/2/" + usuarioTxt.Text + "/" + contraseñaTxt.Text;
            // Enviamos el nombre al servidor
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            //Recibimos la respuesta del servidor
            byte[] msg2 = new byte[80];
            server.Receive(msg2);
            mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

            // Desconexión
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();

            if (mensaje == "0")
            {
                MessageBox.Show("Usuario registrado correctamente.");
            }
            else if (mensaje == "1")
            {
                MessageBox.Show("El nombre de usuario ya existe.");
            }
           
        }

        private void busquedaBtn_Click(object sender, EventArgs e)
        {
            if (nombreTxt.Text != "")
            {
                
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
             
            }
            else
                MessageBox.Show("Escribe un nombre antes.");
        }

        private void conectarBtn_Click(object sender, EventArgs e)
        {
            if (this.BackColor == Color.Gray || this.BackColor == SystemColors.Control)
            {
                //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
                //al que deseamos conectarnos
                IPAddress direc = IPAddress.Parse(ip);
                IPEndPoint ipep = new IPEndPoint(direc, puerto);


                //Creamos el socket 
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    server.Connect(ipep);//Intentamos conectar el socket
                    this.BackColor = Color.Green;
                }
                catch (SocketException)
                {
                    //Si hay excepcion imprimimos error y salimos del programa con return 
                    MessageBox.Show("No he podido conectar con el servidor");
                    return;
                }
            }
            else
                MessageBox.Show("Ya estás conectado.");
        }

        private void desconectarBtn_Click(object sender, EventArgs e)
        {
            if (this.BackColor == Color.Green)
            {
                try
                {
                    string mensaje = "0/0/" + usuarioTxt.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    // Se terminó el servicio. 
                    // Nos desconectamos
                    this.BackColor = Color.Gray;
                    server.Shutdown(SocketShutdown.Both);
                    server.Close();
                }
                catch (SocketException)
                {
                    //Si hay excepcion imprimimos error y salimos del programa con return 
                    MessageBox.Show("No he podido conectar con el servidor");
                    return;
                }
            }
            else
                MessageBox.Show("Ya estás desconectado.");
        }

        private void Inicio_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.BackColor == Color.Green)
            {
                try
                {
                    string mensaje = "0/0/" + usuarioTxt.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    // Se terminó el servicio. 
                    // Nos desconectamos
                    this.BackColor = Color.Gray;
                    server.Shutdown(SocketShutdown.Both);
                    server.Close();
                }
                catch (SocketException)
                {
                    //Si hay excepcion imprimimos error y salimos del programa con return 
                    MessageBox.Show("No he podido conectar con el servidor");
                    return;
                }
            }
            else
                MessageBox.Show("Ya estás desconectado.");
        }

        private void dataGVBtn_Click(object sender, EventArgs e)
        {
            string mensaje = "2/4/Cualquier";
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            byte[] msg2 = new byte[80];
            server.Receive(msg2);
            mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

            string[] elementos = mensaje.Split(',');
            if (Convert.ToInt32(elementos[0]) != 0)
            {
                conectadosGrid.RowCount = Convert.ToInt32(elementos[0]);
                conectadosGrid.RowHeadersVisible = false;
                conectadosGrid.ColumnHeadersVisible = false;
                conectadosGrid.ReadOnly = true;
                conectadosGrid.ScrollBars = ScrollBars.None;

                for (int i = 1; i <= Convert.ToInt32(elementos[0]); i++)
                {
                    conectadosGrid.Rows[i - 1].Cells[0].Value = elementos[i];
                }
            }
            else
            {
                conectadosGrid.Columns.Clear();
                conectadosGrid.Rows.Clear();
            }
            
        }
    }
}
