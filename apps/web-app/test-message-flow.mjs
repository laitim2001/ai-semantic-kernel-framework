/**
 * Message Flow Test
 * Tests the complete message sending and receiving flow
 */

import { chromium } from 'playwright';

async function testMessageFlow() {
  const browser = await chromium.launch({ headless: false });
  const page = await browser.newPage();

  const errors = [];
  const apiCalls = [];

  // Capture errors
  page.on('pageerror', error => {
    errors.push(error.message);
    console.error('❌ Page error:', error.message);
  });

  // Capture API calls
  page.on('response', async response => {
    if (response.url().includes('/api/')) {
      const method = response.request().method();
      const status = response.status();
      const url = response.url();

      apiCalls.push({ method, status, url });

      if (status >= 200 && status < 300) {
        console.log(`✅ API ${method} ${status}: ${url}`);
      } else if (status >= 400) {
        console.error(`❌ API ${method} ${status}: ${url}`);
      }
    }
  });

  try {
    console.log('🧪 Message Flow Test\n');

    // 1. Load page
    console.log('1️⃣ Loading chat page...');
    await page.goto('http://localhost:5177/');
    await page.waitForTimeout(3000); // Wait for conversation creation

    if (errors.length > 0) {
      throw new Error('Page has errors');
    }
    console.log('✅ Page loaded\n');

    // 2. Check conversation was created
    const conversationCreated = apiCalls.some(
      call => call.method === 'POST' && call.url.includes('/conversations') && call.status === 201
    );

    if (conversationCreated) {
      console.log('✅ Conversation created successfully\n');
    } else {
      console.error('❌ Conversation creation failed\n');
    }

    // 3. Find and fill input
    console.log('3️⃣ Testing message input...');
    const textarea = await page.locator('textarea').first();
    await textarea.waitFor({ state: 'visible', timeout: 5000 });
    await textarea.fill('Hello! This is a test message from the integration test.');
    console.log('✅ Message typed\n');

    // 4. Click send button
    console.log('4️⃣ Sending message...');
    const sendButton = await page.getByRole('button', { name: /發送|送出|Send/i }).first();
    await sendButton.click();

    // Wait for message API call
    await page.waitForTimeout(2000);

    // Check if message was sent
    const messageSent = apiCalls.some(
      call => call.method === 'POST' && call.url.includes('/messages')
    );

    if (messageSent) {
      console.log('✅ Message sent to API\n');
    } else {
      console.error('❌ Message not sent to API\n');
    }

    // 5. Check if message appears in UI
    console.log('5️⃣ Checking message display...');
    await page.waitForTimeout(1000);

    const userMessage = await page.locator('text=/This is a test message/').isVisible();
    if (userMessage) {
      console.log('✅ User message displayed in UI\n');
    } else {
      console.error('❌ User message not displayed\n');
    }

    // 6. Check for agent response (simulated)
    console.log('6️⃣ Waiting for agent response...');
    await page.waitForTimeout(2000);

    const agentResponse = await page.locator('text=/simulated agent response/i').isVisible();
    if (agentResponse) {
      console.log('✅ Agent response displayed\n');
    } else {
      console.log('⚠️  Agent response not shown (expected for now)\n');
    }

    // 7. Take screenshot
    console.log('7️⃣ Taking screenshot...');
    await page.screenshot({ path: 'message-flow-test.png', fullPage: true });
    console.log('✅ Screenshot saved: message-flow-test.png\n');

    // 8. Summary
    console.log('📊 Test Summary:');
    console.log(`   - Page errors: ${errors.length}`);
    console.log(`   - Total API calls: ${apiCalls.length}`);
    console.log(`   - Conversation created: ${conversationCreated ? '✅' : '❌'}`);
    console.log(`   - Message sent: ${messageSent ? '✅' : '❌'}`);
    console.log(`   - User message visible: ${userMessage ? '✅' : '❌'}`);
    console.log(`   - Agent response visible: ${agentResponse ? '✅' : '⚠️'}`);

    console.log('\n✅ Message flow test completed!');
    console.log('⏳ Browser will remain open for 10 seconds...\n');
    await page.waitForTimeout(10000);

  } catch (error) {
    console.error('\n❌ Test failed:', error.message);
    await page.screenshot({ path: 'message-flow-error.png' });
    throw error;
  } finally {
    await browser.close();
  }
}

testMessageFlow().catch(console.error);
