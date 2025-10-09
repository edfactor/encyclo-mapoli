import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: './src/test/setup.ts',
    css: true,
    exclude: [
      '**/node_modules/**',
      '**/dist/**',
      '**/e2e/**',
      '**/.{idea,git,cache,output,temp}/**',
    ],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html'],
      exclude: [
        'node_modules/',
        'src/test/',
        'e2e/',
        '**/*.d.ts',
        '**/*.config.*',
        '**/mockData',
        '**/types',
      ],
    },
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
      reduxstore: path.resolve(__dirname, './src/reduxstore'),
      utils: path.resolve(__dirname, './src/utils'),
      Okta: path.resolve(__dirname, './src/Okta'),
      components: path.resolve(__dirname, './src/components'),
      pages: path.resolve(__dirname, './src/pages'),
      schemas: path.resolve(__dirname, './src/schemas'),
      hooks: path.resolve(__dirname, './src/hooks'),
      styles: path.resolve(__dirname, './src/styles'),
    },
  },
});
