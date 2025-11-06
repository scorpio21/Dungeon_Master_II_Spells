using System.Windows.Forms;

namespace SpellBookWinForms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private GroupBox gbPoder;
        private RadioButton rbLo;
        private RadioButton rbUm;
        private RadioButton rbOn;
        private RadioButton rbEe;
        private RadioButton rbPal;
        private RadioButton rbMon;
        private GroupBox gbSeleccion;
        private ComboBox cbClase;
        private ComboBox cbHechizo;
        private Label lblClase;
        private Label lblHechizo;
        private Button btnMostrar;
        private Label lblResultado;
        private FlowLayoutPanel pnlDetalles;
        private FlowLayoutPanel pnlSimbolos;
        private PictureBox picFrasco;
        private ToolTip toolTip1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            gbPoder = new GroupBox();
            rbMon = new RadioButton();
            rbPal = new RadioButton();
            rbEe = new RadioButton();
            rbOn = new RadioButton();
            rbUm = new RadioButton();
            rbLo = new RadioButton();
            gbSeleccion = new GroupBox();
            btnMostrar = new Button();
            cbHechizo = new ComboBox();
            lblHechizo = new Label();
            cbClase = new ComboBox();
            lblClase = new Label();
            lblResultado = new Label();
            pnlDetalles = new FlowLayoutPanel();
            pnlSimbolos = new FlowLayoutPanel();
            picFrasco = new PictureBox();
            toolTip1 = new ToolTip(components);
            gbPoder.SuspendLayout();
            gbSeleccion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picFrasco).BeginInit();
            SuspendLayout();
            // 
            // gbPoder
            // 
            gbPoder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbPoder.Controls.Add(rbMon);
            gbPoder.Controls.Add(rbPal);
            gbPoder.Controls.Add(rbEe);
            gbPoder.Controls.Add(rbOn);
            gbPoder.Controls.Add(rbUm);
            gbPoder.Controls.Add(rbLo);
            gbPoder.Location = new System.Drawing.Point(12, 12);
            gbPoder.Name = "gbPoder";
            gbPoder.Size = new System.Drawing.Size(760, 60);
            gbPoder.TabIndex = 0;
            gbPoder.TabStop = false;
            gbPoder.Text = "Nivel de Poder";
            // 
            // rbMon
            // 
            rbMon.AutoSize = true;
            rbMon.Location = new System.Drawing.Point(630, 25);
            rbMon.Name = "rbMon";
            rbMon.Size = new System.Drawing.Size(90, 19);
            rbMon.TabIndex = 5;
            rbMon.TabStop = true;
            rbMon.Text = "Nivel 6 (Mon)";
            rbMon.UseVisualStyleBackColor = true;
            // 
            // rbPal
            // 
            rbPal.AutoSize = true;
            rbPal.Location = new System.Drawing.Point(510, 25);
            rbPal.Name = "rbPal";
            rbPal.Size = new System.Drawing.Size(90, 19);
            rbPal.TabIndex = 4;
            rbPal.TabStop = true;
            rbPal.Text = "Nivel 5 (Pal)";
            rbPal.UseVisualStyleBackColor = true;
            // 
            // rbEe
            // 
            rbEe.AutoSize = true;
            rbEe.Location = new System.Drawing.Point(390, 25);
            rbEe.Name = "rbEe";
            rbEe.Size = new System.Drawing.Size(88, 19);
            rbEe.TabIndex = 3;
            rbEe.TabStop = true;
            rbEe.Text = "Nivel 4 (Ee)";
            rbEe.UseVisualStyleBackColor = true;
            // 
            // rbOn
            // 
            rbOn.AutoSize = true;
            rbOn.Location = new System.Drawing.Point(270, 25);
            rbOn.Name = "rbOn";
            rbOn.Size = new System.Drawing.Size(94, 19);
            rbOn.TabIndex = 2;
            rbOn.TabStop = true;
            rbOn.Text = "Nivel 3 (On)";
            rbOn.UseVisualStyleBackColor = true;
            // 
            // rbUm
            // 
            rbUm.AutoSize = true;
            rbUm.Location = new System.Drawing.Point(150, 25);
            rbUm.Name = "rbUm";
            rbUm.Size = new System.Drawing.Size(97, 19);
            rbUm.TabIndex = 1;
            rbUm.TabStop = true;
            rbUm.Text = "Nivel 2 (Um)";
            rbUm.UseVisualStyleBackColor = true;
            // 
            // rbLo
            // 
            rbLo.AutoSize = true;
            rbLo.Checked = true;
            rbLo.Location = new System.Drawing.Point(30, 25);
            rbLo.Name = "rbLo";
            rbLo.Size = new System.Drawing.Size(91, 19);
            rbLo.TabIndex = 0;
            rbLo.TabStop = true;
            rbLo.Text = "Nivel 1 (Lo)";
            rbLo.UseVisualStyleBackColor = true;
            // 
            // gbSeleccion
            // 
            gbSeleccion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbSeleccion.Controls.Add(btnMostrar);
            gbSeleccion.Controls.Add(cbHechizo);
            gbSeleccion.Controls.Add(lblHechizo);
            gbSeleccion.Controls.Add(cbClase);
            gbSeleccion.Controls.Add(lblClase);
            gbSeleccion.Location = new System.Drawing.Point(12, 78);
            gbSeleccion.Name = "gbSeleccion";
            gbSeleccion.Size = new System.Drawing.Size(760, 130);
            gbSeleccion.TabIndex = 1;
            gbSeleccion.TabStop = false;
            gbSeleccion.Text = "Selección de Hechizo";
            // 
            // btnMostrar
            // 
            btnMostrar.Anchor = AnchorStyles.Top;
            btnMostrar.Location = new System.Drawing.Point(340, 95);
            btnMostrar.Name = "btnMostrar";
            btnMostrar.Size = new System.Drawing.Size(100, 27);
            btnMostrar.TabIndex = 4;
            btnMostrar.Text = "Mostrar hechizo";
            btnMostrar.UseVisualStyleBackColor = true;
            btnMostrar.Click += btnMostrar_Click;
            // 
            // cbHechizo
            // 
            cbHechizo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbHechizo.DropDownStyle = ComboBoxStyle.DropDownList;
            cbHechizo.Location = new System.Drawing.Point(200, 60);
            cbHechizo.Name = "cbHechizo";
            cbHechizo.Size = new System.Drawing.Size(360, 23);
            cbHechizo.TabIndex = 3;
            cbHechizo.SelectedIndexChanged += cbHechizo_SelectedIndexChanged;
            // 
            // lblHechizo
            // 
            lblHechizo.AutoSize = true;
            lblHechizo.Location = new System.Drawing.Point(80, 63);
            lblHechizo.Name = "lblHechizo";
            lblHechizo.Size = new System.Drawing.Size(97, 15);
            lblHechizo.TabIndex = 2;
            lblHechizo.Text = "Selecciona hechizo:";
            // 
            // cbClase
            // 
            cbClase.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbClase.DropDownStyle = ComboBoxStyle.DropDownList;
            cbClase.Location = new System.Drawing.Point(200, 28);
            cbClase.Name = "cbClase";
            cbClase.Size = new System.Drawing.Size(360, 23);
            cbClase.TabIndex = 1;
            cbClase.SelectedIndexChanged += cbClase_SelectedIndexChanged;
            // 
            // lblClase
            // 
            lblClase.AutoSize = true;
            lblClase.Location = new System.Drawing.Point(80, 31);
            lblClase.Name = "lblClase";
            lblClase.Size = new System.Drawing.Size(90, 15);
            lblClase.TabIndex = 0;
            lblClase.Text = "Selecciona clase:";
            // 
            // lblResultado
            // 
            lblResultado.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblResultado.Location = new System.Drawing.Point(12, 218);
            lblResultado.Name = "lblResultado";
            lblResultado.Size = new System.Drawing.Size(760, 90);
            lblResultado.TabIndex = 2;
            lblResultado.Text = "";
            lblResultado.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlDetalles
            // 
            pnlDetalles.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlDetalles.AutoSize = true;
            pnlDetalles.FlowDirection = FlowDirection.TopDown;
            pnlDetalles.Location = new System.Drawing.Point(12, 311);
            pnlDetalles.Name = "pnlDetalles";
            pnlDetalles.Size = new System.Drawing.Size(760, 10);
            pnlDetalles.TabIndex = 3;
            pnlDetalles.WrapContents = false;
            // 
            // pnlSimbolos
            // 
            pnlSimbolos.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlSimbolos.AutoSize = true;
            pnlSimbolos.Location = new System.Drawing.Point(12, 360);
            pnlSimbolos.Name = "pnlSimbolos";
            pnlSimbolos.Size = new System.Drawing.Size(760, 10);
            pnlSimbolos.TabIndex = 4;
            // 
            // picFrasco
            // 
            picFrasco.Anchor = AnchorStyles.Top;
            picFrasco.Location = new System.Drawing.Point(358, 395);
            picFrasco.Name = "picFrasco";
            picFrasco.Size = new System.Drawing.Size(64, 64);
            picFrasco.SizeMode = PictureBoxSizeMode.Zoom;
            picFrasco.TabIndex = 5;
            picFrasco.TabStop = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(784, 481);
            Controls.Add(picFrasco);
            Controls.Add(pnlSimbolos);
            Controls.Add(pnlDetalles);
            Controls.Add(lblResultado);
            Controls.Add(gbSeleccion);
            Controls.Add(gbPoder);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "📜 Grimorio de Hechizos - Dungeon Master II";
            gbPoder.ResumeLayout(false);
            gbPoder.PerformLayout();
            gbSeleccion.ResumeLayout(false);
            gbSeleccion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picFrasco).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
