---
docid: TPL-018
title: TPL USERDOC 001 USER DOCS TEMPLATE
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: TPL-USERDOC-001
title: User Documentation Template
category: Documentation Templates
type: User Documentation (USERDOC-*)
created: 2026-01-08
---

# User Documentation Template

Use this template for all **USERDOC-*** (User Documentation) files.

---

## Template Usage

1. Copy this file
2. Rename: `USERDOC-[CATEGORY]-[NUMBER]-[short-title].md`
   - Example: `USERDOC-HOW-001-how-to-create-a-product.md`
3. Update the YAML front-matter
4. Fill in all sections with simple, clear language
5. Remove instruction comments (lines starting with `<!--`)

---

```yaml
---
docid: USERDOC-HOW-001-how-to-create-a-product
title: How to Create a Product
category: user/howto
type: How-To Guide
audience: End Users, Business Users, Sales Teams
technical_level: Beginner
status: Active
last_updated: 2026-01-08
version: 1.0
language: en

# Keywords for search/AI indexing
keywords: 
  - product-creation
  - catalog-management
  - getting-started

# Related documents for cross-referencing
related:
  - USERDOC-HOW-002
  - USERDOC-SCREEN-001
  - USERDOC-FAQ-001

# AI metadata for smart assistance
ai_metadata:
  use_cases:
    - customer_support
    - sales_assistance
    - user_onboarding
  difficulty_level: beginner
  time_to_complete_minutes: 5
  includes_screenshots: true
  includes_video_link: false
  step_count: 7
---
```

---

## Document Structure

### 1. 👋 Overview (Required)
**Purpose**: Quick introduction in simple language

```markdown
## Overview

This guide shows you how to [DO SOMETHING]. After reading, you'll be able to:
- Achieve goal 1
- Achieve goal 2
- Achieve goal 3

**Time needed**: About 5 minutes
**Difficulty**: Beginner
**What you need**: [List anything needed, like login credentials]
```

**Tips for AI**:
- Use simple language (no jargon)
- Be specific about outcomes
- Estimate time upfront
- Specify prerequisites

---

### 2. 📋 Quick Links (Recommended)
**Purpose**: Help users jump to what they need

```markdown
## Quick Links

- ⏱️ **In a hurry?** → [Jump to quick steps](#quick-steps)
- 🖥️ **Visual guide?** → [See screenshots](#screenshots)
- 🤔 **Need help?** → [FAQs](#frequently-asked-questions)
- 🆘 **Problem?** → [Troubleshooting](#troubleshooting)
```

**Tips for AI**:
- Makes content scannable
- Helps route user to what they need
- Improves support efficiency

---

### 3. ⚡ Quick Steps (Recommended)
**Purpose**: Short version for experienced users

```markdown
## Quick Steps

1. Go to [Menu name]
2. Click [Button name]
3. Enter [Information]
4. Click [Save]

✅ Done! You've completed this task.

👉 **Want more details?** See [Full step-by-step guide](#step-by-step-guide) below.
```

**Tips for AI**:
- Enables quick resolution
- Reference to detailed version
- Clear success indicator

---

### 4. 🎯 Before You Start (Recommended)
**Purpose**: Prepare users for the task

```markdown
## Before You Start

**You'll need**:
- [ ] Access to [system name]
- [ ] Admin privileges? Yes / No
- [ ] [Information or data]

**Helpful to know**:
- [Context or explanation]
- [Why they might want to do this]

**Related tasks**:
- [USERDOC-HOW-002] - Related task
- [USERDOC-SCREEN-001] - Screen reference
```

**Tips for AI**:
- Checklist format for clarity
- Set expectations
- Reference related content
- Reduce support calls from unprepared users

---

### 5. 👣 Step-by-Step Guide (Required)
**Purpose**: Detailed instructions for task completion

```markdown
## Step-by-Step Guide

### Step 1: Log In

1. Open your web browser
2. Go to [website URL]
3. Enter your username and password
4. Click **Sign In**

✅ You should now see the dashboard

**Troubleshooting**: If you can't log in, see [USERDOC-FAQ-001](#password-reset)

---

### Step 2: Navigate to the Product Menu

1. Look for the **Catalog** menu at the top
2. Click **Products**
3. Click **Add New Product**

🖼️ You should see a screen like this:
![Product creation form](./screenshots/product-creation-form.png)

---

### Step 3: Fill in Basic Information

Fill in these fields:

| Field | What to enter | Example |
|-------|---|---|
| **Product Name** | The name of your product | "Blue T-Shirt" |
| **SKU** | Product identifier code | "SHIRT-BLUE-001" |
| **Description** | Short description (100 characters) | "Comfortable cotton t-shirt" |

**💡 Tip**: Use clear, customer-friendly names. Avoid special characters.

---

### Step 4: Set Pricing

1. In the **Pricing** section, enter:
   - **Price**: [Example: $19.99]
   - **Cost**: [Your cost - only you see this]
2. The system will automatically calculate margin

**⚠️ Important**: Price must be higher than cost

---

### Step 5: Upload Images

1. Click **Add Images**
2. Select up to 5 images from your computer
3. Drag to reorder (first image is the main image)
4. Click **Save**

**Accepted formats**: JPG, PNG (max 5 MB each)

---

### Step 6: Set Categories

1. Click **Categories**
2. Check boxes for categories that apply:
   - [ ] Apparel
   - [ ] Electronics
   - [ ] Other

**💡 Tip**: Pick all that apply. Customers will find products through categories.

---

### Step 7: Publish

1. Click the **Status** dropdown
2. Select **Published**
3. Click **Save and Publish**

✅ **Success!** Your product is now live and visible to customers.

**What's next?**
- [USERDOC-HOW-002] - How to manage inventory
- [USERDOC-HOW-003] - How to run promotions
```

**Tips for AI**:
- Number every step clearly
- Each step is one action or closely related actions
- Include expected results (✅)
- Use tables for data entry
- Include screenshots where helpful
- Add troubleshooting inline
- Provide next steps

---

### 6. 🖼️ Screenshots & Visuals (Recommended)
**Purpose**: Show what users will see

```markdown
## Screenshots

### Main Product Creation Form
![Product creation form showing all fields](./screenshots/product-form-full.png)

*The blue highlight shows where to enter the product name*

### Category Selection
![Category checkboxes](./screenshots/categories-section.png)

*Select all categories that apply to your product*

### Success Message
![Green success message saying Product Created](./screenshots/success-message.png)

*This message appears when you've successfully created a product*
```

**Tips for AI**:
- Use descriptive captions
- Annotate important areas
- Show both empty and filled forms
- Show success states
- Keep images fresh (maintenance task)

---

### 7. 🆘 Troubleshooting (Recommended)
**Purpose**: Solve common problems

```markdown
## Troubleshooting

### Problem: "Price must be higher than cost"

**What you see**: Error message appears when trying to save

**Causes**:
- Your price is lower than the cost you entered
- Pricing has decimals that aren't matching

**Solution**:
1. Check your **Price** field
2. Check your **Cost** field
3. Make sure: Price > Cost
4. Click **Save** again

**Still stuck?** [Contact support](#contact-support)

---

### Problem: "Images won't upload"

**Symptoms**:
- Button doesn't work
- Error message appears
- File doesn't show up

**Causes**:
- File is too large (max 5 MB)
- File format not supported (use JPG or PNG)
- Internet connection is slow

**Solution**:
1. Check image file size
2. Convert to JPG or PNG if needed
3. Try uploading smaller image
4. Try different web browser

**If image is too large**, see [USERDOC-HOW-005] - How to resize images
```

**Tips for AI**:
- Anticipate the most common problems
- Structure: Problem → What you see → Causes → Solutions
- Link to related guides
- Know when to escalate to support

---

### 8. 📞 Get Help (Recommended)
**Purpose**: Support options

```markdown
## Get Help

### FAQs
See [Frequently Asked Questions](#frequently-asked-questions) section below

### Video Tutorial
Watch our video walkthrough: [Link to video]

### Live Chat
Available Mon-Fri, 9 AM - 5 PM EST
[Start live chat]

### Contact Support
Email: support@example.com
Phone: 1-800-XXX-XXXX

**Include in your message**:
- What you were trying to do
- What happened
- Screenshot if possible
```

**Tips for AI**:
- Provide multiple support channels
- Make it easy to escalate
- Suggest additional resources
- Include all contact info

---

### 9. ❓ Frequently Asked Questions (Recommended)
**Purpose**: Answer common questions

```markdown
## Frequently Asked Questions

### Can I edit a product after publishing it?

**Answer**: Yes! 
1. Go to **Catalog → Products**
2. Find your product
3. Click **Edit**
4. Make your changes
5. Click **Save**

Your product is updated immediately.

---

### Can I delete products?

**Answer**: You can hide products from customers, but not delete them.

**Why?** Customers might have purchased it. We keep the record for accounting.

To hide a product:
1. Edit the product
2. Set Status to **Unpublished**
3. Click **Save**

---

### How many products can I add?

**Answer**: Unlimited! Add as many as you need.

---

### Can I import products from a spreadsheet?

**Answer**: Yes! See [USERDOC-HOW-006] - How to bulk import products
```

**Tips for AI**:
- Answer directly and briefly
- Include step-by-step if answer requires it
- Link to full guides
- Collect FAQs from support tickets

---

### 10. 📚 Related Articles (Required)
**Purpose**: Connect to related content

```markdown
## Related Articles

### Learn More About Products
- [USERDOC-HOW-002] - How to manage inventory
- [USERDOC-HOW-003] - How to set up pricing & discounts
- [USERDOC-SCREEN-001] - Product management screen reference

### Related Tasks
- [USERDOC-FEAT-001] - Product features overview
- [USERDOC-HOW-005] - How to resize and optimize images
- [USERDOC-PROC-001] - Product lifecycle process

### Screen Guides
- [USERDOC-SCREEN-002] - Product form fields explained
- [USERDOC-SCREEN-003] - Inventory management dashboard

### FAQs
- [USERDOC-FAQ-001] - General questions
- [USERDOC-FAQ-002] - Pricing questions
```

**Tips for AI**:
- Organize by topic
- Use DocID references
- Creates knowledge graph for AI
- Suggests next learning

---

### 11. 📝 Metadata & Footer (Required)
**Purpose**: Track document information

```markdown
---

## Document Information

| Item | Details |
|------|---------|
| **DocID** | USERDOC-HOW-001 |
| **Status** | Active |
| **Last Updated** | 2026-01-08 |
| **Author** | Support Team |
| **Reviews/Screenshots Updated** | 2026-01-08 |
| **Version** | 1.2 |

### Change Log
| Version | Date | What Changed |
|---------|------|---|
| 1.0 | 2025-12-01 | Initial version |
| 1.1 | 2026-01-01 | Added screenshots |
| 1.2 | 2026-01-08 | Updated for new UI |

### Maintenance Schedule
- Screenshots updated: Monthly
- Content review: Every quarter
- User feedback: Continuous

### Feedback
Was this helpful? Have suggestions?
- [📧 Email us](mailto:support@example.com)
- [💬 Chat with us](#contact-support)
- [📝 Provide feedback](./feedback-form)
```

**Tips for AI**:
- Version tracking helps AI understand document evolution
- Maintenance schedule ensures freshness
- Feedback loop enables continuous improvement

---

## ✅ Quality Checklist

Before publishing, verify:

- [ ] YAML front-matter complete
- [ ] DocID assigned and unique
- [ ] Title is user-friendly (not technical jargon)
- [ ] Overview section uses simple language
- [ ] Step-by-step guide is numbered and sequential
- [ ] Each step has expected result
- [ ] Screenshots are current and clear
- [ ] Troubleshooting covers common issues
- [ ] Related articles section populated
- [ ] No broken links
- [ ] Links use DocID format
- [ ] Metadata section complete
- [ ] Reviewed by support team
- [ ] Tested with 1-2 actual users if possible
- [ ] Ready for AI consumption

---

## 🤖 AI Assistant Notes

This template is designed for:
- **Customer Support Chatbots**: Step-by-step sections enable precise guidance
- **Help Desk Systems**: FAQ section enables quick resolution
- **Sales Assistance**: Overview and quick steps provide training material
- **User Onboarding**: "Before you start" and related articles guide new users
- **Voice Assistants**: Clear structure enables voice-based guidance

**Key for AI Integration**:
1. Step-by-step structure enables exact guidance
2. FAQ section enables quick Q&A chatbot responses
3. Troubleshooting enables diagnostic assistance
4. DocID references create knowledge graph
5. Metadata enables smart filtering and context
6. Simple language enables translation and accessibility

---

## Example: Completed Document

See [USERDOC-HOW-001-example.md](./USERDOC-HOW-001-example.md) for a filled-out example.

---

**Template Version**: 1.0  
**Last Updated**: 2026-01-08  
**Maintained by**: @DocMaintainer