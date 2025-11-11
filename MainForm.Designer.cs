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
            gbPoder.Location = new Point(17, 20);
            gbPoder.Margin = new Padding(4, 5, 4, 5);
            gbPoder.Name = "gbPoder";
            gbPoder.Padding = new Padding(4, 5, 4, 5);
            gbPoder.Size = new Size(1086, 100);
            gbPoder.TabIndex = 0;
            gbPoder.TabStop = false;
            gbPoder.Text = "Nivel de Poder";
            // 
            // rbMon
            // 
            rbMon.AutoSize = true;
            rbMon.Location = new Point(900, 42);
            rbMon.Margin = new Padding(4, 5, 4, 5);
            rbMon.Name = "rbMon";
            rbMon.Size = new Size(143, 29);
            rbMon.TabIndex = 5;
            rbMon.TabStop = true;
            rbMon.Text = "Nivel 6 (Mon)";
            rbMon.UseVisualStyleBackColor = true;
            // 
            // rbPal
            // 
            rbPal.AutoSize = true;
            rbPal.Location = new Point(729, 42);
            rbPal.Margin = new Padding(4, 5, 4, 5);
            rbPal.Name = "rbPal";
            rbPal.Size = new Size(128, 29);
            rbPal.TabIndex = 4;
            rbPal.TabStop = true;
            rbPal.Text = "Nivel 5 (Pal)";
            rbPal.UseVisualStyleBackColor = true;
            // 
            // rbEe
            // 
            rbEe.AutoSize = true;
            rbEe.Location = new Point(557, 42);
            rbEe.Margin = new Padding(4, 5, 4, 5);
            rbEe.Name = "rbEe";
            rbEe.Size = new Size(124, 29);
            rbEe.TabIndex = 3;
            rbEe.TabStop = true;
            rbEe.Text = "Nivel 4 (Ee)";
            rbEe.UseVisualStyleBackColor = true;
            // 
            // rbOn
            // 
            rbOn.AutoSize = true;
            rbOn.Location = new Point(386, 42);
            rbOn.Margin = new Padding(4, 5, 4, 5);
            rbOn.Name = "rbOn";
            rbOn.Size = new Size(130, 29);
            rbOn.TabIndex = 2;
            rbOn.TabStop = true;
            rbOn.Text = "Nivel 3 (On)";
            rbOn.UseVisualStyleBackColor = true;
            // 
            // rbUm
            // 
            rbUm.AutoSize = true;
            rbUm.Location = new Point(214, 42);
            rbUm.Margin = new Padding(4, 5, 4, 5);
            rbUm.Name = "rbUm";
            rbUm.Size = new Size(134, 29);
            rbUm.TabIndex = 1;
            rbUm.TabStop = true;
            rbUm.Text = "Nivel 2 (Um)";
            rbUm.UseVisualStyleBackColor = true;
            // 
            // rbLo
            // 
            rbLo.AutoSize = true;
            rbLo.Checked = true;
            rbLo.Location = new Point(43, 42);
            rbLo.Margin = new Padding(4, 5, 4, 5);
            rbLo.Name = "rbLo";
            rbLo.Size = new Size(125, 29);
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
            gbSeleccion.Location = new Point(17, 130);
            gbSeleccion.Margin = new Padding(4, 5, 4, 5);
            gbSeleccion.Name = "gbSeleccion";
            gbSeleccion.Padding = new Padding(4, 5, 4, 5);
            gbSeleccion.Size = new Size(1086, 217);
            gbSeleccion.TabIndex = 1;
            gbSeleccion.TabStop = false;
            gbSeleccion.Text = "Selección de Hechizo";
            // 
            // btnMostrar
            // 
            btnMostrar.Anchor = AnchorStyles.Top;
            btnMostrar.BackColor = Color.Yellow;
            btnMostrar.ForeColor = Color.Blue;
            btnMostrar.Location = new Point(486, 158);
            btnMostrar.Margin = new Padding(4, 5, 4, 5);
            btnMostrar.Name = "btnMostrar";
            btnMostrar.Size = new Size(177, 49);
            btnMostrar.TabIndex = 4;
            btnMostrar.Text = "Mostrar hechizo";
            btnMostrar.UseVisualStyleBackColor = false;
            btnMostrar.Click += btnMostrar_Click;
            // 
            // cbHechizo
            // 
            cbHechizo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbHechizo.DropDownStyle = ComboBoxStyle.DropDownList;
            cbHechizo.Location = new Point(286, 100);
            cbHechizo.Margin = new Padding(4, 5, 4, 5);
            cbHechizo.Name = "cbHechizo";
            cbHechizo.Size = new Size(513, 33);
            cbHechizo.TabIndex = 3;
            cbHechizo.SelectedIndexChanged += cbHechizo_SelectedIndexChanged;
            // 
            // lblHechizo
            // 
            lblHechizo.AutoSize = true;
            lblHechizo.Location = new Point(114, 105);
            lblHechizo.Margin = new Padding(4, 0, 4, 0);
            lblHechizo.Name = "lblHechizo";
            lblHechizo.Size = new Size(163, 25);
            lblHechizo.TabIndex = 2;
            lblHechizo.Text = "Selecciona hechizo:";
            // 
            // cbClase
            // 
            cbClase.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbClase.DropDownStyle = ComboBoxStyle.DropDownList;
            cbClase.Location = new Point(286, 47);
            cbClase.Margin = new Padding(4, 5, 4, 5);
            cbClase.Name = "cbClase";
            cbClase.Size = new Size(513, 33);
            cbClase.TabIndex = 1;
            cbClase.SelectedIndexChanged += cbClase_SelectedIndexChanged;
            // 
            // lblClase
            // 
            lblClase.AutoSize = true;
            lblClase.Location = new Point(114, 52);
            lblClase.Margin = new Padding(4, 0, 4, 0);
            lblClase.Name = "lblClase";
            lblClase.Size = new Size(141, 25);
            lblClase.TabIndex = 0;
            lblClase.Text = "Selecciona clase:";
            // 
            // lblResultado
            // 
            lblResultado.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblResultado.Location = new Point(17, 363);
            lblResultado.Margin = new Padding(4, 0, 4, 0);
            lblResultado.Name = "lblResultado";
            lblResultado.Size = new Size(1086, 150);
            lblResultado.TabIndex = 2;
            lblResultado.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlDetalles
            // 
            pnlDetalles.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlDetalles.AutoSize = true;
            pnlDetalles.FlowDirection = FlowDirection.TopDown;
            pnlDetalles.Location = new Point(17, 518);
            pnlDetalles.Margin = new Padding(4, 5, 4, 5);
            pnlDetalles.Name = "pnlDetalles";
            pnlDetalles.Size = new Size(1086, 17);
            pnlDetalles.TabIndex = 3;
            pnlDetalles.WrapContents = false;
            // 
            // pnlSimbolos
            // 
            pnlSimbolos.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlSimbolos.AutoSize = true;
            pnlSimbolos.Location = new Point(17, 600);
            pnlSimbolos.Margin = new Padding(4, 5, 4, 5);
            pnlSimbolos.Name = "pnlSimbolos";
            pnlSimbolos.Size = new Size(1086, 17);
            pnlSimbolos.TabIndex = 4;
            // 
            // picFrasco
            // 
            picFrasco.Anchor = AnchorStyles.Top;
            picFrasco.Location = new Point(511, 658);
            picFrasco.Margin = new Padding(4, 5, 4, 5);
            picFrasco.Name = "picFrasco";
            picFrasco.Size = new Size(91, 107);
            picFrasco.SizeMode = PictureBoxSizeMode.Zoom;
            picFrasco.TabIndex = 5;
            picFrasco.TabStop = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1120, 802);
            Controls.Add(picFrasco);
            Controls.Add(pnlSimbolos);
            Controls.Add(pnlDetalles);
            Controls.Add(lblResultado);
            Controls.Add(gbSeleccion);
            Controls.Add(gbPoder);
            Margin = new Padding(4, 5, 4, 5);
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
