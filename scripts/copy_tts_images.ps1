[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$base = [System.IO.Path]::Combine($env:USERPROFILE, 'OneDrive', [char]0xBB38 + [char]0xC11C, 'My Games', 'Tabletop Simulator', 'Mods', 'Images')
$dst = 'D:\roy_pboardgame\public\img\cards'

$pairs = @(
    @('441005847', 'tricks\tricks_front.jpg'),
    @('441006598', 'tricks\tricks_back.jpg'),
    @('441008099', 'performance\perf_front.jpg'),
    @('441008530', 'performance\perf_back.jpg'),
    @('441017409', 'magicians\magician_ability.jpg'),
    @('441017553', 'magicians\magician_portrait.jpg'),
    @('441017704', 'magicians\magician_back.png'),
    @('441004475', 'assignment\permanent_front.jpg'),
    @('441004952', 'assignment\permanent_back.jpg'),
    @('961283784', 'assignment\assign_deck45_front.jpg'),
    @('5104298832115271154', 'assignment\special_deck49_front.png'),
    @('441016502', 'secrets\secrets_front.jpg')
)

foreach ($p in $pairs) {
    $pattern = $p[0]
    $target = Join-Path $dst $p[1]
    $found = Get-ChildItem -Path $base -Filter "*$pattern*" -ErrorAction SilentlyContinue
    if ($found) {
        Copy-Item -LiteralPath $found[0].FullName -Destination $target -Force
        Write-Output "OK: $($p[1]) ($($found[0].Length) bytes)"
    } else {
        Write-Output "NOT FOUND: $pattern (base=$base)"
    }
}

Write-Output "=== DONE ==="
