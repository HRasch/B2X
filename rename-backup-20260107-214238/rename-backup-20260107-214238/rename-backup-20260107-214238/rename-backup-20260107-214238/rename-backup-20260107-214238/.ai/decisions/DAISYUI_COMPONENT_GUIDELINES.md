# B2X Frontend Component Guidelines - DaisyUI Framework

**Date:** December 30, 2025  
**Owner:** @Architect  
**Status:** Active  
**Component Framework:** DaisyUI v5 (Tailwind CSS based)

---

## Quick Reference

### Official Framework
✅ **Component Framework:** DaisyUI v5  
✅ **Styling Engine:** Tailwind CSS v4  
✅ **Language:** Vue 3 + TypeScript  
✅ **Accessibility:** WCAG 2.1 AA compliant  

---

## Component Usage Patterns

### Buttons

**Primary CTA:**
```vue
<button class="btn btn-primary">Submit</button>
<button class="btn btn-primary btn-sm">Small</button>
<button class="btn btn-primary btn-lg">Large</button>
<button class="btn btn-primary loading">Loading</button>
```

**Secondary:**
```vue
<button class="btn btn-secondary">Secondary</button>
```

**Outline:**
```vue
<button class="btn btn-outline">Outline</button>
```

**Ghost (minimal):**
```vue
<button class="btn btn-ghost">Ghost</button>
```

**States:**
```vue
<button class="btn btn-disabled">Disabled</button>
<button class="btn btn-primary loading">Loading</button>
```

---

### Form Inputs

**Text Input:**
```vue
<div class="form-control">
  <label class="label">
    <span class="label-text">Email</span>
  </label>
  <input type="email" placeholder="your@email.com" class="input input-bordered" />
  <label class="label">
    <span class="label-text-alt">Valid email required</span>
  </label>
</div>
```

**With Error:**
```vue
<div class="form-control">
  <label class="label">
    <span class="label-text">Email</span>
  </label>
  <input type="email" class="input input-bordered input-error" />
  <label class="label">
    <span class="label-text-alt text-error">Email is required</span>
  </label>
</div>
```

**Textarea:**
```vue
<div class="form-control">
  <label class="label">
    <span class="label-text">Message</span>
  </label>
  <textarea class="textarea textarea-bordered"></textarea>
</div>
```

**Select:**
```vue
<div class="form-control">
  <label class="label">
    <span class="label-text">Country</span>
  </label>
  <select class="select select-bordered">
    <option disabled selected>Pick one</option>
    <option>Germany</option>
    <option>Austria</option>
  </select>
</div>
```

**Checkbox:**
```vue
<div class="form-control">
  <label class="label cursor-pointer">
    <span class="label-text">I agree to terms</span>
    <input type="checkbox" class="checkbox checkbox-primary" />
  </label>
</div>
```

**Radio:**
```vue
<div class="form-control">
  <label class="label cursor-pointer">
    <span class="label-text">Option 1</span>
    <input type="radio" name="options" class="radio radio-primary" />
  </label>
  <label class="label cursor-pointer">
    <span class="label-text">Option 2</span>
    <input type="radio" name="options" class="radio radio-primary" />
  </label>
</div>
```

---

### Cards

**Basic:**
```vue
<div class="card bg-base-100 shadow-xl">
  <div class="card-body">
    <h2 class="card-title">Card Title</h2>
    <p>Card content goes here</p>
    <div class="card-actions justify-end">
      <button class="btn btn-primary">Action</button>
    </div>
  </div>
</div>
```

**With Image:**
```vue
<div class="card bg-base-100 shadow-xl">
  <figure><img src="image.jpg" alt="Product" /></figure>
  <div class="card-body">
    <h2 class="card-title">Title</h2>
    <p>Description</p>
  </div>
</div>
```

---

### Alerts

**Success:**
```vue
<div class="alert alert-success">
  <svg class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
  </svg>
  <span>Your purchase has been confirmed!</span>
</div>
```

**Warning:**
```vue
<div class="alert alert-warning">
  <svg class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4v2m0 0a9 9 0 11-18 0 9 9 0 0118 0z"></path>
  </svg>
  <span>Warning: Check your input</span>
</div>
```

**Error:**
```vue
<div class="alert alert-error">
  <svg class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l-2-2m0 0l-2-2m2 2l2-2m-2 2l-2 2m0 0l2 2m-2-2l-2 2"></path>
  </svg>
  <span>Error! Something went wrong</span>
</div>
```

**Info:**
```vue
<div class="alert alert-info">
  <svg class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
  </svg>
  <span>New feature available!</span>
</div>
```

---

### Badges

**Primary:**
```vue
<div class="badge badge-primary">Primary</div>
```

**With variants:**
```vue
<div class="badge">Neutral</div>
<div class="badge badge-secondary">Secondary</div>
<div class="badge badge-accent">Accent</div>
<div class="badge badge-ghost">Ghost</div>
<div class="badge badge-success">Success</div>
<div class="badge badge-warning">Warning</div>
<div class="badge badge-error">Error</div>
```

---

### Modals

**Basic:**
```vue
<dialog id="my_modal_1" class="modal">
  <div class="modal-box">
    <h3 class="font-bold text-lg">Modal Title</h3>
    <p class="py-4">Modal content goes here</p>
    <div class="modal-action">
      <form method="dialog">
        <button class="btn">Close</button>
      </form>
    </div>
  </div>
  <form method="dialog" class="modal-backdrop">
    <button>close</button>
  </form>
</dialog>

<!-- Trigger -->
<button class="btn" onclick="my_modal_1.showModal()">Open Modal</button>
```

---

### Navigation

**Navbar:**
```vue
<div class="navbar bg-base-100 shadow">
  <div class="flex-1">
    <a class="btn btn-ghost normal-case text-xl">B2X</a>
  </div>
  <div class="flex-none gap-2">
    <button class="btn btn-ghost">Home</button>
    <button class="btn btn-ghost">Products</button>
    <button class="btn btn-ghost">Cart</button>
  </div>
</div>
```

---

### Breadcrumbs

```vue
<div class="breadcrumbs text-sm">
  <ul>
    <li><a>Home</a></li>
    <li><a>Products</a></li>
    <li>Product Details</li>
  </ul>
</div>
```

---

### Tabs

```vue
<div role="tablist" class="tabs tabs-bordered">
  <input type="radio" name="my_tabs_1" role="tab" class="tab" aria-label="Tab 1" />
  <div role="tabpanel" class="tab-content p-10">Tab 1 content</div>

  <input type="radio" name="my_tabs_1" role="tab" class="tab" aria-label="Tab 2" />
  <div role="tabpanel" class="tab-content p-10">Tab 2 content</div>
</div>
```

---

## Accessibility Checklist

✅ Use semantic HTML (`<button>`, `<form>`, `<nav>`)  
✅ Include `aria-label` on buttons without text  
✅ Use `role="alert"` for dynamic messages  
✅ Ensure focus indicators visible on all interactive elements  
✅ Test keyboard navigation (Tab, Enter, Escape)  
✅ Use color + additional indicators (text, icon)  
✅ Maintain 4.5:1 color contrast ratio for text  

---

## DaisyUI Theme Colors

```
Primary:    #0b98ff (B2X blue)
Secondary:  #8b5cf6 (Purple)
Accent:     #f59e0b (Amber)
Neutral:    #3f3f46 (Dark gray)
Success:    #22c55e (Green)
Warning:    #f59e0b (Amber)
Error:      #ef4444 (Red)
Info:       #0b98ff (Blue)
```

---

## DO's and DON'Ts

### ✅ DO
- Use DaisyUI component classes (`btn`, `input`, `card`, etc.)
- Extend with Tailwind utilities (`flex`, `gap`, `p-4`)
- Follow DaisyUI naming conventions
- Test accessibility with keyboard navigation
- Document custom component behavior

### ❌ DON'T
- Build custom buttons from scratch
- Use Bootstrap classes
- Mix component frameworks
- Skip accessibility attributes
- Override DaisyUI styling without documentation

---

## Common Patterns

### Form with Validation

```vue
<form @submit.prevent="submit" class="space-y-4">
  <div class="form-control">
    <label class="label">
      <span class="label-text">Email</span>
    </label>
    <input 
      v-model="email"
      type="email" 
      class="input input-bordered"
      :class="{ 'input-error': errors.email }"
      @blur="validateEmail"
    />
    <label v-if="errors.email" class="label">
      <span class="label-text-alt text-error">{{ errors.email }}</span>
    </label>
  </div>
  
  <button type="submit" class="btn btn-primary">Submit</button>
</form>
```

### Card List

```vue
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
  <div v-for="item in items" :key="item.id" class="card bg-base-100 shadow">
    <div class="card-body">
      <h3 class="card-title">{{ item.title }}</h3>
      <p>{{ item.description }}</p>
      <div class="card-actions justify-end">
        <button class="btn btn-sm btn-primary">View</button>
      </div>
    </div>
  </div>
</div>
```

---

## Resources

- [DaisyUI Documentation](https://daisyui.com/)
- [DaisyUI Components](https://daisyui.com/components/)
- [Tailwind CSS Docs](https://tailwindcss.com/)
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)

---

## Getting Help

- Check DaisyUI docs first
- Review existing components in codebase
- Ask @TechLead for code review
- Document patterns as you discover them

---

**Effective:** December 30, 2025  
**Author:** @Architect  
**Version:** 1.0  
**Next Review:** Phase 2 completion
