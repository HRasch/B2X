using System.Text;

namespace B2X.CLI.Services;

public class VueComponentTemplate : ITemplateProvider
{
    public Template GenerateTemplate(string name, bool tenantAware = false)
    {
        var componentName = $"{name}Component";
        var fileName = $"{componentName}.vue";

        var content = new StringBuilder();
        content.AppendLine("<template>");
        content.AppendLine("  <div class=\"component-container\">");
        content.AppendLine("    <!-- Error boundary wrapper -->");
        content.AppendLine("    <ErrorBoundary @error=\"handleError\">");
        content.AppendLine("      <div v-if=\"loading\" class=\"loading-state\">");
        content.AppendLine("        <div class=\"spinner\"></div>");
        content.AppendLine("        <p>Loading...</p>");
        content.AppendLine("      </div>");
        content.AppendLine();
        content.AppendLine("      <div v-else-if=\"error\" class=\"error-state\">");
        content.AppendLine("        <div class=\"error-message\">");
        content.AppendLine("          <h3>Oops! Something went wrong</h3>");
        content.AppendLine("          <p>{{ error }}</p>");
        content.AppendLine("          <button @click=\"retry\" class=\"retry-button\">");
        content.AppendLine("            Try Again");
        content.AppendLine("          </button>");
        content.AppendLine("        </div>");
        content.AppendLine("      </div>");
        content.AppendLine();
        content.AppendLine("      <div v-else class=\"content\">");
        content.AppendLine("        <!-- Main component content -->");
        content.AppendLine("        <h2>{{ title }}</h2>");
        content.AppendLine("        <div class=\"component-body\">");
        content.AppendLine("          <!-- TODO: Add your component content here -->");
        content.AppendLine("          <p>Component content goes here</p>");
        content.AppendLine("        </div>");
        content.AppendLine("      </div>");
        content.AppendLine("    </ErrorBoundary>");
        content.AppendLine("  </div>");
        content.AppendLine("</template>");
        content.AppendLine();
        content.AppendLine("<script setup lang=\"ts\">");
        content.AppendLine("import { ref, computed, onMounted, onUnmounted } from 'vue'");
        content.AppendLine("import { useErrorBoundary } from '@/composables/useErrorBoundary'");
        if (tenantAware)
        {
            content.AppendLine("import { useTenant } from '@/composables/useTenant'");
        }
        content.AppendLine();
        content.AppendLine("// Props with validation");
        content.AppendLine("interface Props {");
        if (tenantAware)
        {
            content.AppendLine("  tenantId?: string");
        }
        content.AppendLine("  title?: string");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine("const props = withDefaults(defineProps<Props>(), {");
        if (tenantAware)
        {
            content.AppendLine("  tenantId: undefined,");
        }
        content.AppendLine("  title: 'Default Title'");
        content.AppendLine("})");
        content.AppendLine();
        content.AppendLine("// Emits");
        content.AppendLine("const emit = defineEmits<{");
        content.AppendLine("  error: [error: string]");
        content.AppendLine("  success: [data: any]");
        content.AppendLine("}>()");
        content.AppendLine();
        content.AppendLine("// Reactive state");
        content.AppendLine("const loading = ref(false)");
        content.AppendLine("const error = ref<string | null>(null)");
        content.AppendLine("const data = ref<any>(null)");
        content.AppendLine();
        if (tenantAware)
        {
            content.AppendLine("// Tenant context");
            content.AppendLine("const { currentTenant, isValidTenant } = useTenant()");
            content.AppendLine();
            content.AppendLine("// Computed tenant validation");
            content.AppendLine("const effectiveTenantId = computed(() => {");
            content.AppendLine("  return props.tenantId || currentTenant.value?.id");
            content.AppendLine("})");
            content.AppendLine();
            content.AppendLine("const isTenantValid = computed(() => {");
            content.AppendLine("  return isValidTenant(effectiveTenantId.value)");
            content.AppendLine("})");
            content.AppendLine();
        }
        content.AppendLine("// Error boundary");
        content.AppendLine("const { handleError, clearError } = useErrorBoundary((err: Error) => {");
        content.AppendLine("  console.error('Component error:', err)");
        content.AppendLine("  error.value = err.message");
        content.AppendLine("  emit('error', err.message)");
        content.AppendLine("})");
        content.AppendLine();
        content.AppendLine("// Methods");
        content.AppendLine("const loadData = async () => {");
        content.AppendLine("  try {");
        content.AppendLine("    loading.value = true");
        content.AppendLine("    error.value = null");
        content.AppendLine("    clearError()");
        content.AppendLine();
        if (tenantAware)
        {
            content.AppendLine("    // Validate tenant before API call");
            content.AppendLine("    if (!isTenantValid.value) {");
            content.AppendLine("      throw new Error('Invalid or missing tenant context')");
            content.AppendLine("    }");
            content.AppendLine();
        }
        content.AppendLine("    // TODO: Implement data loading logic");
        content.AppendLine("    // const response = await api.getData({ tenantId: effectiveTenantId.value })");
        content.AppendLine("    // data.value = response.data");
        content.AppendLine();
        content.AppendLine("    // Simulate API call");
        content.AppendLine("    await new Promise(resolve => setTimeout(resolve, 1000))");
        content.AppendLine("    data.value = { message: 'Data loaded successfully' }");
        content.AppendLine();
        content.AppendLine("    emit('success', data.value)");
        content.AppendLine("  } catch (err) {");
        content.AppendLine("    const errorMessage = err instanceof Error ? err.message : 'Unknown error occurred'");
        content.AppendLine("    handleError(err instanceof Error ? err : new Error(errorMessage))");
        content.AppendLine("  } finally {");
        content.AppendLine("    loading.value = false");
        content.AppendLine("  }");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine("const retry = () => {");
        content.AppendLine("  loadData()");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine("// Lifecycle");
        content.AppendLine("onMounted(() => {");
        content.AppendLine("  loadData()");
        content.AppendLine("})");
        content.AppendLine();
        content.AppendLine("onUnmounted(() => {");
        content.AppendLine("  // Cleanup if needed");
        content.AppendLine("})");
        content.AppendLine("</script>");
        content.AppendLine();
        content.AppendLine("<style scoped lang=\"scss\">");
        content.AppendLine(".component-container {");
        content.AppendLine("  position: relative;");
        content.AppendLine("  min-height: 200px;");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine(".loading-state {");
        content.AppendLine("  display: flex;");
        content.AppendLine("  flex-direction: column;");
        content.AppendLine("  align-items: center;");
        content.AppendLine("  justify-content: center;");
        content.AppendLine("  padding: 2rem;");
        content.AppendLine("  color: #666;");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine(".spinner {");
        content.AppendLine("  width: 40px;");
        content.AppendLine("  height: 40px;");
        content.AppendLine("  border: 4px solid #f3f3f3;");
        content.AppendLine("  border-top: 4px solid #3498db;");
        content.AppendLine("  border-radius: 50%;");
        content.AppendLine("  animation: spin 1s linear infinite;");
        content.AppendLine("  margin-bottom: 1rem;");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine("@keyframes spin {");
        content.AppendLine("  0% { transform: rotate(0deg); }");
        content.AppendLine("  100% { transform: rotate(360deg); }");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine(".error-state {");
        content.AppendLine("  display: flex;");
        content.AppendLine("  align-items: center;");
        content.AppendLine("  justify-content: center;");
        content.AppendLine("  padding: 2rem;");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine(".error-message {");
        content.AppendLine("  text-align: center;");
        content.AppendLine("  color: #e74c3c;");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine(".error-message h3 {");
        content.AppendLine("  margin-bottom: 1rem;");
        content.AppendLine("  color: #c0392b;");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine(".retry-button {");
        content.AppendLine("  margin-top: 1rem;");
        content.AppendLine("  padding: 0.5rem 1rem;");
        content.AppendLine("  background-color: #3498db;");
        content.AppendLine("  color: white;");
        content.AppendLine("  border: none;");
        content.AppendLine("  border-radius: 4px;");
        content.AppendLine("  cursor: pointer;");
        content.AppendLine("  transition: background-color 0.2s;");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine(".retry-button:hover {");
        content.AppendLine("  background-color: #2980b9;");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine(".content {");
        content.AppendLine("  padding: 1rem;");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine(".component-body {");
        content.AppendLine("  margin-top: 1rem;");
        content.AppendLine("}");
        content.AppendLine("</style>");

        var warnings = new List<string>();
        warnings.Add("Replace TODO comments with actual component logic");
        warnings.Add("Ensure ErrorBoundary component is available in your project");
        warnings.Add("Verify useErrorBoundary composable exists or create it");
        if (tenantAware)
        {
            warnings.Add("Ensure useTenant composable is implemented");
            warnings.Add("Add tenant validation logic as needed");
        }
        warnings.Add("Test component with different error scenarios");
        warnings.Add("Add proper TypeScript interfaces for API responses");

        return new Template
        {
            FileName = fileName,
            Content = content.ToString(),
            Warnings = warnings
        };
    }
}
