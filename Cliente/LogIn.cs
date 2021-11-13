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
    public partial class LogIn : Form
    {
        Socket server;
        string ip = "192.168.56.102";
        int puerto = 9070;

        public LogIn()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, EventArgs e)
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

                string mensaje = "1/1/" + usuarioTxt.Text + "/" + contraseñaTxt.Text;
                // Enviamos el nombre al servidor
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                this.BackColor = Color.Gray;
                server.Shutdown(SocketShutdown.Both);
                server.Close();

                if (mensaje == "0")
                {
                    Inicio form = new Inicio();
                    form.SetPuerto(ip, puerto);
                    form.ShowDialog();
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
            catch (SocketException)
            {
                // Si hay excepción salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }
        }

        private void registrarseBtn_Click(object sender, EventArgs e)
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
            catch (SocketException)
            {
                // Si hay excepción salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }
        }

        private void LogIn_Load(object sender, EventArgs e)
        {

        }
    }
}
