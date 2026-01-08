import type { Config } from 'tailwindcss';
// DaisyUI v5 is imported via CSS (@import "daisyui"), not as a plugin

export default {
  content: [
    './components/**/*.{vue,js,ts,jsx,tsx}',
    './pages/**/*.{vue,js,ts,jsx,tsx}',
    './layouts/**/*.{vue,js,ts,jsx,tsx}',
    './plugins/**/*.{vue,js,ts,jsx,tsx}',
    './nuxt.config.{js,ts}',
    './app.vue',
  ],
  theme: {
    extend: {
      colors: {
        // B2X Brand Colors - Use these instead of Tailwind defaults
        primary: {
          50: '#f0f7ff',
          100: '#e0f0ff',
          200: '#bae0ff',
          300: '#7cc8ff',
          400: '#36b0ff',
          500: '#0b98ff',
          600: '#0078d4',
          700: '#0064b0',
          800: '#035390',
          900: '#053b73',
        },
        secondary: {
          50: '#f5f3ff',
          100: '#ede9fe',
          200: '#ddd6fe',
          300: '#c4b5fd',
          400: '#a78bfa',
          500: '#8b5cf6',
          600: '#7c3aed',
          700: '#6d28d9',
          800: '#5b21b6',
          900: '#4c1d95',
        },
        danger: {
          50: '#fef2f2',
          100: '#fee2e2',
          200: '#fecaca',
          300: '#fca5a5',
          400: '#f87171',
          500: '#ef4444',
          600: '#dc2626',
          700: '#b91c1c',
          800: '#991b1b',
          900: '#7f1d1d',
        },
        // DaisyUI semantic colors - prefer these over Tailwind defaults
        base: {
          100: 'hsl(var(--b1))',
          200: 'hsl(var(--b2))',
          300: 'hsl(var(--b3))',
          content: 'hsl(var(--bc))',
        },
        'primary-content': 'hsl(var(--pc))',
        'secondary-content': 'hsl(var(--sc))',
        accent: 'hsl(var(--a))',
        'accent-content': 'hsl(var(--ac))',
        neutral: 'hsl(var(--n))',
        'neutral-content': 'hsl(var(--nc))',
        info: 'hsl(var(--in))',
        'info-content': 'hsl(var(--inc))',
        success: 'hsl(var(--su))',
        'success-content': 'hsl(var(--suc))',
        warning: 'hsl(var(--wa))',
        'warning-content': 'hsl(var(--wac))',
        error: 'hsl(var(--er))',
        'error-content': 'hsl(var(--erc))',
      },
      fontFamily: {
        sans: [
          '-apple-system',
          'BlinkMacSystemFont',
          'Segoe UI',
          'Roboto',
          'Oxygen',
          'Ubuntu',
          'Cantarell',
          'Fira Sans',
          'Droid Sans',
          'Helvetica Neue',
          'sans-serif',
        ],
      },
      spacing: {
        // Design tokens for consistent spacing
        xs: '0.25rem', // 4px
        sm: '0.5rem', // 8px
        md: '1rem', // 16px
        lg: '1.5rem', // 24px
        xl: '2rem', // 32px
        '2xl': '3rem', // 48px
        '3xl': '4rem', // 64px
        '128': '32rem',
      },
      fontSize: {
        // Typography scale for consistency
        xs: ['0.75rem', { lineHeight: '1rem' }], // 12px
        sm: ['0.875rem', { lineHeight: '1.25rem' }], // 14px
        base: ['1rem', { lineHeight: '1.5rem' }], // 16px
        lg: ['1.125rem', { lineHeight: '1.75rem' }], // 18px
        xl: ['1.25rem', { lineHeight: '1.75rem' }], // 20px
        '2xl': ['1.5rem', { lineHeight: '2rem' }], // 24px
        '3xl': ['1.875rem', { lineHeight: '2.25rem' }], // 30px
        '4xl': ['2.25rem', { lineHeight: '2.5rem' }], // 36px
        '5xl': ['3rem', { lineHeight: '1' }], // 48px
        '6xl': ['3.75rem', { lineHeight: '1' }], // 60px
      },
      boxShadow: {
        xs: '0 1px 2px 0 rgb(0 0 0 / 0.05)',
        sm: '0 1px 3px 0 rgb(0 0 0 / 0.1), 0 1px 2px -1px rgb(0 0 0 / 0.1)',
        md: '0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)',
        lg: '0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1)',
        xl: '0 20px 25px -5px rgb(0 0 0 / 0.1), 0 8px 10px -6px rgb(0 0 0 / 0.1)',
        '2xl': '0 25px 50px -12px rgb(0 0 0 / 0.25)',
        inner: 'inset 0 2px 4px 0 rgb(0 0 0 / 0.05)',
      },
    },
  },
  // DaisyUI v5 is configured via CSS imports, not plugins
  plugins: [],
} satisfies Config;
