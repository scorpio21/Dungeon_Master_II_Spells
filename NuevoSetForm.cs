using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using SpellBookWinForms.Properties;

namespace SpellBookWinForms
{
    public class NuevoSetForm : Form
    {
        private readonly bool _en = string.Equals(Settings.Default.Idioma, "EN", StringComparison.OrdinalIgnoreCase);

        private TextBox txtId = null!;
        private TextBox txtNombreEs = null!;
        private TextBox txtNombreEn = null!;
        private TextBox txtTipoEs = null!;
        private TextBox txtTipoEn = null!;
        private TextBox txtImagenSet = null!;

        private readonly List<PiezaRow> _piezasBasicas = new();
        private readonly List<PiezaRow> _piezasExtras = new();

        private FlowLayoutPanel panelExtras = null!;

        private sealed class TextoLocalizado
        {
            [JsonPropertyName("es")] public string Es { get; set; } = string.Empty;
            [JsonPropertyName("en")] public string En { get; set; } = string.Empty;
        }

        private sealed class PiezaRow
        {
            public string TipoFijo { get; }
            public ComboBox CbTipo { get; }
            public TextBox TxtNombreEs { get; }
            public TextBox TxtNombreEn { get; }
            public TextBox TxtImagen { get; }

            public PiezaRow(string tipoFijo, ComboBox cbTipo, TextBox txtEs, TextBox txtEn, TextBox txtImg)
            {
                TipoFijo = tipoFijo;
                CbTipo = cbTipo;
                TxtNombreEs = txtEs;
                TxtNombreEn = txtEn;
                TxtImagen = txtImg;
            }
        }

        public NuevoSetForm()
        {
            Text = _en ? "New armor set" : "Nuevo set de armadura";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(820, 640);

            // Menú principal
            var menu = new MenuStrip();

            // Menú Archivo / File (izquierda)
            var archivo = new ToolStripMenuItem(_en ? "File" : "Archivo")
            {
                Alignment = ToolStripItemAlignment.Left
            };
            var itemSalir = new ToolStripMenuItem(_en ? "Exit" : "Salir");
            itemSalir.Click += (_, __) => Close();
            archivo.DropDownItems.Add(itemSalir);

            // Menú Ayuda / Help (alineado a la derecha)
            var ayuda = new ToolStripMenuItem(_en ? "Help" : "Ayuda")
            {
                Alignment = ToolStripItemAlignment.Right
            };
            var itemComo = new ToolStripMenuItem(_en ? "How to add a new set" : "Cómo agregar un nuevo set");
            itemComo.Click += (_, __) => MostrarAyuda();
            ayuda.DropDownItems.Add(itemComo);

            menu.Items.Add(archivo);
            menu.Items.Add(ayuda);
            MainMenuStrip = menu;
            Controls.Add(menu);

            var panelMain = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
            };
            panelMain.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panelMain.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            panelMain.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Datos generales del set
            var panelDatos = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 4,
                AutoSize = true,
                Padding = new Padding(8)
            };
            panelDatos.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panelDatos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            panelDatos.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panelDatos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            txtId = new TextBox { Width = 200 };
            txtNombreEs = new TextBox { Width = 200 };
            txtNombreEn = new TextBox { Width = 200 };
            txtTipoEs = new TextBox { Width = 200, Text = "Set de Armadura" };
            txtTipoEn = new TextBox { Width = 200, Text = "Armor Set" };
            txtImagenSet = new TextBox { Width = 200, ReadOnly = true };

            var btnImagenSet = new Button { Text = "Elegir imagen..." };
            btnImagenSet.Click += (_, __) => SeleccionarImagenEnTextBox(txtImagenSet);

            int fila = 0;
            panelDatos.Controls.Add(new Label { Text = "Id:", AutoSize = true }, 0, fila);
            panelDatos.Controls.Add(txtId, 1, fila);
            panelDatos.Controls.Add(new Label { Text = "Imagen set:", AutoSize = true }, 2, fila);
            panelDatos.Controls.Add(CrearPanelImagen(txtImagenSet, btnImagenSet), 3, fila);
            fila++;

            panelDatos.Controls.Add(new Label { Text = "Nombre (ES):", AutoSize = true }, 0, fila);
            panelDatos.Controls.Add(txtNombreEs, 1, fila);
            panelDatos.Controls.Add(new Label { Text = "Nombre (EN):", AutoSize = true }, 2, fila);
            panelDatos.Controls.Add(txtNombreEn, 3, fila);
            fila++;

            panelDatos.Controls.Add(new Label { Text = "Tipo (ES):", AutoSize = true }, 0, fila);
            panelDatos.Controls.Add(txtTipoEs, 1, fila);
            panelDatos.Controls.Add(new Label { Text = "Tipo (EN):", AutoSize = true }, 2, fila);
            panelDatos.Controls.Add(txtTipoEn, 3, fila);

            // Piezas básicas
            var panelBasicas = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 5,
                Padding = new Padding(8)
            };
            panelBasicas.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panelBasicas.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panelBasicas.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panelBasicas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            panelBasicas.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            panelBasicas.Controls.Add(new Label { Text = "Tipo", AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 0);
            panelBasicas.Controls.Add(new Label { Text = "Nombre (ES)", AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 1, 0);
            panelBasicas.Controls.Add(new Label { Text = "Nombre (EN)", AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 2, 0);
            panelBasicas.Controls.Add(new Label { Text = "Imagen", AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 3, 0);

            string[] tiposBasicos = { "head", "torso", "legs", "feet", "shield", "weapon" };
            for (int i = 0; i < tiposBasicos.Length; i++)
            {
                var tipo = tiposBasicos[i];
                var cbTipo = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Width = 90
                };
                cbTipo.Items.Add(tipo);
                cbTipo.SelectedIndex = 0;

                var txtEs = new TextBox { Width = 160 };
                var txtEn = new TextBox { Width = 160 };
                var txtImg = new TextBox { Width = 200, ReadOnly = true };
                var btnImg = new Button { Text = "..." };
                btnImg.Click += (_, __) => SeleccionarImagenEnTextBox(txtImg);

                int row = i + 1;
                panelBasicas.Controls.Add(cbTipo, 0, row);
                panelBasicas.Controls.Add(txtEs, 1, row);
                panelBasicas.Controls.Add(txtEn, 2, row);
                panelBasicas.Controls.Add(CrearPanelImagen(txtImg, btnImg), 3, row);

                _piezasBasicas.Add(new PiezaRow(tipo, cbTipo, txtEs, txtEn, txtImg));
            }

            // Extras
            panelExtras = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                Padding = new Padding(8)
            };

            var btnAgregarExtra = new Button
            {
                Text = "Añadir extra",
                AutoSize = true,
                Margin = new Padding(8)
            };
            btnAgregarExtra.Click += (_, __) => AgregarFilaExtra();

            panelExtras.Controls.Add(btnAgregarExtra);

            // Botones inferiores
            var panelInferior = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Padding = new Padding(8)
            };

            var btnCerrar = new Button { Text = "Cerrar", AutoSize = true };
            btnCerrar.Click += (_, __) => Close();

            var btnGenerarJson = new Button { Text = "Generar JSON", AutoSize = true };
            btnGenerarJson.Click += (_, __) => GenerarJson();

            panelInferior.Controls.Add(btnCerrar);
            panelInferior.Controls.Add(btnGenerarJson);

            panelMain.Controls.Add(panelDatos, 0, 0);
            panelMain.Controls.Add(panelBasicas, 0, 1);
            panelMain.Controls.Add(panelExtras, 0, 2);
            panelMain.Controls.Add(panelInferior, 0, 3);

            Controls.Add(panelMain);
        }

        private static Control CrearPanelImagen(TextBox txt, Button btn)
        {
            var p = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true
            };
            p.Controls.Add(txt);
            p.Controls.Add(btn);
            return p;
        }

        private void SeleccionarImagenEnTextBox(TextBox destino)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Selecciona imagen",
                Filter = "Imagenes|*.png;*.gif;*.jpg;*.jpeg;*.bmp",
                InitialDirectory = Directory.GetCurrentDirectory()
            };

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                destino.Text = Path.GetFileName(dlg.FileName);
            }
        }

        private void AgregarFilaExtra()
        {
            var fila = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 0)
            };

            var cbTipo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 100
            };
            cbTipo.Items.AddRange(new object[] { "amulet", "ring", "necklace", "arrows" });
            if (cbTipo.Items.Count > 0) cbTipo.SelectedIndex = 0;

            var txtEs = new TextBox { Width = 140 };
            var txtEn = new TextBox { Width = 140 };
            var txtImg = new TextBox { Width = 180, ReadOnly = true };
            var btnImg = new Button { Text = "..." };
            btnImg.Click += (_, __) => SeleccionarImagenEnTextBox(txtImg);

            fila.Controls.Add(new Label { Text = "Tipo:", AutoSize = true, Margin = new Padding(0, 6, 4, 0) });
            fila.Controls.Add(cbTipo);
            fila.Controls.Add(new Label { Text = "ES:", AutoSize = true, Margin = new Padding(8, 6, 4, 0) });
            fila.Controls.Add(txtEs);
            fila.Controls.Add(new Label { Text = "EN:", AutoSize = true, Margin = new Padding(8, 6, 4, 0) });
            fila.Controls.Add(txtEn);
            fila.Controls.Add(CrearPanelImagen(txtImg, btnImg));

            panelExtras.Controls.Add(fila);
            _piezasExtras.Add(new PiezaRow(string.Empty, cbTipo, txtEs, txtEn, txtImg));
        }

        private void GenerarJson()
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Falta el Id del set", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  \"id\": \"{txtId.Text}\",");
            sb.AppendLine($"  \"nombre\": {{ \"es\": \"{txtNombreEs.Text}\", \"en\": \"{txtNombreEn.Text}\" }},");
            sb.AppendLine($"  \"tipo\": {{ \"es\": \"{txtTipoEs.Text}\", \"en\": \"{txtTipoEn.Text}\" }},");
            sb.AppendLine($"  \"imagen\": \"{txtImagenSet.Text}\",");
            sb.AppendLine("  \"piezas\": [");

            var piezas = new List<PiezaRow>();
            piezas.AddRange(_piezasBasicas);
            piezas.AddRange(_piezasExtras);

            for (int i = 0; i < piezas.Count; i++)
            {
                var p = piezas[i];
                string tipo = string.IsNullOrWhiteSpace(p.TipoFijo) ? p.CbTipo.SelectedItem?.ToString() ?? string.Empty : p.TipoFijo;
                string coma = i < piezas.Count - 1 ? "," : string.Empty;

                sb.AppendLine(
                    $"    {{ \"tipo\": \"{tipo}\", \"imagen\": \"{p.TxtImagen.Text}\", \"nombre\": {{ \"es\": \"{p.TxtNombreEs.Text}\", \"en\": \"{p.TxtNombreEn.Text}\" }} }}{coma}");
            }

            sb.AppendLine("  ]");
            sb.AppendLine("}");

            using var dlg = new Form
            {
                Text = "JSON generado",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(640, 480)
            };

            var txtSalida = new TextBox
            {
                Multiline = true,
                ReadOnly = false,
                ScrollBars = ScrollBars.Both,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10f),
                Text = sb.ToString()
            };

            dlg.Controls.Add(txtSalida);
            dlg.ShowDialog(this);
        }

        private void MostrarAyuda()
        {
            string mensajeEs =
                "Guía rápida para crear un nuevo set" + "\n" +
                "-----------------------------------" + "\n\n" +
                "1. Datos generales" + "\n" +
                "   • Id: escribe un identificador único (ejemplo: fire_set_2)." + "\n" +
                "   • Nombre: indica el nombre del set en español e inglés." + "\n" +
                "   • Tipo: puedes dejar el valor por defecto ('Set de Armadura' / 'Armor Set')." + "\n" +
                "   • Imagen del set: pulsa 'Elegir imagen...' y selecciona la imagen principal." + "\n\n" +
                "2. Piezas básicas (head, torso, legs, feet, shield, weapon)" + "\n" +
                "   • Para cada tipo, escribe el nombre en ES/EN." + "\n" +
                "   • Elige la imagen correspondiente en tu carpeta de recursos." + "\n\n" +
                "3. Piezas extra (opcional)" + "\n" +
                "   • Pulsa 'Añadir extra' para crear una nueva fila." + "\n" +
                "   • Elige el tipo (amulet, ring, necklace, arrows, etc.)." + "\n" +
                "   • Escribe los nombres en español e inglés y selecciona la imagen." + "\n\n" +
                "4. Generar el JSON" + "\n" +
                "   • Cuando hayas completado todos los datos, pulsa 'Generar JSON'." + "\n" +
                "   • Se abrirá una ventana con el bloque JSON completo del set." + "\n\n" +
                "5. Guardar el JSON en el proyecto" + "\n" +
                "   • Copia el texto generado (Ctrl+C)." + "\n" +
                "   • Abre data/objetos_sets.json y pégalo dentro del array 'sets'." + "\n" +
                "   • Guarda el archivo y vuelve a abrir el formulario de sets para ver el nuevo set.";

            string mensajeEn =
                "Quick guide to create a new set" + "\n" +
                "--------------------------------" + "\n\n" +
                "1. General data" + "\n" +
                "   • Id: enter a unique identifier (for example: fire_set_2)." + "\n" +
                "   • Name: provide the set name in Spanish and English." + "\n" +
                "   • Type: you can keep the default value ('Set de Armadura' / 'Armor Set')." + "\n" +
                "   • Set image: click 'Elegir imagen...' and choose the main image." + "\n\n" +
                "2. Basic pieces (head, torso, legs, feet, shield, weapon)" + "\n" +
                "   • For each type, enter the name in ES/EN." + "\n" +
                "   • Choose the corresponding image from your resources folder." + "\n\n" +
                "3. Extra pieces (optional)" + "\n" +
                "   • Click 'Añadir extra' to add a new row." + "\n" +
                "   • Select the type (amulet, ring, necklace, arrows, etc.)." + "\n" +
                "   • Enter ES/EN names and select the image." + "\n\n" +
                "4. Generate the JSON" + "\n" +
                "   • When everything is filled, click 'Generar JSON'." + "\n" +
                "   • A window will appear with the full JSON block for the set." + "\n\n" +
                "5. Save the JSON into the project" + "\n" +
                "   • Copy the generated text (Ctrl+C)." + "\n" +
                "   • Open data/objetos_sets.json and paste it inside the 'sets' array." + "\n" +
                "   • Save the file and reopen the sets form to see the new set.";

            MessageBox.Show(_en ? mensajeEn : mensajeEs,
                _en ? "Help" : "Ayuda",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
