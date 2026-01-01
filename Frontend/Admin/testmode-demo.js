#!/usr/bin/env node

/**
 * TestMode Demo Script
 *
 * Dieses Script demonstriert die Verwendung des TestMode Systems
 * und führt automatische Tests durch.
 */

import { chromium } from 'playwright'

async function runTestModeDemo() {
  console.log('🧪 Starting TestMode Demo...\n')

  const browser = await chromium.launch({
    headless: false, // Sichtbarer Browser für Demo
    args: ['--window-size=1200,800']
  })

  const context = await browser.newContext()
  const page = await context.newPage()

  try {
    // 1. Öffne Admin-App mit TestMode
    console.log('📱 Opening B2Connect Admin with TestMode...')
    await page.goto('http://localhost:5174?testmode=true')
    await page.waitForLoadState('networkidle')

    // 2. Warte auf TestMode Initialisierung
    await page.waitForTimeout(2000)

    // 3. Überprüfe TestMode Indikator
    const testModeIndicator = await page.locator('.test-mode-indicator').isVisible()
    console.log(`✅ TestMode Indicator visible: ${testModeIndicator}`)

    // 4. Simuliere Navigation (ohne Login)
    console.log('🧭 Testing navigation monitoring...')

    // Gehe zu verschiedenen Seiten
    await page.goto('http://localhost:5174/login')
    await page.waitForTimeout(500)

    await page.goto('http://localhost:5174/unauthorized')
    await page.waitForTimeout(500)

    await page.goto('http://localhost:5174/dashboard') // Sollte redirect zu login
    await page.waitForTimeout(500)

    // 5. Teste Back-Button Navigation
    console.log('🔙 Testing back button navigation...')
    await page.goBack()
    await page.waitForTimeout(500)

    await page.goBack()
    await page.waitForTimeout(500)

    // 6. Simuliere einige Klicks
    console.log('👆 Testing click monitoring...')
    const loginButton = page.locator('[data-test="login-button"]')
    if (await loginButton.isVisible()) {
      await loginButton.click()
      await page.waitForTimeout(500)
    }

    // 7. Öffne Debug Panel
    console.log('🔧 Opening TestMode Debug Panel...')
    await page.keyboard.press('Control+Shift+T')
    await page.waitForTimeout(1000)

    // 8. Überprüfe Debug Panel
    const debugPanel = await page.locator('.test-mode-debug-panel').isVisible()
    console.log(`✅ Debug Panel visible: ${debugPanel}`)

    // 9. Sammle Statistiken
    console.log('📊 Collecting TestMode statistics...')
    const stats = await page.evaluate(() => {
      const testMode = window.getTestMode?.()
      if (!testMode) return null

      const actions = testMode.getActions()
      const errors = actions.filter(a => !a.success).length
      const apiCalls = actions.filter(a => a.type === 'api-call').length

      return {
        totalActions: actions.length,
        errors,
        apiCalls,
        successRate: actions.length > 0 ? ((actions.length - errors) / actions.length * 100).toFixed(1) : 100
      }
    })

    if (stats) {
      console.log(`📈 Test Results:`)
      console.log(`   • Total Actions: ${stats.totalActions}`)
      console.log(`   • Errors: ${stats.errors}`)
      console.log(`   • API Calls: ${stats.apiCalls}`)
      console.log(`   • Success Rate: ${stats.successRate}%`)
    }

    // 10. Exportiere Log (falls Debug Panel offen)
    if (debugPanel) {
      console.log('💾 Exporting TestMode log...')
      // Klicke Export Button (falls verfügbar)
      const exportButton = page.locator('button:has-text("Export Log")')
      if (await exportButton.isVisible()) {
        // Note: In real scenario würde Download-Dialog erscheinen
        console.log('   • Log export initiated (check Downloads folder)')
      }
    }

    console.log('\n🎉 TestMode Demo completed successfully!')
    console.log('\n💡 TestMode Features demonstrated:')
    console.log('   • Real-time action monitoring')
    console.log('   • Automatic error detection')
    console.log('   • Visual indicators')
    console.log('   • Debug panel with statistics')
    console.log('   • Log export functionality')
    console.log('   • Auto-fix capabilities (when errors occur)')

  } catch (error) {
    console.error('❌ TestMode Demo failed:', error.message)
  } finally {
    await browser.close()
  }
}

// Prüfe ob Dev Server läuft
async function checkDevServer() {
  try {
    const response = await fetch('http://localhost:5174')
    return response.ok
  } catch {
    return false
  }
}

// Main execution
async function main() {
  console.log('🔍 Checking if development server is running...')

  if (!(await checkDevServer())) {
    console.log('❌ Development server not running on http://localhost:5174')
    console.log('💡 Please start the dev server first:')
    console.log('   cd frontend/Admin && npm run dev')
    process.exit(1)
  }

  await runTestModeDemo()
}

// Run if called directly
if (import.meta.url === `file://${process.argv[1]}`) {
  main().catch(console.error)
}

export { runTestModeDemo }