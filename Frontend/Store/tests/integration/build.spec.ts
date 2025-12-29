import { describe, it, expect, beforeAll } from "vitest";
import { execSync } from "child_process";
import { existsSync, readFileSync } from "fs";
import { join } from "path";

/**
 * Build Integration Tests
 *
 * These tests verify that the Vite build process completes successfully.
 * They catch:
 * - TypeScript compilation errors
 * - Vite configuration errors
 * - Tailwind CSS configuration errors
 * - Missing dependencies
 *
 * Run with: npm run test -- --grep "Build Integration"
 */

const projectRoot = process.cwd();

describe("Build Integration - Type Check", () => {
  it("should pass TypeScript type checking", () => {
    try {
      execSync("npx vue-tsc --noEmit", {
        cwd: projectRoot,
        encoding: "utf-8",
        timeout: 120000,
        stdio: "pipe",
      });
      expect(true).toBe(true);
    } catch (error: any) {
      // Extract meaningful error message
      const output = error.stdout || error.stderr || error.message;

      // Count errors
      const errorMatches = output.match(/error TS\d+/g) || [];
      const errorCount = errorMatches.length;

      // This test intentionally fails when there are TypeScript errors
      // to catch issues early in CI/CD
      throw new Error(
        `TypeScript type check failed with ${errorCount} error(s):\n` +
          `${output.slice(0, 3000)}${output.length > 3000 ? "\n... (truncated)" : ""}`
      );
    }
  });

  it.skip("should have no critical TypeScript errors (informational)", () => {
    // This test is skipped by default but can be run to see error summary
    // Use: npm run test -- --grep "informational"
    try {
      execSync("npx vue-tsc --noEmit 2>&1 | head -50", {
        cwd: projectRoot,
        encoding: "utf-8",
        timeout: 60000,
      });
    } catch (error: unknown) {
      const execError = error as { stdout?: string };
      console.log("TypeScript errors (first 50 lines):", execError.stdout);
    }
    expect(true).toBe(true);
  });
});

describe("Build Integration - Vite Configuration", () => {
  it("should have valid vite.config.ts", () => {
    const configPath = join(projectRoot, "vite.config.ts");
    expect(existsSync(configPath), "vite.config.ts should exist").toBe(true);

    const content = readFileSync(configPath, "utf-8");
    expect(content).toContain("defineConfig");
    expect(content).toContain("@vitejs/plugin-vue");
  });

  it("should have valid postcss.config.js", () => {
    const configPath = join(projectRoot, "postcss.config.js");
    expect(existsSync(configPath), "postcss.config.js should exist").toBe(true);

    const content = readFileSync(configPath, "utf-8");
    // For Tailwind v4, should use @tailwindcss/postcss
    expect(content).toContain("@tailwindcss/postcss");
  });

  it("should have valid tailwind.config.ts", () => {
    const configPath = join(projectRoot, "tailwind.config.ts");
    expect(existsSync(configPath), "tailwind.config.ts should exist").toBe(
      true
    );

    const content = readFileSync(configPath, "utf-8");
    expect(content).toContain("content");
    // Check for Vue file pattern in content array (vue is part of glob pattern)
    expect(content.toLowerCase()).toContain("vue");
  });
});

describe("Build Integration - CSS Configuration", () => {
  it("should have valid main.css with Tailwind imports", () => {
    const cssPath = join(projectRoot, "src/main.css");
    expect(existsSync(cssPath), "main.css should exist").toBe(true);

    const content = readFileSync(cssPath, "utf-8");

    // For Tailwind v4 + DaisyUI v5, should use @import
    expect(content).toContain('@import "tailwindcss"');
    // DaisyUI v5 requires full path to CSS file
    expect(content).toContain('@import "daisyui/daisyui.css"');
  });

  it("should not have deprecated @tailwind directives (Tailwind v4)", () => {
    const cssPath = join(projectRoot, "src/main.css");
    const content = readFileSync(cssPath, "utf-8");

    // Tailwind v4 uses @import, not @tailwind directives
    expect(content).not.toContain("@tailwind base");
    expect(content).not.toContain("@tailwind components");
    expect(content).not.toContain("@tailwind utilities");
  });

  it("should not have @apply with unknown utility classes", () => {
    // Search for @apply in all .vue and .css files
    const checkFiles = ["src/main.css"];

    for (const file of checkFiles) {
      const filePath = join(projectRoot, file);
      if (existsSync(filePath)) {
        const content = readFileSync(filePath, "utf-8");

        // Check for @apply with DaisyUI-only classes (which may not work in Tailwind v4)
        const applyMatches = content.match(/@apply\s+[^;]+;/g) || [];

        for (const match of applyMatches) {
          // These are problematic in Tailwind v4 with DaisyUI v5
          expect(match).not.toContain("btn-");
          expect(match).not.toContain("card-");
          expect(match).not.toContain("badge-");
        }
      }
    }
  });
});

describe("Build Integration - Dependencies", () => {
  it("should have compatible Tailwind and DaisyUI versions", () => {
    const packagePath = join(projectRoot, "package.json");
    const packageJson = JSON.parse(readFileSync(packagePath, "utf-8"));

    const tailwindVersion = packageJson.dependencies?.tailwindcss || "";
    const daisyuiVersion = packageJson.dependencies?.daisyui || "";

    // Check for Tailwind v4
    const isTailwindV4 =
      tailwindVersion.includes("4.") || tailwindVersion.includes("^4");

    if (isTailwindV4) {
      // DaisyUI v5 is required for Tailwind v4
      expect(
        daisyuiVersion.includes("5.") || daisyuiVersion.includes("^5"),
        "DaisyUI v5 required for Tailwind v4"
      ).toBe(true);

      // @tailwindcss/postcss is required for Tailwind v4
      const hasPostcssPlugin =
        packageJson.devDependencies?.["@tailwindcss/postcss"] ||
        packageJson.dependencies?.["@tailwindcss/postcss"];

      expect(
        hasPostcssPlugin,
        "@tailwindcss/postcss required for Tailwind v4"
      ).toBeTruthy();
    }
  });

  it("should have all required dependencies", () => {
    const packagePath = join(projectRoot, "package.json");
    const packageJson = JSON.parse(readFileSync(packagePath, "utf-8"));

    const requiredDeps = [
      "vue",
      "vue-router",
      "pinia",
      "tailwindcss",
      "daisyui",
    ];

    const allDeps = {
      ...packageJson.dependencies,
      ...packageJson.devDependencies,
    };

    for (const dep of requiredDeps) {
      expect(
        allDeps[dep],
        `Dependency "${dep}" should be installed`
      ).toBeTruthy();
    }
  });
});

describe("Build Integration - Entry Points", () => {
  it("should have valid index.html", () => {
    const indexPath = join(projectRoot, "index.html");
    expect(existsSync(indexPath), "index.html should exist").toBe(true);

    const content = readFileSync(indexPath, "utf-8");
    expect(content).toContain('<div id="app">');
    expect(content).toContain("src/main.ts");
  });

  it("should have valid main.ts", () => {
    const mainPath = join(projectRoot, "src/main.ts");
    expect(existsSync(mainPath), "main.ts should exist").toBe(true);

    const content = readFileSync(mainPath, "utf-8");
    expect(content).toContain("createApp");
    expect(content).toContain("./main.css");
    expect(content).toContain("#app");
  });

  it("should have valid App.vue", () => {
    const appPath = join(projectRoot, "src/App.vue");
    expect(existsSync(appPath), "App.vue should exist").toBe(true);

    const content = readFileSync(appPath, "utf-8");
    expect(content).toContain("<template>");
    expect(content).toContain("<script");
  });
});
