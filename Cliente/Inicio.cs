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
using System.Threading;

namespace EjGuia
{
    public partial class Inicio : Form
    {
        Socket server;
        Thread atender;
        //string ip = "147.83.117.22";
        //int puerto = 50054;
        string ip = "192.168.56.102";
        int puerto = 9080;
        delegate void DelegadoParaActualizar(string mensaje);
        int tiempoEspera;
        int indicePartida;

        public Inicio()
        {
            InitializeComponent();
            //CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tiempoEspera = 0;
        }

        /// <summary>
        /// Recibe una cadena con el número de jugadores conectados(/) y los nombres separados por comas
        /// </summary>
        /// <param name="mensaje">Cadena</param>
        public void ActualizarDGV(string mensaje)
        {
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

        /// <summary>
        /// Función que atiende un thread en la que se atienden las respuestas del servidor 
        /// </summary>
        private void AtenderServidor()
        {
            while (true)
            {
                //Recibimos del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int form = Convert.ToInt32(trozos[0]);
                int codigo = Convert.ToInt32(trozos[1]);
                string mensaje = trozos[2].Split('\0')[0];
                string auxiliar;

                switch (form)
                {
                    case 1:
                        switch (codigo)
                        {
                            case 1:// Iniciar sesión
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
                                break;
                            case 2: // Registrarse
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
                                break;
                        }
                        break;
                    case 2:
                        switch (codigo)
                        {
                            case 1:// Posición media de x
                                if (mensaje != "0")
                                    MessageBox.Show("La posición media de " + nombreTxt.Text + " es: " + mensaje);
                                else
                                    MessageBox.Show("Jugador no encontrado en la base de datos");
                                break;
                            case 2:// Jugador que más veces ha ganado
                                string nom = mensaje.Split(',')[0];
                                string vegades = mensaje.Split(',')[1];
                                if (nom != "")
                                    MessageBox.Show("El jugador que más veces ha ganado es " + nom + " y ha ganado " + vegades + " veces.");
                                else
                                    MessageBox.Show("No hay datos");
                                break;
                            case 3:// Veces que ha ganado x
                                if (mensaje == "0")
                                    MessageBox.Show(nombreTxt.Text + " no ha ganado nunca.");
                                else
                                    MessageBox.Show(nombreTxt.Text + " ha ganado " + mensaje + " veces.");
                                break;
                            case 4://Actualizar DGV
                                DelegadoParaActualizar delegado = new DelegadoParaActualizar(ActualizarDGV);
                                conectadosGrid.Invoke(delegado, new object[] {mensaje});
                                break;
                            case 5://Recibe la invitación
                                auxiliar = mensaje.Split(',')[0];
                                if (auxiliar == "0")
                                {
                                    string emisor = mensaje.Split(',')[1];
                                    string indice = mensaje.Split(',')[2];
                                    DialogResult dialogResult = MessageBox.Show("Te ha llegado una invitación de " + emisor, "Invitación", MessageBoxButtons.YesNo);
                                    if (dialogResult == DialogResult.Yes)
                                    {
                                        indicePartida = Convert.ToInt32(indice);
                                        string respuesta = "2/6/si/" + indice;
                                        // Enviamos el nombre al servidor
                                        byte[] msg = Encoding.ASCII.GetBytes(respuesta);
                                        server.Send(msg);
                                    }
                                    else if (dialogResult == DialogResult.No)
                                    {
                                        string respuesta = "2/6/no/" + indice;
                                        // Enviamos el nombre al servidor
                                        byte[] msg = Encoding.ASCII.GetBytes(respuesta);
                                        server.Send(msg);
                                    }
                                }
                                else if (auxiliar == "1")
                                {
                                    MessageBox.Show("No sitio disponible para la partida");
                                }
                                else if (auxiliar == "3")
                                {
                                    indicePartida = Convert.ToInt32(mensaje.Split(',')[1]);
                                }
                                else
                                {
                                    MessageBox.Show("Partida llena de jugadores");
                                }
                                break;
                            case 6:// Respuesta de los invitados
                                auxiliar = mensaje.Split(',')[0];
                                if(auxiliar == "0")
                                {
                                    string resultado = mensaje.Split(',')[1];
                                    string indice = mensaje.Split(',')[2];
                                    string emisor = mensaje.Split(',')[3];
                                    if (resultado == "si")
                                    {
                                        MessageBox.Show(emisor + " ha aceptado la partida");
                                    }
                                    else if (resultado == "no")
                                    {
                                        MessageBox.Show(emisor + " ha rechado la partida");
                                    }
                                }
                                else if (auxiliar == "1")
                                {
                                    MessageBox.Show("No sitio disponible para la partida");
                                }
                                else if(auxiliar == "2")
                                {
                                    MessageBox.Show("Partida llena de jugadores");
                                }
                                else
                                {
                                    MessageBox.Show("No has podido entrar en la partida porque ya ha empezado");
                                }
                                break;
                            case 7:
                                auxiliar = mensaje.Split(',')[0];
                                if (auxiliar == "0")
                                {
                                    MessageBox.Show("La partida va a empezar");
                                }
                                else
                                {
                                    MessageBox.Show("Nadie ha aceptado la solicitud");
                                }
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// En caso de que hagamos más de un form. Para pasarnos parametros de interés
        /// </summary>
        /// <param name="ip">Ip del servidor</param>
        /// <param name="puerto">Puerto desde donde nos conectamos</param>
        public void SetPuerto(string ip, int puerto)
        {
            this.ip = ip;
            this.puerto = puerto;
        }

        /// <summary>
        /// Petición de iniciar sesión
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginBtn_Click(object sender, EventArgs e)
        {
            string mensaje = "1/1/" + usuarioTxt.Text + "/" + contraseñaTxt.Text;
            // Enviamos el nombre al servidor
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        /// <summary>
        /// Petición de registrarse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void registrarseBtn_Click(object sender, EventArgs e)
        {
            string mensaje = "1/2/" + usuarioTxt.Text + "/" + contraseñaTxt.Text;
            // Enviamos el nombre al servidor
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        /// <summary>
        /// Petición de busqueda de una consulta concreta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void busquedaBtn_Click(object sender, EventArgs e)
        {
            if (nombreTxt.Text != "")
            {
                
                if (posicionMedia.Checked) //Posición media del jugador x
                {
                    if (nombreTxt.Text != "")
                    {
                        string mensaje = "2/1/" + nombreTxt.Text;
                        // Enviamos el nombre al servidor
                        byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                    }
                    else
                        MessageBox.Show("Escribe un nombre antes.");
                }
                else if (jugMasVeces.Checked) //Jugador con más victorias
                {
                    string mensaje = "2/2/Cualquier";
                    byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
                else if (numVictorias.Checked) //Número de victorias de x
                {
                    if (nombreTxt.Text != "")
                    {
                        string mensaje = "2/3/" + nombreTxt.Text;
                        byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                    }
                    else
                        MessageBox.Show("Escribe un nombre antes.");
                }
             
            }
            else
                MessageBox.Show("Escribe un nombre antes.");
        }

        /// <summary>
        /// Botón para conectarse al servidor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                ThreadStart ts = delegate { AtenderServidor(); };
                atender = new Thread(ts);
                atender.Start();
            }
            else
                MessageBox.Show("Ya estás conectado.");
        }

        /// <summary>
        /// Botón para desconectarse del servidor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    atender.Abort();
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

        /// <summary>
        /// Desconectarse del servidor al pulsar la X del form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    atender.Abort();
                    this.BackColor = Color.Gray;
                    server.Shutdown(SocketShutdown.Both);
                    server.Close();
                }
                catch (SocketException)
                {
                    //Si hay excepcion imprimimos error y salimos del programa con return 
                    MessageBox.Show("No he podido desconectarme del servidor");
                    return;
                }
            }
        }

        /// <summary>
        /// Seleccionamos en la DataGrid View al usuario que queremos invitar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void conectadosGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string nombre_invitado = conectadosGrid.Rows[e.RowIndex].Cells[0].Value.ToString();

            string mensaje = "2/5/" + nombre_invitado;
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            if(tiempoEspera == 0)
            {
                timer1.Start();
                timer1.Interval = 1000;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tiempoEspera++;
            segundosLbl.Text = Convert.ToString(tiempoEspera);
            if (tiempoEspera == 10)
            {
                string mensaje = "2/7/"+ indicePartida;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                timer1.Stop();
                tiempoEspera = 0;//Poner el tiempo a 0 por si hay que volver a invitar a alguien
            }
        }
    }
}
