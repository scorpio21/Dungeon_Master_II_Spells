; Inno Setup script for Grimorio de Hechizos
; Requires Inno Setup 6.x

#define MyAppName "Grimorio de Hechizos"
#define MyAppVersion "1.0.4"
#define MyAppPublisher "scorpio21"
#define MyAppExeName "SpellBookWinForms.exe"

; Update this path if needed to point to the published single-file folder
#define PublishDir "publish\\win-x64-singlefile"

; Ensure the output directory is set correctly
#pragma parseroption -p-

[Setup]
AppId={{7F15B6C5-17E0-4A9C-9A48-9E9C2C3F8C8A}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf64}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableDirPage=no
DisableProgramGroupPage=no
OutputBaseFilename=Grimorio_de_Hechizos_v{#MyAppVersion}_Setup
OutputDir=.\\publish
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "Crear icono en el escritorio"; GroupDescription: "Tareas adicionales:"; Flags: unchecked

[Files]
; Incluir los archivos de la aplicación
Source: "{#PublishDir}\\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion

; Incluir la carpeta img con todas sus subcarpetas y archivos
Source: "img\*"; DestDir: "{app}\img"; Flags: recursesubdirs createallsubdirs ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\\{#MyAppExeName}"
Name: "{group}\Desinstalar {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\\{#MyAppExeName}"; Description: "Ejecutar {#MyAppName}"; Flags: nowait postinstall skipifsilent
