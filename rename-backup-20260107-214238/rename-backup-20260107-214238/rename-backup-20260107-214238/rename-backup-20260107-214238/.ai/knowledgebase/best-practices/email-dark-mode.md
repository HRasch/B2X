---
docid: KB-027
title: Email Dark Mode Best Practices
owner: "@Frontend"
status: Active
last_updated: 2026-01-05
---

# Email Dark Mode Best Practices

## Overview

Modern email clients (iOS Mail, Gmail, Outlook, Apple Mail) support dark mode via `prefers-color-scheme` media queries. B2X's email builder automatically includes dark mode support in generated templates.

## How It Works

### 1. Meta Tags

Every email template includes:

```html
<meta name="color-scheme" content="light dark">
<meta name="supported-color-schemes" content="light dark">
```

These tags signal to email clients that the email supports both light and dark modes.

### 2. Media Query Support

Automatic dark mode CSS injection:

```css
@media (prefers-color-scheme: dark) {
  body { 
    background-color: #1f2937 !important; 
    color: #f9fafb !important; 
  }
  .container { background-color: #111827 !important; }
  a { color: #60a5fa !important; }
  h1, h2, h3, h4, h5, h6 { color: #f9fafb !important; }
}
```

### 3. Editor Preview

**Dark Mode Preview Toggle** (🌙 button in GrapesJS toolbar):
- Click to preview how email appears in dark mode
- No changes to actual email content
- Helps designers verify dark mode appearance

## Email Client Support

| Client | Dark Mode Support | Notes |
|--------|------------------|-------|
| iOS Mail | ✅ Full | Best support, respects `prefers-color-scheme` |
| macOS Mail | ✅ Full | Native support |
| Gmail (iOS/Android) | ✅ Partial | Applies own dark styles |
| Outlook (iOS/Android) | ✅ Partial | Limited CSS support |
| Gmail (Web) | ⚠️ Limited | Strips some styles |
| Outlook (Desktop) | ❌ None | No dark mode support |

## Best Practices

### Colors

**DO:**
- Use semantic color variables
- Test both light and dark modes
- Ensure sufficient contrast (WCAG AA: 4.5:1)
- Use `!important` for critical dark mode overrides

**DON'T:**
- Hardcode white backgrounds
- Use low-contrast colors
- Rely on images for text
- Forget to test in actual email clients

### Images

```html
<!-- Add dark mode alternative images -->
<img src="logo-light.png" class="light-mode-img" alt="Logo">
<img src="logo-dark.png" class="dark-mode-img" alt="Logo">
```

```css
@media (prefers-color-scheme: dark) {
  .light-mode-img { display: none !important; }
  .dark-mode-img { display: block !important; }
}

@media (prefers-color-scheme: light) {
  .light-mode-img { display: block !important; }
  .dark-mode-img { display: none !important; }
}
```

### Tables (Email Safe Layout)

```html
<table role="presentation" cellspacing="0" cellpadding="0" border="0" width="100%">
  <tr>
    <td style="background-color: #ffffff; color: #000000;">
      Content
    </td>
  </tr>
</table>
```

```css
@media (prefers-color-scheme: dark) {
  td { 
    background-color: #1f2937 !important; 
    color: #f9fafb !important; 
  }
}
```

### Typography

```css
/* Light mode */
h1 { color: #1f2937; }
p { color: #4b5563; }

/* Dark mode */
@media (prefers-color-scheme: dark) {
  h1 { color: #f9fafb !important; }
  p { color: #d1d5db !important; }
}
```

## Testing Checklist

- [ ] Preview in GrapesJS dark mode toggle
- [ ] Test in iOS Mail (light & dark)
- [ ] Test in Gmail app (light & dark)
- [ ] Test in macOS Mail
- [ ] Verify logo visibility in both modes
- [ ] Check link colors (sufficient contrast)
- [ ] Validate buttons/CTAs stand out
- [ ] Test on real devices (not just simulators)

## Common Issues

### Issue: Styles Stripped by Gmail

**Solution:** Use inline styles + media queries
```html
<td style="background-color: #ffffff;">
  <!-- Gmail respects inline styles -->
</td>
```

### Issue: Images Invisible in Dark Mode

**Solution:** Add light border or use PNG with transparency
```css
img {
  border: 1px solid rgba(255, 255, 255, 0.1);
}
```

### Issue: Text Unreadable

**Solution:** Always override with `!important`
```css
@media (prefers-color-scheme: dark) {
  * { color: #f9fafb !important; }
}
```

## Implementation in B2X

### EmailBuilderWYSIWYG.vue

Automatically injects dark mode CSS when `component:update` fires:

```typescript
const darkModeCss = `
  @media (prefers-color-scheme: dark) {
    body { background-color: #1f2937 !important; color: #f9fafb !important; }
    .container { background-color: #111827 !important; }
    a { color: #60a5fa !important; }
    h1, h2, h3, h4, h5, h6 { color: #f9fafb !important; }
  }
`;
```

### Dark Mode Preview Command

Toggle canvas preview without modifying email:

```typescript
editor.Commands.add('toggle-dark-preview', {
  run: editor => {
    const wrapper = editor.getWrapper();
    const isDark = wrapper.getClasses().includes('dark-mode-preview');
    if (isDark) {
      wrapper.removeClass('dark-mode-preview');
    } else {
      wrapper.addClass('dark-mode-preview');
    }
  },
});
```

## Resources

- [Litmus Email Dark Mode Guide](https://www.litmus.com/blog/the-ultimate-guide-to-dark-mode-for-email-marketers/)
- [CSS-Tricks Dark Mode Email](https://css-tricks.com/dark-mode-email/)
- [Apple Mail Dark Mode](https://developer.apple.com/design/human-interface-guidelines/dark-mode)
- [Can I Email Support Table](https://www.caniemail.com/search/?s=prefers-color-scheme)

## See Also

- [KB-023] Email Template Best Practices
- [KB-026] Monaco Editor Vue Integration
- [REQ-007] Email WYSIWYG Builder

---

**Maintained by:** @Frontend  
**Review Frequency:** Quarterly (email client support changes)  
**Next Review:** 2026-04-05
