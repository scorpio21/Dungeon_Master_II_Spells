using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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

        // Soporte para arrastrar las piezas del set
        private PictureBox? _draggingSetPb;
        private Point _dragStartMouse;
        private Point _dragStartControl;
        private bool _draggingSet;
        private readonly List<PictureBox> _setExtras = new();

        // Panel de botones relacionados con sets (Añadir extra, coords, JSON)
        private FlowLayoutPanel panelBotonesSet = null!;
        private Button btnAddExtra = null!;
        private Button btnCoords = null!;
        private Button btnJson = null!;
        private Button btnNuevoSet = null!;

        // Último set mostrado, para generar JSON
        private SetArmadura? _setActual;

        private readonly bool _en = string.Equals(Settings.Default.Idioma, "EN", StringComparison.OrdinalIgnoreCase);

        // Modo de edición de sets (solo accesible con combinación de teclas)
        private bool _modoEdicionSets;

        private List<Arma> _todas = new();
        private List<SetArmadura> _sets = new();

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

                var baseDirArmas = Path.Combine(Imagenes.BaseImgPath(), "objetos", "Armas");
                string[] extensiones = { ".png", ".gif", ".jpg", ".jpeg", ".bmp" };

                // Buscar primero en la carpeta de armas
                string? ruta = Imagenes.BuscarImagen(baseDirArmas, nombreArchivo, extensiones);

                // Si no se encuentra, probar también en collares (para extras de los sets)
                if (ruta == null)
                {
                    var baseDirCollares = Path.Combine(Imagenes.BaseImgPath(), "objetos", "collares");
                    ruta = Imagenes.BuscarImagen(baseDirCollares, nombreArchivo, extensiones);
                }

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

        private sealed class PiezaSet
        {
            [JsonPropertyName("tipo")] public string Tipo { get; set; } = string.Empty;
            [JsonPropertyName("imagen")] public string Imagen { get; set; } = string.Empty;
            [JsonPropertyName("nombre")] public TextoLocalizado Nombre { get; set; } = new();
        }

        private sealed class SetArmadura
        {
            [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
            [JsonPropertyName("nombre")] public TextoLocalizado Nombre { get; set; } = new();
            [JsonPropertyName("tipo")] public TextoLocalizado Tipo { get; set; } = new();
            [JsonPropertyName("imagen")] public string Imagen { get; set; } = string.Empty;
            [JsonPropertyName("piezas")] public List<PiezaSet> Piezas { get; set; } = new();
        }

        private sealed class RaizSets
        {
            [JsonPropertyName("sets")] public List<SetArmadura> Sets { get; set; } = new();
        }

        private sealed class ItemLista
        {
            public object Valor { get; }
            private readonly bool _en;
            private readonly bool _esSet;

            public ItemLista(Arma arma, bool en, bool esSet = false) 
            { 
                Valor = arma; 
                _en = en; 
                _esSet = esSet;
            }

            public ItemLista(SetArmadura set, bool en, bool esSet = true)
            {
                Valor = set;
                _en = en;
                _esSet = esSet;
            }

            public override string ToString()
            {
                if (_esSet && Valor is SetArmadura set)
                    return $"[SET] {set.Nombre.Get(_en)}";
                else if (Valor is Arma arma)
                    return arma.Nombre.Get(_en);
                return base.ToString();
            }
        }

        private readonly bool _soloSets;

        public ObjetosForm(bool soloSets = false)
        {
            _soloSets = soloSets;
            Text = _en ? "Weapons" : "Armas";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(960, 640);
            KeyPreview = true;
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
                Text = _en ? "Sets only" : "Solo sets",
                Checked = _soloSets,
                Visible = !_soloSets  // Ocultar el checkbox si ya estamos en modo "solo sets"
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

            ConfigurarDragSet(pbSetHead, "head");
            ConfigurarDragSet(pbSetTorso, "torso");
            ConfigurarDragSet(pbSetLegs, "legs");
            ConfigurarDragSet(pbSetFeet, "feet");
            ConfigurarDragSet(pbSetLeft, "shield");
            ConfigurarDragSet(pbSetRight, "weapon");

            // Hacemos el panel bastante más grande que la imagen original (103x113)
            // para que las piezas de 96x96 tengan espacio suficiente y se vean claras.
            panelSet = new Panel
            {
                Width = 320,
                Height = 360,
                Margin = new Padding(0, 4, 0, 4),
                Visible = false
            };

            try
            {
                var fondoInv = Path.Combine(Imagenes.BaseImgPath(), "inven.png");
                if (File.Exists(fondoInv))
                {
                    panelSet.BackgroundImage = Imagenes.CargarImagenSegura(fondoInv);
                    panelSet.BackgroundImageLayout = ImageLayout.Stretch;
                }
            }
            catch
            {
                // Si falla la carga del fondo, simplemente se deja sin imagen
            }

            // Posiciones fijas del personaje sobre el inventario, ajustadas por el usuario
            // Valores tomados del botón "Ver coordenadas"
            pbSetHead.Location = new Point(106, 93);
            pbSetTorso.Location = new Point(102, 156);
            pbSetLegs.Location = new Point(104, 212);
            pbSetFeet.Location = new Point(103, 286);

            pbSetLeft.Location = new Point(15, 172);   // Escudo
            pbSetRight.Location = new Point(183, 173); // Arma

            panelSet.Controls.Add(pbSetHead);
            panelSet.Controls.Add(pbSetTorso);
            panelSet.Controls.Add(pbSetLegs);
            panelSet.Controls.Add(pbSetFeet);
            panelSet.Controls.Add(pbSetLeft);
            panelSet.Controls.Add(pbSetRight);
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

            btnAddExtra = new Button
            {
                Text = _en ? "Add extra" : "Añadir extra",
                AutoSize = true,
                Margin = new Padding(0, 4, 8, 4)
            };
            btnAddExtra.Click += (_, __) => CrearExtraVisual();

            btnCoords = new Button
            {
                Text = _en ? "Show coords" : "Ver coordenadas",
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 4)
            };
            btnCoords.Click += (_, __) => MostrarCoordenadasSet();

            btnJson = new Button
            {
                Text = _en ? "Generate JSON" : "Generar JSON",
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 4)
            };
            btnJson.Click += (_, __) => GenerarJsonSet();

            btnNuevoSet = new Button
            {
                Text = _en ? "New set" : "Nuevo set",
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 4)
            };
            btnNuevoSet.Click += (_, __) =>
            {
                using var f = new NuevoSetForm();
                f.ShowDialog(this);
            };
            panelBotonesSet = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Visible = false
            };
            panelBotonesSet.Controls.Add(btnAddExtra);
            panelBotonesSet.Controls.Add(btnCoords);
            panelBotonesSet.Controls.Add(btnJson);
            panelBotonesSet.Controls.Add(btnNuevoSet);

            stackDerecha.Controls.Add(panelBotonesSet);
            stackDerecha.Controls.Add(lblStats);
            stackDerecha.Controls.Add(lblEfecto);

            panelDerecho.Controls.Add(stackDerecha);

            Controls.Add(panelDerecho);
            Controls.Add(panelIzquierdo);

            Load += (_, __) =>
            {
                CargarDatos();
                // Si este formulario ya viene en modo solo sets, mostrar botones de set solo en modo edición
                ActualizarVisibilidadBotonesSet();
            };
            txtBuscar.TextChanged += (_, __) => RefrescarLista();
            cbTipo.SelectedIndexChanged += (_, __) => RefrescarLista();
            chkSoloSets.CheckedChanged += (_, __) =>
            {
                RefrescarLista();
                // Solo mostrar los botones de set cuando estamos en modo solo sets y en modo edición
                ActualizarVisibilidadBotonesSet();
            };
            lstObjetos.SelectedIndexChanged += (_, __) => MostrarSeleccion();
            KeyDown += ObjetosForm_KeyDown;
        }

private static bool EsSet(Arma arma)
        {
            if (arma == null) return false;
            
            // Verificar si el ID o el nombre contienen palabras clave de sets
            string id = arma.Id?.ToLowerInvariant() ?? "";
            string nombre = arma.Nombre?.Es?.ToLowerInvariant() ?? "";
            
            return id.Contains("fire_") || id.Contains("mithral_") || id.Contains("ra_sar_") || id.Contains("tech") ||
                   nombre.Contains("fuego") || nombre.Contains("fire") || nombre.Contains("mithral") || 
                   nombre.Contains("ra-sar") || nombre.Contains("ra_sar") || nombre.Contains("tech");
        }

        private static string? PrefijoSet(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            if (id.StartsWith("fire_", StringComparison.OrdinalIgnoreCase)) return "fire_";
            if (id.StartsWith("ra_sar_", StringComparison.OrdinalIgnoreCase)) return "ra_sar_";
            if (id.StartsWith("mithral_", StringComparison.OrdinalIgnoreCase)) return "mithral_";
            if (id.StartsWith("tech", StringComparison.OrdinalIgnoreCase)) return "tech";
            if (id.Contains("plate", StringComparison.OrdinalIgnoreCase) || 
                id.Contains("armet", StringComparison.OrdinalIgnoreCase) || 
                id.Contains("torso", StringComparison.OrdinalIgnoreCase)) 
                return "plate";

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
            // Tamaño ajustado para que las piezas encajen mejor en las casillas del inventario
            return new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = 56,
                Height = 56,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(2)
            };
        }

        private void ConfigurarDragSet(PictureBox pb, string nombre)
        {
            pb.Tag = nombre;
            pb.MouseDown += PbSet_MouseDown;
            pb.MouseMove += PbSet_MouseMove;
            pb.MouseUp += PbSet_MouseUp;
        }

        private void CargarDatos()
        {
            try
            {
                // Cargar armas
                var rutaArmas = Path.Combine(AppContext.BaseDirectory, "data", "objetos_armas.json");
                if (!File.Exists(rutaArmas))
                {
                    var rutaAlt = Path.Combine(Directory.GetCurrentDirectory(), "data", "objetos_armas.json");
                    if (File.Exists(rutaAlt)) rutaArmas = rutaAlt; else throw new FileNotFoundException(rutaArmas);
                }

                using (var fs = File.OpenRead(rutaArmas))
                {
                    var raiz = JsonSerializer.Deserialize<RaizArmas>(fs, new JsonSerializerOptions
                    {
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        AllowTrailingCommas = true,
                        PropertyNameCaseInsensitive = true
                    });
                    _todas = raiz?.Armas ?? new List<Arma>();
                }

                // Cargar sets
                var rutaSets = Path.Combine(AppContext.BaseDirectory, "data", "objetos_sets.json");
                if (!File.Exists(rutaSets))
                {
                    var rutaAlt = Path.Combine(Directory.GetCurrentDirectory(), "data", "objetos_sets.json");
                    if (File.Exists(rutaAlt)) rutaSets = rutaAlt;
                }

                if (File.Exists(rutaSets))
                {
                    using var fsSets = File.OpenRead(rutaSets);
                    var raizSets = JsonSerializer.Deserialize<RaizSets>(fsSets, new JsonSerializerOptions
                    {
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        AllowTrailingCommas = true,
                        PropertyNameCaseInsensitive = true
                    });
                    _sets = raizSets?.Sets ?? new List<SetArmadura>();
                }

                // Actualizar lista de tipos
                var tipos = _todas
                    .Select(a => a.Tipo.Get(_en))
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Concat(_sets.Select(s => s.Tipo.Get(_en)))
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
                MessageBox.Show((_en ? "Error loading data: " : "Error cargando datos: ") + ex.Message,
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefrescarLista()
        {
            var filtroTxt = (txtBuscar.Text ?? string.Empty).Trim().ToLowerInvariant();
            var filtroTipo = cbTipo.SelectedItem?.ToString();
            bool todos = string.IsNullOrWhiteSpace(filtroTipo) || filtroTipo == (_en ? "All" : "Todos");
            bool mostrarSoloSets = _soloSets || chkSoloSets.Checked;

            // Filtrar armas normales
            var listaArmas = _todas
                .Where(a => todos || string.Equals(a.Tipo.Get(_en), filtroTipo, StringComparison.OrdinalIgnoreCase))
                .Where(a => !mostrarSoloSets)  // Solo mostrar armas si no estamos en modo solo sets
                .Where(a => string.IsNullOrWhiteSpace(filtroTxt) || a.Nombre.Get(_en).ToLowerInvariant().Contains(filtroTxt))
                .Select(a => new ItemLista(a, _en, false));

            // Filtrar sets
            var listaSets = _sets
                .Where(s => mostrarSoloSets)  // Solo mostrar sets si estamos en modo solo sets
                .Where(s => todos || string.Equals(s.Tipo.Get(_en), filtroTipo, StringComparison.OrdinalIgnoreCase))
                .Where(s => string.IsNullOrWhiteSpace(filtroTxt) || s.Nombre.Get(_en).ToLowerInvariant().Contains(filtroTxt))
                .Select(s => new ItemLista(s, _en, true));

            // Combinar y ordenar
            var lista = listaArmas.Concat(listaSets)
                                .OrderBy(i => i.ToString())
                                .ToList();

            lstObjetos.BeginUpdate();
            lstObjetos.Items.Clear();
            foreach (var item in lista)
                lstObjetos.Items.Add(item);
            lstObjetos.EndUpdate();
            if (lstObjetos.Items.Count > 0) lstObjetos.SelectedIndex = 0;
        }

        private void ActualizarVisibilidadBotonesSet()
        {
            bool soloSets = _soloSets || chkSoloSets.Checked;
            // El panel solo aparece en modo solo sets
            panelBotonesSet.Visible = soloSets;

            // "Nuevo set" siempre visible cuando el panel está visible
            btnNuevoSet.Visible = panelBotonesSet.Visible;

            // El resto de botones (extras / coords / JSON) solo en modo edición
            bool verHerramientas = panelBotonesSet.Visible && _modoEdicionSets;
            btnAddExtra.Visible = verHerramientas;
            btnCoords.Visible = verHerramientas;
            btnJson.Visible = verHerramientas;
        }

        private void ObjetosForm_KeyDown(object? sender, KeyEventArgs e)
        {
            // Ctrl+F1 para activar/desactivar el modo de edición de sets
            if (e.Control && e.KeyCode == Keys.F1)
            {
                _modoEdicionSets = !_modoEdicionSets;
                ActualizarVisibilidadBotonesSet();

                string msgEs = _modoEdicionSets
                    ? "Modo edición de sets ACTIVADO (puedes mover piezas y ver botones de extras)."
                    : "Modo edición de sets DESACTIVADO (vista normal, sin botones de extras).";
                string msgEn = _modoEdicionSets
                    ? "Set edit mode ENABLED (you can move pieces and see extra buttons)."
                    : "Set edit mode DISABLED (normal view, without extra buttons).";

                MessageBox.Show(_en ? msgEn : msgEs,
                    _en ? "Set edit mode" : "Modo edición de sets",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                e.Handled = true;
            }
        }

        private void MostrarSeleccion()
        {
            if (lstObjetos.SelectedItem is not ItemLista item) return;

            // Limpiar controles
            Imagenes.ReemplazarImagen(picObjeto, null);
            lblStats.Text = string.Empty;
            lblEfecto.Text = string.Empty;

            // Limpiar imágenes del set
            Imagenes.ReemplazarImagen(pbSetHead, null);
            Imagenes.ReemplazarImagen(pbSetTorso, null);
            Imagenes.ReemplazarImagen(pbSetLegs, null);
            Imagenes.ReemplazarImagen(pbSetFeet, null);
            Imagenes.ReemplazarImagen(pbSetLeft, null);
            Imagenes.ReemplazarImagen(pbSetRight, null);

            if (item.Valor is Arma a)
            {
                // Mostrar arma normal
                lblNombre.Text = a.Nombre.Get(_en);
                lblTipo.Text = a.Tipo.Get(_en);
                CargarImagen(a.Imagen);
                panelSet.Visible = false;
                panelBotonesSet.Visible = false;

                // Mostrar estadísticas del arma
                MostrarEstadisticasArma(a);
            }
            else if (item.Valor is SetArmadura set)
            {
                // Mostrar set de armadura
                lblNombre.Text = set.Nombre.Get(_en);
                lblTipo.Text = set.Tipo.Get(_en);
                CargarImagen(set.Imagen);
                panelSet.Visible = true;
                // Solo mostrar los botones de set cuando estamos viendo sets, en modo solo sets y edición
                ActualizarVisibilidadBotonesSet();

                // Recordar el set actual para generar JSON
                _setActual = set;

                // Asegurar que existen hasta 4 extras en sus posiciones fijas
                while (_setExtras.Count < 4)
                {
                    var pbExtra = CrearPictureBoxSet();
                    // Estos extras son automáticos para representar las posiciones fijas;
                    // no les ponemos una etiqueta específica para que en el log salgan como extra1, extra2, etc.
                    ConfigurarDragSet(pbExtra, string.Empty);
                    pbExtra.Tag = null;

                    int x;
                    int y;
                    switch (_setExtras.Count)
                    {
                        case 0: // extra1
                            x = 15;  y = 112; break;
                        case 1: // extra2
                            x = 192; y = 241; break;
                        case 2: // extra3
                            x = 17;  y = 240; break;
                        case 3: // extra4
                            x = 201; y = 113; break;
                        default:
                            x = panelSet.Width - pbExtra.Width - 8;
                            y = panelSet.Height - pbExtra.Height - 8;
                            break;
                    }

                    pbExtra.Location = new Point(x, y);
                    panelSet.Controls.Add(pbExtra);
                    _setExtras.Add(pbExtra);
                }

                // Mostrar piezas del set
                foreach (var pieza in set.Piezas)
                {
                    switch (pieza.Tipo.ToLowerInvariant())
                    {
                        case "head":
                            CargarImagenEn(pbSetHead, pieza.Imagen);
                            break;
                        case "torso":
                            CargarImagenEn(pbSetTorso, pieza.Imagen);
                            break;
                        case "legs":
                            CargarImagenEn(pbSetLegs, pieza.Imagen);
                            break;
                        case "feet":
                            CargarImagenEn(pbSetFeet, pieza.Imagen);
                            break;
                        case "shield":
                            CargarImagenEn(pbSetLeft, pieza.Imagen);
                            break;
                        case "amulet":
                            // Collar del Clan en la posición extra1 (15,112)
                            if (_setExtras.Count >= 1)
                            {
                                _setExtras[0].Tag = Path.GetFileNameWithoutExtension(pieza.Imagen) ?? "extra1";
                                CargarImagenEn(_setExtras[0], pieza.Imagen);
                            }
                            break;
                        case "arrows":
                            // Quiver en la posición extra2 (192,241)
                            if (_setExtras.Count >= 2)
                            {
                                _setExtras[1].Tag = Path.GetFileNameWithoutExtension(pieza.Imagen) ?? "extra2";
                                CargarImagenEn(_setExtras[1], pieza.Imagen);
                            }
                            break;
                        case "ring":
                            // Scarab en la posición extra3 (17,240)
                            if (_setExtras.Count >= 3)
                            {
                                _setExtras[2].Tag = Path.GetFileNameWithoutExtension(pieza.Imagen) ?? "extra3";
                                CargarImagenEn(_setExtras[2], pieza.Imagen);
                            }
                            break;
                        case "necklace":
                            // Baúl en la posición extra4 (201,113)
                            if (_setExtras.Count >= 4)
                            {
                                _setExtras[3].Tag = Path.GetFileNameWithoutExtension(pieza.Imagen) ?? "extra4";
                                CargarImagenEn(_setExtras[3], pieza.Imagen);
                            }
                            break;
                        default:
                            CargarImagenEn(pbSetRight, pieza.Imagen);
                            break;
                    }
                }

                // Mostrar lista de piezas en el efecto
                var piezas = string.Join(", ", set.Piezas.Select(p => p.Nombre.Get(_en)));
                lblEfecto.Text = $"{(_en ? "Includes" : "Incluye")}: {piezas}";
            }
        }

        private void MostrarEstadisticasArma(Arma a)
        {
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

        // Manejadores de arrastre para las piezas del set
        private void PbSet_MouseDown(object? sender, MouseEventArgs e)
        {
            if (!_modoEdicionSets) return;
            if (sender is not PictureBox pb || e.Button != MouseButtons.Left) return;
            _draggingSetPb = pb;
            _dragStartMouse = e.Location;
            _dragStartControl = pb.Location;
            _draggingSet = true;
        }

        private void PbSet_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_modoEdicionSets) return;
            if (!_draggingSet || _draggingSetPb == null) return;

            var dx = e.X - _dragStartMouse.X;
            var dy = e.Y - _dragStartMouse.Y;
            var nueva = new Point(_dragStartControl.X + dx, _dragStartControl.Y + dy);

            // Limitar dentro del panel
            if (_draggingSetPb.Parent is Panel p)
            {
                int maxX = Math.Max(0, p.Width - _draggingSetPb.Width);
                int maxY = Math.Max(0, p.Height - _draggingSetPb.Height);
                nueva.X = Math.Min(Math.Max(0, nueva.X), maxX);
                nueva.Y = Math.Min(Math.Max(0, nueva.Y), maxY);
            }

            _draggingSetPb.Location = nueva;
        }

        private void PbSet_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            _draggingSet = false;
            _draggingSetPb = null;
        }

        private void MostrarCoordenadasSet()
        {
            var sb = new StringBuilder();
            void Add(string nombre, PictureBox pb)
            {
                string etiqueta = pb.Tag as string ?? nombre;
                sb.AppendLine($"{etiqueta}: X={pb.Location.X}, Y={pb.Location.Y}");
            }

            Add("head", pbSetHead);
            Add("torso", pbSetTorso);
            Add("legs", pbSetLegs);
            Add("feet", pbSetFeet);
            Add("shield", pbSetLeft);
            Add("weapon", pbSetRight);

            if (_setExtras.Count > 0)
            {
                for (int i = 0; i < _setExtras.Count; i++)
                {
                    var extra = _setExtras[i];
                    Add($"extra{i + 1}", extra);
                }
            }

            MessageBox.Show(sb.ToString(), _en ? "Set coords" : "Coordenadas del set",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GenerarJsonSet()
        {
            if (_setActual == null)
            {
                MessageBox.Show(_en ? "No set selected" : "No hay set seleccionado",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("\"piezas\": [");

            // Usamos las piezas ya definidas en el JSON (_setActual.Piezas)
            for (int i = 0; i < _setActual.Piezas.Count; i++)
            {
                var p = _setActual.Piezas[i];
                string coma = i < _setActual.Piezas.Count - 1 ? "," : string.Empty;

                sb.AppendLine(
                    $"  {{ \"tipo\": \"{p.Tipo}\", \"imagen\": \"{p.Imagen}\", \"nombre\": {{ \"es\": \"{p.Nombre.Es}\", \"en\": \"{p.Nombre.En}\" }} }}{coma}");
            }

            sb.AppendLine("]");

            using var dlg = new Form
            {
                Text = _en ? "Generated JSON" : "JSON generado",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(600, 400)
            };

            var txt = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10f),
                Text = sb.ToString()
            };

            dlg.Controls.Add(txt);
            dlg.ShowDialog(this);
        }

        private void CrearExtraVisual()
        {
            // Permitir al usuario elegir la imagen del extra desde la carpeta img
            using var dlg = new OpenFileDialog
            {
                Title = _en ? "Select extra image" : "Selecciona imagen para el extra",
                Filter = "Imagenes|*.png;*.gif;*.jpg;*.jpeg;*.bmp",
                InitialDirectory = Imagenes.BaseImgPath()
            };

            if (dlg.ShowDialog(this) != DialogResult.OK)
            {
                return; // No se crea el extra si no se elige imagen
            }

            var pb = CrearPictureBoxSet();
            string nombreBase = Path.GetFileNameWithoutExtension(dlg.FileName) ?? $"extra{_setExtras.Count + 1}";
            ConfigurarDragSet(pb, nombreBase);

            try
            {
                var img = Imagenes.CargarImagenSegura(dlg.FileName);
                Imagenes.ReemplazarImagen(pb, img);
            }
            catch
            {
                // Si falla la carga, no añadimos el extra
                return;
            }

            int x;
            int y;

            // Posiciones fijas para los primeros extras según tus ajustes actuales:
            // extra1: amuleto/collar, extra2/3/4 otras piezas del set.
            switch (_setExtras.Count)
            {
                case 0: // extra1
                    x = 15;  y = 112; break;
                case 1: // extra2
                    x = 192; y = 241; break;
                case 2: // extra3
                    x = 17;  y = 240; break;
                case 3: // extra4
                    x = 201; y = 113; break;
                default:
                    // Resto de extras: esquina inferior derecha del inventario
                    x = panelSet.Width - pb.Width - 8;
                    y = panelSet.Height - pb.Height - 8;
                    break;
            }

            pb.Location = new Point(x, y);

            panelSet.Controls.Add(pb);
            _setExtras.Add(pb);
        }
    }
}
