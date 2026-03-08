import { chromium } from 'playwright';
const browser = await chromium.launch({ headless: true });
const page = await browser.newPage({ viewport: { width: 1920, height: 1080 } });
await page.goto('http://localhost:3000/setup', { waitUntil: 'networkidle' });
await page.waitForTimeout(2000);
const btn = page.locator('button', { hasText: '게임 시작' });
const count = await btn.count();
console.log('Found buttons:', count);
if (count > 0) {
  await btn.first().click();
  await page.waitForTimeout(3000);
  await page.waitForURL('**/game**', { timeout: 5000 }).catch(() => {});
}
console.log('Current URL:', page.url());
await page.screenshot({ path: 'C:/Users/kimwj/AppData/Local/Temp/game_board.png', fullPage: false });
console.log('Done');
await browser.close();
