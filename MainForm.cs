using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Globalization;
using SpellBookWinForms.Properties;
using System.Resources;
using System.Linq;
using System.Windows.Forms;
using System.Text.Json;

namespace SpellBookWinForms
{
    public partial class MainForm : Form
    {
        private readonly Dictionary<string, Dictionary<string, (string Simbolos, string Efecto)>> _hechizos;
        private readonly Dictionary<string, int> _manaPL1;
        private readonly Dictionary<string, int> _diffBase;
        private readonly Dictionary<string, int[]> _manaPorNivel = new();
        private readonly Dictionary<string, string> _simboloDescripcion;
        private readonly Dictionary<string, string> _simboloFamilia;
        private readonly Dictionary<string, int> _poderMap = new() { {"Lo",1},{"Um",2},{"On",3},{"Ee",4},{"Pal",5},{"Mon",6} };

        private MenuStrip menuPrincipal = null!;  // Inicialización no nula asegurada en el constructor
        private ToolStripMenuItem menuUtilidades = null!;
        private ToolStripMenuItem menuCalculadoraMonedas = null!;
        private ToolStripMenuItem menuCriaturas = null!;
        private ToolStripMenuItem menuIdioma = null!;
        private ToolStripMenuItem menuEs = null!;
        private ToolStripMenuItem menuEn = null!;
        private ToolStripMenuItem menuAyuda = null!;
        private ToolStripMenuItem menuAcerca = null!;

        // Soporte de idioma (visualización)
        private enum Idioma { ES, EN }
        private Idioma _idiomaActual = Idioma.ES; // por defecto español

        // Recursos
        private readonly ResourceManager _rm = new ResourceManager("SpellBookWinForms.Resources.Strings", typeof(MainForm).Assembly);
        private string T(string key)
        {
            var ci = _idiomaActual == Idioma.EN ? new CultureInfo("en") : new CultureInfo("es");
            return _rm.GetString(key, ci) ?? key;
        }

        // Item para combos: muestra traducido pero mantiene la clave ES para la lógica
        private sealed class ComboItem
        {
            public string KeyEs { get; }
            public string Display { get; }
            public ComboItem(string keyEs, string display) { KeyEs = keyEs; Display = display; }
            public override string ToString() => Display;
        }

        // Traducciones de clases y hechizos (ES -> EN)
        private readonly Dictionary<string, string> _trClases = new()
        {
            ["Sacerdote"] = "Priest",
            ["Mago"] = "Wizard",
        };

        private readonly Dictionary<string, string> _trHechizosNombre = new()
        {
            // Sacerdote
            ["Poción de Salud"] = "Health Potion",
            ["Poción de Energía"] = "Stamina Potion",
            ["Poción de Maná"] = "Mana Potion",
            ["Poción de Fuerza"] = "Strength Potion",
            ["Poción de Destreza"] = "Dexterity Potion",
            ["Poción de Sabiduría"] = "Wisdom Potion",
            ["Poción de Vitalidad"] = "Vitality Potion",
            ["Curar Veneno"] = "Cure Poison",
            ["Poción de Escudo"] = "Shield Potion",
            ["Escudo de Fuego"] = "Fire Shield",
            ["Escudo Grupal"] = "Party Shield",
            ["Oscuridad"] = "Darkness",
            ["Aura de Fuerza"] = "Aura of Strength",
            ["Aura de Destreza"] = "Aura of Dexterity",
            ["Aura de Vitalidad"] = "Aura of Vitality",
            ["Aura de Sabiduría"] = "Aura of Wisdom",
            ["Escudo Mágico"] = "Magic Shield",
            ["Transportar Esbirro"] = "Minion Transport",
            ["Reflejo de Hechizos"] = "Spell Reflection",
            ["Guardia Esbirro"] = "Minion Guard",

            // Mago
            ["Antorcha"] = "Torch",
            ["Luz"] = "Light",
            ["Abrir Puerta"] = "Open Door",
            ["Invisibilidad"] = "Invisibility",
            ["Dardo Venenoso"] = "Poison Dart",
            ["Nube Venenosa"] = "Poison Cloud",
            ["Debilitar Seres Inmateriales"] = "Weaken Non-Material Beings",
            ["Bola de Fuego"] = "Fireball",
            ["Rayo"] = "Lightning Bolt",
            ["Marca Mágica"] = "Magic Mark",
            ["Empujar"] = "Push",
            ["Atraer"] = "Pull",
            ["Aura de Velocidad"] = "Aura of Speed",
            ["Esbirro de Ataque"] = "Attack Minion",
        };

        private readonly Dictionary<string, string> _trEfecto = new()
        {
            ["Recupera salud"] = "Restores health",
            ["Recupera energía"] = "Restores stamina",
            ["Restaura maná"] = "Restores mana",
            ["Aumenta fuerza"] = "Increases strength",
            ["Mejora destreza"] = "Improves dexterity",
            ["Mejora sabiduría"] = "Improves wisdom",
            ["Mejora vitalidad"] = "Improves vitality",
            ["Elimina veneno"] = "Cures poison",
            ["Protección personal"] = "Personal protection",
            ["Escudo de fuego"] = "Fire protection",
            ["Protección grupal"] = "Party protection",
            ["Oscurece el entorno"] = "Darkens the surroundings",
            ["Aumenta fuerza del grupo"] = "Increases party strength",
            ["Aumenta destreza del grupo"] = "Increases party dexterity",
            ["Aumenta vitalidad del grupo"] = "Increases party vitality",
            ["Aumenta sabiduría del grupo"] = "Increases party wisdom",
            ["Protección mágica"] = "Magic protection",
            ["Transporta esbirro"] = "Transports minion",
            ["Refleja hechizos"] = "Reflects spells",
            ["Protege esbirro"] = "Guards minion",
            ["Crea una antorcha mágica"] = "Creates a magical torch",
            ["Ilumina el entorno"] = "Illuminates the surroundings",
            ["Abre puertas cercanas"] = "Opens nearby doors",
            ["Vuelve al grupo invisible"] = "Turns the party invisible",
            ["Dispara un proyectil venenoso"] = "Shoots a poisonous projectile",
            ["Genera una nube tóxica"] = "Generates a toxic cloud",
            ["Debilita seres no materiales"] = "Weakens non-material beings",
            ["Explosión de fuego"] = "Fire explosion",
            ["Rayo eléctrico"] = "Electric lightning",
            ["Marca mágica"] = "Magical mark",
            ["Empuja objetos o enemigos"] = "Pushes objects or enemies",
            ["Atrae objetos o enemigos"] = "Pulls objects or enemies",
            ["Aumenta velocidad del grupo"] = "Increases party speed",
            ["Ataca a un esbirro"] = "Attacks a minion",
        };

        public MainForm()
        {
            InitializeComponent();
            InicializarMenu();

            // Leer idioma desde configuración
            try
            {
                var lang = Settings.Default.Idioma;
                if (string.IsNullOrWhiteSpace(lang))
                {
                    var sys = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                    lang = string.Equals(sys, "en", StringComparison.OrdinalIgnoreCase) ? "EN" : "ES";
                    Settings.Default.Idioma = lang;
                    Settings.Default.Save();
                }
                _idiomaActual = string.Equals(lang, "EN", StringComparison.OrdinalIgnoreCase) ? Idioma.EN : Idioma.ES;
            }
            catch { /* no-op */ }

            // Datos: mismos que en Python
            _hechizos = new()
            {
                ["Sacerdote"] = new()
                {
                    ["Poción de Salud"] = ("Vi", "Recupera salud"),
                    ["Poción de Energía"] = ("Ya", "Recupera energía"),
                    ["Poción de Maná"] = ("Zo Bro Ra", "Restaura maná"),
                    ["Poción de Fuerza"] = ("Ful Bro Ku", "Aumenta fuerza"),
                    ["Poción de Destreza"] = ("Oh Bro Ros", "Mejora destreza"),
                    ["Poción de Sabiduría"] = ("Ya Bro Dain", "Mejora sabiduría"),
                    ["Poción de Vitalidad"] = ("Ya Bro Neta", "Mejora vitalidad"),
                    ["Curar Veneno"] = ("Vi Bro", "Elimina veneno"),
                    ["Poción de Escudo"] = ("Ya Bro", "Protección personal"),
                    ["Escudo de Fuego"] = ("Ful Bro Neta", "Escudo de fuego"),
                    ["Escudo Grupal"] = ("Ya Ir", "Protección grupal"),
                    ["Oscuridad"] = ("Des Ir Sar", "Oscurece el entorno"),
                    ["Aura de Fuerza"] = ("Oh Ew Ku", "Aumenta fuerza del grupo"),
                    ["Aura de Destreza"] = ("Oh Ew Ros", "Aumenta destreza del grupo"),
                    ["Aura de Vitalidad"] = ("Oh Ew Neta", "Aumenta vitalidad del grupo"),
                    ["Aura de Sabiduría"] = ("Oh Ew Dain", "Aumenta sabiduría del grupo"),
                    ["Escudo Mágico"] = ("Ya Ir Dain", "Protección mágica"),
                    ["Transportar Esbirro"] = ("Zo Ew Ros", "Transporta esbirro"),
                    ["Reflejo de Hechizos"] = ("Zo Bro Ros", "Refleja hechizos"),
                    ["Guardia Esbirro"] = ("Zo Ew Neta", "Protege esbirro"),
                },
                ["Mago"] = new()
                {
                    ["Antorcha"] = ("Ful", "Crea una antorcha mágica"),
                    ["Luz"] = ("Oh Ir Ra", "Ilumina el entorno"),
                    ["Abrir Puerta"] = ("Zo", "Abre puertas cercanas"),
                    ["Invisibilidad"] = ("Oh Ew Sar", "Vuelve al grupo invisible"),
                    ["Dardo Venenoso"] = ("Des Ven", "Dispara un proyectil venenoso"),
                    ["Nube Venenosa"] = ("Oh Ven", "Genera una nube tóxica"),
                    ["Debilitar Seres Inmateriales"] = ("Des Ew", "Debilita seres no materiales"),
                    ["Bola de Fuego"] = ("Ful Ir", "Explosión de fuego"),
                    ["Rayo"] = ("Oh Kath Ra", "Rayo eléctrico"),
                    ["Marca Mágica"] = ("Ya Ew", "Marca mágica"),
                    ["Empujar"] = ("Oh Kath Ku", "Empuja objetos o enemigos"),
                    ["Atraer"] = ("Oh Kath Ros", "Atrae objetos o enemigos"),
                    ["Aura de Velocidad"] = ("Oh Ir Ros", "Aumenta velocidad del grupo"),
                    ["Esbirro de Ataque"] = ("Zo Ew Ku", "Ataca a un esbirro"),
                }
            };

            _manaPL1 = new()
            {
                // Elemental influence
                ["Ya"]=2, ["Vi"]=3, ["Oh"]=4, ["Ful"]=5, ["Des"]=6, ["Zo"]=7,
                // Form
                ["Ven"]=4, ["Ew"]=5, ["Kath"]=6, ["Ir"]=7, ["Bro"]=7, ["Gor"]=9,
                // Class / Alignment
                ["Ku"]=2, ["Ros"]=2, ["Dain"]=3, ["Neta"]=4, ["Ra"]=6, ["Sar"]=7,
            };

            _diffBase = new()
            {
                // Elemental influence
                ["Ya"]=2, ["Vi"]=3, ["Oh"]=4, ["Ful"]=5, ["Des"]=6, ["Zo"]=7,
                // Form
                ["Ven"]=4, ["Ew"]=5, ["Kath"]=6, ["Ir"]=7, ["Bro"]=7, ["Gor"]=9,
                // Class / Alignment
                ["Ku"]=2, ["Ros"]=2, ["Dain"]=3, ["Neta"]=4, ["Ra"]=6, ["Sar"]=7,
            };

            _simboloDescripcion = new()
            {
                ["Zo"]="Vida / restauración",
                ["Vi"]="Energía / vigor",
                ["Ra"]="Maná / esencia mágica",
                ["Ku"]="Fuerza / potencia física",
                ["Ros"]="Destreza / agilidad",
                ["Dain"]="Inteligencia / sabiduría",
                ["Ful"]="Fuego / calor mágico",
                ["Ir"]="Explosión / impacto",
                ["Sar"]="Oscuridad / ocultación",
                ["Ven"]="Veneno / toxicidad",
                ["Oh"]="Puerta / apertura",
                ["Ew"]="Movimiento / impulso",
                ["Bro"]="Ocultación / invisibilidad",
                ["Des"]="Disipar / anulación mágica",
                ["Kath"]="Rayo / energía pura",
                ["Ya"]="Tiempo / duración",
                ["Neta"]="Nube / dispersión",
            };

            _simboloFamilia = new()
            {
                ["Lo"]="Power", ["Um"]="Power", ["On"]="Power", ["Ee"]="Power", ["Pal"]="Power", ["Mon"]="Power",
                ["Ya"]="Elemental influence", ["Vi"]="Elemental influence", ["Oh"]="Elemental influence", ["Ful"]="Elemental influence", ["Des"]="Elemental influence", ["Zo"]="Elemental influence",
                ["Ven"]="Form", ["Ew"]="Form", ["Kath"]="Form", ["Ir"]="Form", ["Bro"]="Form", ["Gor"]="Form",
                ["Ku"]="Class / Alignment", ["Ros"]="Class / Alignment", ["Dain"]="Class / Alignment", ["Neta"]="Class / Alignment", ["Ra"]="Class / Alignment", ["Sar"]="Class / Alignment",
            };

            // Intentar cargar tablas externas (JSON) y sobreescribir valores si procede
            CargarTablasExternas();

            // Inicializar combos
            // Poblar clases con items que conservan clave ES pero muestran según idioma
            cbClase.Items.Clear();
            foreach (var claseEs in _hechizos.Keys)
            {
                var display = _idiomaActual == Idioma.EN && _trClases.TryGetValue(claseEs, out var en) ? en : claseEs;
                cbClase.Items.Add(new ComboItem(claseEs, display));
            }
            if (cbClase.Items.Count > 0) cbClase.SelectedIndex = 0;

            // Ajustes de layout para evitar solapamiento
            pnlDetalles.WrapContents = true;
            pnlSimbolos.WrapContents = true;
            pnlDetalles.AutoSize = true;
            pnlSimbolos.AutoSize = true;
            pnlSimbolos.Padding = new Padding(8, 4, 8, 4);

            // Apilar controles verticalmente
            gbPoder.Dock = DockStyle.Top;
            gbSeleccion.Dock = DockStyle.Top;
            lblResultado.Dock = DockStyle.Top;
            pnlDetalles.Dock = DockStyle.Top;
            pnlSimbolos.Dock = DockStyle.Top;
            picFrasco.Dock = DockStyle.Top;
            Padding = new Padding(8);
            // Ajustes iniciales de layout completados

            // Idioma: leer preferencia guardada o inferir por cultura
            try
            {
                var lang = Settings.Default.Idioma;
                if (string.IsNullOrWhiteSpace(lang))
                {
                    var sys = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                    lang = string.Equals(sys, "en", StringComparison.OrdinalIgnoreCase) ? "EN" : "ES";
                    Settings.Default.Idioma = lang;
                    Settings.Default.Save();
                }
                _idiomaActual = string.Equals(lang, "EN", StringComparison.OrdinalIgnoreCase) ? Idioma.EN : Idioma.ES;
                // Marcar menú y aplicar
                menuEs.Checked = _idiomaActual == Idioma.ES;
                menuEn.Checked = _idiomaActual == Idioma.EN;
                AplicarIdioma();
            }
            catch { /* no-op */ }
        }

        private void cbClase_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cbClase.SelectedItem is not ComboItem claseItem) return;
            var clase = claseItem.KeyEs; // usamos la clave ES para lógica
            cbHechizo.Items.Clear();
            foreach (var hEs in _hechizos[clase].Keys)
            {
                var display = _idiomaActual == Idioma.EN && _trHechizosNombre.TryGetValue(hEs, out var en) ? en : hEs;
                cbHechizo.Items.Add(new ComboItem(hEs, display));
            }
            if (cbHechizo.Items.Count > 0) cbHechizo.SelectedIndex = 0;
            LimpiarResultados();
        }

        private void cbHechizo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            LimpiarResultados();
            
            // Mostrar el frasco vacío para todas las selecciones
            // La imagen completa se cargará solo al hacer clic en "Mostrar Hechizo"
            picFrasco.Image = null;
            
            // Si es una poción, cargar el frasco vacío
            if (cbHechizo.SelectedItem is ComboItem itemSel && EsPocion(itemSel.KeyEs))
            {
                CargarImagenFrasco("vacia.png", esObjeto: false);
            }
        }

        private void btnMostrar_Click(object? sender, EventArgs e)
        {
            if (cbClase.SelectedItem is not ComboItem claseItem2 || cbHechizo.SelectedItem is not ComboItem hechizoItem) return;
            var clase = claseItem2.KeyEs;
            var hechizo = hechizoItem.KeyEs; // clave ES para lógica e imágenes
            var datos = _hechizos[clase][hechizo];
            var poder = ObtenerPoderSeleccionado();
            var simbolos = $"{poder} {datos.Simbolos}";

            var (mana, diff, detalles) = Calcular(simbolos);
            var nombreMostrado = _idiomaActual == Idioma.EN && _trHechizosNombre.TryGetValue(hechizo, out var hEn) ? hEn : hechizo;
            var efectoMostrado = _idiomaActual == Idioma.EN && _trEfecto.TryGetValue(datos.Efecto, out var eEn) ? eEn : datos.Efecto;
            if (_idiomaActual == Idioma.EN)
                lblResultado.Text = $"🔮 Spell: {nombreMostrado}\n✨ Effect: {efectoMostrado}\n🧪 Symbols: {simbolos}\n🔋 Total mana: {mana}\n🎯 Total difficulty: {diff}";
            else
                lblResultado.Text = $"🔮 Hechizo: {nombreMostrado}\n✨ Efecto: {efectoMostrado}\n🧪 Símbolos: {simbolos}\n🔋 Mana total: {mana}\n🎯 Dificultad total: {diff}";

            // Detalles
            pnlDetalles.Controls.Clear();
            foreach (var linea in detalles)
            {
                var l = new Label { AutoSize = true, ForeColor = Color.Gray, Text = linea };
                pnlDetalles.Controls.Add(l);
            }

            // Cargar la imagen del hechizo u objeto
            var (imagenHechizo, esObjeto) = MapearImagenFrasco(hechizo);
            if (imagenHechizo != "vacia.png")
            {
                CargarImagenFrasco(imagenHechizo, esObjeto);
            }

            // Símbolos
            RenderSimbolos(simbolos);
        }

        private void LimpiarResultados()
        {
            lblResultado.Text = string.Empty;
            pnlDetalles.Controls.Clear();
            pnlSimbolos.Controls.Clear();
            picFrasco.Image = null;
        }

        private string ObtenerPoderSeleccionado()
        {
            if (rbMon.Checked) return "Mon";
            if (rbPal.Checked) return "Pal";
            if (rbEe.Checked) return "Ee";
            if (rbOn.Checked) return "On";
            if (rbUm.Checked) return "Um";
            return "Lo";
        }

        private (int mana, int diff, List<string> detalles) Calcular(string simbolos)
        {
            var partes = simbolos.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var detalles = new List<string>();
            if (partes.Length == 0) return (0, 0, detalles);

            var nivel = _poderMap.TryGetValue(partes[0], out var n) ? n : 1;

            int mana = nivel; // poder
            int diff = nivel;
            string manaWord = _idiomaActual == Idioma.EN ? "Mana" : "Mana";
            string diffWord = _idiomaActual == Idioma.EN ? "Difficulty" : "Dificultad";
            string familyWord = _idiomaActual == Idioma.EN ? "Family" : "Familia";
            string unknownFamily = _idiomaActual == Idioma.EN ? "Unknown" : "Desconocido";

            detalles.Add($"{partes[0]}: {manaWord} {nivel}, {diffWord} {nivel} — {familyWord}: {_simboloFamilia.GetValueOrDefault(partes[0], "Power")}");

            double factor = (nivel + 1) / 2.0;
            foreach (var s in partes.Skip(1))
            {
                int baseMana = _manaPL1.TryGetValue(s, out var bm) ? bm : 0;
                int baseDiff = _diffBase.TryGetValue(s, out var bd) ? bd : 0;
                int m;
                if (_manaPorNivel.TryGetValue(s, out var niveles) && niveles.Length >= nivel)
                {
                    m = niveles[nivel - 1];
                }
                else
                {
                    m = (int)Math.Floor(baseMana * factor);
                }
                int d = baseDiff; // la dificultad no escala con el poder
                mana += m; diff += d;
                detalles.Add($"{s}: {manaWord} {m}, {diffWord} {d} — {familyWord}: {_simboloFamilia.GetValueOrDefault(s, unknownFamily)}");
            }
            return (mana, diff, detalles);
        }

        private void CargarTablasExternas()
        {
            try
            {
                var ruta = Path.Combine(AppContext.BaseDirectory, "data", "tabla_dificultad_mana.json");
                if (!File.Exists(ruta))
                {
                    // Intento alternativo: relativo al proyecto/carpeta de trabajo
                    var rutaAlt = Path.Combine(Directory.GetCurrentDirectory(), "data", "tabla_dificultad_mana.json");
                    if (File.Exists(rutaAlt)) ruta = rutaAlt; else return;
                }

                using var fs = File.OpenRead(ruta);
                using var doc = JsonDocument.Parse(fs);
                var root = doc.RootElement;

                if (root.TryGetProperty("mana", out var manaObj) && manaObj.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in manaObj.EnumerateObject())
                    {
                        var key = prop.Name;
                        if (prop.Value.ValueKind == JsonValueKind.Array)
                        {
                            var arr = prop.Value.EnumerateArray().Select(e => e.GetInt32()).ToArray();
                            if (arr.Length >= 1)
                            {
                                _manaPorNivel[key] = arr;
                                // Si tenemos PL1 y coincide con JSON, mantener; si no existe en _manaPL1, usar primer valor como PL1
                                if (!_manaPL1.ContainsKey(key) && arr.Length > 0)
                                    _manaPL1[key] = arr[0];
                            }
                        }
                    }
                }

                if (root.TryGetProperty("diff", out var diffObj) && diffObj.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in diffObj.EnumerateObject())
                    {
                        var key = prop.Name;
                        if (prop.Value.ValueKind == JsonValueKind.Number)
                        {
                            _diffBase[key] = prop.Value.GetInt32();
                        }
                    }
                }
            }
            catch
            {
                // Fallback silencioso a tablas embebidas
            }
        }

        private void RenderSimbolos(string simbolos)
        {
            pnlSimbolos.Controls.Clear();
            var partes = simbolos.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length == 0) return;

            // Poder destacado
            AgregarIcono(partes[0], 40, true);
            pnlSimbolos.Controls.Add(new Label { Text = "→", AutoSize = true, Padding = new Padding(8, 18, 8, 0) });

            foreach (var s in partes.Skip(1))
                AgregarIcono(s, 32, false);
        }

        private void AgregarIcono(string simbolo, int size, bool destacado)
        {
            // Contenedor fijo para cuadrar y centrar imagen + etiqueta
            var panel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = destacado ? Color.Gainsboro : SystemColors.Control,
                Width = size + 16,
                Height = size + 32,
                Margin = new Padding(6)
            };

            // Subcontenedor para la imagen cuadrada
            var imgBoxHost = new Panel
            {
                Width = size,
                Height = size,
                Top = 4,
                Left = 8, // (panel.Width - size) / 2 = 8 cuando panel.Width = size+16
                BackColor = Color.White
            };
            var pb = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Transparent };
            imgBoxHost.Controls.Add(pb);

            // Intentar distintas extensiones y capitalizaciones
            var baseDir = Imagenes.BaseImgPath();
            string lower = simbolo.ToLowerInvariant();
            string upper = simbolo.ToUpperInvariant();
            var candidatos = new[]
            {
                $"{simbolo}.png", $"{simbolo}.gif", $"{simbolo}.PNG", $"{simbolo}.GIF",
                $"{lower}.png", $"{lower}.gif", $"{upper}.png", $"{upper}.gif"
            };
            string? encontrado = candidatos
                .Select(f => Path.Combine(baseDir, f))
                .FirstOrDefault(File.Exists);
            if (encontrado != null)
            {
                var img = Imagenes.CargarImagenSegura(encontrado);
                Imagenes.ReemplazarImagen(pb, img);
            }
            else
            {
                // Fallback a texto centrado si no hay imagen
                var textLbl = new Label { Text = simbolo, AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
                imgBoxHost.Controls.Clear();
                imgBoxHost.Controls.Add(textLbl);
            }

            panel.Controls.Add(imgBoxHost);
            var lbl = new Label { Text = simbolo, AutoSize = false, Width = panel.Width, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 8) };
            lbl.Top = imgBoxHost.Bottom + 2;
            panel.Controls.Add(lbl);
            toolTip1.SetToolTip(panel, TooltipDe(simbolo));
            pnlSimbolos.Controls.Add(panel);
        }

        private string TooltipDe(string simbolo)
        {
            var descEs = _simboloDescripcion.GetValueOrDefault(simbolo, "Símbolo desconocido");
            var fam = _simboloFamilia.GetValueOrDefault(simbolo, _idiomaActual == Idioma.EN ? "Unknown family" : "Familia desconocida");
            if (_idiomaActual == Idioma.EN)
            {
                // Traducciones simples para tooltip (familias ya están en EN)
                var descEn = _trEfecto.TryGetValue(descEs, out var d) ? d : descEs;
                return $"{simbolo}: {descEn} • {fam}";
            }
            return $"{simbolo}: {descEs} • {fam}";
        }

        

        private bool EsPocion(string hechizo)
        {
            // Considera poción si el mapeo devuelve un frasco distinto de 'vacia.png'
            // y no es un objeto
            var (archivo, esObjeto) = MapearImagenFrasco(hechizo);
            return !esObjeto && !string.Equals(archivo, "vacia.png", StringComparison.OrdinalIgnoreCase);
        }

        private void CargarImagenFrasco(string nombreArchivo, bool esObjeto = false)
        {
            // Determinar el directorio base según si es un objeto o una poción
            var subcarpeta = esObjeto ? "objetos" : "posiones";
            var baseDir = Path.Combine(Imagenes.BaseImgPath(), subcarpeta);

            string[] extensiones = new[] { ".png", ".gif", ".bmp", ".jpg", ".jpeg" };
            var ruta = Imagenes.BuscarImagen(baseDir, nombreArchivo, extensiones);
            if (ruta != null)
            {
                var img = Imagenes.CargarImagenSegura(ruta);
                Imagenes.ReemplazarImagen(picFrasco, img);
                return;
            }

            // Si no se encuentra la imagen, limpiar
            Imagenes.ReemplazarImagen(picFrasco, null);
        }

        private (string archivo, bool esObjeto) MapearImagenFrasco(string hechizo)
        {
            // Primero verificamos si es un objeto
            var objeto = MapearObjeto(hechizo);
            if (objeto.archivo != "vacia.png")
            {
                return objeto;
            }
            
            // Si no es un objeto, verificamos si es una poción
            return hechizo switch
            {
                // Pociones (van en la carpeta posiones/)
                "Poción de Salud" => ("Health.gif", false),
                "Poción de Energía" => ("Stamina.png", false),
                "Poción de Maná" => ("Mana.gif", false),
                "Poción de Fuerza" => ("Strength.gif", false),
                "Poción de Destreza" => ("Dexterity.gif", false),
                "Poción de Sabiduría" => ("Wisdom.gif", false),
                "Poción de Vitalidad" => ("Vitality.gif", false),
                "Poción de Escudo" => ("Shield.gif", false),
                "Curar Veneno" => ("veneno.gif", false),
                
                // Por defecto
                _ => ("vacia.png", false)
            };
        }
        
        private (string archivo, bool esObjeto) MapearObjeto(string nombreObjeto)
        {
            return nombreObjeto switch
            {
                "Antorcha" => ("antorcha", true),
                "Esbirro de Ataque" => ("esbirro", true),
                "Guardia Esbirro" => ("guardia.png", true),
                "Transportar Esbirro" => ("guardia.png",true),
                "Escudo de Fuego" => ("escudofuego.png", true),
                "Escudo Grupal" => ("escudogrupal.png", true),
                "Aura de Fuerza" => ("aurafuerza.png", true),
                "Aura de Destreza" => ("auradestreza.png", true),
                "Aura de Vitalidad" => ("auravitalidad.png", true),
                "Aura de Sabiduría" => ("aurasaviduria.png", true),
                "Escudo Mágico" => ("escudomagico.png", true),
                "Reflejo de Hechizos" => ("reflejo.png", true),
                "Dardo Venenoso" => ("dardovenenoso.png", true),
                "Nube Venenosa" => ("nubevenenosa.png", true),
                "Debilitar Seres Inmateriales" => ("inmaterial.png", true),
                "Bola de Fuego" => ("bola.png", true),
                "Rayo" => ("rayo.png", true),
                "Marca Mágica" => ("marca.png", true),
                "Abrir Puerta" => ("abrir.png", true),
                "Empujar" => ("empujar.png", true),
                "Atraer" => ("atraer.png", true),

                // Agregar más objetos aquí cuando sea necesario
                // Ejemplo:
                // "Llave" => ("llave", true),
                _ => ("vacia.png", false)  // Valor por defecto si no coincide ningún objeto
            };
        }

        private void InicializarMenu()
        {
            // Crear la barra de menú
            menuPrincipal = new MenuStrip();
            
            // Menú de Utilidades
            menuUtilidades = new ToolStripMenuItem("Utilidades");
            
            // Opción de Calculadora de Monedas
            menuCalculadoraMonedas = new ToolStripMenuItem("Calculadora de Monedas");
            menuCalculadoraMonedas.Click += (s, e) =>
            {
                var monedasForm = new MonedasForm(_idiomaActual == Idioma.EN);
                monedasForm.ShowDialog();
            };
            
            // Opción de Criaturas
            menuCriaturas = new ToolStripMenuItem(_idiomaActual == Idioma.EN ? "Creatures" : "Criaturas");
            menuCriaturas.Click += (s, e) =>
            {
                var frm = new CriaturasForm();
                frm.ShowDialog();
            };
            
            // Agregar opciones al menú
            menuUtilidades.DropDownItems.Add(menuCalculadoraMonedas);
            menuUtilidades.DropDownItems.Add(menuCriaturas);
            
            // Menú de Idioma
            menuIdioma = new ToolStripMenuItem("Idioma");
            menuEs = new ToolStripMenuItem("Español");
            menuEn = new ToolStripMenuItem("English");
            menuEs.Checked = true;
            menuEs.Click += (s, e) => { _idiomaActual = Idioma.ES; menuEs.Checked = true; menuEn.Checked = false; AplicarIdioma(); Settings.Default.Idioma = "ES"; Settings.Default.Save(); };
            menuEn.Click += (s, e) => { _idiomaActual = Idioma.EN; menuEn.Checked = true; menuEs.Checked = false; AplicarIdioma(); Settings.Default.Idioma = "EN"; Settings.Default.Save(); };
            menuIdioma.DropDownItems.Add(menuEs);
            menuIdioma.DropDownItems.Add(menuEn);

            CargarIconosIdioma();

            // Menú de Ayuda > Acerca de
            menuAyuda = new ToolStripMenuItem("Ayuda");
            menuAcerca = new ToolStripMenuItem("Acerca de...");
            // Alinear el menú Ayuda a la derecha
            menuAyuda.Alignment = ToolStripItemAlignment.Right;
            menuAcerca.Click += (s, e) =>
            {
                var ver = Application.ProductVersion;
                MessageBox.Show(string.Format(T("About.Message"), ver), T("About.Title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            menuAyuda.DropDownItems.Add(menuAcerca);

            // Agregar menú principal
            menuPrincipal.Items.Add(menuUtilidades);
            menuPrincipal.Items.Add(menuIdioma);
            menuPrincipal.Items.Add(menuAyuda);
            
            // Asignar el menú al formulario
            this.MainMenuStrip = menuPrincipal;
            this.Controls.Add(menuPrincipal);
        }

        private void CargarIconosIdioma()
        {
            try
            {
                var dir = Path.Combine(Imagenes.BaseImgPath(), "idiomas");
                var esPng = Path.Combine(dir, "es.png");
                var gbPng = Path.Combine(dir, "gb.png");
                Image? LoadSmall(string path)
                {
                    if (!File.Exists(path)) return null;
                    using var img = Image.FromFile(path);
                    return new Bitmap(img, new Size(18, 12));
                }
                var imgEs = LoadSmall(esPng);
                var imgGb = LoadSmall(gbPng);
                if (imgEs != null) menuEs.Image = imgEs;
                if (imgGb != null) menuEn.Image = imgGb;
            }
            catch { }
        }

        private void AplicarIdioma()
        {
            // Textos de menú
            menuUtilidades.Text = T("Menu.Utilities");
            menuCalculadoraMonedas.Text = T("Menu.CurrencyCalculator");
            menuCriaturas.Text = _idiomaActual == Idioma.EN ? "Creatures" : "Criaturas";
            menuIdioma.Text = T("Menu.Language");
            menuEs.Text = T("Menu.Spanish");
            menuEn.Text = T("Menu.English");
            if (menuAyuda != null) menuAyuda.Text = T("Menu.Help");
            if (menuAcerca != null) menuAcerca.Text = T("Menu.About");

            // Controles de selección (group/labels/botón) definidos en Designer: reasignar textos
            gbPoder.Text = T("UI.PowerLevel");
            gbSeleccion.Text = T("UI.SpellSelection");
            lblClase.Text = T("UI.SelectClass");
            lblHechizo.Text = T("UI.SelectSpell");
            btnMostrar.Text = T("UI.ShowSpell");

            // Repoblar combos con visualización traducida pero manteniendo claves ES
            var claseSeleccionadaKey = (cbClase.SelectedItem as ComboItem)?.KeyEs;
            cbClase.Items.Clear();
            foreach (var claseEs in _hechizos.Keys)
            {
                var display = _idiomaActual == Idioma.EN && _trClases.TryGetValue(claseEs, out var en) ? en : claseEs;
                cbClase.Items.Add(new ComboItem(claseEs, display));
            }
            if (cbClase.Items.Count > 0)
            {
                int idx = 0;
                if (claseSeleccionadaKey != null)
                {
                    idx = cbClase.Items.Cast<ComboItem>().ToList().FindIndex(i => i.KeyEs == claseSeleccionadaKey);
                    if (idx < 0) idx = 0;
                }
                cbClase.SelectedIndex = idx;
            }

            // Repoblar hechizos para la clase actual
            cbClase_SelectedIndexChanged(null, EventArgs.Empty);
        }
    }
}
