/**
 * Debug UI Testing Script
 * Captures console errors and takes screenshots
 */

import { chromium } from 'playwright';

async function debugChatUI() {
  const browser = await chromium.launch({ headless: false });
  const page = await browser.newPage();

  // Capture console messages
  const consoleMessages = [];
  const errors = [];

  page.on('console', msg => {
    consoleMessages.push(`[${msg.type()}] ${msg.text()}`);
    console.log(`瀏覽器控制台 [${msg.type()}]:`, msg.text());
  });

  page.on('pageerror', error => {
    errors.push(error.message);
    console.error('❌ 頁面錯誤:', error.message);
  });

  try {
    console.log('🧪 開始 Debug 測試...\n');

    // Navigate
    console.log('1️⃣ 載入頁面...');
    await page.goto('http://localhost:5174/');
    console.log('✅ 導航完成\n');

    // Wait for React to render
    console.log('2️⃣ 等待 React 渲染 (5秒)...');
    await page.waitForTimeout(5000);

    // Take screenshot
    console.log('3️⃣ 截取畫面...');
    await page.screenshot({ path: 'debug-screenshot.png', fullPage: true });
    console.log('✅ 截圖保存: debug-screenshot.png\n');

    // Check for root element
    const rootContent = await page.locator('#root').innerHTML();
    console.log('4️⃣ Root 元素內容長度:', rootContent.length);

    if (rootContent.length < 100) {
      console.log('⚠️  Root 內容很少,可能渲染失敗');
      console.log('Root innerHTML:', rootContent.substring(0, 500));
    }

    // Summary
    console.log('\n📊 測試總結:');
    console.log(`   - 控制台訊息數: ${consoleMessages.length}`);
    console.log(`   - 錯誤數: ${errors.length}`);

    if (errors.length > 0) {
      console.log('\n❌ 發現的錯誤:');
      errors.forEach((err, i) => console.log(`   ${i + 1}. ${err}`));
    }

    console.log('\n✅ Debug 測試完成!');
    console.log('⏳ 瀏覽器將保持開啟 10 秒以便檢查...');
    await page.waitForTimeout(10000);

  } catch (error) {
    console.error('❌ 測試失敗:', error.message);
    await page.screenshot({ path: 'error-screenshot.png' });
    throw error;
  } finally {
    await browser.close();
  }
}

debugChatUI().catch(console.error);
