---
docid: KB-030
title: SVG and CSS Animations
owner: @Frontend
status: Active
---

# SVG and CSS Animations

**DocID**: `KB-030`  
**Owner**: @Frontend  
**Status**: Active  
**Last Updated**: 5. Januar 2026  

## Overview

This knowledge base article covers SVG and CSS animations, including patterns, antipatterns, and best practices for the B2Connect project. Animations enhance user experience but must be used judiciously for performance and accessibility.

## CSS Animations

CSS animations allow animating CSS property values over time using keyframes.

### @keyframes Rule

Defines the animation sequence:

```css
@keyframes slideIn {
  from {
    transform: translateX(-100%);
  }
  to {
    transform: translateX(0);
  }
}

/* Or with percentages */
@keyframes pulse {
  0% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
    transform: scale(1.1);
  }
  100% {
    opacity: 1;
  }
}
```

### Animation Properties

- **animation-duration**: Sets animation length (e.g., `2s`, `500ms`)
- **animation-timing-function**: Controls speed curve (`ease`, `linear`, `ease-in-out`, `cubic-bezier(0.42, 0, 0.58, 1)`)
- **animation-iteration-count**: Number of repeats (`infinite`, `3`)
- **animation-direction**: Direction (`normal`, `reverse`, `alternate`)
- **animation-delay**: Delay before start (`1s`)
- **animation-play-state**: Play/pause (`running`, `paused`)
- **animation-fill-mode**: Styles before/after animation (`none`, `forwards`, `backwards`, `both`)

### Shorthand

```css
animation: slideIn 2s ease-in-out 1s infinite alternate;
```

### Vue.js Integration

In Vue components, bind animation classes:

```vue
<template>
  <div :class="{ 'animate': isVisible }">Content</div>
</template>

<style>
@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.animate {
  animation: fadeIn 1s ease-in;
}
</style>
```

## SVG Animations with SMIL

SVG supports native animations using SMIL (Synchronized Multimedia Integration Language).

### Animating Attributes

Use `<animate>` to animate element attributes:

```svg
<svg width="300" height="100">
  <circle cx="50" cy="50" r="20" fill="blue">
    <animate attributeName="cx" from="50" to="250" dur="2s" repeatCount="indefinite" />
  </circle>
</svg>
```

### Animating Transforms

Use `<animateTransform>` for transformations:

```svg
<rect x="0" y="0" width="50" height="50" fill="red">
  <animateTransform attributeName="transform" type="rotate" from="0 25 25" to="360 25 25" dur="4s" repeatCount="indefinite" />
</rect>
```

### Motion Along Paths

Use `<animateMotion>` for path-based movement:

```svg
<circle r="10" fill="green">
  <animateMotion path="M 0 0 L 100 0 L 100 100 L 0 100 Z" dur="3s" repeatCount="indefinite" />
</circle>
```

## Patterns

### CSS Animation Patterns

1. **Micro-interactions**: Subtle animations for feedback (hover, click)
2. **Loading states**: Skeleton screens, spinners
3. **Page transitions**: Smooth navigation between views
4. **Progressive enhancement**: Graceful degradation without animations

### SVG Animation Patterns

1. **Icon animations**: Morphing icons on interaction
2. **Data visualization**: Animated charts and graphs
3. **Illustrations**: Breathing life into static graphics
4. **Logos**: Animated branding elements

### Performance Patterns

- Use `transform` and `opacity` for GPU acceleration
- Limit animated elements
- Use `will-change` for complex animations
- Prefer CSS animations over JavaScript for simple cases

## Antipatterns

### CSS Animation Antipatterns

1. **Over-animation**: Excessive animations causing distraction
2. **Ignoring performance**: Animating non-accelerated properties
3. **No accessibility**: Not respecting `prefers-reduced-motion`
4. **Blocking animations**: Long animations preventing interaction

### SVG Animation Antipatterns

1. **SMIL deprecation**: SMIL is deprecated; prefer CSS or Web Animations API
2. **Complex paths**: Overly complex motion paths impacting performance
3. **No fallbacks**: Not providing static versions for unsupported browsers

### General Antipatterns

1. **Magic numbers**: Hardcoded values without explanation
2. **No coordination**: Multiple animations without synchronization
3. **Browser inconsistencies**: Not testing across browsers

## Best Practices

### Accessibility

Respect user preferences:

```css
@media (prefers-reduced-motion: reduce) {
  .animated-element {
    animation: none;
  }
}
```

### Performance

- Use `transform` and `opacity` for smooth animations
- Avoid animating `width`, `height`, `top`, `left` (causes layout shifts)
- Use `contain` CSS property for isolated animations

### Browser Support

- CSS animations: Broad support (IE10+)
- SVG SMIL: Limited support (deprecated in Chrome 45+)
- Prefer CSS animations or Web Animations API for new projects

### Vue.js Specific

- Use transition components for enter/leave animations
- Leverage Vue's reactivity for dynamic animations
- Consider libraries like GSAP for complex sequences

## Examples

### CSS: Button Hover Effect

```css
.button {
  transition: transform 0.2s ease;
}

.button:hover {
  transform: scale(1.05);
}
```

### SVG: Animated Icon

```svg
<svg viewBox="0 0 24 24">
  <path d="M12 2L2 7l10 5 10-5-10-5z">
    <animate attributeName="d" dur="2s" repeatCount="indefinite"
      values="M12 2L2 7l10 5 10-5-10-5z;
              M12 2L5 5l7 5 7-5-7-5z;
              M12 2L2 7l10 5 10-5-10-5z" />
  </path>
</svg>
```

### Vue: Transition Wrapper

```vue
<transition name="fade">
  <div v-if="show">Content</div>
</transition>

<style>
.fade-enter-active, .fade-leave-active {
  transition: opacity 0.5s;
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
}
</style>
```

## Resources

- [MDN CSS Animations](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_Animations)
- [MDN SVG Animation with SMIL](https://developer.mozilla.org/en-US/docs/Web/SVG/SVG_animation_with_SMIL)
- [web.dev CSS Animations](https://web.dev/learn/css/animations/)
- [CSS Triggers](https://csstriggers.com/)

## Related KB Articles

- [KB-007: Vue.js 3](vue.md) - Vue.js integration patterns
- [KB-029: CSS Functions](css-functions.md) - CSS functions including animation-related ones