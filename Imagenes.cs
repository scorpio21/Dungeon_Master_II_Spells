using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SpellBookWinForms
{
    public static class Imagenes
    {
        public static string BaseImgPath()
        {
            var exeDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            var projectDir = exeDir.Parent?.Parent?.Parent;
            if (projectDir != null)
            {
                var projectImg = Path.Combine(projectDir.FullName, "img");
                if (Directory.Exists(projectImg)) return projectImg;
            }

            var dir = exeDir;
            for (int i = 0; i < 8 && dir != null; i++)
            {
                var intento = Path.Combine(dir.FullName, "img");
                if (Directory.Exists(intento)) return intento;

                var intentoMagias = Path.Combine(dir.FullName, "Magias", "img");
                if (Directory.Exists(intentoMagias)) return intentoMagias;

                dir = dir.Parent;
            }

            var alt = @"L:\\Magias\\img";
            if (Directory.Exists(alt)) return alt;

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");
        }

        public static Image? CargarImagenSegura(string ruta)
        {
            if (!File.Exists(ruta)) return null;
            using var fs = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var img = Image.FromStream(fs);
            return new Bitmap(img);
        }

        public static string? BuscarImagen(string directorio, string nombreArchivo, params string[] extensiones)
        {
            // Si incluye extensión, probar exacto primero
            if (Path.HasExtension(nombreArchivo))
            {
                var exacto = Path.Combine(directorio, nombreArchivo);
                if (File.Exists(exacto)) return exacto;
            }

            foreach (var ext in extensiones)
            {
                var ruta = Path.Combine(directorio, Path.GetFileNameWithoutExtension(nombreArchivo) + ext);
                if (File.Exists(ruta)) return ruta;
            }
            return null;
        }

        public static void ReemplazarImagen(PictureBox pb, Image? nueva)
        {
            if (pb.Image != null)
            {
                var old = pb.Image;
                pb.Image = null;
                try { old.Dispose(); } catch { }
            }
            pb.Image = nueva;
        }
    }
}
