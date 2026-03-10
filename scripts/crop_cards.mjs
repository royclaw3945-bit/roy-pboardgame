/**
 * Crop individual card images from TTS sprite sheets.
 * Usage: node scripts/crop_cards.mjs
 */
import sharp from 'sharp';
import { mkdirSync, existsSync } from 'fs';
import { join } from 'path';

const BASE = 'public/img/cards';

/**
 * Sprite sheet definitions — grid values from TTS mod 2905542671.json
 * KEY=NW×NH (NumWidth×NumHeight from CustomDeck)
 */
const SHEETS = [
  {
    name: 'tricks',
    src: `${BASE}/tricks/tricks_front.jpg`,
    outDir: `${BASE}/tricks/individual`,
    cols: 10, rows: 7, // TTS: NW=10 NH=7, FaceID=441005847
    maxCards: 48,
    format: 'jpg',
  },
  {
    name: 'performance',
    src: `${BASE}/performance/perf_front.jpg`,
    outDir: `${BASE}/performance/individual`,
    cols: 10, rows: 7, // TTS: NW=10 NH=7, FaceID=441008099
    maxCards: 36,
    format: 'jpg',
  },
  {
    name: 'magician_ability',
    src: `${BASE}/magicians/magician_ability.jpg`,
    outDir: `${BASE}/magicians/ability`,
    cols: 4, rows: 4, // TTS: NW=4 NH=4, FaceID=441017409 (was wrong: 4×3)
    maxCards: 12,
    format: 'jpg',
  },
  {
    name: 'magician_portrait',
    src: `${BASE}/magicians/magician_portrait.jpg`,
    outDir: `${BASE}/magicians/portrait`,
    cols: 4, rows: 4, // TTS: NW=4 NH=4, FaceID=441017553 (was wrong: 4×3)
    maxCards: 12,
    format: 'jpg',
  },
  {
    name: 'permanent_assignment',
    src: `${BASE}/assignment/permanent_front.jpg`,
    outDir: `${BASE}/assignment/permanent`,
    cols: 10, rows: 7, // TTS: NW=10 NH=7, FaceID=441004475
    maxCards: 48,
    format: 'jpg',
  },
  {
    name: 'regular_assignment',
    src: `${BASE}/assignment/assign_deck45_front.jpg`,
    outDir: `${BASE}/assignment/regular`,
    cols: 8, rows: 7, // TTS: NW=8 NH=7, FaceID=961283784 (was wrong: 7×7)
    maxCards: 45,
    format: 'jpg',
  },
  {
    name: 'special_assignment',
    src: `${BASE}/assignment/special_deck49_front.png`,
    outDir: `${BASE}/assignment/special`,
    cols: 6, rows: 4, // TTS: NW=6 NH=4, FaceID=5104298832115271154
    maxCards: 22,
    format: 'png',
  },
  {
    name: 'secrets',
    src: `${BASE}/secrets/secrets_front.jpg`,
    outDir: `${BASE}/secrets/individual`,
    cols: 6, rows: 4, // TTS: NW=6 NH=4, FaceID=441016502 (was wrong: 10×4)
    maxCards: 20,
    format: 'jpg',
  },
];

async function cropSheet(sheet) {
  if (!existsSync(sheet.src)) {
    console.log(`SKIP: ${sheet.src} not found`);
    return;
  }

  mkdirSync(sheet.outDir, { recursive: true });

  const meta = await sharp(sheet.src).metadata();
  const cellW = Math.floor(meta.width / sheet.cols);
  const cellH = Math.floor(meta.height / sheet.rows);

  console.log(`\n=== ${sheet.name} ===`);
  console.log(`  Source: ${meta.width}x${meta.height}, Grid: ${sheet.cols}x${sheet.rows}, Cell: ${cellW}x${cellH}`);

  let count = 0;
  for (let r = 0; r < sheet.rows; r++) {
    for (let c = 0; c < sheet.cols; c++) {
      const idx = r * sheet.cols + c;
      if (idx >= sheet.maxCards) break;

      const left = c * cellW;
      const top = r * cellH;
      // Ensure we don't exceed image bounds
      const width = Math.min(cellW, meta.width - left);
      const height = Math.min(cellH, meta.height - top);

      const outFile = join(sheet.outDir, `card_${String(idx).padStart(2, '0')}.${sheet.format}`);

      try {
        const pipeline = sharp(sheet.src).extract({ left, top, width, height });

        if (sheet.format === 'jpg') {
          await pipeline.jpeg({ quality: 90 }).toFile(outFile);
        } else {
          await pipeline.png().toFile(outFile);
        }
        count++;
      } catch (err) {
        console.log(`  ERROR card_${idx}: ${err.message}`);
      }
    }
  }
  console.log(`  Cropped: ${count} cards → ${sheet.outDir}`);
}

async function main() {
  console.log('=== Card Sprite Sheet Cropper ===\n');

  for (const sheet of SHEETS) {
    await cropSheet(sheet);
  }

  console.log('\n=== ALL DONE ===');
}

main().catch(console.error);
