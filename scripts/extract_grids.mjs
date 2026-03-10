import { readFileSync } from 'fs';
import { join } from 'path';

// Build path with Korean chars
const base = join(process.env.USERPROFILE, 'OneDrive', '\uBB38\uC11C', 'My Games', 'Tabletop Simulator', 'Mods', 'Workshop', '2905542671.json');
const data = JSON.parse(readFileSync(base, 'utf8'));

const seen = new Set();

function extract(obj, path) {
  if (!obj || typeof obj !== 'object') return;
  if (obj.CustomDeck) {
    for (const [key, deck] of Object.entries(obj.CustomDeck)) {
      const faceUrl = deck.FaceURL || '';
      const m = faceUrl.match(/(\d{5,})/);
      const faceId = m ? m[1] : 'unknown';
      const sig = `${key}|${deck.NumWidth}|${deck.NumHeight}|${faceId}`;
      if (!seen.has(sig)) {
        seen.add(sig);
        console.log(`KEY=${key} NW=${deck.NumWidth} NH=${deck.NumHeight} FaceID=${faceId} Path=${path}`);
      }
    }
  }
  if (Array.isArray(obj.ContainedObjects)) {
    for (const child of obj.ContainedObjects) {
      const nick = child.Nickname || child.Name || '?';
      extract(child, path + ' > ' + nick);
    }
  }
}

for (const obj of data.ObjectStates) {
  const nick = obj.Nickname || obj.Name || 'root';
  extract(obj, nick);
}
