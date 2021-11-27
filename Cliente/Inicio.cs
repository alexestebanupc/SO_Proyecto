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
        //Variables del cliente
        Socket server;
        Thread atender;
        string ip = "147.83.117.22";
        int puerto = 50054;
        //string ip = "192.168.56.102";
        //int puerto = 9060;
        delegate void DelegadoParaActualizar(string mensaje);
        delegate void DelegadoParaLobby();
        delegate void DelegadoParaChat(string nombre, string mensaje);
        delegate void DelegadoParaGraficos(int numeroJugadores);
        delegate void DelegadoParaPosicion(int jugador, int posX, int posY);
        int tiempoEspera;
        int indicePartida;
        string miUsuario;
        int cocheId;

        //Variables de los gráficos

        int carr;
        int up, right, left, down;
        int distancia;

        public Inicio()
        {
            InitializeComponent();
            //CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Load del cliente

            tiempoEspera = 0;
            segundosLbl.Text = "";

            //Empieza con Iniciar sesión y escondemos el resto:
            conectadosGrid.Hide();
            desconectarBtn.Hide();
            nombreTxt.Hide();
            posicionMedia.Hide();
            jugMasVeces.Hide();
            numVictorias.Hide();
            busquedaBtn.Hide();
            meta.Hide();
            carretera.Hide();
            coche1.Hide();
            coche2.Hide();
            coche3.Hide();
            coche4.Hide();
            flechas.Hide();
            this.Size = new Size(300, 250);
            conectadosGrid.Location = new Point(50, 50);
            usuariosConectadosLbl.Location = new Point(conectadosGrid.Location.X,conectadosGrid.Location.Y-15);
            usuariosConectadosLbl.Hide();
            chatMensajeLbl.Hide();
            chatNombreLbl.Location = new Point(900-380,600-100);
            chatNombreLbl.Hide();
            chatBox.Hide();
            chatBox.Location = new Point(chatNombreLbl.Location.X,chatNombreLbl.Location.Y+chatNombreLbl.Height+5);
            chatBox.Width = 300;
            enviarBtn.Location = new Point(chatBox.Location.X+chatBox.Width,chatBox.Location.Y);
            enviarBtn.Size = new Size(enviarBtn.Width, chatBox.Height);
            timer_posicion.Interval = 1000;
        }

        //Funciones del cliente

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
                    conectadosGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    conectadosGrid.Size = new Size(conectadosGrid.Rows[i - 1].Cells[0].Size.Width, conectadosGrid.Rows[i - 1].Cells[0].Size.Height*i);
                }
            }
            else
            {
                conectadosGrid.Columns.Clear();
                conectadosGrid.Rows.Clear();
            }
        }

        public void IniciarLobby()
        {
            conectadosGrid.Show();
            desconectarBtn.Show();
            conectarBtn.Hide();
            this.Size = new Size(900, 600);
            loginBtn.Hide();
            registrarseBtn.Hide();
            usuarioLbl.Hide();
            usuarioTxt.Hide();
            contraseñaLbl.Hide();
            contraseñaTxt.Hide();
            usuariosConectadosLbl.Show();
            chatBox.Show();
        }

        public void MostrarChat(string nombre, string mensaje)
        {
            chatMensajeLbl.Show();
            chatNombreLbl.Show();
            chatNombreLbl.Text = nombre;
            chatMensajeLbl.Text = mensaje;
            chatMensajeLbl.Location = new Point(chatNombreLbl.Location.X+chatNombreLbl.Width,chatNombreLbl.Location.Y);
        }

        public void NuevaPosicion(int jugador,int PosX, int PosY)
        {
            Controls["coche" + Convert.ToString(jugador)].Location = new Point(PosX, -PosY+distancia + ((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Location.Y);
            if ((Controls["coche" + Convert.ToString(jugador)].Location.Y >= carretera.Height + carretera.Location.Y - (Controls["coche" + Convert.ToString(jugador)].Height)) || (Controls["coche" + Convert.ToString(jugador)].Location.Y <= carretera.Location.Y) )
            {
                Controls["coche" + Convert.ToString(jugador)].Hide();
            }
            else 
            {
                Controls["coche" + Convert.ToString(jugador)].Show();
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
                                    //Si iniciamos sesión bien, agrandamos el form mostrando el lobby
                                    DelegadoParaLobby delegado2 = new DelegadoParaLobby(IniciarLobby);
                                    this.Invoke(delegado2, new object[] {  });
                                    miUsuario = usuarioTxt.Text;
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
                                    int numeroJugadores = Convert.ToInt32(mensaje.Split(',')[1]);
                                    cocheId = Convert.ToInt32(mensaje.Split(',')[2]);
                                    MessageBox.Show("La partida va a empezar");
                                    DelegadoParaGraficos delegado4 = new DelegadoParaGraficos(LoadGraficos);
                                    this.Invoke(delegado4, new object[] { numeroJugadores });
                                }
                                else
                                {
                                    MessageBox.Show("Nadie ha aceptado la solicitud");
                                }
                                break;
                            case 8:
                                string nombre = mensaje.Split(',')[0];
                                string texto = mensaje.Split(',')[1];
                                DelegadoParaChat delegado3 = new DelegadoParaChat(MostrarChat);
                                this.Invoke(delegado3, new object[] { nombre, texto });
                                break;
                            case 9:
                                int jugador = Convert.ToInt32(mensaje.Split(',')[0]);
                                int posX = Convert.ToInt32(mensaje.Split(',')[1]);
                                int posY = Convert.ToInt32(mensaje.Split(',')[2]);
                                DelegadoParaPosicion delegado5 = new DelegadoParaPosicion(NuevaPosicion);
                                this.Invoke(delegado5, new object[] { jugador, posX, posY });
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
                    desconectarBtn.Hide();
                    conectarBtn.Show();
                    loginBtn.Show();
                    registrarseBtn.Show();
                    conectadosGrid.Hide();
                    usuarioTxt.Text = "";
                    usuarioTxt.Show();
                    usuarioLbl.Show();
                    contraseñaTxt.Text = "";
                    contraseñaTxt.Show();
                    contraseñaLbl.Show();
                    nombreTxt.Hide();
                    posicionMedia.Hide();
                    jugMasVeces.Hide();
                    numVictorias.Hide();
                    busquedaBtn.Hide();
                    usuariosConectadosLbl.Hide();
                    chatMensajeLbl.Hide();
                    chatNombreLbl.Hide();
                    chatBox.Hide();
                    timer1.Stop();
                    timer_left.Stop();
                    timer_posicion.Stop();
                    timer_right.Stop();
                    this.Size = new Size(300, 250);
                    conectadosGrid.Location = new Point(50, 50);

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
                    timer1.Stop();
                    timer_left.Stop();
                    timer_posicion.Stop();
                    timer_right.Stop();
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
            if (nombre_invitado != miUsuario)
            {
                string mensaje = "2/5/" + nombre_invitado;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                if (tiempoEspera == 0)
                {
                    timer1.Start();
                    timer1.Interval = 1000;
                }
            }
            else
            {
                MessageBox.Show("No puedes invitarte a ti mismo.");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tiempoEspera++;
            segundosLbl.Text = "La partida empezará en...\n" + Convert.ToString(10-tiempoEspera);
            if (tiempoEspera == 10)
            {
                string mensaje = "2/7/"+ indicePartida;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                timer1.Stop();

                //Cambiar el form y poner los gráficos aquí
                conectadosGrid.Hide();
                desconectarBtn.Hide();


                tiempoEspera = 0;//Poner el tiempo a 0 por si hay que volver a invitar a alguien
            }
        }

        private void enviarBtn_Click(object sender, EventArgs e)
        {
            if (chatBox.Text != "") {
                byte[] msg = Encoding.ASCII.GetBytes("2/8/" + chatBox.Text);
                server.Send(msg);
                chatBox.Text = "";
            }
        }

        //Funciones de los gráficos

        /// <summary>
        /// 
        /// </summary>
        public void LoadGraficos(int numeroJugadores)
        {
            carretera.Image = new Bitmap("carretera.jpg");
            carretera.SizeMode = PictureBoxSizeMode.StretchImage;
            carretera.Size = new Size(400, 800);
            carretera.Location = new Point(100,100);
            this.Size = new Size(800,1000);
            for(int i = 1; i <= numeroJugadores; i++)
            {

                ((PictureBox)Controls["coche" + Convert.ToString(i)]).Image = new Bitmap("coche_publi.png");
                if (cocheId == i)
                {
                    ((PictureBox)Controls["coche" + Convert.ToString(i)]).BackColor = Color.Blue;
                }
                else
                {
                    ((PictureBox)Controls["coche" + Convert.ToString(i)]).BackColor = Color.Red;
                }
                ((PictureBox)Controls["coche" + Convert.ToString(i)]).SizeMode = PictureBoxSizeMode.StretchImage;
                ((PictureBox)Controls["coche" + Convert.ToString(i)]).Size = new Size(50, 50);
                ((PictureBox)Controls["coche" + Convert.ToString(i)]).Location = new Point(carretera.Location.X + carretera.Size.Width / (numeroJugadores+1)*i - ((PictureBox)Controls["coche" + Convert.ToString(i)]).Size.Width / 2, carretera.Location.Y + carretera.Height / 2);
                //Cercle
                Rectangle r = new Rectangle(0, 0, ((PictureBox)Controls["coche" + Convert.ToString(i)]).Width, ((PictureBox)Controls["coche" + Convert.ToString(i)]).Height);
                System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
                int d = 50;
                gp.AddArc(r.X, r.Y, d, d, 180, 90);
                gp.AddArc(r.X + r.Width - d, r.Y, d, d, 270, 90);
                gp.AddArc(r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90);
                gp.AddArc(r.X, r.Y + r.Height - d, d, d, 90, 90);
                ((PictureBox)Controls["coche" + Convert.ToString(i)]).Region = new Region(gp);
                //Fi cercle
            }

            flechas.Image = new Bitmap("flechas_none.png");
            flechas.SizeMode = PictureBoxSizeMode.StretchImage;
            flechas.Size = new Size(80, 80);
            meta.Visible = false;
            meta.BackColor = Color.Gold;
            meta.Location = carretera.Location;
            meta.Size = new Size(carretera.Size.Width, 20);

            carr = 0;
            distancia = 0;
            up = 0;
            down = 0;
            right = 0;
            left = 0;
            timer_up.Interval = 150;
            timer_posicion.Start();

            //Escondemos el resto de elementos

            conectadosGrid.Hide();
            desconectarBtn.Hide();
            nombreTxt.Hide();
            posicionMedia.Hide();
            jugMasVeces.Hide();
            numVictorias.Hide();
            busquedaBtn.Hide();
            usuariosConectadosLbl.Hide();
            chatMensajeLbl.Hide();
            chatNombreLbl.Hide();
            chatBox.Hide();
            segundosLbl.Hide();
            enviarBtn.Hide();

            //Mostramos las picture box

            meta.Show();
            carretera.Show();
            coche1.Show();
            coche2.Show();
            coche3.Show();
            coche4.Show();
            flechas.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckFlechas()
        {
            if (up == 0 && right == 0 && left == 0 && down == 0)
                flechas.Image = new Bitmap("flechas_none.png");
            else if (up == 1 && right == 0 && left == 0 && down == 0)
                flechas.Image = new Bitmap("flechas_up.png");
            else if (up == 0 && right == 1 && left == 0 && down == 0)
                flechas.Image = new Bitmap("flechas_right.png");
            else if (up == 0 && right == 0 && left == 1 && down == 0)
                flechas.Image = new Bitmap("flechas_left.png");
            else if (up == 1 && right == 1 && left == 0 && down == 0)
                flechas.Image = new Bitmap("flechas_up_right.png");
            else if (up == 1 && right == 0 && left == 1 && down == 0)
                flechas.Image = new Bitmap("flechas_up_left.png");
            else if (up == 0 && right == 1 && left == 1 && down == 0)
                flechas.Image = new Bitmap("flechas_right_left.png");
            else if (up == 1 && right == 1 && left == 1 && down == 0)
                flechas.Image = new Bitmap("flechas_up_right_left.png");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                up = 1;
                CheckFlechas();
                timer_up.Start();
            }
            if (e.KeyCode == Keys.Right)
            {
                right = 1;
                CheckFlechas();
                timer_right.Start();
            }
            if (e.KeyCode == Keys.Left)
            {
                left = 1;
                CheckFlechas();
                timer_left.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                up = 0;
                CheckFlechas();
                timer_up.Stop();
            }
            if (e.KeyCode == Keys.Right)
            {
                right = 0;
                CheckFlechas();
                timer_right.Stop();
            }
            if (e.KeyCode == Keys.Left)
            {
                left = 0;
                CheckFlechas();
                timer_left.Stop();
            }
        }

        /// <summary>
        /// Enviamos la posición al servidor cada cierto tiempo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_posicion_Tick(object sender, EventArgs e)
        {
            string mensaje = "2/9/"+Convert.ToString(indicePartida) + "," +Convert.ToString(cocheId) + "," + Convert.ToString(((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Location.X) + "," + Convert.ToString(distancia);
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_up_Tick(object sender, EventArgs e)
        {
            distancia += 5;
            if (carr == 0)
            {
                carr = 1;
                carretera.Image = new Bitmap("carretera2.jpg");
            }
            else if (carr == 1)
            {
                carr = 2;
                carretera.Image = new Bitmap("carretera3.jpg");
            }
            else if (carr == 2)
            {
                carr = 0;
                carretera.Image = new Bitmap("carretera.jpg");
            }

            if (distancia > 15)
            {
                meta.Visible = true;
                meta.Location = new Point(meta.Location.X, meta.Location.Y + 15);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_right_Tick(object sender, EventArgs e)
        {
            if (((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Location.X<=(carretera.Location.X + carretera.Width - ((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Width))
            {
                ((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Location = new Point(((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Location.X + 5, ((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Location.Y);
            }
             
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_left_Tick(object sender, EventArgs e)
        {
            if (((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Location.X >= (carretera.Location.X))
            {
                ((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Location = new Point(((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Location.X - 5, ((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Location.Y);
            }
            
        }

    }
}
