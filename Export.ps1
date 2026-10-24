<#
.SYNOPSIS
    构建并打包 DarkestSpire 模组：DarkestSpire.dll + DarkestSpire.pck + DarkestSpire.json。

.DESCRIPTION
    STS2 的 C# 模组由两部分组成：
      * DarkestSpire.dll —— 由 dotnet build 生成（C# 代码）
      * DarkestSpire.pck —— 由 Godot 导出（场景、图片、本地化等资源）
    本脚本依次完成这两步，并把三个文件放进 mods\DarkestSpire\。

.PARAMETER Configuration
    dotnet 构建配置，默认 ExportRelease（对应 Godot 的“发布”导出配置）。

.PARAMETER OutDir
    输出目录，默认 <Sts2Dir>\mods\DarkestSpire（Sts2Dir 来自 Local.props）。

.PARAMETER NoBuild
    跳过 dotnet build，直接用上次构建好的 dll。

.PARAMETER GodotExe
    覆盖 Local.props 中的 GodotEXEPath。

.EXAMPLE
    .\Export.ps1
    构建并导出到游戏的 mods 目录。

.EXAMPLE
    .\Export.ps1 -OutDir .\dist
    只把产物放到项目的 dist 目录，方便打包分享。
#>
[CmdletBinding()]
param(
    [ValidateSet('ExportRelease', 'ExportDebug', 'Debug')]
    [string]$Configuration = 'ExportRelease',
    [string]$OutDir,
    [switch]$NoBuild,
    [string]$GodotExe
)

$ErrorActionPreference = 'Stop'

$root = if ($PSScriptRoot) { $PSScriptRoot } else { (Get-Location).Path }
$projectPath = Join-Path $root 'DarkestSpire.csproj'
$modName = [IO.Path]::GetFileNameWithoutExtension($projectPath)

function Read-LocalProps {
    param([string]$Path)
    if (-not (Test-Path $Path)) {
        throw "找不到 $Path，请先参照 Local.props.example 创建。"
    }
    return ([xml](Get-Content $Path -Raw)).Project.PropertyGroup
}

$props = Read-LocalProps (Join-Path $root 'Local.props')
$sts2Dir = "$($props.Sts2Dir)".Trim().Trim('"')
if (-not $GodotExe) { $GodotExe = "$($props.GodotEXEPath)".Trim().Trim('"') }

if (-not (Test-Path $GodotExe)) {
    throw "找不到 Godot：$GodotExe（可用 -GodotExe 指定，或修改 Local.props 的 GodotEXEPath）。"
}

# dotnet 可能没在 PATH 里
$dotnet = (Get-Command dotnet -ErrorAction SilentlyContinue | Select-Object -First 1).Source
if (-not $dotnet) {
    $candidate = Join-Path $env:ProgramFiles 'dotnet\dotnet.exe'
    if (Test-Path $candidate) { $dotnet = $candidate }
}
if (-not $dotnet) { throw '找不到 dotnet，请安装 .NET SDK，或把 dotnet.exe 加入 PATH。' }

if (-not $NoBuild) {
    Write-Host "==> dotnet build ($Configuration)" -ForegroundColor Cyan
    # SkipModCopy=true：由本脚本负责拷贝，避免构建时直接写游戏目录
    & $dotnet build $projectPath -c $Configuration -p:SkipModCopy=true -v:m
    if ($LASTEXITCODE -ne 0) { throw "dotnet build 失败（exit $LASTEXITCODE）" }
}

$dll = Join-Path $root ".godot\mono\temp\bin\$Configuration\$modName.dll"
if (-not (Test-Path $dll)) {
    throw "找不到构建产物 $dll，请去掉 -NoBuild 重新构建。"
}

if (-not $OutDir) {
    if (-not $sts2Dir) { throw 'Local.props 里没有 Sts2Dir，无法确定模组目录，请用 -OutDir 指定。' }
    $OutDir = Join-Path $sts2Dir "mods\$modName"
}
$OutDir = [IO.Path]::GetFullPath($OutDir)
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null

$pck = Join-Path $OutDir "$modName.pck"
$log = Join-Path $root '.godot\export-pack.log'

Write-Host "==> Godot 导出 pck（进程输出见 $log）" -ForegroundColor Cyan
# Godot 编辑器是 GUI 程序，直接调用不会等待，所以走 cmd /c
$cmd = 'set "PATH={0};%PATH%" && "{1}" --headless --path "{2}" --export-pack "Windows Desktop" "{3}" > "{4}" 2>&1' -f `
    (Split-Path $dotnet), $GodotExe, $root, $pck, $log
cmd /c $cmd | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Host (Get-Content $log -Tail 20) -ForegroundColor DarkGray
    throw "Godot 导出失败（exit $LASTEXITCODE），完整日志：$log"
}

Copy-Item $dll -Destination (Join-Path $OutDir "$modName.dll") -Force
$json = Join-Path $root "$modName.json"
if (Test-Path $json) { Copy-Item $json -Destination (Join-Path $OutDir "$modName.json") -Force }

Write-Host ''
Write-Host "完成，输出目录：$OutDir" -ForegroundColor Green
Get-ChildItem $OutDir -File | Where-Object { $_.Name -like "$modName.*" } |
    Select-Object Name, Length, LastWriteTime | Format-Table -AutoSize
