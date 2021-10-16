
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
            this.posicionMedia = new System.Windows.Forms.RadioButton();
            this.jugMasVeces = new System.Windows.Forms.RadioButton();
            this.nombreTxt = new System.Windows.Forms.TextBox();
            this.conectar = new System.Windows.Forms.Button();
            this.numVictorias = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // posicionMedia
            // 
            this.posicionMedia.AutoSize = true;
            this.posicionMedia.Location = new System.Drawing.Point(163, 239);
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
            this.jugMasVeces.Location = new System.Drawing.Point(163, 289);
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
            this.nombreTxt.Location = new System.Drawing.Point(174, 166);
            this.nombreTxt.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nombreTxt.Name = "nombreTxt";
            this.nombreTxt.Size = new System.Drawing.Size(112, 26);
            this.nombreTxt.TabIndex = 2;
            // 
            // conectar
            // 
            this.conectar.Location = new System.Drawing.Point(174, 408);
            this.conectar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.conectar.Name = "conectar";
            this.conectar.Size = new System.Drawing.Size(84, 29);
            this.conectar.TabIndex = 3;
            this.conectar.Text = "Conectar";
            this.conectar.UseVisualStyleBackColor = true;
            this.conectar.Click += new System.EventHandler(this.conectar_Click);
            // 
            // numVictorias
            // 
            this.numVictorias.AutoSize = true;
            this.numVictorias.Location = new System.Drawing.Point(163, 334);
            this.numVictorias.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numVictorias.Name = "numVictorias";
            this.numVictorias.Size = new System.Drawing.Size(173, 24);
            this.numVictorias.TabIndex = 4;
            this.numVictorias.TabStop = true;
            this.numVictorias.Text = "Número de victorias";
            this.numVictorias.UseVisualStyleBackColor = true;
            // 
            // Inicio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 562);
            this.Controls.Add(this.numVictorias);
            this.Controls.Add(this.conectar);
            this.Controls.Add(this.nombreTxt);
            this.Controls.Add(this.jugMasVeces);
            this.Controls.Add(this.posicionMedia);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Inicio";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton posicionMedia;
        private System.Windows.Forms.RadioButton jugMasVeces;
        private System.Windows.Forms.TextBox nombreTxt;
        private System.Windows.Forms.Button conectar;
        private System.Windows.Forms.RadioButton numVictorias;
    }
}

