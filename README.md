# Grimorio de Hechizos – Dungeon Master II (WinForms)

Aplicación de escritorio en .NET (Windows Forms) para explorar y calcular conjuros de Dungeon Master II. Permite seleccionar el nivel de poder, la clase y el hechizo, mostrando iconografía, coste de maná y dificultad, de acuerdo con la tabla original del juego.

- Framework: .NET 8.0 (Windows)
- UI: Windows Forms (formulario editable desde Visual Studio)
- Imágenes: carga desde disco (no embebidas)

> Fuente de datos: “Dungeon Master II Spells - Dungeon Master II Solutions - Dungeon Master II - Games - Dungeon Master Encyclopaedia”.

## Características

- Selector de nivel de poder (Lo, Um, On, Ee, Pal, Mon).
- Listado de hechizos por clase (Priest/Wizard) con símbolos y efecto.
- Cálculo del coste total de Maná/Dificultad por nivel:
  - Poder = nivel (1..6).
  - Cada símbolo escala como floor(PL1 × (nivel + 1) / 2).
- Iconografía de símbolos y frascos de pociones.
- Tooltips con descripción y familia de cada símbolo.

## Requisitos

- Windows 10/11
- .NET SDK 8.0 (o superior) con soporte Windows Desktop
- Visual Studio 2022 (recomendado) o `dotnet` CLI

## Estructura del proyecto

```magias
Magias/
  SpellBookWinForms/
    SpellBookWinForms.csproj
    Program.cs
    MainForm.cs
    MainForm.Designer.cs
    MainForm.resx
    README.md
    .gitignore
    img/                      # Iconos de símbolos y poder
      lo.png, um.png, ...
    img/posiones/            # Frascos de pociones
      Health.gif, Mana.gif, Shield.gif, vacia.png, ...
```

## Colocación de recursos (imágenes)

- Símbolos y poder: `img/`
  - Ejemplos: `lo.png`, `um.png`, `on.png`, `ee.png`, `pal.png`, `mon.png`, `ya.png`, `vi.png`, etc.
  - Se soporta `.png` y `.gif` en mayúsculas/minúsculas.
- Frascos de pociones: `img/posiones/`
  - Ejemplos: `Health.gif`, `Mana.gif`, `Shield.gif`, `vacia.png`, `veneno.gif`.

La aplicación detecta primero `img/` dentro de la carpeta del proyecto y, si no existe, busca rutas alternativas conocidas.

## Compilación y ejecución

Con Visual Studio:

- Abrir `SpellBookWinForms.csproj` y ejecutar (F5).

Con CLI:

```bash
# En la carpeta del proyecto
dotnet restore
dotnet build -c Debug
# Ejecutar
dotnet run
```

## Uso

1. Selecciona nivel de poder.
2. Selecciona clase (Priest/Wizard) y hechizo.
3. Pulsa “Mostrar hechizo”.
4. Observa coste total, detalle por símbolo y los iconos.

## Hoja de ruta (ideas)

- Exportar hechizo a imagen/PNG con composición de iconos.
- Soporte WPF con escalado DPI avanzado.
- Localización completa (ES/EN).
- Test de regresión para tabla de costes.

## Licencia

MIT. Consulta el archivo `LICENSE` (o confirma la licencia preferida).

## Créditos

- Datos y tabla de conjuros: Dungeon Master Encyclopaedia.
- Iconografía: materiales del usuario y recursos propios del proyecto.

---
¿Problemas con imágenes?

- Verifica que `img/` y `img/posiones/` existan y contengan los archivos esperados.
- Los nombres deben coincidir con los símbolos (se prueban variantes en minúsculas/mayúsculas y `.png`/`.gif`).
- Ejemplo Cure Poison: `img/posiones/veneno.gif`.
