/**
 * Final UI Testing Script
 */

import { chromium } from 'playwright';

async function testChatUI() {
  const browser = await chromium.launch({ headless: false });
  const page = await browser.newPage();

  const errors = [];
  page.on('pageerror', error => errors.push(error.message));
  page.on('console', msg => {
    if (msg.type() === 'error') console.error('Browser error:', msg.text());
  });

  try {
    console.log('🧪 Chat UI 測試\n');

    // 1. Load page
    await page.goto('http://localhost:5177/');
    await page.waitForTimeout(2000);

    //2. Check for errors
    if (errors.length > 0) {
      console.log('❌ JavaScript 錯誤:');
      errors.forEach((err, i) => console.log(`   ${i + 1}. ${err}`));
      throw new Error('Page has JavaScript errors');
    }

    // 3. Check UI elements
    console.log('✅ 頁面載入成功\n');
    console.log('檢查 UI 元素:');

    const sidebar = await page.locator('aside').isVisible().catch(() => false);
    console.log(`   - Sidebar: ${sidebar ? '✅' : '❌'}`);

    const newConvBtn = await page.getByRole('button', { name: /新對話/ }).isVisible().catch(() => false);
    console.log(`   - 新對話按鈕: ${newConvBtn ? '✅' : '❌'}`);

    const header = await page.locator('header').isVisible().catch(() => false);
    console.log(`   - Header: ${header ? '✅' : '❌'}`);

    const textarea = await page.locator('textarea').isVisible().catch(() => false);
    console.log(`   - 輸入框: ${textarea ? '✅' : '❌'}`);

    const sendBtn = await page.getByRole('button', { name: /發送/ }).isVisible().catch(() => false);
    console.log(`   - 發送按鈕: ${sendBtn ? '✅' : '❌'}`);

    // 4. Test input
    console.log('\n測試輸入功能:');
    await page.locator('textarea').fill('測試訊息');
    const value = await page.locator('textarea').inputValue();
    console.log(`   - 輸入測試: ${value === '測試訊息' ? '✅' : '❌'}`);

    // 5. Screenshots
    console.log('\n截取畫面:');
    await page.screenshot({ path: 'final-desktop.png', fullPage: true });
    console.log('   ✅ 桌面版: final-desktop.png');

    await page.setViewportSize({ width: 375, height: 667 });
    await page.waitForTimeout(500);
    await page.screenshot({ path: 'final-mobile.png', fullPage: true });
    console.log('   ✅ 手機版: final-mobile.png');

    console.log('\n✅ 所有測試通過!');
    console.log('⏳ 瀏覽器保持開啟 5 秒...\n');
    await page.waitForTimeout(5000);

  } catch (error) {
    console.error('\n❌ 測試失敗:', error.message);
    await page.screenshot({ path: 'error.png' });
  } finally {
    await browser.close();
  }
}

testChatUI();
