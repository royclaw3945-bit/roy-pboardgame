# Generate simple SVG placeholder portraits for expansion magicians
for name_color in "electra:#f39c12:E" "gentleman:#8e44ad:G" "red_lotus:#c0392b:R" "yoruba:#2980b9:Y"; do
  IFS=':' read -r name color initial <<< "$name_color"
  cat > "D:/roy_pboardgame/public/img/${name}_portrait.svg" << SVG
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100">
  <rect width="100" height="100" fill="#1a1535"/>
  <circle cx="50" cy="50" r="40" fill="${color}22" stroke="${color}" stroke-width="2"/>
  <text x="50" y="58" text-anchor="middle" fill="${color}" font-family="serif" font-size="36" font-weight="bold">${initial}</text>
</svg>
SVG
done
