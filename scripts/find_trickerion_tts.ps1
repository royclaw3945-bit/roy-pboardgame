[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$base = [System.IO.Path]::Combine($env:USERPROFILE, 'OneDrive', [char]0xBB38 + [char]0xC11C, 'My Games', 'Tabletop Simulator')

# Search for 2905542671 or any Trickerion mod
$found = Get-ChildItem -Path "$base\Mods\Workshop" -Filter '*.json' -ErrorAction SilentlyContinue |
  Where-Object { $_.Name -match '29055' -or $_.Name -match 'trick' }
if ($found) {
  foreach ($f in $found) { Write-Output "FOUND: $($f.FullName) ($($f.Length) bytes)" }
} else {
  Write-Output "Not found by name. Searching content..."
  Get-ChildItem -Path "$base\Mods\Workshop" -Filter '*.json' -ErrorAction SilentlyContinue |
    ForEach-Object {
      $content = Get-Content $_.FullName -Raw -ErrorAction SilentlyContinue
      if ($content -match 'Trickerion' -or $content -match '441017409') {
        Write-Output "MATCH: $($_.FullName) ($($_.Length) bytes)"
      }
    }
}
