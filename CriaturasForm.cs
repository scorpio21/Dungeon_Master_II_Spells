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

            txtBuscar = new TextBox { PlaceholderText = _en ? "Search..." : "Buscar...", Dock = DockStyle.Top, Margin = new Padding(8) };
            cbHabitat = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(8) };
            lst = new ListBox { Dock = DockStyle.Left, Width = 320 };            

            var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            lblNombre = new Label { AutoSize = true, Font = new Font("Segoe UI", 16, FontStyle.Bold) };
            lblHabitat = new Label { AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Italic) };
            pic = new PictureBox { SizeMode = PictureBoxSizeMode.Zoom, Width = 320, Height = 320, BorderStyle = BorderStyle.FixedSingle };
            lblStats = new Label { AutoSize = true, Font = new Font("Segoe UI", 9), MaximumSize = new Size(600, 0) };
            lblHabs = new Label { AutoSize = true, Font = new Font("Segoe UI", 9), MaximumSize = new Size(600, 0) };
            lblNotes = new Label { AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Italic), ForeColor = Color.DimGray, MaximumSize = new Size(600, 0) };

            var filterPanel = new Panel { Dock = DockStyle.Top, Height = 56, Padding = new Padding(8) };
            var lblFiltro = new Label { Text = _en ? "Habitat:" : "Hábitat:", AutoSize = true, Left = 8, Top = 8 };
            cbHabitat.Left = lblFiltro.Right + 8; cbHabitat.Top = 4; cbHabitat.Width = 200;
            txtBuscar.Top = 4; txtBuscar.Left = cbHabitat.Right + 12; txtBuscar.Width = 300;
            filterPanel.Controls.Add(lblFiltro);
            filterPanel.Controls.Add(cbHabitat);
            filterPanel.Controls.Add(txtBuscar);

            var leftPanel = new Panel { Dock = DockStyle.Left, Width = 320 };
            leftPanel.Controls.Add(lst);
            leftPanel.Controls.Add(filterPanel);

            var stack = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true };
            stack.Controls.Add(lblNombre);
            stack.Controls.Add(lblHabitat);
            stack.Controls.Add(pic);
            stack.Controls.Add(lblStats);
            stack.Controls.Add(lblHabs);
            stack.Controls.Add(lblNotes);
            rightPanel.Controls.Add(stack);

            Controls.Add(rightPanel);
            Controls.Add(leftPanel);

            Load += (_, __) => CargarDatos();
            txtBuscar.TextChanged += (_, __) => RefrescarLista();
            cbHabitat.SelectedIndexChanged += (_, __) => RefrescarLista();
            lst.SelectedIndexChanged += (_, __) => MostrarSeleccion();
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
            }
            catch (Exception ex)
            {
                MessageBox.Show((_en ? "Error loading creatures: " : "Error cargando criaturas: ") + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

            string imgDir = Path.Combine(AppContext.BaseDirectory, "img", "criaturas");
            string imgPath = Path.Combine(imgDir, c.Image);
            pic.Image = File.Exists(imgPath) ? Image.FromFile(imgPath) : null;

            lblStats.Text = FormatearStats(c.Stats);
            var ab = c.SpecialAbilities.Get(_en);
            lblHabs.Text = ($"{(_en ? "Special Abilities" : "Habilidades especiales")}: " + (string.IsNullOrWhiteSpace(ab) ? "-" : ab)).Trim();
            var notes = c.Notes.Get(_en);
            lblNotes.Text = string.IsNullOrWhiteSpace(notes) ? "" : ($"{(_en ? "Notes" : "Notas")}: " + notes);
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
