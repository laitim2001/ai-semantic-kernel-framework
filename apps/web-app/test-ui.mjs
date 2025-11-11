/**
 * UI Testing Script using Playwright
 * Tests the Chat UI components
 */

import { chromium } from 'playwright';

async function testChatUI() {
  const browser = await chromium.launch({ headless: false });
  const page = await browser.newPage();

  try {
    console.log('🧪 開始 UI 測試...\n');

    // 1. 導航到應用
    console.log('1️⃣ 載入應用頁面...');
    await page.goto('http://localhost:5174/');
    await page.waitForTimeout(1000);
    console.log('✅ 頁面載入成功\n');

    // 2. 檢查基本元素是否存在
    console.log('2️⃣ 檢查基本 UI 元素...');

    // 檢查 Sidebar
    const sidebar = await page.locator('aside').count();
    console.log(`   - Sidebar: ${sidebar > 0 ? '✅ 存在' : '❌ 不存在'}`);

    // 檢查新對話按鈕
    const newConvButton = await page.getByRole('button', { name: /新對話/i }).count();
    console.log(`   - 新對話按鈕: ${newConvButton > 0 ? '✅ 存在' : '❌ 不存在'}`);

    // 檢查對話列表
    const conversationItems = await page.locator('[role="article"]').count();
    console.log(`   - 對話項目數量: ${conversationItems}`);

    // 檢查 Chat Header
    const chatHeader = await page.locator('header').count();
    console.log(`   - Chat Header: ${chatHeader > 0 ? '✅ 存在' : '❌ 不存在'}`);

    // 檢查訊息列表
    const messages = await page.locator('[role="article"]').count();
    console.log(`   - 訊息數量: ${messages}\n`);

    // 3. 測試輸入框
    console.log('3️⃣ 測試輸入框功能...');
    const textarea = await page.locator('textarea');
    await textarea.fill('測試訊息');
    await page.waitForTimeout(500);
    const inputValue = await textarea.inputValue();
    console.log(`   - 輸入測試: ${inputValue === '測試訊息' ? '✅ 正常' : '❌ 異常'}`);

    // 檢查字數統計
    const charCountText = await page.locator('text=/\\d+ \\/ \\d+/').textContent();
    console.log(`   - 字數統計: ${charCountText}\n`);

    // 4. 截圖
    console.log('4️⃣ 截取畫面...');
    await page.screenshot({ path: 'screenshot-desktop.png', fullPage: true });
    console.log('   ✅ 桌面版截圖已保存: screenshot-desktop.png\n');

    // 5. 測試響應式設計
    console.log('5️⃣ 測試響應式設計...');
    await page.setViewportSize({ width: 375, height: 667 }); // iPhone SE
    await page.waitForTimeout(500);
    const sidebarMobile = await page.locator('aside').isVisible();
    console.log(`   - 手機版 Sidebar 隱藏: ${!sidebarMobile ? '✅ 正確' : '❌ 未隱藏'}`);
    await page.screenshot({ path: 'screenshot-mobile.png', fullPage: true });
    console.log('   ✅ 手機版截圖已保存: screenshot-mobile.png\n');

    console.log('✅ 所有測試完成!\n');

  } catch (error) {
    console.error('❌ 測試失敗:', error.message);
    throw error;
  } finally {
    await browser.close();
  }
}

testChatUI().catch(console.error);
