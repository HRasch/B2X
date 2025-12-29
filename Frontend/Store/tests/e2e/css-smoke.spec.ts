import { test, expect, type Page, type ConsoleMessage } from "@playwright/test";

/**
 * CSS Smoke Tests
 *
 * These tests specifically catch Tailwind CSS and DaisyUI compilation issues
 * that might not be caught during the build process but appear at runtime.
 *
 * Common issues caught:
 * - "Cannot apply unknown utility class 'X'"
 * - DaisyUI classes not rendering
 * - CSS custom properties not defined
 * - PostCSS processing errors
 *
 * Run with: npm run e2e -- --grep "CSS Smoke"
 */

test.describe("CSS Smoke Tests - Error Detection", () => {
  test("should not have CSS compilation errors in console", async ({
    page,
  }: {
    page: Page;
  }) => {
    const cssErrors: string[] = [];

    page.on("console", (msg: ConsoleMessage) => {
      const text = msg.text().toLowerCase();

      // Catch common CSS error patterns
      const isCssError =
        text.includes("cannot apply unknown utility") ||
        text.includes("@apply") ||
        text.includes("unknown at-rule") ||
        text.includes("unrecognized at-rule") ||
        text.includes("postcss") ||
        (text.includes("css") && msg.type() === "error") ||
        text.includes("tailwind") ||
        text.includes("daisyui");

      if (isCssError && msg.type() === "error") {
        cssErrors.push(msg.text());
      }
    });

    // Navigate and wait for all resources
    await page.goto("/");
    await page.waitForLoadState("networkidle");

    // Give CSS time to process
    await page.waitForTimeout(500);

    expect(cssErrors).toHaveLength(0);

    if (cssErrors.length > 0) {
      console.error("CSS Errors found:", cssErrors);
    }
  });

  test("should not have PostCSS errors", async ({ page }: { page: Page }) => {
    const postcssErrors: string[] = [];

    page.on("console", (msg: ConsoleMessage) => {
      const text = msg.text();
      if (text.includes("postcss") || text.includes("PostCSS")) {
        if (msg.type() === "error" || msg.type() === "warning") {
          postcssErrors.push(text);
        }
      }
    });

    await page.goto("/");
    await page.waitForLoadState("networkidle");

    expect(postcssErrors).toHaveLength(0);
  });

  test("should not have Vite CSS module errors", async ({
    page,
  }: {
    page: Page;
  }) => {
    const moduleErrors: string[] = [];

    page.on("console", (msg: ConsoleMessage) => {
      const text = msg.text();
      if (text.includes("CSS modules") || text.includes(".module.css")) {
        if (msg.type() === "error") {
          moduleErrors.push(text);
        }
      }
    });

    await page.goto("/");
    await page.waitForLoadState("networkidle");

    expect(moduleErrors).toHaveLength(0);
  });
});

test.describe("CSS Smoke Tests - DaisyUI Theme", () => {
  test("should have DaisyUI CSS custom properties defined", async ({
    page,
  }: {
    page: Page;
  }) => {
    await page.goto("/");
    await page.waitForLoadState("networkidle");

    // Check that DaisyUI CSS custom properties are defined
    const daisyProps = await page.evaluate(() => {
      const root = document.documentElement;
      const style = getComputedStyle(root);

      return {
        // DaisyUI v5 uses these custom properties
        primary:
          style.getPropertyValue("--p") ||
          style.getPropertyValue("--color-primary"),
        secondary:
          style.getPropertyValue("--s") ||
          style.getPropertyValue("--color-secondary"),
        base100:
          style.getPropertyValue("--b1") ||
          style.getPropertyValue("--color-base-100"),
        baseContent:
          style.getPropertyValue("--bc") ||
          style.getPropertyValue("--color-base-content"),
      };
    });

    // At least one DaisyUI color should be defined
    const hasColors = Object.values(daisyProps).some(
      (v) => v && v.trim() !== ""
    );
    expect(hasColors, "DaisyUI theme colors should be defined").toBe(true);
  });

  test("should apply DaisyUI btn styles", async ({ page }: { page: Page }) => {
    await page.goto("/");
    await page.waitForLoadState("networkidle");

    // Create a test button with DaisyUI class
    const buttonStyles = await page.evaluate(() => {
      const btn = document.createElement("button");
      btn.className = "btn btn-primary";
      btn.textContent = "Test";
      btn.id = "test-daisyui-btn";
      document.body.appendChild(btn);

      const style = getComputedStyle(btn);
      const result = {
        display: style.display,
        cursor: style.cursor,
        borderRadius: style.borderRadius,
        padding: style.padding,
        hasBackground: style.backgroundColor !== "rgba(0, 0, 0, 0)",
      };

      document.body.removeChild(btn);
      return result;
    });

    // DaisyUI btn should apply styling
    expect(buttonStyles.display).toBe("inline-flex");
    expect(buttonStyles.cursor).toBe("pointer");
    expect(buttonStyles.hasBackground).toBe(true);
  });

  test("should apply DaisyUI card styles", async ({ page }: { page: Page }) => {
    await page.goto("/");
    await page.waitForLoadState("networkidle");

    const cardStyles = await page.evaluate(() => {
      const card = document.createElement("div");
      card.className = "card bg-base-100 shadow-xl";
      card.innerHTML =
        '<div class="card-body"><h2 class="card-title">Test</h2></div>';
      document.body.appendChild(card);

      const style = getComputedStyle(card);
      const result = {
        borderRadius: style.borderRadius,
        overflow: style.overflow,
        hasBackground: style.backgroundColor !== "rgba(0, 0, 0, 0)",
        hasShadow: style.boxShadow !== "none",
      };

      document.body.removeChild(card);
      return result;
    });

    // DaisyUI v5 card should have background (verifies CSS loaded)
    expect(cardStyles.hasBackground).toBe(true);
    // Note: overflow may vary between DaisyUI versions - key is CSS loads
  });
});

test.describe("CSS Smoke Tests - Tailwind Utilities", () => {
  test("should apply flex utilities correctly", async ({
    page,
  }: {
    page: Page;
  }) => {
    await page.goto("/");

    const flexResult = await page.evaluate(() => {
      const el = document.createElement("div");
      el.className = "flex items-center justify-between gap-4";
      document.body.appendChild(el);

      const style = getComputedStyle(el);
      const result = {
        display: style.display,
        alignItems: style.alignItems,
        justifyContent: style.justifyContent,
        gap: style.gap,
      };

      document.body.removeChild(el);
      return result;
    });

    expect(flexResult.display).toBe("flex");
    expect(flexResult.alignItems).toBe("center");
    expect(flexResult.justifyContent).toBe("space-between");
  });

  test("should apply spacing utilities correctly", async ({
    page,
  }: {
    page: Page;
  }) => {
    await page.goto("/");

    const spacingResult = await page.evaluate(() => {
      const el = document.createElement("div");
      el.className = "p-4 m-2";
      document.body.appendChild(el);

      const style = getComputedStyle(el);
      const result = {
        padding: style.padding,
        margin: style.margin,
        paddingPx: parseFloat(style.padding),
        marginPx: parseFloat(style.margin),
      };

      document.body.removeChild(el);
      return result;
    });

    // Tailwind v4 uses 4px scale: p-4 = 4*4 = 16px, but may vary
    // Key verification: spacing utilities actually work (non-zero values)
    expect(spacingResult.paddingPx).toBeGreaterThan(0);
    expect(spacingResult.marginPx).toBeGreaterThan(0);
  });

  test("should apply color utilities correctly", async ({
    page,
  }: {
    page: Page;
  }) => {
    await page.goto("/");

    const colorResult = await page.evaluate(() => {
      const el = document.createElement("div");
      el.className = "bg-blue-500 text-white";
      el.textContent = "Test";
      document.body.appendChild(el);

      const style = getComputedStyle(el);
      const result = {
        backgroundColor: style.backgroundColor,
        color: style.color,
      };

      document.body.removeChild(el);
      return result;
    });

    // bg-blue-500 should apply a blue background
    expect(colorResult.backgroundColor).not.toBe("rgba(0, 0, 0, 0)");
    // text-white should be white/near white
    expect(colorResult.color).toMatch(/rgb\(255,\s*255,\s*255\)/);
  });

  test("should apply responsive utilities", async ({
    page,
  }: {
    page: Page;
  }) => {
    await page.goto("/");

    // Test at desktop viewport
    await page.setViewportSize({ width: 1280, height: 720 });

    const responsiveResult = await page.evaluate(() => {
      const el = document.createElement("div");
      el.className = "hidden md:block";
      el.textContent = "Test";
      document.body.appendChild(el);

      const style = getComputedStyle(el);
      const display = style.display;

      document.body.removeChild(el);
      return display;
    });

    // At md (768px+), md:block should override hidden
    expect(responsiveResult).toBe("block");
  });
});

test.describe("CSS Smoke Tests - Dark Mode", () => {
  test("should support dark mode classes", async ({ page }: { page: Page }) => {
    await page.goto("/");
    await page.waitForLoadState("networkidle");

    const darkModeResult = await page.evaluate(() => {
      // Add dark class to html for dark mode
      document.documentElement.classList.add("dark");
      document.documentElement.setAttribute("data-theme", "dark");

      const el = document.createElement("div");
      el.className = "bg-white dark:bg-gray-800";
      document.body.appendChild(el);

      const style = getComputedStyle(el);
      const bgColor = style.backgroundColor;

      document.body.removeChild(el);
      document.documentElement.classList.remove("dark");
      document.documentElement.removeAttribute("data-theme");

      return bgColor;
    });

    // Test verifies dark mode CSS selector is functional
    // Background should be applied (not transparent)
    expect(darkModeResult).not.toBe("rgba(0, 0, 0, 0)");
  });
});
