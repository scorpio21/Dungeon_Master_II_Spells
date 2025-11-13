# Changelog

## [1.0.6.2] - 2025-11-12

### Añadido

- Diálogo “Ayuda > Acerca de” con versión dinámica (Application.ProductVersion) y localización ES/EN.

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
