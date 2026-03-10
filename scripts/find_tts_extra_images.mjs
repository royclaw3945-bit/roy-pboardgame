/**
 * Extract all unique image URLs from TTS mod JSON and check which ones
 * are available in local cache.
 */
import { readFileSync, existsSync, readdirSync } from 'fs';
import { join } from 'path';

const base = join(process.env.USERPROFILE, 'OneDrive', '\uBB38\uC11C', 'My Games', 'Tabletop Simulator');
const modPath = join(base, 'Mods', 'Workshop', '2905542671.json');
const imgCache = join(base, 'Mods', 'Images');

const data = JSON.parse(readFileSync(modPath, 'utf8'));

// Collect all image URLs
const urls = new Map(); // url -> context

function scan(obj, path) {
  if (!obj || typeof obj !== 'object') return;

  // Check for image URLs in CustomDeck
  if (obj.CustomDeck) {
    for (const [key, deck] of Object.entries(obj.CustomDeck)) {
      if (deck.FaceURL) urls.set(deck.FaceURL, `${path} Deck[${key}] Face`);
      if (deck.BackURL) urls.set(deck.BackURL, `${path} Deck[${key}] Back`);
    }
  }

  // Check CustomImage
  if (obj.CustomImage) {
    if (obj.CustomImage.ImageURL) urls.set(obj.CustomImage.ImageURL, `${path} CustomImage`);
    if (obj.CustomImage.ImageSecondaryURL) urls.set(obj.CustomImage.ImageSecondaryURL, `${path} CustomImage2nd`);
  }

  // Check CustomMesh
  if (obj.CustomMesh) {
    if (obj.CustomMesh.DiffuseURL) urls.set(obj.CustomMesh.DiffuseURL, `${path} Mesh.Diffuse`);
  }

  // Recurse
  if (Array.isArray(obj.ContainedObjects)) {
    for (const child of obj.ContainedObjects) {
      const nick = child.Nickname || child.Name || '?';
      scan(child, `${path} > ${nick}`);
    }
  }
  if (Array.isArray(obj.States)) {
    for (const [k, st] of Object.entries(obj.States || {})) {
      scan(st, `${path} State[${k}]`);
    }
  }
}

for (const obj of data.ObjectStates) {
  scan(obj, obj.Nickname || obj.Name || 'root');
}

// Known FaceIDs we already have
const KNOWN = new Set([
  '441005847', '441006598', '441008099', '441008530',
  '441017409', '441017553', '441017704',
  '441004475', '441004952', '961283784',
  '5104298832115271154', '441016502'
]);

// Check cache
const cacheFiles = existsSync(imgCache) ? readdirSync(imgCache) : [];

console.log('=== Unknown Image URLs (not yet downloaded) ===\n');
let count = 0;
for (const [url, ctx] of urls.entries()) {
  // Extract ID from URL
  const m = url.match(/(\d{5,})/);
  const id = m ? m[1] : null;
  if (id && KNOWN.has(id)) continue;

  // Check if in cache
  const inCache = id ? cacheFiles.some(f => f.includes(id)) : false;

  console.log(`${inCache ? '✓ CACHED' : '✗ MISSING'} [${id || 'no-id'}] ${ctx}`);
  console.log(`  ${url.substring(0, 120)}`);
  count++;
}
console.log(`\nTotal unknown URLs: ${count}`);
