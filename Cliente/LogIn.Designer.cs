
namespace EjGuia
{
    partial class LogIn
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
            this.usuarioTxt = new System.Windows.Forms.TextBox();
            this.contraseñaTxt = new System.Windows.Forms.TextBox();
            this.usuarioLbl = new System.Windows.Forms.Label();
            this.contraseñaLbl = new System.Windows.Forms.Label();
            this.registrarseBtn = new System.Windows.Forms.Button();
            this.loginBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // usuarioTxt
            // 
            this.usuarioTxt.Location = new System.Drawing.Point(186, 220);
            this.usuarioTxt.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.usuarioTxt.Name = "usuarioTxt";
            this.usuarioTxt.Size = new System.Drawing.Size(112, 26);
            this.usuarioTxt.TabIndex = 0;
            // 
            // contraseñaTxt
            // 
            this.contraseñaTxt.Location = new System.Drawing.Point(186, 270);
            this.contraseñaTxt.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.contraseñaTxt.Name = "contraseñaTxt";
            this.contraseñaTxt.Size = new System.Drawing.Size(112, 26);
            this.contraseñaTxt.TabIndex = 1;
            // 
            // usuarioLbl
            // 
            this.usuarioLbl.AutoSize = true;
            this.usuarioLbl.Location = new System.Drawing.Point(76, 224);
            this.usuarioLbl.Name = "usuarioLbl";
            this.usuarioLbl.Size = new System.Drawing.Size(64, 20);
            this.usuarioLbl.TabIndex = 2;
            this.usuarioLbl.Text = "Usuario";
            // 
            // contraseñaLbl
            // 
            this.contraseñaLbl.AutoSize = true;
            this.contraseñaLbl.Location = new System.Drawing.Point(65, 274);
            this.contraseñaLbl.Name = "contraseñaLbl";
            this.contraseñaLbl.Size = new System.Drawing.Size(92, 20);
            this.contraseñaLbl.TabIndex = 3;
            this.contraseñaLbl.Text = "Contraseña";
            // 
            // registrarseBtn
            // 
            this.registrarseBtn.Location = new System.Drawing.Point(171, 428);
            this.registrarseBtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.registrarseBtn.Name = "registrarseBtn";
            this.registrarseBtn.Size = new System.Drawing.Size(152, 29);
            this.registrarseBtn.TabIndex = 4;
            this.registrarseBtn.Text = "Registrarse";
            this.registrarseBtn.UseVisualStyleBackColor = true;
            this.registrarseBtn.Click += new System.EventHandler(this.registrarseBtn_Click);
            // 
            // loginBtn
            // 
            this.loginBtn.Location = new System.Drawing.Point(171, 366);
            this.loginBtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.loginBtn.Name = "loginBtn";
            this.loginBtn.Size = new System.Drawing.Size(152, 29);
            this.loginBtn.TabIndex = 5;
            this.loginBtn.Text = "Iniciar sesión";
            this.loginBtn.UseVisualStyleBackColor = true;
            this.loginBtn.Click += new System.EventHandler(this.loginBtn_Click);
            // 
            // LogIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 562);
            this.Controls.Add(this.loginBtn);
            this.Controls.Add(this.registrarseBtn);
            this.Controls.Add(this.contraseñaLbl);
            this.Controls.Add(this.usuarioLbl);
            this.Controls.Add(this.contraseñaTxt);
            this.Controls.Add(this.usuarioTxt);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "LogIn";
            this.Text = "LogIn";
            this.Load += new System.EventHandler(this.LogIn_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox usuarioTxt;
        private System.Windows.Forms.TextBox contraseñaTxt;
        private System.Windows.Forms.Label usuarioLbl;
        private System.Windows.Forms.Label contraseñaLbl;
        private System.Windows.Forms.Button registrarseBtn;
        private System.Windows.Forms.Button loginBtn;
    }
}