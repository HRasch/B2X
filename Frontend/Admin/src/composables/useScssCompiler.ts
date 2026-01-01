import { ref, readonly } from "vue";
import * as sass from "sass";

export interface CompiledTheme {
  css: string;
  variables: Record<string, string>;
  errors: string[];
}

export const useScssCompiler = () => {
  const isCompiling = ref(false);
  const lastError = ref<string | null>(null);

  const compileScss = async (
    scssContent: string,
    variables: Record<string, string> = {}
  ): Promise<CompiledTheme> => {
    isCompiling.value = true;
    lastError.value = null;

    try {
      // Prepare SCSS with variables
      let scssWithVars = scssContent;

      // Replace variable placeholders with actual values
      Object.entries(variables).forEach(([key, value]) => {
        const varPattern = new RegExp(`\\$\\{${key}\\}`, "g");
        scssWithVars = scssWithVars.replace(varPattern, value);
      });

      // Compile SCSS to CSS
      const result = await sass.compileStringAsync(scssWithVars, {
        style: "compressed",
        sourceMap: false,
      });

      return {
        css: result.css,
        variables,
        errors: [],
      };
    } catch (error) {
      const errorMessage =
        error instanceof Error ? error.message : "Unknown compilation error";
      lastError.value = errorMessage;

      return {
        css: "",
        variables,
        errors: [errorMessage],
      };
    } finally {
      isCompiling.value = false;
    }
  };

  const applyCompiledCss = (css: string, themeId: string) => {
    // Remove existing theme styles
    const existingStyle = document.getElementById(`theme-${themeId}`);
    if (existingStyle) {
      existingStyle.remove();
    }

    // Create new style element
    const styleElement = document.createElement("style");
    styleElement.id = `theme-${themeId}`;
    styleElement.textContent = css;
    document.head.appendChild(styleElement);
  };

  const removeTheme = (themeId: string) => {
    const styleElement = document.getElementById(`theme-${themeId}`);
    if (styleElement) {
      styleElement.remove();
    }
  };

  return {
    isCompiling: readonly(isCompiling),
    lastError: readonly(lastError),
    compileScss,
    applyCompiledCss,
    removeTheme,
  };
};
