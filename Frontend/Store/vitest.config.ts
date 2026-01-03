import { defineConfig } from 'vitest/config';
import vue from '@vitejs/plugin-vue';
import path from 'path';

export default defineConfig({
  plugins: [vue()],
  test: {
    globals: true,
    environment: 'happy-dom',
    setupFiles: ['./tests/setup.ts'],
    include: [
      'tests/unit/**/*.spec.ts',
      'tests/components/**/*.spec.ts',
      'tests/views/**/*.spec.ts',
      'tests/integration/**/*.spec.ts',
    ],
    exclude: ['tests/e2e/**'],
    /**
     * Tests that use async component loading will have unhandled rejections
     * for missing widget components in test environment.
     * These are handled gracefully by defineAsyncComponent and don't affect test results.
     */
    testTimeout: 15000,
    hookTimeout: 15000,
    teardownTimeout: 15000,
    mockReset: true,
    restoreMocks: true,
    isolate: false,
    threads: false,
    fileParallelism: false,
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html', 'lcov'],
      exclude: ['node_modules/', 'dist/', 'tests/'],
    },
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
});
