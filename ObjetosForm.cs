using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using SpellBookWinForms.Properties;

namespace SpellBookWinForms
{
    public class ObjetosForm : Form
    {
        private TextBox txtBuscar = null!;
        private ComboBox cbTipo = null!;
        private CheckBox chkSoloSets = null!;
        private ListBox lstObjetos = null!;
        private PictureBox picObjeto = null!;
        private Panel panelSet = null!;
        private TableLayoutPanel tablaSet = null!;
        private PictureBox pbSetHead = null!;
        private PictureBox pbSetTorso = null!;
        private PictureBox pbSetLegs = null!;
        private PictureBox pbSetFeet = null!;
        private PictureBox pbSetLeft = null!;
        private PictureBox pbSetRight = null!;
        private Label lblNombre = null!;
        private Label lblTipo = null!;
        private Label lblStats = null!;
        private Label lblEfecto = null!;
        private Button btnCerrar = null!;

        private readonly bool _en = string.Equals(Settings.Default.Idioma, "EN", StringComparison.OrdinalIgnoreCase);

        private List<Arma> _todas = new();

        private sealed class TextoLocalizado
        {
            [JsonPropertyName("es")] public string Es { get; set; } = string.Empty;
            [JsonPropertyName("en")] public string En { get; set; } = string.Empty;
            public string Get(bool en) => en ? En : Es;
        }

        private void CargarImagenEn(PictureBox destino, string nombreArchivo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombreArchivo))
                {
                    Imagenes.ReemplazarImagen(destino, null);
                    return;
                }

                var baseDir = Path.Combine(Imagenes.BaseImgPath(), "objetos", "Armas");
                string[] extensiones = { ".png", ".gif", ".jpg", ".jpeg", ".bmp" };
                string? ruta = Imagenes.BuscarImagen(baseDir, nombreArchivo, extensiones);
                var img = ruta != null ? Imagenes.CargarImagenSegura(ruta) : null;
                Imagenes.ReemplazarImagen(destino, img);
            }
            catch
            {
                Imagenes.ReemplazarImagen(destino, null);
            }
        }

        private sealed class Arma
        {
            [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
            [JsonPropertyName("nombre")] public TextoLocalizado Nombre { get; set; } = new();
            [JsonPropertyName("tipo")] public TextoLocalizado Tipo { get; set; } = new();
            [JsonPropertyName("peso")] public double Peso { get; set; }
            [JsonPropertyName("valor")] public int Valor { get; set; }
            [JsonPropertyName("danioAtaque")] public int DanioAtaque { get; set; }
            [JsonPropertyName("danioLanzado")] public int? DanioLanzado { get; set; }
            [JsonPropertyName("acciones")] public List<string> Acciones { get; set; } = new();
            [JsonPropertyName("efecto")] public TextoLocalizado Efecto { get; set; } = new();
            [JsonPropertyName("imagen")] public string Imagen { get; set; } = string.Empty;
        }

        private sealed class RaizArmas
        {
            [JsonPropertyName("armas")] public List<Arma> Armas { get; set; } = new();
        }

        private sealed class ItemLista
        {
            public Arma Valor { get; }
            private readonly bool _en;
            public ItemLista(Arma arma, bool en) { Valor = arma; _en = en; }
            public override string ToString() => Valor.Nombre.Get(_en);
        }

        public ObjetosForm()
        {
            Text = _en ? "Weapons" : "Armas";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(960, 640);
            MinimizeBox = false;
            MaximizeBox = true;

            txtBuscar = new TextBox
            {
                PlaceholderText = _en ? "Search..." : "Buscar...",
                Dock = DockStyle.None,
                Margin = new Padding(6, 2, 6, 2),
                Width = 220
            };

            cbTipo = new ComboBox
            {
                Dock = DockStyle.None,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(8),
                Width = 200
            };

            chkSoloSets = new CheckBox
            {
                Dock = DockStyle.None,
                AutoSize = true,
                Margin = new Padding(8, 6, 0, 0),
                Text = _en ? "Sets only" : "Solo sets"
            };

            lstObjetos = new ListBox { Dock = DockStyle.Fill };

            var panelIzquierdo = new Panel { Dock = DockStyle.Left, Width = 380 };
            var panelFiltro = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 72,
                Padding = new Padding(8),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = false
            };

            var lblTipoFiltro = new Label
            {
                Text = _en ? "Type:" : "Tipo:",
                AutoSize = true,
                Margin = new Padding(0, 6, 6, 0)
            };

            btnCerrar = new Button
            {
                Text = _en ? "Close" : "Cerrar",
                Width = 90,
                Margin = new Padding(8, 2, 0, 2)
            };
            btnCerrar.Click += (_, __) => Close();

            panelFiltro.Controls.Add(lblTipoFiltro);
            panelFiltro.Controls.Add(cbTipo);
            panelFiltro.Controls.Add(chkSoloSets);
            panelFiltro.Controls.Add(txtBuscar);
            panelFiltro.Controls.Add(btnCerrar);

            panelIzquierdo.Controls.Add(lstObjetos);
            panelIzquierdo.Controls.Add(panelFiltro);

            var panelDerecho = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            lblNombre = new Label { AutoSize = true, Font = new Font("Segoe UI", 16, FontStyle.Bold) };
            lblTipo = new Label { AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Italic) };
            picObjeto = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = 64,
                Height = 64,
                BorderStyle = BorderStyle.FixedSingle
            };

            pbSetHead = CrearPictureBoxSet();
            pbSetTorso = CrearPictureBoxSet();
            pbSetLegs = CrearPictureBoxSet();
            pbSetFeet = CrearPictureBoxSet();
            pbSetLeft = CrearPictureBoxSet();
            pbSetRight = CrearPictureBoxSet();

            tablaSet = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 4,
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 4)
            };

            tablaSet.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tablaSet.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tablaSet.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tablaSet.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tablaSet.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tablaSet.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tablaSet.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Disposición: fila 0 centro = cabeza, fila 1: izquierda escudo, centro torso, derecha arma,
            // fila 2 centro = piernas, fila 3 centro = pies
            tablaSet.Controls.Add(pbSetHead, 1, 0);
            tablaSet.Controls.Add(pbSetLeft, 0, 1);
            tablaSet.Controls.Add(pbSetTorso, 1, 1);
            tablaSet.Controls.Add(pbSetRight, 2, 1);
            tablaSet.Controls.Add(pbSetLegs, 1, 2);
            tablaSet.Controls.Add(pbSetFeet, 1, 3);

            panelSet = new Panel
            {
                AutoSize = true,
                Visible = false
            };

            panelSet.Controls.Add(tablaSet);
            lblStats = new Label { AutoSize = true, Font = new Font("Segoe UI", 9), MaximumSize = new Size(560, 0) };
            lblEfecto = new Label { AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Italic), MaximumSize = new Size(560, 0) };

            var stackDerecha = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true
            };

            stackDerecha.Controls.Add(lblNombre);
            stackDerecha.Controls.Add(lblTipo);
            stackDerecha.Controls.Add(picObjeto);
            stackDerecha.Controls.Add(panelSet);
            stackDerecha.Controls.Add(lblStats);
            stackDerecha.Controls.Add(lblEfecto);

            panelDerecho.Controls.Add(stackDerecha);

            Controls.Add(panelDerecho);
            Controls.Add(panelIzquierdo);

            Load += (_, __) => CargarDatos();
            txtBuscar.TextChanged += (_, __) => RefrescarLista();
            cbTipo.SelectedIndexChanged += (_, __) => RefrescarLista();
            chkSoloSets.CheckedChanged += (_, __) => RefrescarLista();
            lstObjetos.SelectedIndexChanged += (_, __) => MostrarSeleccion();
        }

        private static bool EsSet(Arma arma)
        {
            if (arma == null || string.IsNullOrWhiteSpace(arma.Id)) return false;

            return arma.Id.StartsWith("fire_", StringComparison.OrdinalIgnoreCase)
                   || arma.Id.StartsWith("ra_sar_", StringComparison.OrdinalIgnoreCase)
                   || arma.Id.StartsWith("mithral_", StringComparison.OrdinalIgnoreCase)
                   || arma.Id.StartsWith("tech", StringComparison.OrdinalIgnoreCase);
        }

        private static string? PrefijoSet(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            if (id.StartsWith("fire_", StringComparison.OrdinalIgnoreCase)) return "fire_";
            if (id.StartsWith("ra_sar_", StringComparison.OrdinalIgnoreCase)) return "ra_sar_";
            if (id.StartsWith("mithral_", StringComparison.OrdinalIgnoreCase)) return "mithral_";
            if (id.StartsWith("tech", StringComparison.OrdinalIgnoreCase)) return "tech";

            return null;
        }

        private static bool EsParteNombre(string id, params string[] tokens)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;
            foreach (var t in tokens)
            {
                if (id.Contains(t, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        private PictureBox CrearPictureBoxSet()
        {
            return new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = 64,
                Height = 64,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(2)
            };
        }

        private void CargarDatos()
        {
            try
            {
                var ruta = Path.Combine(AppContext.BaseDirectory, "data", "objetos_armas.json");
                if (!File.Exists(ruta))
                {
                    var rutaAlt = Path.Combine(Directory.GetCurrentDirectory(), "data", "objetos_armas.json");
                    if (File.Exists(rutaAlt)) ruta = rutaAlt; else throw new FileNotFoundException(ruta);
                }

                using var fs = File.OpenRead(ruta);
                var raiz = JsonSerializer.Deserialize<RaizArmas>(fs, new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true
                });

                _todas = raiz?.Armas ?? new List<Arma>();

                var tipos = _todas
                    .Select(a => a.Tipo.Get(_en))
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();

                tipos.Insert(0, _en ? "All" : "Todos");
                cbTipo.Items.Clear();
                cbTipo.Items.AddRange(tipos.Cast<object>().ToArray());
                if (cbTipo.Items.Count > 0) cbTipo.SelectedIndex = 0;

                RefrescarLista();
            }
            catch (Exception ex)
            {
                MessageBox.Show((_en ? "Error loading weapons: " : "Error cargando armas: ") + ex.Message,
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefrescarLista()
        {
            var filtroTxt = (txtBuscar.Text ?? string.Empty).Trim().ToLowerInvariant();
            var filtroTipo = cbTipo.SelectedItem?.ToString();
            bool todos = string.IsNullOrWhiteSpace(filtroTipo) || filtroTipo == (_en ? "All" : "Todos");
            bool soloSets = chkSoloSets.Checked;

            var lista = _todas
                .Where(a => todos || string.Equals(a.Tipo.Get(_en), filtroTipo, StringComparison.OrdinalIgnoreCase))
                .Where(a => !soloSets || EsSet(a))
                .Where(a => string.IsNullOrWhiteSpace(filtroTxt) || a.Nombre.Get(_en).ToLowerInvariant().Contains(filtroTxt))
                .OrderBy(a => a.Nombre.Get(_en))
                .ToList();

            lstObjetos.BeginUpdate();
            lstObjetos.Items.Clear();
            foreach (var a in lista)
            {
                lstObjetos.Items.Add(new ItemLista(a, _en));
            }
            lstObjetos.EndUpdate();
            if (lstObjetos.Items.Count > 0) lstObjetos.SelectedIndex = 0;
        }

        private void MostrarSeleccion()
        {
            if (lstObjetos.SelectedItem is not ItemLista it) return;
            var a = it.Valor;

            lblNombre.Text = a.Nombre.Get(_en);
            lblTipo.Text = ($"{(_en ? "Type" : "Tipo")}: " + a.Tipo.Get(_en)).Trim();

            bool mostrarSet = chkSoloSets.Checked && EsSet(a);

            picObjeto.Visible = !mostrarSet;
            panelSet.Visible = mostrarSet;
            LimpiarPictureBoxSet();

            if (mostrarSet)
            {
                var prefijo = PrefijoSet(a.Id);
                if (!string.IsNullOrEmpty(prefijo))
                {
                    var miembros = _todas
                        .Where(x => x.Id.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase))
                        .OrderBy(x => x.Nombre.Get(_en))
                        .ToList();

                    foreach (var m in miembros)
                    {
                        // Casco
                        if (EsParteNombre(m.Id, "helm", "helmet"))
                        {
                            CargarImagenEn(pbSetHead, m.Imagen);
                        }
                        // Escudo
                        else if (EsParteNombre(m.Id, "shield"))
                        {
                            CargarImagenEn(pbSetLeft, m.Imagen);
                        }
                        // Botas / pies
                        else if (EsParteNombre(m.Id, "boot", "foot", "feet"))
                        {
                            CargarImagenEn(pbSetFeet, m.Imagen);
                        }
                        // Piernas / muslos
                        else if (EsParteNombre(m.Id, "greave", "poleyn", "hosen", "leg", "thigh"))
                        {
                            CargarImagenEn(pbSetLegs, m.Imagen);
                        }
                        // Torso
                        else if (EsParteNombre(m.Id, "plate", "mail", "hauberk", "jerkin", "brigand", "armor"))
                        {
                            CargarImagenEn(pbSetTorso, m.Imagen);
                        }
                        // Resto (arma u otro)
                        else
                        {
                            CargarImagenEn(pbSetRight, m.Imagen);
                        }
                    }
                }
            }
            else
            {
                CargarImagen(a.Imagen);
            }

            string pesoLabel = _en ? "Weight" : "Peso";
            string valorLabel = _en ? "Value" : "Valor";
            string danioAtkLabel = _en ? "Attack" : "Daño ataque";
            string danioThrowLabel = _en ? "Throw" : "Daño lanzamiento";

            var lineas = new List<string>
            {
                $"{pesoLabel}: {a.Peso:0.##}",
                $"{valorLabel}: {a.Valor}"
            };

            lineas.Add(a.DanioLanzado.HasValue
                ? $"{danioAtkLabel}: {a.DanioAtaque}  /  {danioThrowLabel}: {a.DanioLanzado}"
                : $"{danioAtkLabel}: {a.DanioAtaque}");

            if (a.Acciones != null && a.Acciones.Count > 0)
            {
                string accionesLabel = _en ? "Actions" : "Acciones";
                lineas.Add($"{accionesLabel}: {string.Join(", ", a.Acciones)}");
            }

            lblStats.Text = string.Join(Environment.NewLine, lineas);

            var efectoTexto = a.Efecto.Get(_en);
            if (string.IsNullOrWhiteSpace(efectoTexto))
            {
                lblEfecto.Text = string.Empty;
            }
            else
            {
                string efectoLabel = _en ? "Effect" : "Efecto";
                lblEfecto.Text = $"{efectoLabel}: {efectoTexto}";
            }
        }

        private void CargarImagen(string nombreArchivo)
        {
            CargarImagenEn(picObjeto, nombreArchivo);
        }

        private void LimpiarPictureBoxSet()
        {
            CargarImagenEn(pbSetHead, string.Empty);
            CargarImagenEn(pbSetTorso, string.Empty);
            CargarImagenEn(pbSetLegs, string.Empty);
            CargarImagenEn(pbSetFeet, string.Empty);
            CargarImagenEn(pbSetLeft, string.Empty);
            CargarImagenEn(pbSetRight, string.Empty);
        }
    }
}
