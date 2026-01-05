---
docid: KB-029
title: CSS Functions
owner: GitHub Copilot
status: Active
---

# CSS Functions

**Source:** MDN Web Docs - https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_Functions

**Last Updated:** January 5, 2026

## Overview

CSS value functions are statements that invoke special data processing or calculations to return a CSS value for a CSS property. They represent more complex data types and may take input arguments to calculate the return value.

## Syntax

```css
selector {
  property: function([argument]? [, argument]*);
}
```

Functions start with the function name, followed by parentheses containing arguments separated by commas or spaces.

## Categories of CSS Functions

### Transform Functions

Used with the `transform` property to modify the appearance of elements.

#### Translate Functions
- `translateX(value)` - Translates horizontally
- `translateY(value)` - Translates vertically  
- `translateZ(value)` - Translates along z-axis
- `translate(x, y)` - Translates on 2D plane
- `translate3d(x, y, z)` - Translates in 3D space

#### Rotation Functions
- `rotateX(angle)` - Rotates around horizontal axis
- `rotateY(angle)` - Rotates around vertical axis
- `rotateZ(angle)` - Rotates around z-axis
- `rotate(angle)` - Rotates around fixed point on 2D plane
- `rotate3d(x, y, z, angle)` - Rotates around fixed axis in 3D space

#### Scaling Functions
- `scaleX(value)` - Scales horizontally
- `scaleY(value)` - Scales vertically
- `scaleZ(value)` - Scales along z-axis
- `scale(x, y)` - Scales on 2D plane
- `scale3d(x, y, z)` - Scales in 3D space

#### Skew Functions
- `skewX(angle)` - Skews horizontally
- `skewY(angle)` - Skews vertically
- `skew(x-angle, y-angle)` - Skews on 2D plane

#### Matrix Functions
- `matrix(a, b, c, d, tx, ty)` - Describes 2D transformation matrix
- `matrix3d(...)` - Describes 3D transformation matrix

#### Perspective Functions
- `perspective(length)` - Sets distance between user and z=0 plane

### Math Functions

Allow CSS numeric values to be written as mathematical expressions.

#### Basic Arithmetic
- `calc(expression)` - Performs basic arithmetic calculations
- `calc-size(expression)` - Calculations on intrinsic size values

#### Comparison Functions
- `min(value1, value2, ...)` - Calculates smallest value
- `max(value1, value2, ...)` - Calculates largest value
- `clamp(min, central, max)` - Calculates central value within range

#### Stepped Value Functions
- `round(strategy, value)` - Rounds number based on strategy
- `mod(dividend, divisor)` - Calculates modulus
- `rem(dividend, divisor)` - Calculates remainder
- `progress(start, end, current)` - Calculates progress between values

#### Trigonometric Functions
- `sin(angle)` - Sine
- `cos(angle)` - Cosine
- `tan(angle)` - Tangent
- `asin(value)` - Inverse sine
- `acos(value)` - Inverse cosine
- `atan(value)` - Inverse tangent
- `atan2(y, x)` - Inverse tangent of two numbers

#### Exponential Functions
- `pow(base, exponent)` - Base raised to power
- `sqrt(value)` - Square root
- `hypot(value1, value2, ...)` - Square root of sum of squares
- `log(value)` - Logarithm
- `exp(value)` - e raised to power

#### Sign-related Functions
- `abs(value)` - Absolute value
- `sign(value)` - Sign (positive/negative)

### Filter Functions

Represent graphical effects that can change appearance of input images. Used with `filter` and `backdrop-filter` properties.

- `blur(radius)` - Gaussian blur
- `brightness(amount)` - Brightens/darkens image
- `contrast(amount)` - Increases/decreases contrast
- `drop-shadow(offset-x offset-y blur-radius color)` - Drop shadow
- `grayscale(amount)` - Converts to grayscale
- `hue-rotate(angle)` - Changes hue
- `invert(amount)` - Inverts colors
- `opacity(amount)` - Adds transparency
- `saturate(amount)` - Changes saturation
- `sepia(amount)` - Increases sepia

### Color Functions

Specify different color representations.

- `rgb(r, g, b, a?)` - Red, green, blue, alpha
- `hsl(h, s, l, a?)` - Hue, saturation, lightness, alpha
- `hwb(h, w, b, a?)` - Hue, whiteness, blackness
- `lch(l, c, h, a?)` - Lightness, chroma, hue
- `oklch(l, c, h, a?)` - Lightness, chroma, hue (OKLab)
- `lab(l, a, b, a?)` - Lightness, a-axis, b-axis
- `oklab(l, a, b, a?)` - Lightness, a-axis, b-axis (OKLab)
- `color(colorspace params)` - Specific colorspace
- `color-mix(method, color1, color2)` - Mixes two colors
- `contrast-color(color)` - Maximum contrast color
- `device-cmyk(c, m, y, k, a?)` - CMYK colors
- `light-dark(light-color, dark-color)` - Based on color scheme

### Image Functions

Provide graphical representation of images or gradients.

#### Gradient Functions
- `linear-gradient(direction?, color-stop, ...)` - Linear gradients
- `radial-gradient(shape?, position?, color-stop, ...)` - Radial gradients
- `conic-gradient(position?, color-stop, ...)` - Conic gradients
- `repeating-linear-gradient(...)` - Repeating linear gradients
- `repeating-radial-gradient(...)` - Repeating radial gradients
- `repeating-conic-gradient(...)` - Repeating conic gradients

#### Other Image Functions
- `image(url, ...)` - Image with fallbacks
- `image-set(url resolution, ...)` - Appropriate image for pixel density
- `cross-fade(image1, image2, percentage)` - Blends images
- `element(id)` - Image from HTML element
- `paint(worklet-name, ...)` - Image from PaintWorklet

### Counter Functions

Used with `content` property for counters.

- `counter(name, style?)` - Current counter value
- `counters(name, string, style?)` - Nested counter values
- `symbols(type, symbol1, ...)` - Counter styles inline

### Shape Functions

Represent graphical shapes for `clip-path`, `offset-path`, `shape-outside`.

#### Basic Shapes
- `circle(radius at position?)` - Circle shape
- `ellipse(rx ry at position?)` - Ellipse shape
- `inset(top right bottom left round radii?)` - Inset rectangle
- `rect(top, right, bottom, left)` - Rectangle
- `xywh(x, y, width, height)` - Rectangle by position and size
- `polygon(fill-rule?, point1, ...)` - Polygon shape
- `path(fill-rule?, d)` - SVG path shape
- `shape(commands)` - Shape from commands

#### Other Shape Functions
- `ray(angle, size?, contain?)` - Line segment for animations
- `superellipse(rx ry at position?)` - Curved ellipse

### Reference Functions

Reference values defined elsewhere.

- `attr(attribute-name, type?, fallback?)` - HTML attribute values
- `env(variable, fallback?)` - User-agent environment variables
- `if(condition, true-value, false-value)` - Conditional values
- `url(url)` - File from URL
- `var(custom-property, fallback?)` - Custom property values

### Grid Functions

Used for CSS Grid layouts.

- `fit-content(value)` - Clamps to available size
- `minmax(min, max)` - Size range
- `repeat(count, track-list)` - Repeated tracks

### Font Functions

Control font variant alternates.

- `stylistic(name)` - Stylistic alternates
- `styleset(name)` - Stylistic sets
- `character-variant(name)` - Character variants
- `swash(name)` - Swash glyphs
- `ornaments(name)` - Ornaments
- `annotation(name)` - Annotations

### Easing Functions

Mathematical functions for transitions and animations.

- `linear(points)` - Linear interpolation
- `cubic-bezier(x1, y1, x2, y2)` - Cubic Bézier curve
- `steps(number, direction?)` - Step function

### Animation Functions

Used with animation-timeline properties.

- `scroll(axis?, scroller?)` - Scroll progress timeline
- `view(axis?, inset?)` - View progress timeline

### Anchor Positioning Functions

Position elements relative to anchor elements.

- `anchor(side)` - Length relative to anchor edges
- `anchor-size(dimension)` - Length relative to anchor size

### Tree Counting Functions

Return integers based on DOM tree.

- `sibling-index()` - Position among siblings
- `sibling-count()` - Total sibling count

## Best Practices

- Use `calc()` for responsive calculations combining different units
- Prefer `clamp()` for fluid typography and spacing
- Use CSS custom properties with `var()` for maintainable values
- Leverage `min()`, `max()` for responsive design constraints
- Combine transform functions efficiently (e.g., `translate()` + `scale()`)
- In Vue.js components, bind CSS functions dynamically: `<div :style="{ transform: `translateX(${offset}px)` }">`

## Browser Support

Most modern functions have good support in current browsers. Check MDN for specific compatibility details.

## Patterns and Antipatterns

### Recommended Patterns

#### 1. **Use calc() for Responsive Calculations**
Combine different units for flexible layouts:
```css
/* Good: Responsive padding */
.container {
  padding: calc(1rem + 2vw);
}

/* Good: Full height minus header */
.content {
  height: calc(100vh - 80px);
}
```

#### 2. **Leverage min(), max(), and clamp() for Fluid Design**
Create responsive constraints without media queries:
```css
/* Good: Fluid typography */
h1 {
  font-size: clamp(1.5rem, 2vw + 1rem, 3rem);
}

/* Good: Responsive spacing */
.card {
  width: min(100%, 400px);
}
```

#### 3. **Combine with Custom Properties**
Use CSS variables for maintainable calculations:
```css
:root {
  --base-size: 1rem;
  --scale-factor: 1.2;
}

.element {
  font-size: calc(var(--base-size) * var(--scale-factor));
}
```

#### 4. **Use Trigonometric Functions for Animations**
Create smooth, mathematical animations:
```css
@keyframes rotate {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

.oscillate {
  transform: translateY(calc(sin(var(--angle)) * 20px));
}
```

#### 5. **Filter Functions for Visual Effects**
Apply multiple filters efficiently:
```css
.image {
  filter: brightness(1.2) contrast(1.1) saturate(1.3);
}
```

### Antipatterns to Avoid

#### 1. **Overusing calc() for Simple Values**
```css
/* Bad: Unnecessary complexity */
.element {
  width: calc(100px); /* Just use 100px */
}

/* Good: Only when combining units */
.element {
  width: calc(100px + 2rem);
}
```

#### 2. **Deep Nesting of Functions**
```css
/* Bad: Hard to read and debug */
.element {
  width: calc(calc(100% - calc(2rem * 2)) - 10px);
}

/* Good: Simplify or use intermediate variables */
.element {
  --padding: 2rem;
  --border: 10px;
  width: calc(100% - calc(var(--padding) * 2) - var(--border));
}
```

#### 3. **Ignoring Browser Support**
```css
/* Bad: Using unsupported functions without fallbacks */
.element {
  width: clamp(200px, 50%, 800px); /* No IE support */
}

/* Good: Provide fallbacks */
.element {
  width: 400px; /* Fallback */
  width: clamp(200px, 50%, 800px);
}
```

#### 4. **Misusing Comparison Functions**
```css
/* Bad: min() when max() is needed */
.element {
  width: min(100%, 300px); /* Always ≤ 300px */
}

/* Good: Choose correct function */
.element {
  width: max(100%, 300px); /* Always ≥ 300px */
}
```

#### 5. **Performance-Heavy Calculations**
```css
/* Bad: Expensive operations in animations */
.element {
  transform: translateX(calc(sin(var(--angle)) * cos(var(--angle)) * 100px));
}

/* Good: Pre-calculate where possible */
.element {
  --wave: calc(sin(var(--angle)) * 50px);
  transform: translateX(var(--wave));
}
```

#### 6. **Incompatible Unit Mixing**
```css
/* Bad: Mixing absolute and relative incorrectly */
.element {
  font-size: calc(16px + 50%); /* Percentage of what? */
}

/* Good: Clear intent */
.element {
  font-size: calc(1rem + 0.5vw); /* Relative to root and viewport */
}
```

### Best Practices Summary

- **Test Across Browsers**: Use tools like Can I Use for support
- **Keep Calculations Simple**: Break complex math into steps
- **Use Fallbacks**: Always provide non-math alternatives
- **Document Intent**: Comment complex calculations
- **Performance First**: Avoid expensive operations in frequently updated properties
- **Maintainability**: Use custom properties for reusable values