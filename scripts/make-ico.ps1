Param(
  [string]$PngPath = "img/ico/4112-512x512.png",
  [string]$OutIco = "img/app.ico"
)

# Crea un ICO con una sola entrada (256x256) usando PNG como payload
# Nota: En formato ICO, un width/height de 0 equivale a 256.

$pngBytes = [System.IO.File]::ReadAllBytes($PngPath)
$fs = [System.IO.File]::Open($OutIco, [System.IO.FileMode]::Create)
$bw = New-Object System.IO.BinaryWriter($fs)

# ICONDIR header
$bw.Write([UInt16]0)      # reserved
$bw.Write([UInt16]1)      # type (1 = icon)
$bw.Write([UInt16]1)      # count (images)

# ICONDIRENTRY
$bw.Write([Byte]0)        # width (0 => 256)
$bw.Write([Byte]0)        # height (0 => 256)
$bw.Write([Byte]0)        # color count
$bw.Write([Byte]0)        # reserved
$bw.Write([UInt16]0)      # planes
$bw.Write([UInt16]32)     # bit count
$bw.Write([UInt32]$pngBytes.Length)  # bytes in resource
$bw.Write([UInt32]22)     # offset to image data (6+16)

# Image data (PNG)
$bw.Write($pngBytes)
$bw.Flush()
$bw.Close()
$fs.Close()

Write-Host "ICO generado en $OutIco"
