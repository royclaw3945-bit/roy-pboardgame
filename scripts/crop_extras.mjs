/**
 * Crop additional sprite sheets: classroom, practice rooms, prophecy, card backs
 */
import sharp from 'sharp';
import { mkdirSync, existsSync } from 'fs';
import { join } from 'path';

const BASE = 'public/img';

const SHEETS = [
  // Classroom cards (Academy expansion)
  { name: 'classroom_lv1', src: `${BASE}/cards/classroom/lv1_front.jpg`, outDir: `${BASE}/cards/classroom/lv1`, cols: 3, rows: 3, maxCards: 8, format: 'jpg' },
  { name: 'classroom_lv2', src: `${BASE}/cards/classroom/lv2_front.jpg`, outDir: `${BASE}/cards/classroom/lv2`, cols: 3, rows: 3, maxCards: 8, format: 'jpg' },
  { name: 'classroom_lv3', src: `${BASE}/cards/classroom/lv3_front.jpg`, outDir: `${BASE}/cards/classroom/lv3`, cols: 3, rows: 3, maxCards: 8, format: 'jpg' },

  // Practice rooms
  { name: 'practice_lv1', src: `${BASE}/cards/practice/lv1_front.jpg`, outDir: `${BASE}/cards/practice/lv1`, cols: 3, rows: 2, maxCards: 5, format: 'jpg' },
  { name: 'practice_lv2', src: `${BASE}/cards/practice/lv2_front.jpg`, outDir: `${BASE}/cards/practice/lv2`, cols: 3, rows: 3, maxCards: 8, format: 'jpg' },
  { name: 'practice_lv3', src: `${BASE}/cards/practice/lv3_front.jpg`, outDir: `${BASE}/cards/practice/lv3`, cols: 3, rows: 3, maxCards: 8, format: 'jpg' },

  // Prophecy tokens/cards
  { name: 'prophecy', src: `${BASE}/cards/prophecy/prophecy_front.jpg`, outDir: `${BASE}/cards/prophecy/individual`, cols: 10, rows: 7, maxCards: 34, format: 'jpg' },
];

// Card backs: extract single card (cell 0,0) from each back sprite sheet
const BACKS = [
  { src: `${BASE}/cards/tricks/tricks_back.jpg`, out: `${BASE}/cards/backs/trick_back.jpg`, cols: 10, rows: 7 },
  { src: `${BASE}/cards/performance/perf_back.jpg`, out: `${BASE}/cards/backs/perf_back_single.jpg`, cols: 10, rows: 7 },
  { src: `${BASE}/cards/assignment/permanent_back.jpg`, out: `${BASE}/cards/backs/permanent_back_single.jpg`, cols: 10, rows: 7 },
];

async function cropSheet(sheet) {
  if (!existsSync(sheet.src)) { console.log(`SKIP: ${sheet.src}`); return; }
  mkdirSync(sheet.outDir, { recursive: true });

  const meta = await sharp(sheet.src).metadata();
  const cellW = Math.floor(meta.width / sheet.cols);
  const cellH = Math.floor(meta.height / sheet.rows);

  console.log(`\n=== ${sheet.name} === ${meta.width}x${meta.height} Grid:${sheet.cols}x${sheet.rows} Cell:${cellW}x${cellH}`);

  let count = 0;
  for (let r = 0; r < sheet.rows; r++) {
    for (let c = 0; c < sheet.cols; c++) {
      const idx = r * sheet.cols + c;
      if (idx >= sheet.maxCards) break;

      const left = c * cellW;
      const top = r * cellH;
      const width = Math.min(cellW, meta.width - left);
      const height = Math.min(cellH, meta.height - top);
      const outFile = join(sheet.outDir, `card_${String(idx).padStart(2, '0')}.${sheet.format}`);

      try {
        await sharp(sheet.src).extract({ left, top, width, height }).jpeg({ quality: 90 }).toFile(outFile);
        count++;
      } catch (e) { console.log(`  ERR ${idx}: ${e.message}`); }
    }
  }
  console.log(`  Cropped: ${count} → ${sheet.outDir}`);
}

async function cropBack(b) {
  if (!existsSync(b.src)) { console.log(`SKIP back: ${b.src}`); return; }
  const meta = await sharp(b.src).metadata();
  const cellW = Math.floor(meta.width / b.cols);
  const cellH = Math.floor(meta.height / b.rows);
  await sharp(b.src).extract({ left: 0, top: 0, width: cellW, height: cellH }).jpeg({ quality: 90 }).toFile(b.out);
  console.log(`Back: ${b.out} (${cellW}x${cellH})`);
}

async function main() {
  console.log('=== Extra Sprite Sheet Cropper ===');

  for (const s of SHEETS) await cropSheet(s);

  console.log('\n=== Card Backs ===');
  for (const b of BACKS) await cropBack(b);

  console.log('\n=== DONE ===');
}

main().catch(console.error);
