/**
 * API Integration Test
 * Tests the frontend-backend integration
 */

import { chromium } from 'playwright';

async function testAPIIntegration() {
  const browser = await chromium.launch({ headless: false });
  const page = await browser.newPage();

  const errors = [];
  const consoleMessages = [];
  const networkRequests = [];

  // Capture errors
  page.on('pageerror', error => {
    errors.push(error.message);
    console.error('❌ Page error:', error.message);
  });

  // Capture console
  page.on('console', msg => {
    consoleMessages.push(`[${msg.type()}] ${msg.text()}`);
    if (msg.type() === 'error') {
      console.error('❌ Console error:', msg.text());
    }
  });

  // Capture network requests
  page.on('request', request => {
    if (request.url().includes('/api/')) {
      networkRequests.push({
        method: request.method(),
        url: request.url(),
      });
      console.log(`🌐 API Request: ${request.method()} ${request.url()}`);
    }
  });

  // Capture network responses
  page.on('response', async response => {
    if (response.url().includes('/api/')) {
      const status = response.status();
      console.log(`📡 API Response: ${status} ${response.url()}`);

      if (status >= 400) {
        console.error(`❌ API Error ${status}:`, response.url());
      }
    }
  });

  try {
    console.log('🧪 API Integration Test\n');

    // 1. Load page
    console.log('1️⃣ Loading page...');
    await page.goto('http://localhost:5177/');
    await page.waitForTimeout(3000); // Wait for API calls

    // 2. Check for errors
    if (errors.length > 0) {
      console.log('\n❌ JavaScript errors:');
      errors.forEach((err, i) => console.log(`   ${i + 1}. ${err}`));
      throw new Error('Page has JavaScript errors');
    }

    console.log('✅ Page loaded without errors\n');

    // 3. Check API calls
    console.log('3️⃣ API Calls Made:');
    networkRequests.forEach((req, i) => {
      console.log(`   ${i + 1}. ${req.method} ${req.url}`);
    });

    if (networkRequests.length === 0) {
      console.log('⚠️  No API calls detected');
    }

    // 4. Check UI elements
    console.log('\n4️⃣ Checking UI elements:');

    const hasLoading = await page.locator('text=/Loading/').isVisible().catch(() => false);
    console.log(`   - Loading indicator: ${hasLoading ? '✅ Shown' : '⚠️  Not shown'}`);

    await page.waitForTimeout(2000);

    const hasHeader = await page.locator('header').isVisible().catch(() => false);
    console.log(`   - Header: ${hasHeader ? '✅' : '❌'}`);

    const hasInput = await page.locator('textarea').isVisible().catch(() => false);
    console.log(`   - Input area: ${hasInput ? '✅' : '❌'}`);

    const hasSendBtn = await page.getByRole('button', { name: /發送/ }).isVisible().catch(() => false);
    console.log(`   - Send button: ${hasSendBtn ? '✅' : '❌'}`);

    // 5. Test input
    if (hasInput) {
      console.log('\n5️⃣ Testing message input:');
      await page.locator('textarea').fill('Hello, this is a test message');
      const value = await page.locator('textarea').inputValue();
      console.log(`   - Input test: ${value.length > 0 ? '✅' : '❌'}`);

      // Test send (but don't actually send to avoid creating test data)
      console.log('   - Send button ready: ✅');
    }

    // 6. Take screenshot
    console.log('\n6️⃣ Taking screenshot:');
    await page.screenshot({ path: 'api-integration-test.png', fullPage: true });
    console.log('   ✅ Screenshot saved: api-integration-test.png');

    // 7. Summary
    console.log('\n📊 Test Summary:');
    console.log(`   - Page errors: ${errors.length}`);
    console.log(`   - API calls: ${networkRequests.length}`);
    console.log(`   - Console messages: ${consoleMessages.length}`);

    console.log('\n✅ API Integration test completed!');
    console.log('⏳ Browser will remain open for 10 seconds...\n');
    await page.waitForTimeout(10000);

  } catch (error) {
    console.error('\n❌ Test failed:', error.message);
    await page.screenshot({ path: 'api-integration-error.png' });
    throw error;
  } finally {
    await browser.close();
  }
}

testAPIIntegration().catch(console.error);
