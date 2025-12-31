# Supported Markdown Syntax on GitHub (GFM)

This page summarizes the Markdown features supported by GitHub (GitHub Flavored Markdown — GFM), with short examples and notes about limitations.

## Overview

- GitHub renders Markdown using GFM. It supports standard Markdown plus several useful extensions and HTML blocks.
- Reference: https://docs.github.com/en/get-started/writing-on-github/working-with-advanced-formatting/

## Core Markdown (supported)

- **Headings**: `#`, `##`, `###`, etc.
- **Emphasis**: `*italic*`, `**bold**`, `***bold italic***`.
- **Inline code**: `` `code` `` and **code fences** for blocks:

```
```csharp
// fenced code block with language
Console.WriteLine("Hello");
```
```
- **Ordered & unordered lists**: `-`, `*`, `1.`
- **Blockquotes**: `> quote`
- **Horizontal rules**: `---`, `***`, `___`

## GitHub Extensions

- **Tables**: simple pipe-separated tables

  | Name | Description |
  |------|-------------|
  | Foo  | Example     |

- **Task lists**: `- [ ]` and `- [x]` (render as checkboxes)
- **Strikethrough**: `~~deleted~~`
- **Automatic linking**: GitHub autolinks bare URLs and recognizes issue/PR/commit/sha references when appropriate (e.g., `#123`, `owner/repo#123`, `@username`).
- **Emoji shortcodes**: `:smile:` → 😄 (most common emoji supported)
- **Mentioning**: `@username` and team/org mentions (subject to permissions)
- **Syntax highlighting**: specify language after fence (e.g., ```js)
- **HTML blocks**: raw HTML is allowed in most cases (useful for tables, details/summary, styling). Use carefully — some sanitization applies.

## Images and Links

- **Images**: `![alt text](path/to/image.png)` — relative paths work in repos.
- **Reference-style links**: supported.

## Advanced / Special Cases

- **Collapsible sections**: use HTML `<details>` / `<summary>` blocks.
- **Task list check/uncheck in PRs**: interactive in GitHub UI when authored in issues/PR descriptions.
- **Front matter**: used by GitHub Pages (Jekyll) on branch pages, not for repo README rendering.

## Limitations & Gotchas

- **Math**: GitHub README rendering does not natively support LaTeX math. Math can be used on GitHub Pages via MathJax or other tools, but not in plain repo README rendering.
- **Footnotes**: not universally supported as a core GFM feature; behavior can vary — prefer inline or HTML alternatives.
- **Raw HTML sanitization**: some HTML may be sanitized or altered for security.
- **Extensions vary**: some Markdown processors (e.g., Jekyll, kramdown) add features that GitHub's repo viewer does not.

## Quick Examples

- Heading

```
# Project title
```

- Table

```
| Column | Type |
|--------|------|
| A      | int  |
```

- Task list

```
- [ ] Write tests
- [x] Add docs
```

## Where to check for updates

- Official docs: https://docs.github.com/en/get-started/writing-on-github/working-with-advanced-formatting/
- For renderer changes look at GitHub release notes and repository docs.

---

If you want, I can also:

- add this file to a README index under .ai/knowledgebase/,
- or expand this doc with more examples (images, HTML snippets, PR/issue autolink examples).
