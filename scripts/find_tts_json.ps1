[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$base = [System.IO.Path]::Combine($env:USERPROFILE, 'OneDrive', [char]0xBB38 + [char]0xC11C, 'My Games', 'Tabletop Simulator')
Get-ChildItem -Path $base -Filter '*.json' -Recurse -ErrorAction SilentlyContinue |
  Where-Object { $_.Length -gt 100000 } |
  Select-Object FullName, @{N='SizeKB';E={[math]::Round($_.Length/1024)}} |
  Format-Table -AutoSize
