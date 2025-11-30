# Changelog

## [1.0.7] - 2025-11-30

### Añadido

- Panel de inventario para **sets de armadura** con fondo `img/inven.png` y colocación precisa de casco, torso, piernas, botas, escudo y arma.
- Soporte para **piezas extra** de set (`amulet`, `ring`, `necklace`, `arrows`) con imágenes en `img/objetos/collares/` y mapeo a posiciones fijas.
- Botones específicos de sets en el formulario **Objetos**: `Añadir extra`, `Ver coordenadas` y `Generar JSON`, visibles solo cuando está activo el modo "Solo sets".
- Función de **drag & drop** para mover piezas del set y extras sobre el inventario, con botón para mostrar las coordenadas exactas.
- Nuevo formulario **`NuevoSetForm`** para definir un set completo (datos generales, piezas básicas y extras) y generar un bloque JSON listo para pegar en `data/objetos_sets.json`.
- Menú de ayuda (ES/EN) en `NuevoSetForm` con guía rápida para crear y registrar nuevos sets.

### Instalador

- Actualización prevista de `installer.iss` a versión 1.0.7 para incluir el nuevo fondo `img/inven.png` y las imágenes de `img/objetos/collares/` en Setup y versión portable.

## [1.0.6.4] - 2025-11-15

### Añadido

- Nueva pantalla "Objetos" con listado, búsqueda y filtro por tipo (Espada, Arco, Bastón, Armadura, Escudo, etc.).
- Archivo `data/objetos_armas.json` con armas ofensivas (espadas, hachas, mazas, arcos, bastones, bombas, minions, escudos) y armaduras (ligeras, pesadas, cascos, botas) con datos reales (ES/EN) e imágenes.
- Vista de conjuntos ("Solo sets") para Fire / Ra Sar / Mithral / Tech que muestra las piezas en disposición de maniquí (casco, torso, piernas, botas, escudo, arma).

### Mejorado

- Tamaño y carga de imágenes de objetos para sprites pequeños (16x16), evitando escalados exagerados.
- Detalles de interfaz y filtros en el formulario de Objetos.

### Instalador

- Actualización de `installer.iss` a versión 1.0.6.4. El instalador incluye `img/` y `data/` con los nuevos objetos.

## [1.0.6.3] - 2025-11-14

### Añadido

- Nueva pantalla "Criaturas" con búsqueda, filtro por hábitat y panel de detalle (imagen, stats, habilidades, notas).
- Cálculo de "Golpes necesarios" para derrotar criaturas con Fireball/Lightning por nivel de poder, configurable en `data/spell_damage.json`.

### Mejorado

- Datos de criaturas completados (ES/EN) y assets DOS copiados a `img/criaturas/`.
- Carga de imágenes de criaturas usando `Imagenes.BaseImgPath()` y `Imagenes.CargarImagenSegura()`.
- Localización del menú “Criaturas” (ES/EN) y layout del formulario (botón Salir visible, sin solapes).

### Instalador

- Actualización de `installer.iss` a versión 1.0.6.3. El instalador incluye `img/` y `data/`.

### Notas

- El modelo de daño es aproximado y configurable; se puede ajustar `resistanceScale` y tablas por poder en `data/spell_damage.json`.

## [1.0.6.2] - 2025-11-12

### Añadido

- Diálogo “Ayuda > Acerca de” con versión dinámica (Application.ProductVersion) y localización ES/EN.
- Tablas externas de costes/dificultad: `data/tabla_dificultad_mana.json` (carga con fallback a valores internos).

### Mejorado

- Instalador multilenguaje (ES/EN) con autodetección por idioma del SO (LanguageDetectionMethod=uilanguage).
- Alineado `MyAppVersion` del instalador a 1.0.6.2.
- ZIP portable incluye la carpeta `img/`.
- Refactor de utilidades de imágenes (clase `Imagenes`): rutas centralizadas, carga segura y reemplazo con dispose.
- MonedasForm: botón Cerrar junto a Calcular y textos localizados; sin solapes.

### CI/CD

- Workflow de GitHub Actions actualizado para disparar en tags `v*` y crear releases automáticos adjuntando Setup y ZIPs.
- Permisos `contents: write` añadidos y uso de tag limpio en acción de release.

### Correcciones

- Ajustes de permisos del token de GitHub para crear releases (error 403 resuelto en nueva ejecución con tag v1.0.6.2).
- Corrección de cálculo: la Dificultad total no debe ser igual al Maná total. Ahora la dificultad se calcula como (poder) + suma de dificultades base de las rúbricas, mientras que el maná escala con el poder. (fix en 7484265)

## [1.0.5] - 2025-11-12

### Añadido

- Soporte multilenguaje en la aplicación (Español/Inglés) con selector en el menú.

### Mejorado

- Traducciones para clases, hechizos, efectos, etiquetas y tooltips.
- Ajuste de detalles en EN (Mana/Difficulty/Family) y tooltips.

### Corregido

- Warnings del instalador (Inno Setup): uso de `x64compatible` y `commonpf64`.

## [1.0.4] - 2025-11-11

### Añadido

- Nuevos objetos con imágenes en `img/objetos/` (ej.: Guardia Esbirro, escudos, auras, dardo/nube venenosa, bola de fuego, rayo, etc.)
- Archivo `GUIA_COMPILAR_E_INSTALAR.txt` con pasos para compilar y generar el instalador

### Mejorado

- Carga de imágenes más robusta: intenta primero el archivo con extensión y libera la imagen previa para evitar bloqueos

### Corregido

- Al seleccionar una poción ahora se muestra el frasco vacío hasta pulsar "Mostrar hechizo"
- Mapeos de objetos ajustados para apuntar a archivos con extensión (p.ej., `guardia.png`)

## [1.0.3] - 2025-11-11

### Añadido

- **Nueva calculadora de monedas** con conversión entre diferentes tipos de moneda del juego
- **Imágenes de monedas y gemas** para una mejor experiencia visual
- **Menú de utilidades** para acceder a la calculadora de monedas

#### Mejoras

- Interfaz de usuario con imágenes de monedas en la calculadora
- Manejo de imágenes con rutas relativas mejorado
- Sistema de carga de imágenes con múltiples ubicaciones de búsqueda
- Documentación actualizada con las nuevas características

#### Correcciones

- Problemas de visualización en la interfaz de usuario
- Errores en el cálculo de conversiones de monedas
- Mejora en el manejo de archivos de imagen faltantes

## [1.0.2] - 2025-11-07

### Mejoras

- Sistema de instalación mejorado con Inno Setup
- Workflow de GitHub Actions para builds automáticos
- Optimización del tamaño de los archivos generados
- Mejor manejo de rutas de imágenes

### Correcciones

- Problemas de carga de imágenes en el instalador
- Configuración de rutas relativas para mayor portabilidad

## [1.0.0] - 2025-11-06

### Características iniciales

- Primera versión pública
