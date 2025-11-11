# Changelog

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
