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
    public class CriaturasForm : Form
    {
        private TextBox txtBuscar = null!;
        private ComboBox cbHabitat = null!;
        private ListBox lst = null!;
        private PictureBox pic = null!;
        private Label lblNombre = null!;
        private Label lblHabitat = null!;
        private Label lblStats = null!;
        private Label lblHabs = null!;
        private Label lblNotes = null!;
        private Button btnSalir = null!;

        // DPS / golpes necesarios
        private ComboBox cbHechizoDps = null!;
        private ComboBox cbPoderDps = null!;
        private Label lblDps = null!;
        private readonly string[] _niveles = new[] { "Lo", "Um", "On", "Ee", "Pal", "Mon" };
        private Dictionary<string, int[]> _spellDamage = new();
        private int _resScale = 20;

        private List<Creature> _all = new();
        private bool _en => string.Equals(Settings.Default.Idioma, "EN", StringComparison.OrdinalIgnoreCase);

        private sealed class CreatureStats
        {
            [JsonPropertyName("hp")] public int Hp { get; set; }
            [JsonPropertyName("weight")] public int Weight { get; set; }
            [JsonPropertyName("attackStrength")] public int AttackStrength { get; set; }
            [JsonPropertyName("armorStrength")] public int ArmorStrength { get; set; }
            [JsonPropertyName("dexterity")] public int Dexterity { get; set; }
            [JsonPropertyName("attackFerocity")] public int AttackFerocity { get; set; }
            [JsonPropertyName("retreatFerocity")] public int RetreatFerocity { get; set; }
            [JsonPropertyName("poisonStrength")] public int PoisonStrength { get; set; }
            [JsonPropertyName("magicResistance")] public int MagicResistance { get; set; }
            [JsonPropertyName("fireResistance")] public int FireResistance { get; set; }
            [JsonPropertyName("poisonResistance")] public int PoisonResistance { get; set; }
        }
        private sealed class Localized
        {
            [JsonPropertyName("es")] public string Es { get; set; } = "";
            [JsonPropertyName("en")] public string En { get; set; } = "";
            public string Get(bool en) => en ? En : Es;
        }
        private sealed class Creature
        {
            [JsonPropertyName("id")] public string Id { get; set; } = "";
            [JsonPropertyName("name")] public Localized Name { get; set; } = new();
            [JsonPropertyName("habitat")] public Localized Habitat { get; set; } = new();
            [JsonPropertyName("stats")] public CreatureStats Stats { get; set; } = new();
            [JsonPropertyName("specialAbilities")] public Localized SpecialAbilities { get; set; } = new();
            [JsonPropertyName("image")] public string Image { get; set; } = "";
            [JsonPropertyName("notes")] public Localized Notes { get; set; } = new();
        }
        private sealed class CreatureRoot
        {
            [JsonPropertyName("creatures")] public List<Creature> Creatures { get; set; } = new();
        }

        public CriaturasForm()
        {
            Text = _en ? "Creatures" : "Criaturas";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(980, 640);
            MinimizeBox = false;
            MaximizeBox = true;

            txtBuscar = new TextBox { PlaceholderText = _en ? "Search..." : "Buscar...", Dock = DockStyle.None, Margin = new Padding(6, 2, 6, 2), Width = 220 };
            cbHabitat = new ComboBox { Dock = DockStyle.None, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(8) };
            lst = new ListBox { Dock = DockStyle.Fill };            

            var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            lblNombre = new Label { AutoSize = true, Font = new Font("Segoe UI", 16, FontStyle.Bold) };
            lblHabitat = new Label { AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Italic) };
            pic = new PictureBox { SizeMode = PictureBoxSizeMode.Zoom, Width = 320, Height = 320, BorderStyle = BorderStyle.FixedSingle };
            lblStats = new Label { AutoSize = true, Font = new Font("Segoe UI", 9), MaximumSize = new Size(600, 0) };
            lblHabs = new Label { AutoSize = true, Font = new Font("Segoe UI", 9), MaximumSize = new Size(600, 0) };
            lblNotes = new Label { AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Italic), ForeColor = Color.DimGray, MaximumSize = new Size(600, 0) };

            var filterPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 72, Padding = new Padding(8), FlowDirection = FlowDirection.LeftToRight, WrapContents = true, AutoSize = false };
            var lblFiltro = new Label { Text = _en ? "Habitat:" : "Hábitat:", AutoSize = true, Margin = new Padding(0, 6, 6, 0) };
            cbHabitat.DropDownStyle = ComboBoxStyle.DropDownList; cbHabitat.Width = 200; cbHabitat.Margin = new Padding(0, 2, 8, 2);
            btnSalir = new Button { Text = _en ? "Close" : "Salir", Width = 90, Margin = new Padding(8, 2, 0, 2) };
            btnSalir.Click += (_, __) => Close();
            filterPanel.Controls.Add(lblFiltro);
            filterPanel.Controls.Add(cbHabitat);
            filterPanel.Controls.Add(txtBuscar);
            filterPanel.Controls.Add(btnSalir);

            var leftPanel = new Panel { Dock = DockStyle.Left, Width = 420 };
            // Orden: primero la lista (Fill), luego el filtro (Top)
            leftPanel.Controls.Add(lst);
            filterPanel.Dock = DockStyle.Top;
            leftPanel.Controls.Add(filterPanel);

            var stack = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true };
            stack.Controls.Add(lblNombre);
            stack.Controls.Add(lblHabitat);
            stack.Controls.Add(pic);
            stack.Controls.Add(lblStats);
            stack.Controls.Add(lblHabs);
            stack.Controls.Add(lblNotes);

            // Bloque de cálculo de golpes
            var dpsPanel = new FlowLayoutPanel { Dock = DockStyle.Top, FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            var dpsTitulo = new Label { AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), Text = _en ? "Hits to kill" : "Golpes necesarios" };
            cbHechizoDps = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 140 };
            cbHechizoDps.Items.AddRange(new object[] { _en ? "Fireball" : "Bola de Fuego", _en ? "Lightning" : "Rayo" });
            cbHechizoDps.SelectedIndex = 0;
            cbPoderDps = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 90 };
            cbPoderDps.Items.AddRange(_niveles);
            cbPoderDps.SelectedIndex = 5; // Mon por defecto
            lblDps = new Label { AutoSize = true, Font = new Font("Segoe UI", 9), Margin = new Padding(12, 6, 0, 0) };
            dpsPanel.Controls.Add(dpsTitulo);
            dpsPanel.Controls.Add(cbHechizoDps);
            dpsPanel.Controls.Add(cbPoderDps);
            dpsPanel.Controls.Add(lblDps);
            stack.Controls.Add(dpsPanel);
            rightPanel.Controls.Add(stack);

            Controls.Add(rightPanel);
            Controls.Add(leftPanel);

            Load += (_, __) => CargarDatos();
            txtBuscar.TextChanged += (_, __) => RefrescarLista();
            cbHabitat.SelectedIndexChanged += (_, __) => RefrescarLista();
            lst.SelectedIndexChanged += (_, __) => MostrarSeleccion();
            cbHechizoDps.SelectedIndexChanged += (_, __) => ActualizarDps();
            cbPoderDps.SelectedIndexChanged += (_, __) => ActualizarDps();
        }

        private void CargarDatos()
        {
            try
            {
                var ruta = Path.Combine(AppContext.BaseDirectory, "data", "criaturas.json");
                if (!File.Exists(ruta))
                {
                    var rutaAlt = Path.Combine(Directory.GetCurrentDirectory(), "data", "criaturas.json");
                    if (File.Exists(rutaAlt)) ruta = rutaAlt; else throw new FileNotFoundException(ruta);
                }
                using var fs = File.OpenRead(ruta);
                var root = JsonSerializer.Deserialize<CreatureRoot>(fs, new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true
                });
                _all = root?.Creatures ?? new List<Creature>();

                var habitats = _all
                    .Select(c => (string.IsNullOrWhiteSpace(c.Habitat.Es) && string.IsNullOrWhiteSpace(c.Habitat.En)) ? "" : c.Habitat.Get(_en))
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();
                habitats.Insert(0, _en ? "All" : "Todos");
                cbHabitat.Items.Clear();
                cbHabitat.Items.AddRange(habitats.Cast<object>().ToArray());
                if (cbHabitat.Items.Count > 0) cbHabitat.SelectedIndex = 0;

                RefrescarLista();

                // Cargar tabla de daño por hechizo
                CargarSpellDamage();
            }
            catch (Exception ex)
            {
                MessageBox.Show((_en ? "Error loading creatures: " : "Error cargando criaturas: ") + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarSpellDamage()
        {
            try
            {
                var ruta = Path.Combine(AppContext.BaseDirectory, "data", "spell_damage.json");
                if (!File.Exists(ruta))
                {
                    var rutaAlt = Path.Combine(Directory.GetCurrentDirectory(), "data", "spell_damage.json");
                    if (File.Exists(rutaAlt)) ruta = rutaAlt; else return;
                }
                using var fs = File.OpenRead(ruta);
                using var doc = JsonDocument.Parse(fs);
                _spellDamage.Clear();
                if (doc.RootElement.TryGetProperty("Fireball", out var fb) && fb.ValueKind == JsonValueKind.Array)
                    _spellDamage["Fireball"] = fb.EnumerateArray().Select(e => e.GetInt32()).ToArray();
                if (doc.RootElement.TryGetProperty("Lightning", out var lt) && lt.ValueKind == JsonValueKind.Array)
                    _spellDamage["Lightning"] = lt.EnumerateArray().Select(e => e.GetInt32()).ToArray();
                if (doc.RootElement.TryGetProperty("resistanceScale", out var rs) && rs.ValueKind == JsonValueKind.Number)
                    _resScale = rs.GetInt32();
            }
            catch { }
        }

        private void RefrescarLista()
        {
            var filtroTxt = (txtBuscar.Text ?? string.Empty).Trim().ToLowerInvariant();
            var filtroHab = cbHabitat.SelectedItem?.ToString();
            bool allHab = string.IsNullOrWhiteSpace(filtroHab) || filtroHab == (_en ? "All" : "Todos");

            var lista = _all
                .Where(c => allHab || string.Equals(c.Habitat.Get(_en), filtroHab, StringComparison.OrdinalIgnoreCase))
                .Where(c => string.IsNullOrWhiteSpace(filtroTxt) || c.Name.Get(_en).ToLowerInvariant().Contains(filtroTxt))
                .OrderBy(c => c.Name.Get(_en))
                .ToList();

            lst.BeginUpdate();
            lst.Items.Clear();
            foreach (var c in lista)
                lst.Items.Add(new Item(c));
            lst.EndUpdate();
            if (lst.Items.Count > 0) lst.SelectedIndex = 0;
        }

        private void MostrarSeleccion()
        {
            if (lst.SelectedItem is not Item it) return;
            var c = it.Value;
            lblNombre.Text = c.Name.Get(_en);
            lblHabitat.Text = ($"{(_en ? "Habitat" : "Hábitat")}: " + c.Habitat.Get(_en)).Trim();

            string imgDir = Path.Combine(Imagenes.BaseImgPath(), "criaturas");
            string? ruta = Imagenes.BuscarImagen(imgDir, c.Image, ".png", ".gif", ".jpg", ".jpeg");
            var img = ruta != null ? Imagenes.CargarImagenSegura(ruta) : null;
            Imagenes.ReemplazarImagen(pic, img);

            lblStats.Text = FormatearStats(c.Stats);
            var ab = c.SpecialAbilities.Get(_en);
            lblHabs.Text = ($"{(_en ? "Special Abilities" : "Habilidades especiales")}: " + (string.IsNullOrWhiteSpace(ab) ? "-" : ab)).Trim();
            var notes = c.Notes.Get(_en);
            lblNotes.Text = string.IsNullOrWhiteSpace(notes) ? "" : ($"{(_en ? "Notes" : "Notas")}: " + notes);

            ActualizarDps();
        }

        private void ActualizarDps()
        {
            if (lst.SelectedItem is not Item it) { lblDps.Text = string.Empty; return; }
            var c = it.Value;
            if (cbHechizoDps.SelectedIndex < 0 || cbPoderDps.SelectedIndex < 0) { lblDps.Text = string.Empty; return; }

            string spellKey = cbHechizoDps.SelectedIndex == 0 ? "Fireball" : "Lightning";
            if (!_spellDamage.TryGetValue(spellKey, out var tabla) || tabla.Length < 6) { lblDps.Text = string.Empty; return; }
            int idx = Math.Clamp(cbPoderDps.SelectedIndex, 0, 5);
            double baseDmg = tabla[idx];

            int res = 0;
            if (spellKey == "Fireball") res = c.Stats.FireResistance;
            else if (spellKey == "Lightning") res = c.Stats.MagicResistance;

            double factor = 1.0 - (Math.Max(0, res) / (double)Math.Max(1, _resScale));
            if (factor < 0.05) factor = 0.05; // mínimo 5% de daño
            double dmg = Math.Max(1.0, baseDmg * factor);
            int hits = (int)Math.Ceiling(c.Stats.Hp / dmg);

            string spellDisplay = _en ? spellKey : (spellKey == "Fireball" ? "Bola de Fuego" : "Rayo");
            string outText = _en ? $"{spellDisplay} ({_niveles[idx]}): {hits} hits" : $"{spellDisplay} ({_niveles[idx]}): {hits} golpes";
            lblDps.Text = outText;
        }

        private string FormatearStats(CreatureStats s)
        {
            if (s == null) return string.Empty;
            if (_en)
            {
                return $"HP {s.Hp}  •  Wt {s.Weight}\nAtk {s.AttackStrength}  •  Arm {s.ArmorStrength}\nDex {s.Dexterity}  •  Fero {s.AttackFerocity}/{s.RetreatFerocity}\nRes: Mag {s.MagicResistance}  Fire {s.FireResistance}  Pois {s.PoisonResistance}";
            }
            return $"HP {s.Hp}  •  Peso {s.Weight}\nAtaque {s.AttackStrength}  •  Armadura {s.ArmorStrength}\nDestreza {s.Dexterity}  •  Ferocidad {s.AttackFerocity}/{s.RetreatFerocity}\nResist.: Mag {s.MagicResistance}  Fuego {s.FireResistance}  Ven {s.PoisonResistance}";
        }

        private sealed class Item
        {
            public Creature Value { get; }
            public Item(Creature c) { Value = c; }
            public override string ToString() => Value.Name.Get(string.Equals(Settings.Default.Idioma, "EN", StringComparison.OrdinalIgnoreCase));
        }
    }
}
