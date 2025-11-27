# Grimorio de Hechizos – Dungeon Master II (WinForms) v1.0.6.4

## Capturas

<p align="center">
<img src="img/menu.png" alt="Menú de la aplicación" width="300"> <img src="img/monedas.png" alt="Calculadora de Monedas" width="300"> <img src="img/armas.png" alt="Calculadora de Monedas" width="300"> <img src="img/criaturas.png" alt="Calculadora de Monedas" width="300">
</p>

Aplicación de escritorio en .NET (Windows Forms) para explorar y calcular conjuros de Dungeon Master II. Permite seleccionar el nivel de poder, la clase y el hechizo, mostrando iconografía, coste de maná y dificultad, de acuerdo con la tabla original del juego. Incluye una calculadora de monedas para conversiones entre diferentes tipos de monedas del juego.

- Framework: .NET 8.0 (Windows)
- UI: Windows Forms (formulario editable desde Visual Studio)
- Imágenes: carga desde disco (no embebidas)

> Fuente de datos: “Dungeon Master II Spells - Dungeon Master II Solutions - Dungeon Master II - Games - Dungeon Master Encyclopaedia”.

> Novedades v1.0.6.4:
> - Nueva pantalla **“Objetos”** con listado, búsqueda y filtro por tipo (Espada, Arco, Bastón, Armadura, Escudo, etc.).
> - Archivo `data/objetos_armas.json` con todas las armas ofensivas (espadas, hachas, mazas, arcos, bastones, bombas, minions, escudos) y armaduras (ligeras, pesadas, cascos, botas) con datos reales (ES/EN) e imágenes.
> - Vista de **sets de armadura** (Fire, Ra Sar, Mithral, Tech…) con disposición en “maniquí” (casco, torso, piernas, botas, escudo, arma) al activar el filtro "Solo sets".
> - Ajustes de carga de imágenes de objetos usando `Imagenes.BaseImgPath()` y nuevo visor de imágenes con tamaño optimizado para sprites 16x16.
> - Pequeñas mejoras de interfaz en la pantalla de Objetos y filtros.

## Características

- Selector de nivel de poder (Lo, Um, On, Ee, Pal, Mon).
- Listado de hechizos por clase (Priest/Wizard) con símbolos y efecto.
- Calculadora de monedas con conversión entre diferentes tipos:
  - Monedas de Cobre (1)
  - Monedas de Plata (4)
  - Monedas de Oro (16)
  - Gemas Verdes (64)
  - Gemas Rojas (256)
  - Gemas Azules (1024)
- Cálculo del coste total de Maná/Dificultad por nivel:
  - Maná: el poder afecta al coste, usando la tabla por nivel (Lo..Mon) cargada desde `data/tabla_dificultad_mana.json`.
  - Dificultad: no escala con poder; se calcula como (poder) + suma de dificultades base por rúbrica.
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
    MonedasForm.cs
    MainForm.resx
    README.md
    .gitignore
    data/
      tabla_dificultad_mana.json   # Tablas externas de maná por nivel y dificultad base
    img/                      # Iconos de símbolos y poder
      lo.png, um.png, ...
    img/posiones/            # Frascos de pociones
      Health.gif, Mana.gif, Shield.gif, vacia.png, ...
    img/objetos/             # Objetos varios
      monedas/               # Imágenes de monedas y gemas
        Copper_Coin.png
        Silver_Coin.png
        oro.png
        Green_Gem.png
        Red_Gem.png
        Blue_Gem.png
```

## Colocación de recursos (imágenes)

- Símbolos y poder: `img/`
  - Ejemplos: `lo.png`, `um.png`, `on.png`, `ee.png`, `pal.png`, `mon.png`, `ya.png`, `vi.png`, etc.
  - Se soporta `.png` y `.gif` en mayúsculas/minúsculas.
- Frascos de pociones: `img/posiones/`
- Imágenes de monedas: `img/objetos/monedas/`
  - `Copper_Coin.png`, `Silver_Coin.png`, `oro.png`
  - `Green_Gem.png`, `Red_Gem.png`, `Blue_Gem.png`
  - Ejemplos: `Health.gif`, `Mana.gif`, `Shield.gif`, `vacia.png`, `veneno.gif`.

La aplicación detecta primero `img/` dentro de la carpeta del proyecto y, si no existe, busca rutas alternativas conocidas.

## Descarga

- Descarga el instalador o la versión portable desde “Releases”:
  - https://github.com/scorpio21/Dungeon_Master_II_Spells/releases
  - Instalador: `Grimorio_de_Hechizos_vX.Y.Z_Setup.exe` (selecciona idioma automáticamente según Windows).
  - Portable: `SpellBookWinForms_portable.zip` o `Grimorio_de_Hechizos_vX.Y.Z_Portable.zip` (incluye `img/`).

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

## Cambios en la versión 1.0.3

- **Nueva característica**: Añadida calculadora de monedas con conversión entre diferentes tipos de moneda del juego.
- **Mejora**: Actualizada la interfaz de usuario para mostrar imágenes de monedas.
- **Correcciones**: Mejorado el manejo de imágenes y recursos.

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
