/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{vue,js,ts,jsx,tsx}"],
  darkMode: "class",
  theme: {
    extend: {
      colors: {
        // Soft UI Custom Colors
        soft: {
          50: "#f8f9fa",
          100: "#f0f2f5",
          200: "#e9ecef",
          300: "#dee2e6",
          400: "#ced4da",
          500: "#adb5bd",
          600: "#6c757d",
          700: "#495057",
          800: "#343a40",
          900: "#212529",
        },
        // Primary Colors (Corporate Blue)
        primary: {
          50: "#f0f9ff",
          100: "#e0f2fe",
          200: "#bae6fd",
          300: "#7dd3fc",
          400: "#38bdf8",
          500: "#0ea5e9",
          600: "#0284c7",
          700: "#0369a1",
          800: "#075985",
          900: "#0c2d57",
        },
        // Success Colors
        success: {
          50: "#f0fdf4",
          100: "#dcfce7",
          200: "#bbf7d0",
          300: "#86efac",
          400: "#4ade80",
          500: "#22c55e",
          600: "#16a34a",
          700: "#15803d",
          800: "#166534",
          900: "#14532d",
        },
        // Warning Colors
        warning: {
          50: "#fffbeb",
          100: "#fef3c7",
          200: "#fde68a",
          300: "#fcd34d",
          400: "#fbbf24",
          500: "#f59e0b",
          600: "#d97706",
          700: "#b45309",
          800: "#92400e",
          900: "#78350f",
        },
        // Danger Colors
        danger: {
          50: "#fef2f2",
          100: "#fee2e2",
          200: "#fecaca",
          300: "#fca5a5",
          400: "#f87171",
          500: "#ef4444",
          600: "#dc2626",
          700: "#b91c1c",
          800: "#991b1b",
          900: "#7f1d1d",
        },
        // Info Colors
        info: {
          50: "#ecf0ff",
          100: "#e0e7ff",
          200: "#c7d2fe",
          300: "#a5b4fc",
          400: "#818cf8",
          500: "#6366f1",
          600: "#4f46e5",
          700: "#4338ca",
          800: "#3730a3",
          900: "#312e81",
        },
      },
      boxShadow: {
        // Soft UI Shadows
        "soft-xs": "0 2px 4px rgba(52, 71, 103, 0.1)",
        "soft-sm": "0 4px 6px rgba(52, 71, 103, 0.1)",
        "soft-md": "0 6px 12px rgba(52, 71, 103, 0.1)",
        "soft-lg": "0 8px 16px rgba(52, 71, 103, 0.12)",
        "soft-xl": "0 12px 24px rgba(52, 71, 103, 0.15)",
        "soft-2xl": "0 16px 32px rgba(52, 71, 103, 0.18)",
        "soft-inner": "inset 0 2px 4px rgba(255, 255, 255, 0.5)",
        "soft-bottom":
          "0 8px 16px rgba(52, 71, 103, 0.15), inset 0 -2px 0 rgba(255, 255, 255, 0.5)",
      },
      borderRadius: {
        soft: "0.75rem",
        "soft-lg": "1rem",
        "soft-xl": "1.5rem",
        "soft-2xl": "2rem",
      },
      spacing: {
        safe: "1.5rem",
        "safe-lg": "2.5rem",
      },
      backgroundImage: {
        "gradient-soft-cyan":
          "linear-gradient(135deg, #0ea5e9 0%, #06b6d4 100%)",
        "gradient-soft-blue":
          "linear-gradient(135deg, #3b82f6 0%, #0284c7 100%)",
        "gradient-soft-purple":
          "linear-gradient(135deg, #8b5cf6 0%, #6d28d9 100%)",
        "gradient-soft-green":
          "linear-gradient(135deg, #10b981 0%, #059669 100%)",
        "gradient-soft-orange":
          "linear-gradient(135deg, #f59e0b 0%, #d97706 100%)",
        "gradient-soft-red":
          "linear-gradient(135deg, #ef4444 0%, #dc2626 100%)",
      },
      fontFamily: {
        sans: ["Inter", "Segoe UI", "sans-serif"],
      },
      opacity: {
        soft: "0.85",
      },
      transitionTimingFunction: {
        soft: "cubic-bezier(0.4, 0, 0.2, 1)",
      },
    },
  },
  plugins: [require("@tailwindcss/forms")],
};
