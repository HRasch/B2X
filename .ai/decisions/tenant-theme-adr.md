# ADR: Tenant Theme Architecture

## Status
Proposed

## Context
B2Connect requires tenant-specific theming capabilities to allow customization of the frontend appearance per tenant. This includes dynamic theme application, AI-assisted theme generation, and admin interfaces.

## Decision
We will implement a theme system using:
- Backend: Domain Service for theme management with CRUD operations, including SCSS file storage and background compilation jobs
- Frontend: DaisyUI integration with build-time SCSS compilation via PostCSS and runtime SCSS compilation for dynamic themes, Monaco editor for SCSS editing
- Build Process: Integrate Sass/SCSS alongside PostCSS for enhanced styling capabilities
- Design System: Documented design system based on DaisyUI with CSS custom properties for tenant theming
- AI: External API integration for natural language theme generation
- Storage: Database persistence with tenant isolation for themes and SCSS files
- Caching: Distributed cache with invalidation after theme changes
- Background Jobs: Wolverine-based background processing for SCSS compilation

### Build Process Integration
1. Configure Vite/PostCSS to process SCSS files alongside CSS
2. Add sass as a dependency for SCSS compilation
3. Set up SCSS entry points for tenant themes
4. Ensure PostCSS plugins (Tailwind, DaisyUI) work with SCSS imports
5. Implement build-time theme compilation for static assets
6. Maintain compatibility with runtime SCSS compilation for dynamic changes

## Consequences
### Positive
- Flexible theming system with both full SCSS compilation and lightweight CSS custom property overrides
- Design system ensures consistency across tenants
- AI enhances user experience
- Runtime compilation allows dynamic changes
- DaisyUI provides consistent component styling

### Negative
- Performance overhead from runtime compilation
- Complexity in AI integration
- Potential security risks in dynamic CSS
- Maintenance of design system documentation

### Risks
- SCSS compilation performance
- AI API reliability
- Theme validation and sanitization

## Alternatives Considered
- Static themes only: Too limiting
- Client-side only theming: No persistence
- CSS variables only: Limited customization

## Implementation Notes
- Use sass.js for runtime compilation when full theming is needed
- Implement CSS custom properties for lightweight runtime customization
- Define and document design system tokens based on DaisyUI
- Implement theme validation middleware
- Cache compiled CSS for performance
- Ensure tenant data isolation
- Integrate SCSS in build process:
  - Add `sass` package to frontend dependencies
  - Configure Vite to handle .scss files
  - Set up PostCSS with Tailwind and DaisyUI to process SCSS
  - Create SCSS mixins and variables for theme tokens
  - Ensure build-time compilation produces optimized CSS
- Maintain compatibility between build-time and runtime SCSS compilation
- SCSS File Management:
  - Store SCSS files as entities in database with tenant isolation
  - Use Monaco editor in Admin Frontend for editing with SCSS syntax support
  - Background job compiles SCSS to CSS using sass.js or server-side compiler
  - Invalidate cache after successful compilation
  - Integrate with existing runtime compilation for seamless theming
- @UI to lead design system definition
- @Frontend to implement build process integration and Monaco editor
- @Backend to implement SCSS storage, background jobs, and cache invalidation
- @DevOps to configure caching infrastructure
- @Architect to review architecture alignment