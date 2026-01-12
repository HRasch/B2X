#!/usr/bin/env node

/**
 * Debug System Integration Test
 * Tests the realtime debug functionality including SignalR streaming
 */

const { exec } = require('child_process');
const fs = require('fs');
const path = require('path');

console.log('🧪 Testing B2X Realtime Debug System Integration\n');

// Test 1: Check if backend is running
console.log('1. Checking backend status...');
exec('curl -s http://localhost:15500/health || echo "Backend not ready"', (error, stdout) => {
  if (stdout.includes('Backend not ready')) {
    console.log('❌ Backend not running on port 15500');
  } else {
    console.log('✅ Backend is running');
  }

  // Test 2: Check if frontend is running
  console.log('\n2. Checking frontend status...');
  exec('curl -s http://localhost:3000/ | grep -q "<!DOCTYPE html>" && echo "Frontend ready" || echo "Frontend not ready"', (error, stdout) => {
    if (stdout.includes('Frontend ready')) {
      console.log('✅ Frontend is running');
    } else {
      console.log('❌ Frontend not running on port 3000');
    }

    // Test 3: Check debug components exist
    console.log('\n3. Checking debug components...');
    const debugComponents = [
      'src/components/DebugTrigger.vue',
      'src/components/DebugFeedbackWidget.vue',
      'src/composables/useDebugContext.ts',
      'src/stores/debug.ts',
      'src/services/debugApi.ts',
      'src/config/debug.ts',
      'src/utils/debug.ts'
    ];

    let componentsFound = 0;
    debugComponents.forEach(component => {
      const fullPath = path.join(__dirname, '..', 'src/frontend/Store', component);
      if (fs.existsSync(fullPath)) {
        console.log(`✅ ${component}`);
        componentsFound++;
      } else {
        console.log(`❌ ${component} - MISSING`);
      }
    });

    // Test 4: Check backend SignalR components
    console.log('\n4. Checking backend SignalR components...');
    const backendComponents = [
      'src/BoundedContexts/Monitoring/API/Hubs/DebugHub.cs',
      'src/BoundedContexts/Monitoring/API/Services/DebugEventBroadcaster.cs'
    ];

    let backendFound = 0;
    backendComponents.forEach(component => {
      const fullPath = path.join(__dirname, '..', component);
      if (fs.existsSync(fullPath)) {
        console.log(`✅ ${component}`);
        backendFound++;
      } else {
        console.log(`❌ ${component} - MISSING`);
      }
    });

    // Test 5: Check package dependencies
    console.log('\n5. Checking dependencies...');
    const packageJsonPath = path.join(__dirname, '..', 'src/frontend/Store', 'package.json');
    if (fs.existsSync(packageJsonPath)) {
      const packageJson = JSON.parse(fs.readFileSync(packageJsonPath, 'utf8'));
      const hasSignalR = packageJson.dependencies && packageJson.dependencies['@microsoft/signalr'];
      if (hasSignalR) {
        console.log('✅ SignalR dependency found');
      } else {
        console.log('❌ SignalR dependency missing');
      }
    } else {
      console.log('❌ package.json not found');
    }

    // Summary
    console.log('\n📊 Test Summary:');
    console.log(`Debug Components: ${componentsFound}/${debugComponents.length} found`);
    console.log(`Backend Components: ${backendFound}/${backendComponents.length} found`);

    if (componentsFound === debugComponents.length && backendFound === backendComponents.length) {
      console.log('\n🎉 All debug system components are present!');
      console.log('\n🚀 Next Steps:');
      console.log('1. Open http://localhost:3000/ in browser');
      console.log('2. Look for debug trigger button (usually in bottom-right)');
      console.log('3. Test debug session recording and feedback submission');
      console.log('4. Check browser console for SignalR connection logs');
      console.log('5. Verify realtime events are received from backend');
    } else {
      console.log('\n⚠️  Some components are missing. Check the implementation.');
    }
  });
});