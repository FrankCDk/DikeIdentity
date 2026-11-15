param([string]$OutputDirectory = (Split-Path -Parent $PSScriptRoot))

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$width = 3360
$height = 2780
$bitmap = [System.Drawing.Bitmap]::new($width, $height)
$bitmap.SetResolution(144, 144)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$graphics.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
$graphics.Clear([System.Drawing.ColorTranslator]::FromHtml('#F4F7FB'))
$svg = [System.Collections.Generic.List[string]]::new()
$svg.Add('<svg xmlns="http://www.w3.org/2000/svg" width="3360" height="2780" viewBox="0 0 3360 2780"><title>Mapa relacional de Dike Identity</title><desc>19 tablas sin campos. Relaciones de dominio, Identity e integración, con auditoría ampliada.</desc><rect width="3360" height="2780" fill="#F4F7FB"/>')
$nodes = @{}

function Escape-Xml([string]$value) { [System.Security.SecurityElement]::Escape($value) }
function Draw-Text([string]$value, [float]$x, [float]$y, [float]$size = 24, [string]$color = '#1E293B', [bool]$bold = $false) {
    $style = if ($bold) { [System.Drawing.FontStyle]::Bold } else { [System.Drawing.FontStyle]::Regular }
    $font = [System.Drawing.Font]::new('Segoe UI', $size, $style, [System.Drawing.GraphicsUnit]::Pixel)
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml($color))
    $graphics.DrawString($value, $font, $brush, $x, $y)
    $weight = if ($bold) { '700' } else { '400' }
    $escaped = Escape-Xml $value
    $baseline = $y + $size * 1.06
    $svg.Add("<text x='$x' y='$baseline' font-family='Segoe UI,Arial,sans-serif' font-size='$size' font-weight='$weight' fill='$color'>$escaped</text>")
    $font.Dispose()
    $brush.Dispose()
}
function Draw-Box([float]$x, [float]$y, [float]$w, [float]$h, [string]$fill, [string]$stroke = '#D8E1EC', [float]$strokeWidth = 2) {
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml($fill))
    $pen = [System.Drawing.Pen]::new([System.Drawing.ColorTranslator]::FromHtml($stroke), $strokeWidth)
    $graphics.FillRectangle($brush, $x, $y, $w, $h)
    $graphics.DrawRectangle($pen, $x, $y, $w, $h)
    $svg.Add("<rect x='$x' y='$y' width='$w' height='$h' fill='$fill' stroke='$stroke' stroke-width='$strokeWidth'/>")
    $brush.Dispose()
    $pen.Dispose()
}
function Draw-Line([array]$points, [string]$color = '#71839A', [bool]$dashed = $false, [float]$lineWidth = 3) {
    $pen = [System.Drawing.Pen]::new([System.Drawing.ColorTranslator]::FromHtml($color), $lineWidth)
    if ($dashed) { $pen.DashStyle = [System.Drawing.Drawing2D.DashStyle]::Dash }
    $drawingPoints = [System.Drawing.PointF[]]@($points | ForEach-Object { [System.Drawing.PointF]::new([float]$_[0], [float]$_[1]) })
    $graphics.DrawLines($pen, $drawingPoints)
    $coordinates = ($points | ForEach-Object { "${_}" -replace ' ', ',' }) -join ' '
    $dash = if ($dashed) { "stroke-dasharray='9 7'" } else { '' }
    $svg.Add("<polyline points='$coordinates' fill='none' stroke='$color' stroke-width='$lineWidth' stroke-linejoin='round' $dash/>")
    $pen.Dispose()
}
function Draw-Label([string]$value, [float]$x, [float]$y, [float]$w = 124, [string]$color = '#475569') {
    Draw-Box $x $y $w 36 '#FFFFFF' '#E2E8F0' 1
    Draw-Text $value ($x + 10) ($y + 3) 21 $color $true
}
function Add-Table([string]$id, [string]$name, [string]$physical, [float]$x, [float]$y, [string]$color, [string]$kind, [float]$w = 420, [bool]$reference = $false) {
    $h = if ($reference) { 88 } else { 126 }
    Draw-Box ($x + 5) ($y + 7) $w $h '#E8EDF4' '#E8EDF4' 1
    Draw-Box $x $y $w $h '#FFFFFF' '#CBD5E1' 2
    Draw-Box $x $y $w 8 $color $color 1
    $titleSize = if ($name.Length -gt 25) { 22 } else { 25 }
    Draw-Text $name ($x + 20) ($y + 19) $titleSize '#172B45' $true
    if ($reference) {
        Draw-Text 'Referencia a la misma tabla' ($x + 20) ($y + 54) 18 '#64748B'
    } else {
        Draw-Text $physical ($x + 20) ($y + 58) 19 '#475569'
        Draw-Text $kind ($x + 20) ($y + 93) 15 $color $true
    }
    $nodes[$id] = @{ X = $x; Y = $y; W = $w; H = $h }
}

# Encabezado y leyenda.
Draw-Text 'DIKE IDENTITY' 70 40 25 '#3165A0' $true
Draw-Text 'Mapa relacional de tablas' 70 82 54 '#142C49' $true
Draw-Text 'Modelo propuesto para Code First  /  ASP.NET Core Identity + PostgreSQL  /  Sin campos' 73 156 25 '#52647C'
Draw-Box 2480 68 800 108 '#FFFFFF' '#DBE4EF'
Draw-Text '19 TABLAS' 2502 84 27 '#142C49' $true
Draw-Text '1 : 0..N  uno a muchos     1 : 0..1  opcional' 2502 128 22 '#52647C'
Draw-Text 'Cada línea indica una relación; la etiqueta expresa sus cardinalidades.' 73 215 22 '#52647C'

$blue = '#2563AB'
$teal = '#087F8C'
$purple = '#8056AD'
$amber = '#B87815'
Draw-Box 60 280 1040 1470 '#F9FBFE' '#DCE5F1'
Draw-Box 1145 280 1095 1470 '#F8FCFC' '#D7E9EA'
Draw-Box 2285 280 1015 1470 '#FCFAFE' '#E6DDF1'
Draw-Text '01  USUARIOS E IDENTITY' 88 307 27 $blue $true
Draw-Text '02  APLICACIONES Y ACCESO' 1173 307 27 $teal $true
Draw-Text '03  ADMINISTRACIÓN' 2313 307 27 $purple $true

# Conectores detrás de las tablas. Los caminos compartidos son ramas del mismo padre.
Draw-Line @(@(620,453),@(565,453),@(565,703),@(510,703)) $blue
Draw-Line @(@(565,703),@(565,913),@(510,913)) $blue
Draw-Line @(@(565,913),@(565,1123),@(510,1123)) $blue
Draw-Line @(@(1040,453),@(1118,453),@(1118,913),@(1240,913)) $teal
Draw-Line @(@(1450,516),@(1450,850)) $teal
Draw-Line @(@(1450,976),@(1450,1080),@(830,1080),@(830,1240)) $teal
Draw-Line @(@(830,1366),@(830,1490)) $blue
Draw-Line @(@(1660,453),@(1720,453),@(1770,453)) $teal
Draw-Line @(@(1720,453),@(1720,703),@(1770,703)) $teal
Draw-Line @(@(1720,703),@(1720,913),@(1770,913)) $teal
Draw-Line @(@(1980,976),@(1980,1240)) $teal
Draw-Line @(@(2820,453),@(2895,453),@(2895,703),@(2820,703)) $purple
Draw-Line @(@(2895,703),@(2895,913),@(2820,913)) $purple
Draw-Line @(@(2895,913),@(2895,1123),@(2820,1123)) $purple
Draw-Line @(@(2400,453),@(2334,453),@(2334,1303),@(2400,1303)) $purple
Draw-Line @(@(2610,1366),@(2610,1490)) $purple

# Tablas (18 en la vista principal y la tabla 19 en el detalle de auditoría).
Add-Table 'Usuarios' 'Usuarios' 'usuarios.AspNetUsers' 620 390 $blue 'IDENTITY · CUENTA CENTRALIZADA'
Add-Table 'ClaimsUsuario' 'ClaimsUsuario' 'usuarios.AspNetUserClaims' 90 640 $blue 'IDENTITY'
Add-Table 'LoginsExternosUsuario' 'LoginsExternosUsuario' 'usuarios.AspNetUserLogins' 90 850 $blue 'IDENTITY · GOOGLE Y OTROS'
Add-Table 'TokensIdentityUsuario' 'TokensIdentityUsuario' 'usuarios.AspNetUserTokens' 90 1060 $blue 'IDENTITY · ALMACÉN INTERNO'
Add-Table 'SesionesUsuario' 'SesionesUsuario' 'dike.SesionesUsuario' 620 1240 $blue 'DOMINIO · HISTORIAL DE SESIONES'
Add-Table 'TokensSesionUsuario' 'TokensSesionUsuario' 'dike.TokensSesionUsuario' 620 1490 $blue 'DOMINIO · EMISIÓN Y RENOVACIÓN'

Add-Table 'Aplicaciones' 'Aplicaciones' 'dike.Aplicaciones' 1240 390 $teal 'DOMINIO · APLICACIÓN CLIENTE'
Add-Table 'CredencialesAplicacion' 'CredencialesAplicacion' 'dike.CredencialesAplicacion' 1770 390 $teal 'INTEGRACIÓN'
Add-Table 'EndpointsNotificacion' 'EndpointsNotificacion' 'dike.EndpointsNotificacion' 1770 640 $teal 'INTEGRACIÓN · ENDPOINT OPCIONAL'
Add-Table 'AsignacionesAcceso' 'AsignacionesAcceso' 'dike.AsignacionesAcceso' 1240 850 $teal 'DOMINIO · USUARIO + APLICACIÓN'
Add-Table 'Notificaciones' 'Notificaciones' 'dike.Notificaciones' 1770 850 $teal 'INTEGRACIÓN · BANDEJA DE SALIDA'
Add-Table 'IntentosEntregaNotificacion' 'IntentosEntregaNotificacion' 'dike.IntentosEntregaNotificacion' 1770 1240 $teal 'INTEGRACIÓN · HISTORIAL DE ENTREGAS'

Add-Table 'Administradores' 'Administradores' 'administracion.AspNetUsers' 2400 390 $purple 'IDENTITY · CUENTA INDEPENDIENTE'
Add-Table 'ClaimsAdministrador' 'ClaimsAdministrador' 'administracion.AspNetUserClaims' 2400 640 $purple 'IDENTITY'
Add-Table 'LoginsExternosAdministrador' 'LoginsExternosAdministrador' 'administracion.AspNetUserLogins' 2400 850 $purple 'IDENTITY'
Add-Table 'TokensIdentityAdministrador' 'TokensIdentityAdministrador' 'administracion.AspNetUserTokens' 2400 1060 $purple 'IDENTITY · ALMACÉN INTERNO'
Add-Table 'SesionesAdministrativas' 'SesionesAdministrativas' 'dike.SesionesAdministrativas' 2400 1240 $purple 'DOMINIO · HISTORIAL ADMINISTRATIVO'
Add-Table 'TokensSesionAdministrativa' 'TokensSesionAdministrativa' 'dike.TokensSesionAdministrativa' 2400 1490 $purple 'SOLO SI ADMINISTRACIÓN USA TOKENS'

# Cardinalidades próximas al hijo.
foreach ($labelY in @(590,800,1010)) { Draw-Label '1 : 0..N' 290 $labelY 128 $blue }
Draw-Label '1 : 0..N' 1125 785 128 $teal
Draw-Label '1 : 0..N' 1385 650 128 $teal
Draw-Label '1 : 0..N' 766 1145 128 $blue
Draw-Label '1 : 0..N' 766 1400 128 $blue
Draw-Label '1 : 0..N' 1780 335 128 $teal
Draw-Label '1 : 0..1' 1780 590 128 $teal
Draw-Label '1 : 0..N' 1780 800 128 $teal
Draw-Label '1 : 0..N' 1916 1090 128 $teal
foreach ($labelY in @(590,800,1010)) { Draw-Label '1 : 0..N' 2685 $labelY 128 $purple }
Draw-Label '1 : 0..N' 2350 1180 128 $purple
Draw-Label '1 : 0..N' 2546 1400 128 $purple

Draw-Text 'Un usuario puede acceder a varias aplicaciones mediante asignaciones.' 1195 1510 20 '#52647C'
Draw-Text 'Una sola asignación por combinación usuario + aplicación.' 1195 1550 20 '#52647C'
Draw-Text 'Máximo una sesión activa por cuenta de usuario en todo el ecosistema.' 90 1670 21 $blue $true
Draw-Text 'Máximo una sesión administrativa activa por administrador.' 2310 1670 20 $purple $true

# Auditoría: referencias repetidas para evitar cruzar las líneas de toda la vista principal.
Draw-Box 60 1810 3240 725 '#FFFCF6' '#EDDFC4'
Draw-Text '04  AUDITORÍA Y TRAZABILIDAD' 90 1836 27 $amber $true
Draw-Text 'Los recuadros de referencia repiten tablas de la vista principal; no agregan tablas nuevas.' 90 1883 22 '#71634D'

# Tres orígenes, cada uno con dos relaciones diferentes hacia auditoría.
$originXs = @(290,1470,2650)
$originNames = @('Usuarios','Aplicaciones','Administradores')
$originColors = @($blue,$teal,$purple)
for ($i = 0; $i -lt 3; $i++) {
    $center = $originXs[$i] + 210
    $destination = 1630 + ($i - 1) * 55
    Draw-Line @(@($center,2028),@($center,2090),@($destination,2090),@($destination,2160)) $amber $true 2
    Add-Table "ref-origin-$i" $originNames[$i] '' $originXs[$i] 1940 $originColors[$i] '' 420 $true
    Draw-Text 'Actor / entidad afectada' ($originXs[$i] + 70) 2041 19 '#71634D'
}
Draw-Label '0..1 : 0..N por relación' 1990 2095 290 $amber
Add-Table 'EventosAuditoria' 'EventosAuditoria' 'dike.EventosAuditoria' 1420 2160 $amber 'DOMINIO · HISTORIAL INMUTABLE'
Draw-Text 'Dos vínculos distintos por cuenta o aplicación:' 90 2150 21 '#71634D'
Draw-Text 'quién origina el evento y qué entidad resulta afectada.' 90 2185 21 '#71634D'
Draw-Text 'Un proceso interno o intento fallido' 2350 2150 21 '#71634D'
Draw-Text 'puede no tener un actor identificado.' 2350 2185 21 '#71634D'

$subjectNames = @('AsignacionesAcceso','SesionesUsuario','TokensSesionUsuario','SesionesAdministrativas','TokensSesionAdministrativa','Notificaciones')
$subjectColors = @($teal,$blue,$blue,$purple,$purple,$teal)
for ($i = 0; $i -lt 6; $i++) {
    $subjectX = 90 + 535 * $i
    $subjectCenter = $subjectX + 210
    $port = 1490 + $i * 56
    $routeY = 2310 + $i * 9
    Draw-Line @(@($subjectCenter,2400),@($subjectCenter,$routeY),@($port,$routeY),@($port,2286)) $amber $true 2
    Add-Table "ref-subject-$i" $subjectNames[$i] '' $subjectX 2400 $subjectColors[$i] '' 420 $true
}
Draw-Label '0..1 : 0..N por relación' 1435 2337 290 $amber

# Notas de alcance y enlace documental.
Draw-Text 'ALCANCE DE ESTA IMAGEN' 90 2580 20 '#3165A0' $true
Draw-Text 'Propuesta base de 19 tablas. Roles y passkeys son ampliaciones condicionadas por la configuración de Identity.' 90 2618 24 '#52647C'
Draw-Text 'Los tokens internos de Identity y los registros de tokens de sesión tienen responsabilidades diferentes.' 90 2660 24 '#52647C'
Draw-Text 'Fuente: docs/Mapa_Relacional.md  ·  Las cardinalidades incluyen registros históricos, no solo activos.' 90 2710 22 '#718096'

if ($nodes.Count -ne 28) { throw "Inventario inesperado: $($nodes.Count). Se esperaban 19 tablas y 9 referencias visuales." }
$svg.Add('</svg>')
$pngPath = Join-Path $OutputDirectory 'Mapa_Relacional.png'
$svgPath = Join-Path $OutputDirectory 'Mapa_Relacional.svg'
$bitmap.Save($pngPath, [System.Drawing.Imaging.ImageFormat]::Png)
[System.IO.File]::WriteAllLines($svgPath, $svg, [System.Text.UTF8Encoding]::new($false))
$graphics.Dispose()
$bitmap.Dispose()
Write-Output $pngPath
Write-Output $svgPath
