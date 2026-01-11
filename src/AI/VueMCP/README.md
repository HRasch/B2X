# Vue.js MCP Server

A Model Context Protocol (MCP) server for Vue.js development assistance in the B2X project. This server provides AI-powered analysis and tooling for Vue 3 components, Pinia state management, Vite tooling, i18n validation, and responsive design checks.

## Features

### 🧩 Component Analysis
- Parse Vue SFC (Single File Components)
- Analyze component structure (props, emits, slots)
- Extract composition API usage
- **Enhanced script setup analysis** with reactive variables, computed properties, and methods
- **Template AST parsing** for directive and element analysis

### 🔍 Component Usage Tracking
- Find all usages of Vue components across the project
- Track component dependencies and relationships

### 🌐 i18n Key Validation
- Extract and validate i18n key usage in components
- Check for hardcoded strings (following [GL-042] i18n strategy)
- **Validate keys against locale files** for missing translations
- Ensure proper translation key patterns

### 📱 Responsive Design Checks
- Analyze Tailwind CSS responsive classes
- Validate breakpoint usage across components
- Check mobile-first design patterns

### 🏪 Pinia Store Analysis
- Analyze Pinia store definitions
- Check state, getters, and actions structure
- Validate store composition patterns

### ⚡ Vite Tooling Integration
- **Build configuration analysis** with plugin detection
- **Bundle size analysis** for performance monitoring
- Dependency tracking and optimization suggestions

### ♿ Accessibility Checks
- **WCAG compliance validation** using axe-core
- Automated accessibility scoring
- Violation detection and reporting

## Installation

```bash
cd tools/VueMCP
npm install
npm run build
```

## Usage

### Development
```bash
npm run dev
```

### Production
```bash
npm run build
npm start
```

## MCP Tools

### `analyze_vue_component`
Analyzes a Vue component's structure and composition.

**Parameters:**
- `filePath`: Path to the Vue component file
- `workspacePath`: Workspace root directory

### `find_component_usage`
Finds all usages of a specific Vue component.

**Parameters:**
- `componentName`: Name of the component to find
- `workspacePath`: Workspace root directory

### `validate_i18n_keys`
Validates i18n key usage and checks for translation issues.

**Parameters:**
- `workspacePath`: Workspace root directory
- `componentPath`: Optional specific component to analyze

### `check_responsive_design`
Analyzes responsive design implementation.

**Parameters:**
- `filePath`: Path to the Vue component file
- `workspacePath`: Workspace root directory

### `analyze_pinia_store`
Analyzes Pinia store structure and patterns.

**Parameters:**
- `filePath`: Path to the Pinia store file
- `workspacePath`: Workspace root directory

## Integration with B2X

This MCP server is designed to work with the B2X frontend architecture:

- **Vue 3** with Composition API
- **Nuxt 3** framework
- **Pinia** for state management
- **Vue i18n** following [GL-042] strategy
- **Tailwind CSS** with DaisyUI
- **Vite** build tooling

## Development Status

This is a **prototype implementation** with basic functionality. Full features will be implemented in future iterations.

## Next Steps

1. **Enhanced Component Analysis**: Deep parsing of script setup, template AST
2. **i18n Integration**: Direct validation against locale files
3. **Vite Tooling**: Build analysis, dependency checking
4. **Performance Analysis**: Bundle size, lazy loading validation
5. **Testing Integration**: Component test coverage analysis
6. **Accessibility Checks**: WCAG compliance validation

## Contributing

Follow B2X development guidelines and coordinate with @Frontend agent for feature additions.