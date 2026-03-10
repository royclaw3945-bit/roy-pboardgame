[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$base = [System.IO.Path]::Combine($env:USERPROFILE, 'OneDrive', [char]0xBB38 + [char]0xC11C, 'My Games', 'Tabletop Simulator', 'Mods', 'Images')

$dstBase = 'D:\roy_pboardgame\public\img'

# Create directories
$dirs = @(
    "$dstBase\tokens",
    "$dstBase\cards\classroom",
    "$dstBase\cards\practice",
    "$dstBase\cards\backs",
    "$dstBase\tiles",
    "$dstBase\cards\da_tricks",
    "$dstBase\cards\prophecy"
)
foreach ($d in $dirs) { New-Item -ItemType Directory -Path $d -Force | Out-Null }

$pairs = @(
    # === Card Backs ===
    @('441006598', 'cards\backs\tricks_back.jpg'),
    @('441008530', 'cards\backs\perf_back.jpg'),
    @('441004952', 'cards\backs\permanent_back.jpg'),
    @('441017704', 'cards\backs\magician_back.png'),
    @('441016608', 'cards\backs\secrets_back.jpg'),
    @('441015171', 'cards\backs\classroom_back.jpg'),
    @('441015905', 'cards\backs\practice_back.jpg'),

    # === Classroom Cards (Academy) ===
    @('441015058', 'cards\classroom\lv2_front.jpg'),       # Level 2 Classrooms NW=3 NH=3
    @('441015301', 'cards\classroom\lv1_front.jpg'),       # Level 1 Classrooms NW=3 NH=3
    @('441015514', 'cards\classroom\lv3_front.jpg'),       # Level 3 Classrooms NW=3 NH=3

    # === Practice Room Cards (Academy) ===
    @('441015775', 'cards\practice\lv1_front.jpg'),        # Level 1 Practice NW=3 NH=2
    @('441016028', 'cards\practice\lv2_front.jpg'),        # Level 2 Practice NW=3 NH=3
    @('441016240', 'cards\practice\lv3_front.jpg'),        # Level 3 Practice NW=3 NH=3

    # === Board Tiles ===
    @('441014491', 'tiles\academy_board.jpg'),             # Academy Board
    @('441021867', 'tiles\closed_room.jpg'),               # Closed Room tile
    @('441021996', 'tiles\tile_extra.jpg'),                # Extra tile

    # === Misc ===
    @('588072646', 'tiles\custom_tile_1.jpg'),
    @('588097278', 'tiles\custom_tile_2.jpg'),

    # === DA Expansion Tricks ===
    @('697611693', 'cards\da_tricks\da_tricks_1.jpg'),     # Dawn of Technology NW=10 NH=7
    @('697798672', 'cards\da_tricks\da_tricks_2.jpg'),     # Dawn of Technology NW=10 NH=7

    # === Prophecy / DA Assignment ===
    @('588302422', 'cards\prophecy\prophecy_front.jpg'),   # NW=10 NH=7 (prophecy-like deck)
    @('588300337', 'cards\prophecy\prophecy_back.jpg')
)

$copied = 0
$notFound = 0
foreach ($p in $pairs) {
    $pattern = $p[0]
    $target = Join-Path $dstBase $p[1]
    $found = Get-ChildItem -Path $base -Filter "*$pattern*" -ErrorAction SilentlyContinue
    if ($found) {
        Copy-Item -LiteralPath $found[0].FullName -Destination $target -Force
        $sizeKB = [math]::Round($found[0].Length / 1024)
        Write-Output "OK: $($p[1]) (${sizeKB}KB)"
        $copied++
    } else {
        Write-Output "NOT FOUND: $pattern -> $($p[1])"
        $notFound++
    }
}

# === Worker Tokens (Mesh Diffuse textures - UV maps for 3D models) ===
$workers = @(
    @('440997254', 'tokens\symbol_spade.jpg'),      # ♠
    @('440997404', 'tokens\symbol_heart.jpg'),       # ♥
    @('440997533', 'tokens\symbol_diamond.jpg'),     # ♦
    @('440997662', 'tokens\symbol_club.jpg'),        # ♣
    @('440997862', 'tokens\worker_blue_asst.jpg'),
    @('440998000', 'tokens\worker_green_asst.jpg'),
    @('440998096', 'tokens\worker_orange_asst.jpg'),
    @('440998303', 'tokens\worker_red_asst.jpg'),
    @('440998440', 'tokens\worker_blue_mgr.jpg'),
    @('440998542', 'tokens\worker_green_mgr.jpg'),
    @('440998666', 'tokens\worker_orange_mgr.jpg'),
    @('440998797', 'tokens\worker_red_mgr.jpg')
)

foreach ($p in $workers) {
    $pattern = $p[0]
    $target = Join-Path $dstBase $p[1]
    $found = Get-ChildItem -Path $base -Filter "*$pattern*" -ErrorAction SilentlyContinue
    if ($found) {
        Copy-Item -LiteralPath $found[0].FullName -Destination $target -Force
        $sizeKB = [math]::Round($found[0].Length / 1024)
        Write-Output "OK: $($p[1]) (${sizeKB}KB)"
        $copied++
    } else {
        Write-Output "NOT FOUND: $pattern -> $($p[1])"
        $notFound++
    }
}

Write-Output "`n=== DONE: $copied copied, $notFound not found ==="
