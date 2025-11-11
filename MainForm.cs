using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SpellBookWinForms
{
    public partial class MainForm : Form
    {
        private readonly Dictionary<string, Dictionary<string, (string Simbolos, string Efecto)>> _hechizos;
        private readonly Dictionary<string, (int mana, int diff)> _valoresPL1;
        private readonly Dictionary<string, string> _simboloDescripcion;
        private readonly Dictionary<string, string> _simboloFamilia;
        private readonly Dictionary<string, int> _poderMap = new() { {"Lo",1},{"Um",2},{"On",3},{"Ee",4},{"Pal",5},{"Mon",6} };

        private MenuStrip menuPrincipal = null!;  // Inicialización no nula asegurada en el constructor
        private ToolStripMenuItem menuUtilidades = null!;
        private ToolStripMenuItem menuCalculadoraMonedas = null!;

        public MainForm()
        {
            InitializeComponent();
            InicializarMenu();

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


            _valoresPL1 = new()
            {
                // Poder (se suma nivel directamente)
                ["Lo"]=(1,1), ["Um"]=(2,2), ["On"]=(3,3), ["Ee"]=(4,4), ["Pal"]=(5,5), ["Mon"]=(6,6),
                // Elemental influence
                ["Ya"]=(2,2), ["Vi"]=(3,3), ["Oh"]=(4,4), ["Ful"]=(5,5), ["Des"]=(6,6), ["Zo"]=(7,7),
                // Form
                ["Ven"]=(4,4), ["Ew"]=(5,5), ["Kath"]=(6,6), ["Ir"]=(7,7), ["Bro"]=(7,7), ["Gor"]=(9,9),
                // Class / Alignment
                ["Ku"]=(2,2), ["Ros"]=(2,2), ["Dain"]=(3,3), ["Neta"]=(4,4), ["Ra"]=(6,6), ["Sar"]=(7,7),
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

            // Inicializar combos
            cbClase.Items.AddRange(_hechizos.Keys.Cast<object>().ToArray());
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
        }

        private void cbClase_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cbClase.SelectedItem is not string clase) return;
            cbHechizo.Items.Clear();
            foreach (var h in _hechizos[clase].Keys)
                cbHechizo.Items.Add(h);
            if (cbHechizo.Items.Count > 0) cbHechizo.SelectedIndex = 0;
            LimpiarResultados();
        }

        private void cbHechizo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            LimpiarResultados();
            
            // Solo mostrar el frasco vacío si es una poción
            if (cbHechizo.SelectedItem is string hechizo && EsPocion(hechizo))
            {
                var (imagenHechizo, _) = MapearImagenFrasco(hechizo);
                CargarImagenFrasco(imagenHechizo, esObjeto: false);
            }
            else
            {
                // Para otros hechizos, incluyendo objetos como la antorcha,
                // no mostramos la imagen hasta que se haga clic en "Mostrar Hechizo"
                picFrasco.Image = null;
            }
        }

        private void btnMostrar_Click(object? sender, EventArgs e)
        {
            if (cbClase.SelectedItem is not string clase || cbHechizo.SelectedItem is not string hechizo) return;
            var datos = _hechizos[clase][hechizo];
            var poder = ObtenerPoderSeleccionado();
            var simbolos = $"{poder} {datos.Simbolos}";

            var (mana, diff, detalles) = Calcular(simbolos);
            lblResultado.Text = $"🔮 Hechizo: {hechizo}\n✨ Efecto: {datos.Efecto}\n🧪 Símbolos: {simbolos}\n🔋 Mana total: {mana}\n🎯 Dificultad total: {diff}";

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
            detalles.Add($"{partes[0]}: Mana {nivel}, Dificultad {nivel} — Familia: {_simboloFamilia.GetValueOrDefault(partes[0], "Power")}");

            double factor = (nivel + 1) / 2.0;
            foreach (var s in partes.Skip(1))
            {
                // Desestructurar para no perder los nombres de tupla al usar valores por defecto
                (int baseMana, int baseDiff) = _valoresPL1.TryGetValue(s, out var tmp)
                    ? tmp
                    : (0, 0);
                int m = (int)Math.Floor(baseMana * factor);
                int d = (int)Math.Floor(baseDiff * factor);
                mana += m; diff += d;
                detalles.Add($"{s}: Mana {m}, Dificultad {d} — Familia: {_simboloFamilia.GetValueOrDefault(s, "Desconocido")}");
            }
            return (mana, diff, detalles);
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
            var baseDir = BaseImgPath();
            string lower = simbolo.ToLowerInvariant();
            string upper = simbolo.ToUpperInvariant();
            string[] posibles = new[]
            {
                Path.Combine(baseDir, $"{simbolo}.png"),
                Path.Combine(baseDir, $"{simbolo}.PNG"),
                Path.Combine(baseDir, $"{simbolo}.gif"),
                Path.Combine(baseDir, $"{simbolo}.GIF"),
                Path.Combine(baseDir, $"{lower}.png"),
                Path.Combine(baseDir, $"{lower}.gif"),
                Path.Combine(baseDir, $"{upper}.png"),
                Path.Combine(baseDir, $"{upper}.gif"),
            };
            string? encontrado = posibles.FirstOrDefault(File.Exists);
            if (encontrado != null)
            {
                pb.Image = Image.FromFile(encontrado);
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
            var desc = _simboloDescripcion.GetValueOrDefault(simbolo, "Símbolo desconocido");
            var fam = _simboloFamilia.GetValueOrDefault(simbolo, "Familia desconocida");
            return $"{simbolo}: {desc} • {fam}";
        }

        private string BaseImgPath()
        {
            // 1) Priorizar la carpeta img del proyecto: L:\Magias\SpellBookWinForms\img
            var exeDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory); // ...\bin\Debug\net8.0-windows
            var projectDir = exeDir.Parent?.Parent?.Parent; // ...\SpellBookWinForms
            if (projectDir != null)
            {
                var projectImg = Path.Combine(projectDir.FullName, "img");
                if (Directory.Exists(projectImg)) return projectImg;
            }

            // 2) Si no existe, buscar una carpeta 'img' ascendiendo
            var dir = exeDir;
            for (int i = 0; i < 8 && dir != null; i++)
            {
                var intento = Path.Combine(dir.FullName, "img");
                if (Directory.Exists(intento)) return intento;

                var intentoMagias = Path.Combine(dir.FullName, "Magias", "img");
                if (Directory.Exists(intentoMagias)) return intentoMagias;

                dir = dir.Parent;
            }

            // 3) Fallback conocido
            var alt = @"L:\\Magias\\img";
            if (Directory.Exists(alt)) return alt;

            // 4) Último recurso
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");
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
            var baseDir = Path.Combine(BaseImgPath(), subcarpeta);
            
            // Buscar la imagen con diferentes extensiones
            string[] extensiones = { "", ".png", ".gif", ".bmp", ".jpg", ".jpeg" };
            
            foreach (var ext in extensiones)
            {
                string rutaCompleta = Path.Combine(baseDir, $"{nombreArchivo}{ext}");
                if (File.Exists(rutaCompleta))
                {
                    picFrasco.Image = Image.FromFile(rutaCompleta);
                    return;
                }
            }
            
            // Si no se encuentra la imagen, limpiar
            picFrasco.Image = null;
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
                // Agregar más objetos aquí cuando sea necesario
                // Ejemplo:
                // "Llave" => ("llave", true),
                _ => (string.Empty, false)  // Valor por defecto si no coincide ningún objeto
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
                var monedasForm = new MonedasForm();
                monedasForm.ShowDialog();
            };
            
            // Agregar opciones al menú
            menuUtilidades.DropDownItems.Add(menuCalculadoraMonedas);
            
            // Agregar menú principal
            menuPrincipal.Items.Add(menuUtilidades);
            
            // Asignar el menú al formulario
            this.MainMenuStrip = menuPrincipal;
            this.Controls.Add(menuPrincipal);
        }
    }
}
