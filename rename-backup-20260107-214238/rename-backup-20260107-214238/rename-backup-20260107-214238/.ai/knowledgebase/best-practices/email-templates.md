# Email Template Best Practices

**DocID**: `KB-BP-EMAIL`  
**Last Updated**: 2026-01-03  
**Maintained By**: GitHub Copilot  
**Status**: ✅ Active

---

## Overview

Creating HTML emails that render consistently across email clients requires understanding the unique constraints of email rendering engines. Unlike modern browsers, email clients have limited and inconsistent CSS support, making email development more challenging than web development.

---

## Official Resources

- **Can I Email** - CSS support reference for email clients
  - URL: https://www.caniemail.com/
  - Status: ✅ Essential reference

- **Email on Acid** - Testing and best practices
  - URL: https://www.emailonacid.com/blog/
  - Status: ✅ Industry standard

- **Litmus** - Email testing and analytics
  - URL: https://www.litmus.com/resources/
  - Status: ✅ Industry standard

- **Campaign Monitor CSS Guide**
  - URL: https://www.campaignmonitor.com/css/
  - Status: ✅ Comprehensive CSS support guide

- **Really Good Emails** - Email design inspiration
  - URL: https://reallygoodemails.com/
  - Status: ✅ Design reference

---

## Core Principles

### 1. Use Tables for Layout

**Why**: Email clients have inconsistent support for modern CSS layout (Flexbox, Grid). Tables are the most reliable layout method.

```html
<!-- ✅ DO: Use tables for layout -->
<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%">
  <tr>
    <td align="center">
      <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600">
        <tr>
          <td>Content here</td>
        </tr>
      </table>
    </td>
  </tr>
</table>

<!-- ❌ DON'T: Use div-based layouts -->
<div style="display: flex; justify-content: center;">
  <div style="max-width: 600px;">Content</div>
</div>
```

### 2. Inline CSS Styles

**Why**: Many email clients strip `<style>` blocks from the `<head>`. Inline styles ensure consistent rendering.

```html
<!-- ✅ DO: Inline styles -->
<td style="font-family: Arial, Helvetica, sans-serif; font-size: 16px; color: #333333; padding: 20px;">
  Your content here
</td>

<!-- ❌ DON'T: Rely only on external/head styles -->
<td class="content">Your content here</td>
```

### 3. Use Web-Safe Fonts

**Why**: Custom fonts have limited support. Always provide fallbacks.

```html
<!-- ✅ DO: Web-safe font stack with fallbacks -->
<td style="font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;">

<!-- For custom fonts (when supported) -->
<td style="font-family: 'Open Sans', 'Helvetica Neue', Helvetica, Arial, sans-serif;">
```

**Web-Safe Font Stacks:**
| Type | Stack |
|------|-------|
| Sans-serif | `Arial, Helvetica, sans-serif` |
| Serif | `Georgia, Times, 'Times New Roman', serif` |
| Monospace | `'Courier New', Courier, monospace` |

---

## CSS Support Reference

### ✅ Well-Supported Properties

| Property | Support | Notes |
|----------|---------|-------|
| `background-color` | ✅ Universal | Use hex colors |
| `color` | ✅ Universal | Use hex colors |
| `font-family` | ✅ Universal | Use web-safe stacks |
| `font-size` | ✅ Universal | Use px units |
| `font-weight` | ✅ Universal | `normal`, `bold`, or numeric |
| `line-height` | ✅ Universal | Use px or unitless |
| `text-align` | ✅ Universal | `left`, `center`, `right` |
| `padding` | ✅ Universal | Use on `<td>` elements |
| `border` | ✅ Universal | Shorthand supported |
| `width` | ✅ Universal | Use px or % |
| `height` | ⚠️ Partial | Avoid on `<td>` |

### ⚠️ Partially Supported Properties

| Property | Support | Workaround |
|----------|---------|------------|
| `margin` | ⚠️ Limited | Use `padding` on parent `<td>` |
| `max-width` | ⚠️ Limited | Use wrapper table with fixed width |
| `border-radius` | ⚠️ Limited | Works on images, not all elements |
| `box-shadow` | ⚠️ Limited | Use image fallback |
| `background-image` | ⚠️ Limited | Use VML for Outlook fallback |

### ❌ Avoid These Properties

| Property | Reason |
|----------|--------|
| `display: flex` | No Outlook support |
| `display: grid` | No Outlook support |
| `position` | Inconsistent support |
| `float` | Unreliable in emails |
| `calc()` | Limited support |
| `var()` | No support |
| `@media` queries | Partial support (see Responsive section) |

---

## Email Template Structure

### Basic Template Skeleton

```html
<!DOCTYPE html>
<html lang="en" xmlns="http://www.w3.org/1999/xhtml" xmlns:v="urn:schemas-microsoft-com:vml" xmlns:o="urn:schemas-microsoft-com:office:office">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <meta http-equiv="X-UA-Compatible" content="IE=edge">
  <meta name="x-apple-disable-message-reformatting">
  <meta name="format-detection" content="telephone=no, date=no, address=no, email=no">
  <title>Email Subject Line</title>
  
  <!--[if mso]>
  <noscript>
    <xml>
      <o:OfficeDocumentSettings>
        <o:PixelsPerInch>96</o:PixelsPerInch>
      </o:OfficeDocumentSettings>
    </xml>
  </noscript>
  <![endif]-->
  
  <style>
    /* Reset styles */
    body, table, td, p, a, li { -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }
    table, td { mso-table-lspace: 0pt; mso-table-rspace: 0pt; }
    img { -ms-interpolation-mode: bicubic; border: 0; height: auto; line-height: 100%; outline: none; text-decoration: none; }
    body { margin: 0; padding: 0; width: 100% !important; height: 100% !important; }
    
    /* Client-specific resets */
    #outlook a { padding: 0; }
    .ReadMsgBody { width: 100%; }
    .ExternalClass { width: 100%; }
    .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div { line-height: 100%; }
    
    /* Responsive styles (for supported clients) */
    @media screen and (max-width: 600px) {
      .mobile-full-width { width: 100% !important; }
      .mobile-padding { padding: 10px !important; }
      .mobile-hide { display: none !important; }
      .mobile-center { text-align: center !important; }
    }
  </style>
</head>
<body style="margin: 0; padding: 0; background-color: #f4f4f4;">
  
  <!-- Preview text (hidden) -->
  <div style="display: none; max-height: 0; overflow: hidden;">
    Preview text appears in email client preview...
    &nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;
  </div>
  
  <!-- Email wrapper -->
  <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="background-color: #f4f4f4;">
    <tr>
      <td align="center" style="padding: 20px 10px;">
        
        <!-- Email container (600px max) -->
        <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600" class="mobile-full-width" style="background-color: #ffffff;">
          
          <!-- Header -->
          <tr>
            <td align="center" style="padding: 30px 20px; background-color: #1a1a1a;">
              <img src="logo.png" alt="Company Logo" width="150" style="display: block;">
            </td>
          </tr>
          
          <!-- Content -->
          <tr>
            <td style="padding: 40px 30px; font-family: Arial, Helvetica, sans-serif; font-size: 16px; line-height: 24px; color: #333333;">
              <h1 style="margin: 0 0 20px; font-size: 24px; font-weight: bold; color: #1a1a1a;">
                Email Heading
              </h1>
              <p style="margin: 0 0 20px;">
                Your email content goes here. Keep paragraphs short and scannable.
              </p>
              
              <!-- CTA Button -->
              <table role="presentation" cellpadding="0" cellspacing="0" border="0">
                <tr>
                  <td align="center" style="border-radius: 4px; background-color: #007bff;">
                    <a href="https://example.com" target="_blank" style="display: inline-block; padding: 14px 30px; font-family: Arial, Helvetica, sans-serif; font-size: 16px; font-weight: bold; color: #ffffff; text-decoration: none;">
                      Call to Action
                    </a>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          
          <!-- Footer -->
          <tr>
            <td style="padding: 30px; background-color: #f8f9fa; font-family: Arial, Helvetica, sans-serif; font-size: 12px; line-height: 18px; color: #666666; text-align: center;">
              <p style="margin: 0 0 10px;">
                © 2026 Company Name. All rights reserved.
              </p>
              <p style="margin: 0;">
                <a href="#" style="color: #666666;">Unsubscribe</a> | 
                <a href="#" style="color: #666666;">Privacy Policy</a>
              </p>
            </td>
          </tr>
          
        </table>
        
      </td>
    </tr>
  </table>
  
</body>
</html>
```

---

## Button Best Practices

### Bulletproof Button (Works Everywhere)

```html
<!-- Bulletproof button using table -->
<table role="presentation" cellpadding="0" cellspacing="0" border="0">
  <tr>
    <td align="center" bgcolor="#007bff" style="border-radius: 4px;">
      <!--[if mso]>
      <v:roundrect xmlns:v="urn:schemas-microsoft-com:vml" xmlns:w="urn:schemas-microsoft-com:office:word" href="https://example.com" style="height:44px;v-text-anchor:middle;width:200px;" arcsize="10%" strokecolor="#007bff" fillcolor="#007bff">
        <w:anchorlock/>
        <center style="color:#ffffff;font-family:Arial,sans-serif;font-size:16px;font-weight:bold;">
          Button Text
        </center>
      </v:roundrect>
      <![endif]-->
      <!--[if !mso]><!-->
      <a href="https://example.com" target="_blank" style="display: inline-block; padding: 12px 24px; font-family: Arial, Helvetica, sans-serif; font-size: 16px; font-weight: bold; color: #ffffff; text-decoration: none; border-radius: 4px; background-color: #007bff;">
        Button Text
      </a>
      <!--<![endif]-->
    </td>
  </tr>
</table>
```

### Button Color Palette

| Purpose | Background | Text |
|---------|------------|------|
| Primary | `#007bff` | `#ffffff` |
| Success | `#28a745` | `#ffffff` |
| Warning | `#ffc107` | `#212529` |
| Danger | `#dc3545` | `#ffffff` |
| Secondary | `#6c757d` | `#ffffff` |

---

## Images in Email

### Image Best Practices

```html
<!-- ✅ DO: Proper image markup -->
<img 
  src="https://example.com/image.jpg" 
  alt="Descriptive alt text" 
  width="600" 
  height="300"
  style="display: block; max-width: 100%; height: auto; border: 0;"
>

<!-- For responsive images -->
<img 
  src="https://example.com/image.jpg" 
  alt="Descriptive alt text" 
  width="600"
  style="display: block; width: 100%; max-width: 600px; height: auto; border: 0;"
  class="mobile-full-width"
>
```

### Image Checklist

- ✅ Always include `alt` text
- ✅ Set explicit `width` and `height` attributes
- ✅ Use `display: block` to prevent gaps
- ✅ Host images on HTTPS URLs
- ✅ Use absolute URLs (not relative)
- ✅ Compress images (aim for < 200KB total)
- ✅ Use `border: 0` to prevent blue link borders
- ⚠️ Don't rely on images for critical content (images may be blocked)

### Image Formats

| Format | Use Case | Support |
|--------|----------|---------|
| JPG | Photos, complex images | ✅ Universal |
| PNG | Logos, graphics with transparency | ✅ Universal |
| GIF | Simple animations | ✅ Universal |
| SVG | ❌ Avoid | ❌ Poor support |
| WebP | ❌ Avoid | ❌ Limited support |

---

## Responsive Email Design

### Mobile-First Approach

```html
<style>
  /* Base styles (mobile) */
  .content { padding: 20px; }
  .column { width: 100%; display: block; }
  
  /* Desktop styles (for clients that support media queries) */
  @media screen and (min-width: 600px) {
    .content { padding: 40px; }
    .column { width: 50%; display: inline-block; }
  }
</style>
```

### Responsive Column Layout

```html
<!-- Two-column layout that stacks on mobile -->
<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%">
  <tr>
    <!--[if mso]>
    <td valign="top" width="300">
    <![endif]-->
    <!--[if !mso]><!-->
    <td valign="top" style="display: inline-block; width: 100%; max-width: 300px;">
    <!--<![endif]-->
      <!-- Column 1 content -->
    </td>
    <!--[if mso]>
    </td><td valign="top" width="300">
    <![endif]-->
    <!--[if !mso]><!-->
    <td valign="top" style="display: inline-block; width: 100%; max-width: 300px;">
    <!--<![endif]-->
      <!-- Column 2 content -->
    </td>
  </tr>
</table>
```

### Media Query Support

| Client | @media Support |
|--------|---------------|
| Apple Mail | ✅ Full |
| iOS Mail | ✅ Full |
| Gmail App (Android) | ✅ Full |
| Gmail (Web) | ⚠️ Limited |
| Outlook (Desktop) | ❌ None |
| Outlook.com | ⚠️ Limited |
| Yahoo Mail | ✅ Full |

---

## Dark Mode Support

### Dark Mode Detection

```html
<style>
  /* Default (light mode) */
  .dark-mode-bg { background-color: #ffffff !important; }
  .dark-mode-text { color: #333333 !important; }
  
  /* Dark mode overrides */
  @media (prefers-color-scheme: dark) {
    .dark-mode-bg { background-color: #1a1a1a !important; }
    .dark-mode-text { color: #f4f4f4 !important; }
  }
  
  /* Outlook dark mode */
  [data-ogsc] .dark-mode-bg { background-color: #1a1a1a !important; }
  [data-ogsc] .dark-mode-text { color: #f4f4f4 !important; }
</style>
```

### Dark Mode Best Practices

- ✅ Use transparent PNGs for logos (avoid white backgrounds)
- ✅ Test with both light and dark backgrounds
- ✅ Ensure sufficient contrast in both modes
- ✅ Use `color-scheme: light dark;` meta tag
- ⚠️ Some clients invert colors automatically

---

## Accessibility

### WCAG Email Compliance

```html
<!-- ✅ Semantic structure -->
<h1 style="...">Main Heading</h1>
<p style="...">Paragraph content</p>

<!-- ✅ Accessible links -->
<a href="https://example.com" style="color: #007bff; text-decoration: underline;">
  Read more about our products
</a>

<!-- ✅ Image alt text -->
<img src="product.jpg" alt="Red running shoes, Nike Air Max, $129.99">

<!-- ✅ Table accessibility -->
<table role="presentation" ...> <!-- Layout tables -->
<table role="table" ...> <!-- Data tables with headers -->

<!-- ✅ Language attribute -->
<html lang="en">
```

### Accessibility Checklist

- ✅ Use `role="presentation"` on layout tables
- ✅ Maintain 4.5:1 contrast ratio for text
- ✅ Don't rely on color alone to convey information
- ✅ Use descriptive link text (not "click here")
- ✅ Include alt text for all images
- ✅ Use semantic headings (h1, h2, etc.)
- ✅ Keep line length under 80 characters
- ✅ Use minimum 14px font size
- ✅ Test with screen readers

---

## Outlook-Specific Fixes

### Conditional Comments

```html
<!--[if mso]>
  Outlook-only code here
<![endif]-->

<!--[if !mso]><!-->
  Non-Outlook code here
<!--<![endif]-->

<!--[if mso 12]>
  Outlook 2007 only
<![endif]-->

<!--[if gte mso 15]>
  Outlook 2013 and later
<![endif]-->
```

### Common Outlook Fixes

```html
<!-- Fix Outlook DPI scaling -->
<!--[if gte mso 9]>
<xml>
  <o:OfficeDocumentSettings>
    <o:AllowPNG/>
    <o:PixelsPerInch>96</o:PixelsPerInch>
  </o:OfficeDocumentSettings>
</xml>
<![endif]-->

<!-- Fix Outlook table spacing -->
<table style="mso-table-lspace: 0pt; mso-table-rspace: 0pt;">

<!-- Outlook-safe line height -->
<td style="line-height: 24px; mso-line-height-rule: exactly;">

<!-- Force Outlook to respect width -->
<!--[if mso]>
<table role="presentation" cellpadding="0" cellspacing="0" width="600">
<tr><td>
<![endif]-->
  <!-- Content -->
<!--[if mso]>
</td></tr>
</table>
<![endif]-->
```

---

## Testing Checklist

### Pre-Send Testing

- [ ] Test in major email clients (use Litmus or Email on Acid)
- [ ] Verify all links work correctly
- [ ] Check images load properly
- [ ] Test with images disabled
- [ ] Validate HTML
- [ ] Check spam score
- [ ] Review on mobile devices
- [ ] Test dark mode rendering
- [ ] Run accessibility check
- [ ] Verify personalization tokens

### Major Clients to Test

| Priority | Client | Market Share |
|----------|--------|-------------|
| 🔴 Critical | Apple Mail | ~58% |
| 🔴 Critical | Gmail | ~28% |
| 🔴 Critical | Outlook | ~6% |
| 🟡 Important | Yahoo Mail | ~3% |
| 🟡 Important | Samsung Mail | ~2% |
| 🟢 Good-to-have | Thunderbird | <1% |

---

## Performance Optimization

### File Size Guidelines

| Element | Recommended Max |
|---------|----------------|
| Total email size | < 100KB |
| HTML | < 30KB |
| All images combined | < 200KB |
| Individual image | < 50KB |

### Optimization Tips

- ✅ Minify HTML before sending
- ✅ Compress images (TinyPNG, ImageOptim)
- ✅ Use image CDN with caching
- ✅ Avoid embedded base64 images
- ✅ Limit number of images
- ✅ Remove unnecessary whitespace
- ✅ Consolidate inline styles where possible

---

## Common Mistakes to Avoid

| Mistake | Problem | Solution |
|---------|---------|----------|
| Using `<div>` for layout | Inconsistent rendering | Use tables |
| External CSS only | Styles stripped | Inline all styles |
| Missing alt text | Accessibility fail | Always include alt |
| Relative image URLs | Images won't load | Use absolute HTTPS URLs |
| Over 100KB email | Clipped in Gmail | Optimize and reduce |
| No fallback fonts | Font may not render | Always use web-safe stack |
| Using JavaScript | Not supported | Remove all JS |
| Form elements | Not supported | Link to web form |
| Video embeds | Limited support | Use GIF or image with play button |

---

## B2X Implementation

### Email Templates Location
- Backend templates: `backend/services/email/templates/`
- Use Razor templates with inline CSS generation

### Recommended Libraries

**.NET Email Libraries:**
- `FluentEmail` - Fluent API for sending emails
- `Razor.Templating.Core` - Razor template rendering
- `PreMailer.Net` - Inline CSS processing
- `HtmlAgilityPack` - HTML manipulation

### Example Template Service

```csharp
// Inline CSS processing before sending
var inlinedHtml = PreMailer.Net.PreMailer.MoveCssInline(
    html,
    removeStyleElements: true,
    ignoreElements: "#outlook-fix"
).Html;
```

---

## Related Documentation

- [OWASP Security for Email](./owasp-top-ten.md) - Security considerations
- [Accessibility Standards](../standards/wcag-2.1.md) - WCAG compliance
- [.NET Email Services](../dotnet-features.md) - Backend implementation

---

**Next Review**: 2026-04-03 (Quarterly)
