using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Resources;

namespace SpellBookWinForms
{
    public class MonedasForm : Form
    {
        private readonly bool _en; // true = inglés (EN), false = español (ES)
        private readonly ResourceManager _rm = new ResourceManager("SpellBookWinForms.Resources.Strings", typeof(MonedasForm).Assembly);
        private string T(string key)
        {
            var ci = _en ? new CultureInfo("en") : new CultureInfo("es");
            return _rm.GetString(key, ci) ?? key;
        }
        // Valores base en Copper Coins
        private readonly Dictionary<string, (int valor, string imagen)> monedas = new()
        {
            ["Moneda de Cobre"] = (1, "Copper_Coin.png"),
            ["Moneda de Plata"] = (4, "Silver_Coin.png"),
            ["Moneda de Oro"] = (16, "oro.png"),
            ["Gema Verde"] = (64, "Green_Gem.png"),
            ["Gema Roja"] = (256, "Red_Gem.png"),
            ["Gema Azul"] = (1024, "Blue_Gem.png")
        };
        
        private Dictionary<string, Image> imagenesMonedas = new();
        private string rutaImagenes = Path.Combine(Directory.GetCurrentDirectory(), "img", "objetos", "monedas");
        private ComboBox cmbMoneda = null!;

        private sealed class ComboItem
        {
            public string KeyEs { get; }
            public string Display { get; }
            public ComboItem(string keyEs, string display) { KeyEs = keyEs; Display = display; }
            public override string ToString() => Display;
        }

        private readonly Dictionary<string, string> _trMonedas = new()
        {
            ["Moneda de Cobre"] = "Copper Coin",
            ["Moneda de Plata"] = "Silver Coin",
            ["Moneda de Oro"] = "Gold Coin",
            ["Gema Verde"] = "Green Gem",
            ["Gema Roja"] = "Red Gem",
            ["Gema Azul"] = "Blue Gem",
        };

        public MonedasForm(bool english = false)
        {
            _en = english;
            this.Text = T("CF.Title");
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(550, 650);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Padding = new Padding(20);
            
            // Configurar controles
            var lblTitulo = new Label
            {
                Text = T("CF.Header"),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            
            // Centrar el título
            lblTitulo.Left = (this.ClientSize.Width - lblTitulo.Width) / 2;

            var lblMoneda = new Label
            {
                Text = T("CF.CurrencyType"),
                Location = new Point(20, 70),
                AutoSize = true
            };

            // Crear un ComboBox con imágenes
            cmbMoneda = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(20, 95),
                Width = 200,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 30
            };
            
            // Configurar el dibujado personalizado del ComboBox
            cmbMoneda.DrawItem += (s, e) =>
            {
                e.DrawBackground();
                if (e.Index >= 0 && e.Index < cmbMoneda.Items.Count)
                {
                    var item = cmbMoneda.Items[e.Index] as ComboItem;
                    var nombreMoneda = item?.KeyEs;
                    if (!string.IsNullOrEmpty(nombreMoneda) && monedas.TryGetValue(nombreMoneda, out var monedaInfo))
                    {
                        // Dibujar la imagen si existe
                        if (imagenesMonedas.TryGetValue(monedaInfo.imagen, out var imagen) && imagen != null)
                        {
                            try { e.Graphics.DrawImage(imagen, e.Bounds.Left + 2, e.Bounds.Top + 2, 26, 26); } catch { /* imagen inválida */ }
                        }
                        
                        // Dibujar el texto
                        using (var brush = new SolidBrush(e.ForeColor))
                        using (var font = new Font(e.Font ?? SystemFonts.DefaultFont, FontStyle.Regular))
                        {
                            var display = item?.Display ?? nombreMoneda;
                            e.Graphics.DrawString(display, font, brush, e.Bounds.Left + 35, e.Bounds.Top + 6);
                        }
                    }
                }
            };
            
            // Agregar las monedas al ComboBox con visualización traducida
            foreach (var nombreEs in monedas.Keys)
            {
                var display = _en && _trMonedas.TryGetValue(nombreEs, out var en) ? en : nombreEs;
                cmbMoneda.Items.Add(new ComboItem(nombreEs, display));
            }
            cmbMoneda.SelectedIndex = 0;

            var lblCantidad = new Label
            {
                Text = T("CF.Amount"),
                Location = new Point(20, 130),
                AutoSize = true
            };

            var numCantidad = new NumericUpDown
            {
                Location = new Point(20, 155),
                Width = 200,
                Minimum = 1,
                Maximum = 1000000,
                Value = 1
            };

            var btnCalcular = new Button
            {
                Text = T("CF.Calculate"),
                Location = new Point(20, 190),
                Size = new Size(100, 30)
            };

            var pnlResultados = new Panel
            {
                Location = new Point(20, 230),
                Size = new Size(490, 350),
                AutoScroll = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnCalcular.Click += (s, e) =>
            {
                var monedaSeleccionada = (cmbMoneda.SelectedItem as ComboItem)?.KeyEs;
                if (monedaSeleccionada == null) return;

                var cantidad = (int)numCantidad.Value;
                var totalCobre = monedas[monedaSeleccionada].valor * cantidad;

                // Limpiar resultados anteriores
                pnlResultados.Controls.Clear();

                // Mostrar el total en cobre
                var pnlTitulo = new Panel
                {
                    Location = new Point(10, 10),
                    Size = new Size(pnlResultados.Width - 40, 40),
                    BackColor = Color.Transparent
                };

                // Agregar imagen de la moneda seleccionada
                if (monedas.TryGetValue(monedaSeleccionada, out var monedaInfo) && 
                    imagenesMonedas.TryGetValue(monedaInfo.imagen, out var imgMoneda))
                {
                    var picMoneda = new PictureBox
                    {
                        Image = new Bitmap(imgMoneda),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Size = new Size(30, 30),
                        Location = new Point(0, 5)
                    };
                    pnlTitulo.Controls.Add(picMoneda);
                }

                var lblTitulo = new Label
                {
                    Text = $" {cantidad} {monedaSeleccionada} {T("CF.EqualTo")}",
                    Location = new Point(35, 10),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };
                pnlTitulo.Controls.Add(lblTitulo);
                pnlResultados.Controls.Add(pnlTitulo);

                // Panel para el total en cobre
                var pnlTotalCobre = new Panel
                {
                    Location = new Point(10, 50),
                    Size = new Size(pnlResultados.Width - 40, 40),
                    BackColor = Color.Transparent
                };

                // Agregar imagen de moneda de cobre
                if (imagenesMonedas.TryGetValue("Copper_Coin.png", out var imgCobre))
                {
                    var picCobre = new PictureBox
                    {
                        Image = new Bitmap(imgCobre),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Size = new Size(30, 30),
                        Location = new Point(0, 5)
                    };
                    pnlTotalCobre.Controls.Add(picCobre);
                }

                var lblTotal = new Label
                {
                    Text = $"= {totalCobre:N0} {T("CF.CopperCoins")}",
                    Location = new Point(35, 10),
                    AutoSize = true
                };
                pnlTotalCobre.Controls.Add(lblTotal);
                pnlResultados.Controls.Add(pnlTotalCobre);

                // Ordenar las monedas de mayor a menor valor
                var monedasOrdenadas = monedas
                    .Where(m => m.Key != monedaSeleccionada) // Excluir la moneda seleccionada
                    .OrderByDescending(x => x.Value.valor);
                
                int yPos = 80;

                // Mostrar el total en la moneda seleccionada primero
                var pnlMonedaSeleccionada = new Panel
                {
                    Location = new Point(10, yPos),
                    Size = new Size(pnlResultados.Width - 40, 40),
                    BackColor = Color.Transparent
                };

                // Agregar imagen de la moneda seleccionada
                if (monedas.TryGetValue(monedaSeleccionada, out var monedaSelInfo) && 
                    imagenesMonedas.TryGetValue(monedaSelInfo.imagen, out var imgSeleccionada))
                {
                    var picMonedaSel = new PictureBox
                    {
                        Image = new Bitmap(imgSeleccionada),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Size = new Size(30, 30),
                        Location = new Point(0, 5)
                    };
                    pnlMonedaSeleccionada.Controls.Add(picMonedaSel);
                }

                var lblMonedaSeleccionada = new Label
                {
                    Text = $"= {cantidad:N0} {(cmbMoneda.SelectedItem as ComboItem)?.Display}",
                    Location = new Point(35, 10),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };
                pnlMonedaSeleccionada.Controls.Add(lblMonedaSeleccionada);
                pnlResultados.Controls.Add(pnlMonedaSeleccionada);
                yPos += 40;

                // Agregar línea separadora
                var separador = new Label
                {
                    BorderStyle = BorderStyle.Fixed3D,
                    Height = 2,
                    Width = pnlResultados.Width - 40,
                    Location = new Point(10, yPos)
                };
                pnlResultados.Controls.Add(separador);
                yPos += 20;

                // Mostrar las conversiones a otras monedas
                foreach (var (nombre, (valor, imagenNombre)) in monedasOrdenadas)
                {
                    if (valor == 0) continue;
                    double cantidadEquivalente = (double)totalCobre / valor;
                    
                    // Mostrar solo si es mayor o igual a 1
                    if (cantidadEquivalente >= 1)
                    {
                        // Formatear para mostrar enteros cuando corresponda
                        string cantidadFormateada = cantidadEquivalente % 1 == 0 ? 
                            cantidadEquivalente.ToString("N0") : 
                            cantidadEquivalente.ToString("N2").TrimEnd('0').TrimEnd(',');
                        
                        // Crear un panel para cada fila de resultado
                        var pnlFila = new Panel
                        {
                            Location = new Point(10, yPos),
                            Size = new Size(pnlResultados.Width - 40, 40),
                            BackColor = Color.Transparent
                        };

                        // Agregar imagen de la moneda
                        if (imagenesMonedas.TryGetValue(imagenNombre, out var imagen) && imagen != null)
                        {
                            var picMoneda = new PictureBox
                            {
                                Image = new Bitmap(imagen), // Crear una nueva instancia de la imagen
                                SizeMode = PictureBoxSizeMode.Zoom,
                                Size = new Size(30, 30),
                                Location = new Point(0, 5)
                            };
                            pnlFila.Controls.Add(picMoneda);
                        }
                        else
                        {
                            // Mostrar un cuadro vacío si no hay imagen
                            var picPlaceholder = new PictureBox
                            {
                                BackColor = Color.LightGray,
                                Size = new Size(30, 30),
                                Location = new Point(0, 5)
                            };
                            pnlFila.Controls.Add(picPlaceholder);
                        }

                        // Agregar cantidad
                        var lblCantidad = new Label
                        {
                            Text = cantidadFormateada,
                            Location = new Point(40, 10),
                            AutoSize = true,
                            Font = new Font("Segoe UI", 10, FontStyle.Bold)
                        };
                        pnlFila.Controls.Add(lblCantidad);

                        // Agregar nombre de la moneda
                        var lblNombre = new Label
                        {
                            Text = _en && _trMonedas.TryGetValue(nombre, out var nombreEn) ? nombreEn : nombre,
                            Location = new Point(120, 10),
                            AutoSize = true
                        };
                        pnlFila.Controls.Add(lblNombre);

                        pnlResultados.Controls.Add(pnlFila);
                        yPos += 45;
                    }
                }
            };

            // Agregar controles al formulario
            this.Controls.AddRange(new Control[] {
                lblTitulo, lblMoneda, cmbMoneda, lblCantidad,
                numCantidad, btnCalcular, pnlResultados
            });

            // Calcular automáticamente al cambiar valores
            cmbMoneda.SelectedIndexChanged += (s, e) => btnCalcular.PerformClick();
            numCantidad.ValueChanged += (s, e) => btnCalcular.PerformClick();
            
            // Cargar imágenes de monedas
            CargarImagenesMonedas();
            
            // Calcular al cargar el formulario
            btnCalcular.PerformClick();
        }
        
        private void CargarImagenesMonedas()
        {
            try
            {
                // Intentar varias rutas posibles
                var rutasPosibles = new[]
                {
                    rutaImagenes,
                    Path.Combine(Directory.GetCurrentDirectory(), "img", "objetos", "monedas"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img", "objetos", "monedas"),
                    Path.Combine("..", "..", "..", "img", "objetos", "monedas"),
                    Path.Combine(Environment.CurrentDirectory, "img", "objetos", "monedas")
                };

                bool carpetaEncontrada = false;
                foreach (var ruta in rutasPosibles)
                {
                    if (Directory.Exists(ruta))
                    {
                        rutaImagenes = ruta;
                        carpetaEncontrada = true;
                        break;
                    }
                }

                if (!carpetaEncontrada)
                {
                    MessageBox.Show($"No se encontró la carpeta de imágenes. Asegúrate de tener la carpeta 'img/objetos/monedas' en la ruta correcta.\nRutas probadas:\n{string.Join("\n", rutasPosibles)}", 
                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Console.WriteLine($"Cargando imágenes desde: {rutaImagenes}");
                
                // Verificar archivos en la carpeta
                Console.WriteLine("Archivos en la carpeta:");
                if (Directory.Exists(rutaImagenes))
                {
                    foreach (var file in Directory.GetFiles(rutaImagenes))
                    {
                        Console.WriteLine($"- {Path.GetFileName(file)}");
                    }
                }

                // Cargar cada imagen
                foreach (var moneda in monedas)
                {
                    string rutaImagen = Path.Combine(rutaImagenes, moneda.Value.imagen);
                    if (File.Exists(rutaImagen))
                    {
                        try
                        {
                            try
                            {
                                using (var tempImg = Image.FromFile(rutaImagen))
                                {
                                    // Crear una copia en memoria para evitar bloqueos
                                    var bmp = new Bitmap(tempImg);
                                    imagenesMonedas[moneda.Value.imagen] = bmp;
                                    Console.WriteLine($"Imagen cargada correctamente: {moneda.Value.imagen}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error al cargar {rutaImagen}: {ex.Message}");
                                // Crear una imagen de relleno para evitar errores
                                imagenesMonedas[moneda.Value.imagen] = CrearImagenDeRelleno(moneda.Value.imagen);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error al cargar la imagen {rutaImagen}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar imágenes: {ex.Message}");
            }
        }
        
        // Método para crear una imagen de relleno cuando no se encuentra la imagen
        private Bitmap CrearImagenDeRelleno(string nombreImagen)
        {
            var bmp = new Bitmap(30, 30);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                using (var font = new Font("Arial", 8))
                using (var brush = new SolidBrush(Color.Black))
                {
                    g.DrawString("?", font, brush, 5, 5);
                }
            }
            return bmp;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Evitar owner-draw durante el cierre para que no intente repintar con imágenes ya liberadas
            try { if (cmbMoneda != null) cmbMoneda.DrawMode = DrawMode.Normal; } catch { }

            // Liberar recursos de las imágenes
            foreach (var imagen in imagenesMonedas.Values)
            {
                imagen?.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}
