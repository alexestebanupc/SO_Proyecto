
namespace EjGuia
{
    partial class Inicio
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.posicionMedia = new System.Windows.Forms.RadioButton();
            this.jugMasVeces = new System.Windows.Forms.RadioButton();
            this.nombreTxt = new System.Windows.Forms.TextBox();
            this.busquedaBtn = new System.Windows.Forms.Button();
            this.numVictorias = new System.Windows.Forms.RadioButton();
            this.conectarBtn = new System.Windows.Forms.Button();
            this.desconectarBtn = new System.Windows.Forms.Button();
            this.loginBtn = new System.Windows.Forms.Button();
            this.registrarseBtn = new System.Windows.Forms.Button();
            this.contraseñaLbl = new System.Windows.Forms.Label();
            this.usuarioLbl = new System.Windows.Forms.Label();
            this.contraseñaTxt = new System.Windows.Forms.TextBox();
            this.usuarioTxt = new System.Windows.Forms.TextBox();
            this.conectadosGrid = new System.Windows.Forms.DataGridView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.segundosLbl = new System.Windows.Forms.Label();
            this.usuariosConectadosLbl = new System.Windows.Forms.Label();
            this.chatBox = new System.Windows.Forms.TextBox();
            this.chatNombreLbl = new System.Windows.Forms.Label();
            this.chatMensajeLbl = new System.Windows.Forms.Label();
            this.enviarBtn = new System.Windows.Forms.Button();
            this.meta = new System.Windows.Forms.PictureBox();
            this.carretera = new System.Windows.Forms.PictureBox();
            this.coche1 = new System.Windows.Forms.PictureBox();
            this.flechas = new System.Windows.Forms.PictureBox();
            this.timer_up = new System.Windows.Forms.Timer(this.components);
            this.timer_left = new System.Windows.Forms.Timer(this.components);
            this.timer_right = new System.Windows.Forms.Timer(this.components);
            this.coche2 = new System.Windows.Forms.PictureBox();
            this.coche3 = new System.Windows.Forms.PictureBox();
            this.coche4 = new System.Windows.Forms.PictureBox();
            this.timer_posicion = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.conectadosGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.meta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.carretera)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.coche1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.flechas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.coche2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.coche3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.coche4)).BeginInit();
            this.SuspendLayout();
            // 
            // posicionMedia
            // 
            this.posicionMedia.AutoSize = true;
            this.posicionMedia.Location = new System.Drawing.Point(554, 219);
            this.posicionMedia.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.posicionMedia.Name = "posicionMedia";
            this.posicionMedia.Size = new System.Drawing.Size(222, 24);
            this.posicionMedia.TabIndex = 0;
            this.posicionMedia.TabStop = true;
            this.posicionMedia.Text = "Posición media del jugador";
            this.posicionMedia.UseVisualStyleBackColor = true;
            // 
            // jugMasVeces
            // 
            this.jugMasVeces.AutoSize = true;
            this.jugMasVeces.Location = new System.Drawing.Point(554, 260);
            this.jugMasVeces.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.jugMasVeces.Name = "jugMasVeces";
            this.jugMasVeces.Size = new System.Drawing.Size(282, 24);
            this.jugMasVeces.TabIndex = 1;
            this.jugMasVeces.TabStop = true;
            this.jugMasVeces.Text = "Jugador que más veces ha ganado";
            this.jugMasVeces.UseVisualStyleBackColor = true;
            // 
            // nombreTxt
            // 
            this.nombreTxt.Location = new System.Drawing.Point(633, 158);
            this.nombreTxt.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nombreTxt.Name = "nombreTxt";
            this.nombreTxt.Size = new System.Drawing.Size(112, 26);
            this.nombreTxt.TabIndex = 2;
            // 
            // busquedaBtn
            // 
            this.busquedaBtn.Location = new System.Drawing.Point(565, 379);
            this.busquedaBtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.busquedaBtn.Name = "busquedaBtn";
            this.busquedaBtn.Size = new System.Drawing.Size(112, 29);
            this.busquedaBtn.TabIndex = 3;
            this.busquedaBtn.Text = "Busqueda";
            this.busquedaBtn.UseVisualStyleBackColor = true;
            this.busquedaBtn.Click += new System.EventHandler(this.busquedaBtn_Click);
            // 
            // numVictorias
            // 
            this.numVictorias.AutoSize = true;
            this.numVictorias.Location = new System.Drawing.Point(554, 305);
            this.numVictorias.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numVictorias.Name = "numVictorias";
            this.numVictorias.Size = new System.Drawing.Size(173, 24);
            this.numVictorias.TabIndex = 4;
            this.numVictorias.TabStop = true;
            this.numVictorias.Text = "Número de victorias";
            this.numVictorias.UseVisualStyleBackColor = true;
            // 
            // conectarBtn
            // 
            this.conectarBtn.Location = new System.Drawing.Point(42, 205);
            this.conectarBtn.Name = "conectarBtn";
            this.conectarBtn.Size = new System.Drawing.Size(117, 53);
            this.conectarBtn.TabIndex = 5;
            this.conectarBtn.Text = "Conectarse";
            this.conectarBtn.UseVisualStyleBackColor = true;
            this.conectarBtn.Click += new System.EventHandler(this.conectarBtn_Click);
            // 
            // desconectarBtn
            // 
            this.desconectarBtn.Location = new System.Drawing.Point(699, 49);
            this.desconectarBtn.Name = "desconectarBtn";
            this.desconectarBtn.Size = new System.Drawing.Size(131, 53);
            this.desconectarBtn.TabIndex = 6;
            this.desconectarBtn.Text = "Desconectarse";
            this.desconectarBtn.UseVisualStyleBackColor = true;
            this.desconectarBtn.Click += new System.EventHandler(this.desconectarBtn_Click);
            // 
            // loginBtn
            // 
            this.loginBtn.Location = new System.Drawing.Point(206, 195);
            this.loginBtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.loginBtn.Name = "loginBtn";
            this.loginBtn.Size = new System.Drawing.Size(152, 29);
            this.loginBtn.TabIndex = 12;
            this.loginBtn.Text = "Iniciar sesión";
            this.loginBtn.UseVisualStyleBackColor = true;
            this.loginBtn.Click += new System.EventHandler(this.loginBtn_Click);
            // 
            // registrarseBtn
            // 
            this.registrarseBtn.Location = new System.Drawing.Point(206, 257);
            this.registrarseBtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.registrarseBtn.Name = "registrarseBtn";
            this.registrarseBtn.Size = new System.Drawing.Size(152, 29);
            this.registrarseBtn.TabIndex = 11;
            this.registrarseBtn.Text = "Registrarse";
            this.registrarseBtn.UseVisualStyleBackColor = true;
            this.registrarseBtn.Click += new System.EventHandler(this.registrarseBtn_Click);
            // 
            // contraseñaLbl
            // 
            this.contraseñaLbl.AutoSize = true;
            this.contraseñaLbl.Location = new System.Drawing.Point(100, 103);
            this.contraseñaLbl.Name = "contraseñaLbl";
            this.contraseñaLbl.Size = new System.Drawing.Size(92, 20);
            this.contraseñaLbl.TabIndex = 10;
            this.contraseñaLbl.Text = "Contraseña";
            // 
            // usuarioLbl
            // 
            this.usuarioLbl.AutoSize = true;
            this.usuarioLbl.Location = new System.Drawing.Point(111, 53);
            this.usuarioLbl.Name = "usuarioLbl";
            this.usuarioLbl.Size = new System.Drawing.Size(64, 20);
            this.usuarioLbl.TabIndex = 9;
            this.usuarioLbl.Text = "Usuario";
            // 
            // contraseñaTxt
            // 
            this.contraseñaTxt.Location = new System.Drawing.Point(221, 99);
            this.contraseñaTxt.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.contraseñaTxt.Name = "contraseñaTxt";
            this.contraseñaTxt.Size = new System.Drawing.Size(112, 26);
            this.contraseñaTxt.TabIndex = 8;
            // 
            // usuarioTxt
            // 
            this.usuarioTxt.Location = new System.Drawing.Point(221, 49);
            this.usuarioTxt.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.usuarioTxt.Name = "usuarioTxt";
            this.usuarioTxt.Size = new System.Drawing.Size(112, 26);
            this.usuarioTxt.TabIndex = 7;
            // 
            // conectadosGrid
            // 
            this.conectadosGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.conectadosGrid.Location = new System.Drawing.Point(42, 305);
            this.conectadosGrid.Name = "conectadosGrid";
            this.conectadosGrid.RowHeadersWidth = 62;
            this.conectadosGrid.RowTemplate.Height = 28;
            this.conectadosGrid.Size = new System.Drawing.Size(241, 226);
            this.conectadosGrid.TabIndex = 13;
            this.conectadosGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.conectadosGrid_CellClick);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // segundosLbl
            // 
            this.segundosLbl.AutoSize = true;
            this.segundosLbl.Location = new System.Drawing.Point(413, 105);
            this.segundosLbl.Name = "segundosLbl";
            this.segundosLbl.Size = new System.Drawing.Size(82, 20);
            this.segundosLbl.TabIndex = 14;
            this.segundosLbl.Text = "Segundos";
            // 
            // usuariosConectadosLbl
            // 
            this.usuariosConectadosLbl.AutoSize = true;
            this.usuariosConectadosLbl.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Bold);
            this.usuariosConectadosLbl.Location = new System.Drawing.Point(383, 65);
            this.usuariosConectadosLbl.Name = "usuariosConectadosLbl";
            this.usuariosConectadosLbl.Size = new System.Drawing.Size(217, 23);
            this.usuariosConectadosLbl.TabIndex = 15;
            this.usuariosConectadosLbl.Text = "Usuarios Conectados";
            // 
            // chatBox
            // 
            this.chatBox.Location = new System.Drawing.Point(530, 521);
            this.chatBox.Name = "chatBox";
            this.chatBox.Size = new System.Drawing.Size(265, 26);
            this.chatBox.TabIndex = 16;
            // 
            // chatNombreLbl
            // 
            this.chatNombreLbl.AutoSize = true;
            this.chatNombreLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chatNombreLbl.Location = new System.Drawing.Point(526, 498);
            this.chatNombreLbl.Name = "chatNombreLbl";
            this.chatNombreLbl.Size = new System.Drawing.Size(53, 20);
            this.chatNombreLbl.TabIndex = 17;
            this.chatNombreLbl.Text = "Juan:";
            // 
            // chatMensajeLbl
            // 
            this.chatMensajeLbl.AutoSize = true;
            this.chatMensajeLbl.Location = new System.Drawing.Point(585, 498);
            this.chatMensajeLbl.Name = "chatMensajeLbl";
            this.chatMensajeLbl.Size = new System.Drawing.Size(42, 20);
            this.chatMensajeLbl.TabIndex = 18;
            this.chatMensajeLbl.Text = "Hola";
            // 
            // enviarBtn
            // 
            this.enviarBtn.Location = new System.Drawing.Point(813, 521);
            this.enviarBtn.Name = "enviarBtn";
            this.enviarBtn.Size = new System.Drawing.Size(75, 26);
            this.enviarBtn.TabIndex = 19;
            this.enviarBtn.Text = "Enviar";
            this.enviarBtn.UseVisualStyleBackColor = true;
            this.enviarBtn.Click += new System.EventHandler(this.enviarBtn_Click);
            // 
            // meta
            // 
            this.meta.Location = new System.Drawing.Point(919, 128);
            this.meta.Name = "meta";
            this.meta.Size = new System.Drawing.Size(100, 50);
            this.meta.TabIndex = 20;
            this.meta.TabStop = false;
            // 
            // carretera
            // 
            this.carretera.Location = new System.Drawing.Point(997, 250);
            this.carretera.Name = "carretera";
            this.carretera.Size = new System.Drawing.Size(100, 50);
            this.carretera.TabIndex = 21;
            this.carretera.TabStop = false;
            // 
            // coche1
            // 
            this.coche1.Location = new System.Drawing.Point(1144, 212);
            this.coche1.Name = "coche1";
            this.coche1.Size = new System.Drawing.Size(100, 50);
            this.coche1.TabIndex = 22;
            this.coche1.TabStop = false;
            // 
            // flechas
            // 
            this.flechas.Location = new System.Drawing.Point(1071, 344);
            this.flechas.Name = "flechas";
            this.flechas.Size = new System.Drawing.Size(100, 50);
            this.flechas.TabIndex = 23;
            this.flechas.TabStop = false;
            // 
            // timer_up
            // 
            this.timer_up.Tick += new System.EventHandler(this.timer_up_Tick);
            // 
            // timer_left
            // 
            this.timer_left.Tick += new System.EventHandler(this.timer_left_Tick);
            // 
            // timer_right
            // 
            this.timer_right.Tick += new System.EventHandler(this.timer_right_Tick);
            // 
            // coche2
            // 
            this.coche2.Location = new System.Drawing.Point(1220, 318);
            this.coche2.Name = "coche2";
            this.coche2.Size = new System.Drawing.Size(100, 50);
            this.coche2.TabIndex = 24;
            this.coche2.TabStop = false;
            // 
            // coche3
            // 
            this.coche3.Location = new System.Drawing.Point(1220, 374);
            this.coche3.Name = "coche3";
            this.coche3.Size = new System.Drawing.Size(100, 50);
            this.coche3.TabIndex = 25;
            this.coche3.TabStop = false;
            // 
            // coche4
            // 
            this.coche4.Location = new System.Drawing.Point(1220, 430);
            this.coche4.Name = "coche4";
            this.coche4.Size = new System.Drawing.Size(100, 50);
            this.coche4.TabIndex = 26;
            this.coche4.TabStop = false;
            // 
            // timer_posicion
            // 
            this.timer_posicion.Tick += new System.EventHandler(this.timer_posicion_Tick);
            // 
            // Inicio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1365, 562);
            this.Controls.Add(this.coche4);
            this.Controls.Add(this.coche3);
            this.Controls.Add(this.coche2);
            this.Controls.Add(this.flechas);
            this.Controls.Add(this.coche1);
            this.Controls.Add(this.carretera);
            this.Controls.Add(this.meta);
            this.Controls.Add(this.enviarBtn);
            this.Controls.Add(this.chatMensajeLbl);
            this.Controls.Add(this.chatNombreLbl);
            this.Controls.Add(this.chatBox);
            this.Controls.Add(this.usuariosConectadosLbl);
            this.Controls.Add(this.segundosLbl);
            this.Controls.Add(this.conectadosGrid);
            this.Controls.Add(this.loginBtn);
            this.Controls.Add(this.registrarseBtn);
            this.Controls.Add(this.contraseñaLbl);
            this.Controls.Add(this.usuarioLbl);
            this.Controls.Add(this.contraseñaTxt);
            this.Controls.Add(this.usuarioTxt);
            this.Controls.Add(this.desconectarBtn);
            this.Controls.Add(this.conectarBtn);
            this.Controls.Add(this.numVictorias);
            this.Controls.Add(this.busquedaBtn);
            this.Controls.Add(this.nombreTxt);
            this.Controls.Add(this.jugMasVeces);
            this.Controls.Add(this.posicionMedia);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Inicio";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Inicio_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.conectadosGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.meta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.carretera)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.coche1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.flechas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.coche2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.coche3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.coche4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton posicionMedia;
        private System.Windows.Forms.RadioButton jugMasVeces;
        private System.Windows.Forms.TextBox nombreTxt;
        private System.Windows.Forms.Button busquedaBtn;
        private System.Windows.Forms.RadioButton numVictorias;
        private System.Windows.Forms.Button conectarBtn;
        private System.Windows.Forms.Button desconectarBtn;
        private System.Windows.Forms.Button loginBtn;
        private System.Windows.Forms.Button registrarseBtn;
        private System.Windows.Forms.Label contraseñaLbl;
        private System.Windows.Forms.Label usuarioLbl;
        private System.Windows.Forms.TextBox contraseñaTxt;
        private System.Windows.Forms.TextBox usuarioTxt;
        private System.Windows.Forms.DataGridView conectadosGrid;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label segundosLbl;
        private System.Windows.Forms.Label usuariosConectadosLbl;
        private System.Windows.Forms.TextBox chatBox;
        private System.Windows.Forms.Label chatNombreLbl;
        private System.Windows.Forms.Label chatMensajeLbl;
        private System.Windows.Forms.Button enviarBtn;
        private System.Windows.Forms.PictureBox meta;
        private System.Windows.Forms.PictureBox carretera;
        private System.Windows.Forms.PictureBox coche1;
        private System.Windows.Forms.PictureBox flechas;
        private System.Windows.Forms.Timer timer_up;
        private System.Windows.Forms.Timer timer_left;
        private System.Windows.Forms.Timer timer_right;
        private System.Windows.Forms.PictureBox coche2;
        private System.Windows.Forms.PictureBox coche3;
        private System.Windows.Forms.PictureBox coche4;
        private System.Windows.Forms.Timer timer_posicion;
    }
}

