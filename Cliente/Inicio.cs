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
        int puerto = 50055;
        //string ip = "192.168.56.102";
        //int puerto = 9055;
        delegate void DelegadoParaActualizar(string mensaje);
        delegate void DelegadoParaLobby();
        delegate void DelegadoParaChat(string nombre, string mensaje);
        delegate void DelegadoParaGraficos(int numeroJugadores);
        delegate void DelegadoParaPosicion(int jugador, int posX, int posY);
        delegate void DelegadoParaEsconder(int cocheGameOver);
        delegate void DelegadoParaRanking(string mensaje);
        delegate void DelegadoParaBorrar();
        int tiempoEspera;
        int indicePartida;
        string miUsuario;
        int cocheId;
        bool PartidaEmpezada;




        //Variables de los gráficos

        int up, right, left, down;
        int distancia;
        int mover_lineas;
        int mover_obs;
        int mover_premios;
        int premiosOn;
        int gameOver;
        int numRnd;
        int vel_elementos;
        int metaOn;
        bool fin;
        PictureBox[] lineas = new PictureBox[12]; //Vector que contiene las lineas de la carretera
        PictureBox[] obstaculos = new PictureBox[3]; //Vector que contiene los obstaculos
        PictureBox[] premios = new PictureBox[4]; //Vcetor que contiene los premios
        Random Rnd = new Random();
        bool primeraVez;

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
            Image wallpaper = new Bitmap("iniciar.jpg");
            this.BackgroundImage = wallpaper;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            //Empieza con Iniciar sesión y escondemos el resto:
            conectadosGrid.Hide();
            desconectarBtn.Hide();
            borrarUsuarioBtn.Hide();
            nombreTxt.Hide();
            posicionMedia.Hide();
            jugMasVeces.Hide();
            numVictorias.Hide();
            jugadoBtn.Hide();
            busquedaBtn.Hide();
            meta.Hide();
            carretera.Hide();
            coche1.Hide();
            coche2.Hide();
            coche3.Hide();
            coche4.Hide();
            flechas.Hide();
            rankingGrid.Hide();
            segundosLbl.Location = new Point(50, 300);
            rankingGrid.Location = new Point(600,100);

            usuarioLbl.BackColor = System.Drawing.Color.Transparent;
            contraseñaLbl.BackColor = System.Drawing.Color.Transparent;
            linea1Pic.Hide();
            linea2Pic.Hide();
            linea3Pic.Hide();
            linea4Pic.Hide();
            linea5Pic.Hide();
            linea6Pic.Hide();
            linea7Pic.Hide();
            linea8Pic.Hide();
            linea9Pic.Hide();
            linea10Pic.Hide();
            linea11Pic.Hide();
            linea12Pic.Hide();
            lineas[0] = linea1Pic;
            lineas[1] = linea2Pic;
            lineas[2] = linea3Pic;
            lineas[3] = linea4Pic;
            lineas[4] = linea5Pic;
            lineas[5] = linea6Pic;
            lineas[6] = linea7Pic;
            lineas[7] = linea8Pic;
            lineas[8] = linea9Pic;
            lineas[9] = linea10Pic;
            lineas[10] = linea11Pic;
            lineas[11] = linea12Pic;
            obs1Pic.Hide();
            obs2Pic.Hide();
            obs3Pic.Hide();
            obstaculos[0] = obs1Pic;
            obstaculos[1] = obs2Pic;
            obstaculos[2] = obs3Pic;
            premio1Pic.Hide();
            premio2Pic.Hide();
            premio3Pic.Hide();
            premio4Pic.Hide();
            premios[0] = premio1Pic;
            premios[1] = premio2Pic;
            premios[2] = premio3Pic;
            premios[3] = premio4Pic;
            calamarPic.Hide();
            copaPic.Hide();
            for (int k = 0; k<4; k++)
            {
                premios[k].Size = new Size(premio1Pic.Height, premio1Pic.Height);
            }
            this.Size = new Size(300, 250);
            
            chatMensajeLbl.Hide();
            chatNombreLbl.Location = new Point(900-380,600-100);
            chatNombreLbl.Hide();
            chatBox.Hide();
            chatBox.Location = new Point(chatNombreLbl.Location.X,chatNombreLbl.Location.Y+chatNombreLbl.Height+5);
            chatBox.Width = 300;
            enviarBtn.Location = new Point(chatBox.Location.X+chatBox.Width,chatBox.Location.Y);
            enviarBtn.Size = new Size(enviarBtn.Width, chatBox.Height);
            conectadosGrid.Location = new Point(enviarBtn.Location.X + enviarBtn.Width - conectadosGrid.Width, 50);
            usuariosConectadosLbl.Location = new Point(conectadosGrid.Location.X + conectadosGrid.Width - usuariosConectadosLbl.Width-50, conectadosGrid.Location.Y - 15);
            usuariosConectadosLbl.Hide();
            timer_posicion.Interval = 100;
            timerPremio.Interval = 11000;
            timer_victoria.Interval = 5000;
            numRnd = 0;
            cocheId = 0;
            PartidaEmpezada = false;
            usuariosConectadosLbl.BackColor = System.Drawing.Color.Transparent;
            chatMensajeLbl.BackColor = System.Drawing.Color.Transparent;
            chatMensajeLbl.ForeColor = Color.White;
            chatNombreLbl.BackColor = System.Drawing.Color.Transparent;
            chatNombreLbl.ForeColor = Color.White;
            segundosLbl.BackColor = System.Drawing.Color.Transparent;


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
                    conectadosGrid.Rows[i - 1].Cells[0].Value = elementos[i].Split('*')[0];
                    conectadosGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    conectadosGrid.Size = new Size(conectadosGrid.Rows[i - 1].Cells[0].Size.Width, conectadosGrid.Rows[i - 1].Cells[0].Size.Height*i);
                    if(Convert.ToInt32(elementos[i].Split('*')[1]) == 0)
                    {
                        conectadosGrid.Rows[i - 1].DefaultCellStyle.BackColor = Color.Green;
                        conectadosGrid.Rows[i - 1].DefaultCellStyle.SelectionBackColor = Color.Green;
                        conectadosGrid.Rows[i - 1].DefaultCellStyle.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        conectadosGrid.Rows[i - 1].DefaultCellStyle.BackColor = Color.Red;
                        conectadosGrid.Rows[i - 1].DefaultCellStyle.SelectionBackColor = Color.Red;
                        conectadosGrid.Rows[i - 1].DefaultCellStyle.SelectionForeColor = Color.Black;
                        conectadosGrid.Rows[i - 1].Cells[0].Value = elementos[i].Split('*')[0] + " (en partida)";
                    }
                }
            }
            else
            {
                conectadosGrid.Columns.Clear();
                conectadosGrid.Rows.Clear();
            }
        }

        /// <summary>
        /// Calcula las posiciones de todos y envia al servidor la del cliente
        /// </summary>
        /// <param name="mensaje"></param>
        public void ActualizarRanking(string mensaje)
        {
            endLbl.Hide();
            timer_victoria.Start();
            timer_up.Stop();
            fin = true;
            distancia = 0;
            meta.Location = new Point(carretera.Location.X, carretera.Location.Y);
            string[] elementos = mensaje.Split(',');

            rankingGrid.RowCount = Convert.ToInt32(elementos[0]);
            rankingGrid.ColumnCount = 2;
            rankingGrid.RowHeadersVisible = false;
            rankingGrid.ColumnHeadersVisible = false;
            rankingGrid.ReadOnly = true;
            rankingGrid.ScrollBars = ScrollBars.None;

            string[] nombres = { elementos[1], elementos[3], elementos[5], elementos[7] };
            int[] distancias = { Convert.ToInt32(elementos[2]), Convert.ToInt32(elementos[4]), Convert.ToInt32(elementos[6]), Convert.ToInt32(elementos[8]) };
            for (int i = 0; i < 3; i++)
            {
                for (int j = i + 1; j < 4; j++)
                {
                    if (distancias[j] > distancias[i])
                    {
                        string aux1 = nombres[i];
                        int aux2 = distancias[i];
                        nombres[i] = nombres[j];
                        distancias[i] = distancias[j];
                        nombres[j] = aux1;
                        distancias[j] = aux2;
                    }
                }
            }

            for (int i = 0; i < Convert.ToInt32(elementos[0]); i++)
            {
                rankingGrid.Rows[i].Cells[0].Value = i + 1;
                rankingGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                rankingGrid.Rows[i].Cells[1].Value = nombres[i];
                rankingGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            rankingGrid.Size = new Size(rankingGrid.Rows[0].Cells[0].Size.Width * 2, rankingGrid.Rows[0].Cells[0].Size.Height * Convert.ToInt32(elementos[0]));


            int posicion = 1;
            if (nombres[1] == usuarioTxt.Text)
                posicion = 2;
            else if (nombres[2] == usuarioTxt.Text)
                posicion = 3;
            else if (nombres[3] == usuarioTxt.Text)
                posicion = 4;

            
            mensaje = "2/13/" + usuarioTxt.Text + "/" + elementos[9] + "/" + Convert.ToString(posicion);
            // Enviamos el nombre al servidor
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

    
        /// <summary>
        /// Iniciamos la pantalla principal del juego, después de iniciar sesión o de acabar una partida
        /// </summary>
        public void IniciarLobby()
        {
            nombreTxt.Show();
            nombreTxt.Location = new Point(50, 50);
            //nombreTxt.BackColor = System.Drawing.Color.Transparent;
            posicionMedia.Show();
            posicionMedia.BackColor = System.Drawing.Color.Transparent;
            posicionMedia.Location = new Point(50, 100);
            jugMasVeces.Show();
            jugMasVeces.BackColor = System.Drawing.Color.Transparent;
            jugMasVeces.Location = new Point(50, 150);
            numVictorias.Show();
            numVictorias.BackColor = System.Drawing.Color.Transparent;
            numVictorias.Location = new Point(50, 200);
            jugadoBtn.Show();
            jugadoBtn.BackColor = System.Drawing.Color.Transparent;
            jugadoBtn.Location = new Point(50, 250);
            busquedaBtn.Show();
            busquedaBtn.BackColor = System.Drawing.Color.Transparent;
            busquedaBtn.Location = new Point(50, 300);
            conectadosGrid.Show();
            desconectarBtn.Show();
            borrarUsuarioBtn.Show();
            conectarBtn.Hide();
            this.Size = new Size(900, 600);
            desconectarBtn.Location = new Point(50, 500);
            borrarUsuarioBtn.Location = new Point(150, 500);
            loginBtn.Hide();
            registrarseBtn.Hide();
            usuarioLbl.Hide();
            usuarioTxt.Hide();
            contraseñaLbl.Hide();
            contraseñaTxt.Hide();
            usuariosConectadosLbl.Show();
            chatBox.Show();
            endLbl.Hide();
            enviarBtn.Show();
            segundosLbl.Hide();
            Image wallpaper = new Bitmap("cover.jpg");
            this.BackgroundImage = wallpaper;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

        }


        public void MostrarChat(string nombre, string mensaje)
        {
            chatMensajeLbl.Show();
            chatNombreLbl.Show();
            chatNombreLbl.Text = nombre;
            chatMensajeLbl.Text = mensaje;
            chatMensajeLbl.Location = new Point(chatNombreLbl.Location.X+chatNombreLbl.Width,chatNombreLbl.Location.Y);
        }
        
        /// <summary>
        /// Pone la nueva posición de todos los coches rivales.
        /// </summary>
        /// <param name="jugador"></param>
        /// <param name="PosX"></param>
        /// <param name="PosY"></param>
        public void NuevaPosicion(int jugador,int PosX, int PosY)
        {
            Controls["coche" + Convert.ToString(jugador)].Location = new Point(PosX, -PosY+ distancia + ((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Location.Y);
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
        /// Cuando un coche se choca lo escondemos
        /// </summary>
        /// <param name="cocheGameOver"></param>
        public void EsconderCoche(int cocheGameOver)
        {
            ((PictureBox)Controls["coche" + Convert.ToString(cocheGameOver)]).Hide();
        }

        /// <summary>
        /// Cargamos la pantalla de inicio una vez nos desconectamos del lobby o de si borramos el usuario
        /// </summary>
        public void LoadInicio()
        {
            this.BackColor = Color.Gray;
            Image wallpaper = new Bitmap("iniciar.jpg");
            this.BackgroundImage = wallpaper;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            conectarBtn.BackColor = Color.Gray;
            loginBtn.BackColor = Color.Gray;
            registrarseBtn.BackColor = Color.Gray;

            desconectarBtn.Hide();
            borrarUsuarioBtn.Hide();
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
            jugadoBtn.Hide();
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
                                    else if (resultado == "no" && !PartidaEmpezada)
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
                            case 7://Empezamos la partida
                                auxiliar = mensaje.Split(',')[0];
                                if (auxiliar == "0")
                                {
                                    int numeroJugadores = Convert.ToInt32(mensaje.Split(',')[1]);
                                    cocheId = Convert.ToInt32(mensaje.Split(',')[2]);
                                    //MessageBox.Show("La partida va a empezar");
                                    DelegadoParaGraficos delegado4 = new DelegadoParaGraficos(LoadGraficos);
                                    this.Invoke(delegado4, new object[] { numeroJugadores });
                                }
                                else
                                {
                                    MessageBox.Show("Nadie ha aceptado la solicitud");
                                }
                                break;
                            case 8://Recibe un mensaje de chat
                                string nombre = mensaje.Split(',')[0];
                                string texto = mensaje.Split(',')[1];
                                DelegadoParaChat delegado3 = new DelegadoParaChat(MostrarChat);
                                this.Invoke(delegado3, new object[] { nombre, texto });
                                break;
                            case 9://Recibe la posición del resto de los jugadores
                                int jugador = Convert.ToInt32(mensaje.Split(',')[0]);
                                int posX = Convert.ToInt32(mensaje.Split(',')[1]);
                                int posY = Convert.ToInt32(mensaje.Split(',')[2]);
                                DelegadoParaPosicion delegado5 = new DelegadoParaPosicion(NuevaPosicion);
                                this.Invoke(delegado5, new object[] { jugador, posX, posY });
                                break;
                            case 10://Escondemos el coche cuando se choca
                                int cocheGameOver = Convert.ToInt32(mensaje);
                                DelegadoParaEsconder delegado6 = new DelegadoParaEsconder(EsconderCoche);
                                this.Invoke(delegado6, new object[] {cocheGameOver});
                                break;
                            case 11://Alguien ha ganado y calculamos el ranking
                                DelegadoParaRanking delegado7 = new DelegadoParaRanking(ActualizarRanking);
                                this.Invoke(delegado7, new object[] { mensaje });
                                break;
                            case 12://Borrar Usuario
                                if (mensaje == "0")
                                {
                                    MessageBox.Show("Se ha eliminado correctamente");
                                    DelegadoParaBorrar delegado8 = new DelegadoParaBorrar(LoadInicio);
                                    this.Invoke(delegado8, new object[] { });
                                }
                                else
                                {
                                    MessageBox.Show("No se ha eliminado correctamente");
                                }
                                break;
                            case 14: //Con quien ha jugado X persona
                                if(mensaje == "0")
                                {
                                    MessageBox.Show(trozos[3].Replace(',','\n'));
                                }
                                else
                                {
                                    MessageBox.Show("Aun no ha jugado contra nadie");
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
            if (this.BackColor == Color.Green)
            {
                string mensaje = "1/1/" + usuarioTxt.Text + "/" + contraseñaTxt.Text;
                // Enviamos el nombre al servidor
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
                
        }

        /// <summary>
        /// Petición de registrarse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void registrarseBtn_Click(object sender, EventArgs e)
        {
            if(this.BackColor==Color.Green)
            {
                string mensaje = "1/2/" + usuarioTxt.Text + "/" + contraseñaTxt.Text;
                // Enviamos el nombre al servidor
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
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
                else if (jugadoBtn.Checked)
                {
                    if (nombreTxt.Text != "")
                    {
                        string mensaje = "2/14/" + nombreTxt.Text;
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
                    conectarBtn.BackColor = Color.Green;
                    loginBtn.BackColor = Color.Green;
                    registrarseBtn.BackColor = Color.Green;
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

                    server.Shutdown(SocketShutdown.Both);
                    server.Close();
                    LoadInicio();

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
                    string mensaje;
                    byte[] msg;
                    if (cocheId!=0)
                    {
                        mensaje = "2/10/" + Convert.ToString(cocheId) + "/" + Convert.ToString(indicePartida);
                        // Enviamos el coche chocado al servidor
                        msg = Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                    }
                    mensaje = "0/0/" + usuarioTxt.Text;
                    // Enviamos al servidor el nombre tecleado
                    msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
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
            if (nombre_invitado == miUsuario)
            {
                MessageBox.Show("No puedes invitarte a ti mismo.");
            }
            else if(conectadosGrid.Rows[e.RowIndex].Cells[0].Value.ToString().EndsWith("(en partida)"))
            {
                MessageBox.Show("Este jugador ya está jugando otra partida.");
            }
            else
            {
                string mensaje = "2/5/" + nombre_invitado;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                if (tiempoEspera == 0)
                {
                    segundosLbl.Show();
                    timer1.Start();
                    timer1.Interval = 1000;
                }
            }
        }

        /// <summary>
        /// Cuenta atrás, antes de empezar la partida
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            tiempoEspera++;
            segundosLbl.Text = "La partida empezará en...\n" + Convert.ToString(8-tiempoEspera);
            if (tiempoEspera == 8)
            {
                string mensaje = "2/7/"+ indicePartida;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                timer1.Stop();
                PartidaEmpezada = true;

                //Cambiar el form y poner los gráficos aquí
                conectadosGrid.Hide();
                desconectarBtn.Hide();


                tiempoEspera = 0;//Poner el tiempo a 0 por si hay que volver a invitar a alguien
            }
        }

        /// <summary>
        /// Enviamos mensaje en el chat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// Cargamos los gráficos del juego, cuando empieza la partida
        /// </summary>
        public void LoadGraficos(int numeroJugadores)
        {
            gameOver = 0;
            primeraVez = true;
            fin = false;

            //carretera.Size = new Size(600, this.Height);
            this.BackgroundImage = null;
            this.BackColor = Color.Green;
            carretera.Size = new Size(600, 800);
            carretera.Location = new Point(50,0);
            //Controls["linea1Pic"].Location = new Point(carretera.Location.X + (carretera.Width / 3) - ((Controls["linea1Pic"].Width) / 2), carretera.Location.Y);
            this.Height = carretera.Height;
            int g;
            for (g = 1; g <= 12; g++)
            {
                
                ((PictureBox)Controls["linea" + Convert.ToString(g) + "Pic"]).Size = new Size(8,65);
                if (g <= 4)
                {
                    ((PictureBox)Controls["linea" + Convert.ToString(g) + "Pic"]).Location = new Point((carretera.Location.X + (carretera.Width / 4) - ((Controls["linea" + Convert.ToString(g) + "Pic"].Width) / 2)), (carretera.Location.Y + (g*carretera.Height / 4) - ((Controls["linea" + Convert.ToString(g) + "Pic"].Height) / 2)));
                }
                else if (g <= 8)
                {
                    ((PictureBox)Controls["linea" + Convert.ToString(g) + "Pic"]).Location = new Point((carretera.Location.X + (2 * carretera.Width / 4) - ((Controls["linea" + Convert.ToString(g) + "Pic"].Width) / 2)), (carretera.Location.Y + ((g-4) * carretera.Height / 4) - ((Controls["linea" + Convert.ToString(g) + "Pic"].Height) / 2)));
                }
                else
                {
                    ((PictureBox)Controls["linea" + Convert.ToString(g) + "Pic"]).Location = new Point((carretera.Location.X + (3 * carretera.Width / 4) - ((Controls["linea" + Convert.ToString(g) + "Pic"].Width) / 2)), (carretera.Location.Y + ((g - 8) * carretera.Height / 4) - ((Controls["linea" + Convert.ToString(g) + "Pic"].Height) / 2)));
                }
            }

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
                ((PictureBox)Controls["coche" + Convert.ToString(i)]).Size = new Size(70, 70);
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
            meta.BackColor = Color.Gold;
            meta.Size = new Size(carretera.Width + carretera.Location.X-50, 70);
            meta.Image = new Bitmap("meta.png");
            meta.SizeMode = PictureBoxSizeMode.StretchImage;
            copaPic.Image = new Bitmap("copa.gif");
            copaPic.SizeMode = PictureBoxSizeMode.StretchImage;
            copaPic.Size = new Size(450, 450);
            copaPic.Location = new Point(carretera.Location.X + (carretera.Width / 2) - (copaPic.Width / 2), carretera.Location.Y + (carretera.Height / 2) - (copaPic.Height / 2));
            obs1Pic.Location = new Point(carretera.Location.X + carretera.Width/4,carretera.Location.Y);
            obs2Pic.Location = new Point(carretera.Location.X + carretera.Width/4*2, carretera.Location.Y);
            obs3Pic.Location = new Point(carretera.Location.X + carretera.Width/4*3, carretera.Location.Y);
            endLbl.Location = new Point(carretera.Location.X + (carretera.Width/2),carretera.Location.Y+(carretera.Height/2));
            premio1Pic.Image = new Bitmap("premio.png");
            premio1Pic.SizeMode = PictureBoxSizeMode.StretchImage;
            premio2Pic.Image = new Bitmap("premio.png");
            premio2Pic.SizeMode = PictureBoxSizeMode.StretchImage;
            premio3Pic.Image = new Bitmap("premio.png");
            premio3Pic.SizeMode = PictureBoxSizeMode.StretchImage;
            premio4Pic.Image = new Bitmap("premio.png");
            premio4Pic.SizeMode = PictureBoxSizeMode.StretchImage;
            premio1Pic.Size = coche1.Size;
            premio2Pic.Size = coche1.Size;
            premio3Pic.Size = coche1.Size;
            premio4Pic.Size = coche1.Size;
            calamarPic.Image = new Bitmap("calamardo.gif");
            calamarPic.SizeMode = PictureBoxSizeMode.StretchImage;
            calamarPic.Size = new Size(450, 450);
            calamarPic.Location = new Point(carretera.Location.X + (carretera.Width/2) - (calamarPic.Width/2), carretera.Location.Y + (carretera.Height/2) - (calamarPic.Height/2));
            obs1Pic.Image = new Bitmap("obs.png");
            obs1Pic.SizeMode = PictureBoxSizeMode.StretchImage;
            obs2Pic.Image = new Bitmap("obs.png");
            obs2Pic.SizeMode = PictureBoxSizeMode.StretchImage;
            obs3Pic.Image = new Bitmap("obs.png");
            obs3Pic.SizeMode = PictureBoxSizeMode.StretchImage;
            obs1Pic.Size = new Size(50,50);
            obs2Pic.Size = new Size(75,75);
            obs3Pic.Size = new Size(150,150);


            distancia = 0;
            up = 0;
            down = 0;
            right = 0;
            left = 0;
            timer_up.Interval = 150;
            timer_posicion.Start();
            metaOn = 0;

            //Escondemos el resto de elementos

            borrarUsuarioBtn.Hide();
            conectadosGrid.Hide();
            desconectarBtn.Hide();
            nombreTxt.Hide();
            posicionMedia.Hide();
            jugMasVeces.Hide();
            numVictorias.Hide();
            jugadoBtn.Hide();
            busquedaBtn.Hide();
            usuariosConectadosLbl.Hide();
            chatMensajeLbl.Hide();
            chatNombreLbl.Hide();
            chatBox.Hide();
            segundosLbl.Hide();
            enviarBtn.Hide();

            //Mostramos las picture box

            carretera.Show();
            coche1.Show();
            coche2.Show();
            coche3.Show();
            coche4.Show();
            flechas.Show();
            linea1Pic.Show();
            linea2Pic.Show();
            linea3Pic.Show();
            linea4Pic.Show();
            linea5Pic.Show();
            linea6Pic.Show();
            linea7Pic.Show();
            linea8Pic.Show();
            linea9Pic.Show();
            linea10Pic.Show();
            linea11Pic.Show();
            linea12Pic.Show();
            obs1Pic.Show();
            obs2Pic.Show();
            obs3Pic.Show();
        }

        /// <summary>
        /// Actualizar la imagen de las flechas que estas clicando
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
        /// Inicializa los timers para moverte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameOver == 0 && fin==false)
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
        }

        /// <summary>
        /// Para los timers para dejar de moverte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if(gameOver== 0 && fin == false)
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
        }

        /// <summary>
        /// Enviamos la posición al servidor cada cierto tiempo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_posicion_Tick(object sender, EventArgs e)
        {
            if (gameOver == 0 && fin == false)
            {
                string mensaje = "2/9/" + Convert.ToString(indicePartida) + "," + Convert.ToString(cocheId) + "," + Convert.ToString(((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Location.X) + "," + Convert.ToString(distancia);
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            
        }

        /// <summary>
        /// Movemos los elementos(lineas,obstaculos,premios) de la carretera hacia abajo para dar la sensación de que el coche se mueve hacia arriba
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_up_Tick(object sender, EventArgs e)
        {
            if (gameOver == 0 && fin == false && numRnd != 3)
            {
                vel_elementos = 12;
                for (mover_lineas = 0; mover_lineas < 12; mover_lineas++)
                {
                    distancia += 1;
                    lineas[mover_lineas].Top += vel_elementos;
                    if (lineas[mover_lineas].Top >= carretera.Height)
                    {
                        lineas[mover_lineas].Top = -lineas[mover_lineas].Height;
                    }
                }
                for (mover_obs = 0; mover_obs < 3; mover_obs++)
                {
                    obstaculos[mover_obs].Top += vel_elementos;

                    if (obstaculos[mover_obs].Top >= carretera.Height)
                    {
                        obstaculos[mover_obs].Top = -obstaculos[mover_obs].Height;
                        obstaculos[mover_obs].Left = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Rnd.Next(carretera.Location.X, carretera.Location.X + carretera.Width - obstaculos[mover_obs].Width))));
                    }
                }
                if (distancia == 240 || distancia == 660)
                {
                    premiosOn = 1;
                    premio1Pic.Show();
                    premio2Pic.Show();
                    premio3Pic.Show();
                    premio4Pic.Show();
                    premio1Pic.Location = new Point(carretera.Location.X + 1 * carretera.Width / 8 - premio1Pic.Width / 2, carretera.Location.Y - premio1Pic.Height);
                    premio2Pic.Location = new Point(carretera.Location.X + 3 * carretera.Width / 8 - premio1Pic.Width / 2, carretera.Location.Y - premio1Pic.Height);
                    premio3Pic.Location = new Point(carretera.Location.X + 5 * carretera.Width / 8 - premio1Pic.Width / 2, carretera.Location.Y - premio1Pic.Height);
                    premio4Pic.Location = new Point(carretera.Location.X + 7 * carretera.Width / 8 - premio1Pic.Width / 2, carretera.Location.Y - premio1Pic.Height);
                }
                if (distancia == 1200)
                {
                    meta.Show();
                    meta.Location = new Point(carretera.Location.X, carretera.Location.Y);
                    metaOn = 1;
                }
                if (metaOn == 1)//Movemos la meta
                {
                    meta.Top += vel_elementos;
                }

                if (Controls["coche" + Convert.ToString(cocheId)].Bounds.IntersectsWith(meta.Bounds) && fin==false)
                {
                    Victoria();
                }

                if (premiosOn == 1)
                {
                    for (mover_premios = 0; mover_premios < 4; mover_premios++)
                    {

                        premios[mover_premios].Top += vel_elementos;

                        if (premios[mover_premios].Top >= carretera.Height)
                        {
                            premios[mover_premios].Hide();
                            premiosOn = 0;
                        }
                    }
                }

                //if (numRnd == 0) esto ponerlo en el timer de cuando se pasa el tiempo de premio
                //{
                //    ((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Image = new Bitmap("coche_publi.png");
                //}

                if (numRnd != 1)
                {
                    if (Controls["coche" + Convert.ToString(cocheId)].Bounds.IntersectsWith(obs1Pic.Bounds))
                    {
                        GameOver();
                    }
                    if (Controls["coche" + Convert.ToString(cocheId)].Bounds.IntersectsWith(obs2Pic.Bounds))
                    {
                        GameOver();
                    }
                    if (Controls["coche" + Convert.ToString(cocheId)].Bounds.IntersectsWith(obs3Pic.Bounds))
                    {
                        GameOver();
                    }
                }

                if (numRnd == 0) //Poner el rnd a 0 cuando se acabe el tiempo de premio
                {
                    if (Controls["coche" + Convert.ToString(cocheId)].Bounds.IntersectsWith(premio1Pic.Bounds))
                    {
                        Premios();
                    }
                    if (Controls["coche" + Convert.ToString(cocheId)].Bounds.IntersectsWith(premio2Pic.Bounds))
                    {
                        Premios();
                    }
                    if (Controls["coche" + Convert.ToString(cocheId)].Bounds.IntersectsWith(premio3Pic.Bounds))
                    {
                        Premios();
                    }
                    if (Controls["coche" + Convert.ToString(cocheId)].Bounds.IntersectsWith(premio4Pic.Bounds))
                    {
                        Premios();
                    }
                }
            }
        }

        /// <summary>
        /// Cuando te chocas con un obstaculo se acaba la partida
        /// </summary>
        private void GameOver()
        {
            endLbl.Visible = true;
            (Controls["coche" + Convert.ToString(cocheId)]).Hide();
            gameOver = 1;
            timer_up.Stop();
            timer_right.Stop();
            timer_left.Stop();
            timer_posicion.Stop(); //Este hay que pararlo?
            timer_right.Stop();
            timer_left.Stop();
            string mensaje = "2/10/" + Convert.ToString(cocheId) +"/"+ Convert.ToString(indicePartida);
            // Enviamos el coche chocado al servidor
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            //No se para porque al clickar otra vez la flecha se enciende el timer, arreglar esto
            //Enviar mensaje al servidor para que le envie al resto indicando que jugador ha quedado eliminado y para que lo hidee
        }

        /// <summary>
        /// Alguien ha cruzado la meta
        /// </summary>
        private void Victoria()
        {
            copaPic.Show();
            meta.Hide();
            endLbl.Hide();
            string mensaje = "2/11/" + Convert.ToString(indicePartida);
            // Enviamos el coche chocado al servidor
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        /// <summary>
        /// Tiempo en la que es valido el premio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void timerPremio_Tick(object sender, EventArgs e)
        {
            if (numRnd == 2)
            {
                timer_posicion.Interval = timer_posicion.Interval *2;
            }
            else if(numRnd == 3)
            {
                timerPremio.Interval = timerPremio.Interval *3;
            }
            else if(numRnd == 4)
            {
                obs1Pic.Image = new Bitmap("obs.png");
                obs1Pic.SizeMode = PictureBoxSizeMode.StretchImage;
                obs2Pic.Image = new Bitmap("obs.png");
                obs2Pic.SizeMode = PictureBoxSizeMode.StretchImage;
                obs3Pic.Image = new Bitmap("obs.png");
                obs3Pic.SizeMode = PictureBoxSizeMode.StretchImage;
                calamarPic.Hide();
            }
            numRnd = 0;
            timerPremio.Stop();
            ((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Image = new Bitmap("coche_publi.png");
            ((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).SizeMode = PictureBoxSizeMode.StretchImage;
        }

        /// <summary>
        /// Tiempo en el que se muestra el gif de victoria
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_victoria_Tick(object sender, EventArgs e)
        {
            //Esconder el gif de copa y todo lo de la carrera y showear las tabla de posiciones.
            if (primeraVez)
            {
                copaPic.Hide();
                carretera.Hide();
                coche1.Hide();
                coche2.Hide();
                coche3.Hide();
                coche4.Hide();
                flechas.Hide();
                linea1Pic.Hide();
                linea2Pic.Hide();
                linea3Pic.Hide(); 
                linea4Pic.Hide();
                linea5Pic.Hide();
                linea6Pic.Hide();
                linea7Pic.Hide();
                linea8Pic.Hide();
                linea9Pic.Hide();
                linea10Pic.Hide();
                linea11Pic.Hide();
                linea12Pic.Hide();
                obs1Pic.Hide();
                obs2Pic.Hide();
                obs3Pic.Hide();
                meta.Hide();
                premio1Pic.Hide();
                premio2Pic.Hide();
                premio3Pic.Hide();
                premio4Pic.Hide();
                rankingGrid.Show();

                Image wallpaper = new Bitmap("rank.jpg");
                this.Size = new Size(900, 600);
                this.BackgroundImage = wallpaper;
                this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

                primeraVez = false;
            }
            else
            {
                IniciarLobby();
                primeraVez = true;
                rankingGrid.Hide();
                timer_victoria.Stop();
            }
            
        }

        /// <summary>
        /// Borrar todos los datos del usuario con el que te has logueado de la BBDD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void borrarUsuarioBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult2 = MessageBox.Show("Estás seguro de que quieres borrar tu usuario permanentemente?", "Borrar Usuario",MessageBoxButtons.YesNo);
            if (dialogResult2 == DialogResult.Yes)
            {
                string mensaje = "2/12/" + usuarioTxt.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
        }

        /// <summary>
        /// Se escoge el premio de la caja sorpresa
        /// </summary>
        private void Premios()
        {
            numRnd = Rnd.Next(1, 5);
            if (numRnd == 1)//No choca con obstaculos
            {
                ((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Image = new Bitmap("premio1.png");
            }
            else if (numRnd == 2) //Mas velocidad
            {
                ((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Image = new Bitmap("cohete.gif");
                timer_up.Interval = timer_up.Interval/2;
            }
            else if (numRnd==3) //Telaraña
            {
                ((PictureBox)Controls["coche" + Convert.ToString(cocheId)]).Image = new Bitmap("tela.png");
                timerPremio.Interval = timerPremio.Interval / 3;
            }
            else //Calamar
            {
                obs1Pic.Image = new Bitmap("gris.png");
                obs1Pic.SizeMode = PictureBoxSizeMode.StretchImage;
                obs2Pic.Image = new Bitmap("gris.png");
                obs2Pic.SizeMode = PictureBoxSizeMode.StretchImage;
                obs3Pic.Image = new Bitmap("gris.png");
                obs3Pic.SizeMode = PictureBoxSizeMode.StretchImage;
                calamarPic.Show();
            }
            timerPremio.Start();
        }
        /// <summary>
        /// Moverse hacia la derecha
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
        /// Moverse hacia la izquierda
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
