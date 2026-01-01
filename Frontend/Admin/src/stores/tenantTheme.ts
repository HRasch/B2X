import { defineStore } from "pinia";
import { ref, computed } from "vue";
import axios from "axios";
// SCSS compiler is commented out - sass package doesn't work in browser
// TODO: Use a browser-compatible SCSS library like sass.js or move compilation to backend
// import { useScssCompiler } from "@/composables/useScssCompiler";

export interface TenantTheme {
  id: string;
  tenantId: string;
  name: string;
  description?: string;
  primaryColor?: string;
  secondaryColor?: string;
  tertiaryColor?: string;
  isActive: boolean;
  version: number;
  variables: DesignVariable[];
  variants: ThemeVariant[];
  scssFiles: ScssFile[];
  createdAt: string;
  updatedAt: string;
}

export interface DesignVariable {
  id: string;
  name: string;
  value: string;
  category: string;
  description: string;
  type: "string" | "color" | "size" | "number" | "boolean" | "json";
}

export interface ThemeVariant {
  id: string;
  name: string;
  description: string;
  variableOverrides: Record<string, string>;
  isEnabled: boolean;
}

export interface ScssFile {
  id?: string;
  fileName: string;
  content: string;
  description?: string;
  isActive: boolean;
  order: number;
}

export const useTenantThemeStore = defineStore("tenantTheme", () => {
  // State
  const themes = ref<TenantTheme[]>([]);
  const currentTheme = ref<TenantTheme | null>(null);
  const activeTheme = ref<TenantTheme | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);

  // SCSS Compiler - stubbed out (sass package doesn't work in browser)
  // TODO: Use a browser-compatible SCSS library or move compilation to backend
  const isCompiling = ref(false);
  const lastError = ref<string | null>(null);

  // Stub functions for SCSS compilation
  const compileScss = async (
    _scssContent: string,
    _variables: Record<string, string> = {}
  ) => {
    console.warn(
      "SCSS compilation is not available in browser. Use backend API instead."
    );
    return {
      css: "",
      variables: _variables,
      errors: ["SCSS compilation not available in browser"],
    };
  };

  const applyCompiledCss = (_css: string, _themeId: string) => {
    console.warn("applyCompiledCss is not available in browser.");
  };

  const removeTheme = (_themeId: string) => {
    console.warn("removeTheme CSS is not available in browser.");
  };

  // Getters
  const publishedThemes = computed(() =>
    themes.value.filter((theme) => theme.isActive)
  );

  // Actions
  const fetchThemes = async () => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get<TenantTheme[]>("/api/themes");
      themes.value = response.data;
    } catch (err) {
      error.value =
        err instanceof Error ? err.message : "Failed to fetch themes";
      console.error("Failed to fetch themes:", err);
    } finally {
      loading.value = false;
    }
  };

  const fetchActiveTheme = async () => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get<TenantTheme>("/api/themes/active");
      activeTheme.value = response.data;
      return response.data;
    } catch (err) {
      if (axios.isAxiosError(err) && err.response?.status === 404) {
        activeTheme.value = null;
        return null;
      }
      error.value =
        err instanceof Error ? err.message : "Failed to fetch active theme";
      console.error("Failed to fetch active theme:", err);
      return null;
    } finally {
      loading.value = false;
    }
  };

  const fetchThemeById = async (themeId: string) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get<TenantTheme>(`/api/themes/${themeId}`);
      currentTheme.value = response.data;
      return response.data;
    } catch (err) {
      error.value =
        err instanceof Error ? err.message : "Failed to fetch theme";
      console.error("Failed to fetch theme:", err);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const createTheme = async (themeData: {
    name: string;
    description?: string;
    isActive?: boolean;
  }) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.post<TenantTheme>("/api/themes", themeData);
      const newTheme = response.data;
      themes.value.push(newTheme);
      return newTheme;
    } catch (err) {
      error.value =
        err instanceof Error ? err.message : "Failed to create theme";
      console.error("Failed to create theme:", err);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const updateTheme = async (
    themeId: string,
    themeData: {
      name?: string;
      description?: string;
      isActive?: boolean;
    }
  ) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.put<TenantTheme>(
        `/api/themes/${themeId}`,
        themeData
      );
      const updatedTheme = response.data;
      const index = themes.value.findIndex((t) => t.id === themeId);
      if (index >= 0) {
        themes.value[index] = updatedTheme;
      }
      if (currentTheme.value?.id === themeId) {
        currentTheme.value = updatedTheme;
      }
      return updatedTheme;
    } catch (err) {
      error.value =
        err instanceof Error ? err.message : "Failed to update theme";
      console.error("Failed to update theme:", err);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const deleteTheme = async (themeId: string) => {
    loading.value = true;
    error.value = null;
    try {
      await axios.delete(`/api/themes/${themeId}`);
      themes.value = themes.value.filter((t) => t.id !== themeId);
      if (currentTheme.value?.id === themeId) {
        currentTheme.value = null;
      }
    } catch (err) {
      error.value =
        err instanceof Error ? err.message : "Failed to delete theme";
      console.error("Failed to delete theme:", err);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const publishTheme = async (themeId: string) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.post<TenantTheme>(
        `/api/themes/${themeId}/publish`
      );
      const publishedTheme = response.data;
      const index = themes.value.findIndex((t) => t.id === themeId);
      if (index >= 0) {
        themes.value[index] = publishedTheme;
      }
      return publishedTheme;
    } catch (err) {
      error.value =
        err instanceof Error ? err.message : "Failed to publish theme";
      console.error("Failed to publish theme:", err);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const unpublishTheme = async (themeId: string) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.post<TenantTheme>(
        `/api/themes/${themeId}/unpublish`
      );
      const unpublishedTheme = response.data;
      const index = themes.value.findIndex((t) => t.id === themeId);
      if (index >= 0) {
        themes.value[index] = unpublishedTheme;
      }
      return unpublishedTheme;
    } catch (err) {
      error.value =
        err instanceof Error ? err.message : "Failed to unpublish theme";
      console.error("Failed to unpublish theme:", err);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  // SCSS File operations
  const createScssFile = async (
    themeId: string,
    fileData: {
      fileName: string;
      content: string;
      description?: string;
      isActive?: boolean;
      order?: number;
    }
  ) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.post<ScssFile>(
        `/api/themes/${themeId}/scss-files`,
        fileData
      );
      const newFile = response.data;
      // Update current theme if it's the one being modified
      if (currentTheme.value?.id === themeId) {
        currentTheme.value.scssFiles.push(newFile);
      }
      return newFile;
    } catch (err) {
      error.value =
        err instanceof Error ? err.message : "Failed to create SCSS file";
      console.error("Failed to create SCSS file:", err);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const updateScssFile = async (
    themeId: string,
    fileId: string,
    fileData: {
      fileName?: string;
      content?: string;
      description?: string;
      isActive?: boolean;
      order?: number;
    }
  ) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.put<ScssFile>(
        `/api/themes/${themeId}/scss-files/${fileId}`,
        fileData
      );
      const updatedFile = response.data;
      // Update current theme if it's the one being modified
      if (currentTheme.value?.id === themeId) {
        const index = currentTheme.value.scssFiles.findIndex(
          (f) => f.id === fileId
        );
        if (index >= 0) {
          currentTheme.value.scssFiles[index] = updatedFile;
        }
      }
      return updatedFile;
    } catch (err) {
      error.value =
        err instanceof Error ? err.message : "Failed to update SCSS file";
      console.error("Failed to update SCSS file:", err);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const deleteScssFile = async (themeId: string, fileId: string) => {
    loading.value = true;
    error.value = null;
    try {
      await axios.delete(`/api/themes/${themeId}/scss-files/${fileId}`);
      // Update current theme if it's the one being modified
      if (currentTheme.value?.id === themeId) {
        currentTheme.value.scssFiles = currentTheme.value.scssFiles.filter(
          (f) => f.id !== fileId
        );
      }
    } catch (err) {
      error.value =
        err instanceof Error ? err.message : "Failed to delete SCSS file";
      console.error("Failed to delete SCSS file:", err);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const fetchScssFiles = async (themeId: string) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get<ScssFile[]>(
        `/api/themes/${themeId}/scss-files`
      );
      // Update current theme if it's the one being modified
      if (currentTheme.value?.id === themeId) {
        currentTheme.value.scssFiles = response.data;
      }
      return response.data;
    } catch (err) {
      error.value =
        err instanceof Error ? err.message : "Failed to fetch SCSS files";
      console.error("Failed to fetch SCSS files:", err);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  // Generate CSS
  const generateCSS = async (themeId: string) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get<string>(`/api/themes/${themeId}/css`);
      return response.data;
    } catch (err) {
      error.value =
        err instanceof Error ? err.message : "Failed to generate CSS";
      console.error("Failed to generate CSS:", err);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  // SCSS Compilation Methods
  const compileAndApplyTheme = async (theme: TenantTheme) => {
    try {
      // Combine all SCSS files for the theme
      const scssContent = theme.scssFiles
        .filter((file) => file.isActive)
        .sort((a, b) => a.order - b.order)
        .map((file) => file.content)
        .join("\n\n");

      // Prepare variables
      const variables: Record<string, string> = {};
      theme.variables.forEach((variable) => {
        variables[variable.name] = variable.value;
      });

      // Add theme colors as variables
      if (theme.primaryColor) variables["primary-color"] = theme.primaryColor;
      if (theme.secondaryColor)
        variables["secondary-color"] = theme.secondaryColor;
      if (theme.tertiaryColor)
        variables["tertiary-color"] = theme.tertiaryColor;

      // Compile SCSS
      const compiled = await compileScss(scssContent, variables);

      if (compiled.errors.length > 0) {
        error.value = `SCSS Compilation Error: ${compiled.errors.join(", ")}`;
        return false;
      }

      // Apply the compiled CSS
      applyCompiledCss(compiled.css, theme.id);
      return true;
    } catch (err) {
      error.value =
        err instanceof Error ? err.message : "Failed to compile theme";
      console.error("Theme compilation error:", err);
      return false;
    }
  };

  const removeAppliedTheme = (themeId: string) => {
    removeTheme(themeId);
  };

  return {
    // State
    themes,
    currentTheme,
    activeTheme,
    loading,
    error,

    // Getters
    publishedThemes,

    // Actions
    fetchThemes,
    getThemes: fetchThemes, // Alias for component compatibility
    fetchActiveTheme,
    fetchThemeById,
    createTheme,
    updateTheme,
    deleteTheme,
    publishTheme,
    unpublishTheme,
    createScssFile,
    updateScssFile,
    deleteScssFile,
    fetchScssFiles,
    getScssFiles: fetchScssFiles, // Alias for component compatibility
    generateCSS,

    // SCSS Compilation
    compileAndApplyTheme,
    removeAppliedTheme,
    isCompiling,
    compilationError: lastError,
  };
});
