[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$path = [System.IO.Path]::Combine($env:USERPROFILE, 'OneDrive', [char]0xBB38 + [char]0xC11C, 'My Games', 'Tabletop Simulator', 'Mods', 'Workshop', '2905542671.json')
$json = Get-Content $path -Raw | ConvertFrom-Json

function ExtractDecks($obj, $prefix) {
    if ($null -eq $obj) { return }
    if ($obj.PSObject.Properties.Name -contains 'CustomDeck') {
        $cd = $obj.CustomDeck
        foreach ($key in $cd.PSObject.Properties.Name) {
            $deck = $cd.$key
            $face = $deck.FaceURL
            $back = $deck.BackURL
            $nw = $deck.NumWidth
            $nh = $deck.NumHeight
            # Extract image ID from URL
            $faceId = if ($face -match '(\d{5,})') { $matches[1] } else { 'unknown' }
            Write-Output "$prefix KEY=$key NW=$nw NH=$nh FaceID=$faceId"
        }
    }
    if ($obj.PSObject.Properties.Name -contains 'ContainedObjects') {
        foreach ($child in $obj.ContainedObjects) {
            $nick = if ($child.Nickname) { $child.Nickname } else { 'unnamed' }
            ExtractDecks $child "$prefix > $nick"
        }
    }
}

foreach ($obj in $json.ObjectStates) {
    $nick = if ($obj.Nickname) { $obj.Nickname } else { 'root' }
    ExtractDecks $obj $nick
}
