# MCP Tools Code Style Prevention

**Date**: 6. Januar 2026  
**Status**: ✅ Implemented

## Problem
TypeScript-based MCP tools (TypeScriptMCP, VueMCP, HtmlCssMCP, SecurityMCP, etc.) had no linting/formatting, unlike the frontend which has comprehensive Prettier + ESLint setup.

## Solution Implemented

### 1. Added Linting to All TypeScript MCP Tools

**Updated package.json files for:**
- TypeScriptMCP
- VueMCP  
- HtmlCssMCP
- SecurityMCP
- WolverineMCP
- B2XMCP
- PlaywrightMCP

**Added scripts:**
```json
{
  "format": "prettier --write \"src/**/*.{ts,js,json}\"",
  "format:check": "prettier --check \"src/**/*.{ts,js,json}\"",
  "lint": "eslint src --ext .ts",
  "lint:fix": "eslint src --ext .ts --fix"
}
```

**Added devDependencies:**
```json
"@typescript-eslint/eslint-plugin": "^6.0.0",
"@typescript-eslint/parser": "^6.0.0", 
"eslint": "^8.0.0",
"prettier": "^3.3.3"
```

### 2. Shared Configuration Files

**Created:**
- [tools/.eslintrc.js](tools/.eslintrc.js) - TypeScript ESLint rules
- [tools/.prettierrc](tools/.prettierrc) - Prettier formatting rules

### 3. Updated Pre-commit Hooks

**Modified [.husky/pre-commit](.husky/pre-commit):**
- Added TypeScript file formatting for all MCP tools
- Formats staged `.ts`, `.js`, `.json` files in tools/ with Prettier

### 4. Updated Root Scripts

**Added to [package.json](package.json):**
```json
"format:tools": "for dir in tools/TypeScriptMCP tools/VueMCP...; do (cd $dir && npm run format); done",
"check:tools": "for dir in tools/TypeScriptMCP...; do (cd $dir && npm run format:check); done",
"format:all": "npm run format && npm run format:backend && npm run format:tools",
"check:all": "npm run format:check && npm run check:backend && npm run check:tools"
```

### 5. Updated lint-staged

**Added to [package.json](package.json):**
```json
"tools/**/*.{ts,js,json}": [
  "prettier --write --ignore-unknown",
  "eslint --fix"
]
```

### 6. Updated Documentation

**Modified [tools/README.md](tools/README.md):**
- Added TypeScript MCP tools code style section
- Updated development workflow

## Developer Workflow

### Before Every Commit
Pre-commit hooks run automatically, or manually:
```bash
npm run format:all
```

### Verify Changes
```bash
npm run check:all
```

### Manual Formatting
```bash
# Format all tools
npm run format:tools

# Format specific tool
cd tools/TypeScriptMCP && npm run format
```

## Quality Gates

All code changes must pass:
- ✅ `npm run check:all` (frontend + backend + tools formatting)
- ✅ ESLint rules (TypeScript best practices)
- ✅ Prettier formatting (consistent style)
- ✅ Pre-commit hooks (automatic formatting)

## Configuration Files

| File | Purpose | Scope |
|------|---------|-------|
| `tools/.eslintrc.js` | TypeScript ESLint rules | All MCP tools |
| `tools/.prettierrc` | Prettier formatting | All MCP tools |
| `tools/Directory.Build.props` | C# build settings | C# MCP tools |
| `tools/stylecop.json` | StyleCop rules | C# MCP tools |
| `.husky/pre-commit` | Git hook | All staged files |
| `package.json` scripts | Tool formatting | Root commands |
| `package.json` lint-staged | Pre-commit config | Staged files |

## What Changed for Developers

### Before
```bash
# Inconsistent: Frontend had linting, MCP tools had none
git commit
# Sometimes: Style issues in MCP tools
```

### After
```bash
# Consistent: All code follows same standards
git commit
# Hook runs: dotnet format (C#) + prettier (TypeScript) + eslint
# ✅ All code formatted before push
```

## Verification

- ✅ Installed dependencies for all MCP tools
- ✅ Added linting scripts to all TypeScript MCP package.json files
- ✅ Created shared ESLint/Prettier configs
- ✅ Updated pre-commit hooks
- ✅ Updated root npm scripts
- ✅ Updated lint-staged configuration

## Next Steps

1. ✅ All TypeScript MCP tools now have linting/formatting
2. ✅ Pre-commit hooks prevent unformatted commits  
3. ✅ CI/CD can now check all code consistently
4. 📝 Update developer onboarding docs

## References

- [GL-006] Token Optimization Strategy
- [INS-001] Backend Instructions  
- [ADR-020] PR Quality Gate
- [.ai/knowledgebase/code-style-prevention.md](.ai/knowledgebase/code-style-prevention.md)

---

**Implemented by**: @CopilotExpert  
**Approved by**: @TechLead
