# GitHub Backlog Issue Creation Guide

## 📋 What's Included

The script `CREATE_BACKLOG_ISSUES.sh` will create **10 GitHub issues** from the P0.6 backlog refinement:

| # | Story ID | Title | Effort | Sprint |
|---|----------|-------|--------|--------|
| 1 | P0.6-US-001 | B2C Price Transparency (PAngV) | 12h | S1 |
| 2 | P0.6-US-002 | B2B VAT-ID Validation (Reverse Charge) | 20h | S1 |
| 3 | P0.6-US-003 | 14-Day Withdrawal Right (VVVG) | 18h | S2 |
| 4 | P0.6-US-004 | Order Confirmation Email (TMG) | 10h | S2 |
| 5 | P0.6-US-005 | Invoice Generation & 10-Year Archival | 25h | S2-3 |
| 6 | P0.6-US-006 | AGB & Datenschutz Acceptance | 12h | S3 |
| 7 | P0.6-US-007 | Impressum & Privacy Links (TMG) | 8h | S3 |
| 8 | P0.6-US-008 | B2B Payment Terms Configuration | 15h | S3-4 |
| 9 | P0.6-US-009 | Shipping Cost Transparency | 14h | S4 |
| 10 | P0.6-US-010 | Dispute Resolution (ODR-VO) | 10h | S4 |

**Total Effort:** 144 hours (18 story points if using Fibonacci)  
**Sprint Distribution:**
- Sprint 1: 32 hours (2 stories)
- Sprint 2: 53 hours (4 stories)
- Sprint 3: 35 hours (4 stories)
- Sprint 4: 39 hours (4 stories)

---

## ✅ Prerequisites

Before running the script, ensure:

```bash
# 1. GitHub CLI installed
which gh
# Output: /usr/local/bin/gh (or similar)

# 2. Authenticated with GitHub
gh auth status
# Output: Logged in to github.com as <username> (HTTPS)

# 3. Have write access to repository
# (Should see "✓" next to your username)
```

### Setup (if not authenticated):
```bash
gh auth login
# Follow prompts:
# - Choose: GitHub.com
# - Choose: HTTPS
# - Authenticate with browser
```

---

## 🚀 How to Run

### Option 1: Direct Execution (Recommended)
```bash
cd /Users/holger/Documents/Projekte/B2Connect
./CREATE_BACKLOG_ISSUES.sh
```

### Option 2: With Output Logging
```bash
./CREATE_BACKLOG_ISSUES.sh | tee backlog_creation.log
```

### Option 3: Dry Run (Review only, don't create)
```bash
# Edit the script to comment out gh issue create commands
# Or manually review each issue in the script
```

---

## 📊 Expected Output

```
🚀 Creating P0.6 Backlog Issues...
✅ Issue 1 created
✅ Issue 2 created
✅ Issue 3 created
✅ Issue 4 created
✅ Issue 5 created
✅ Issue 6 created
✅ Issue 7 created
✅ Issue 8 created
✅ Issue 9 created
✅ Issue 10 created

🎉 All 10 backlog issues created successfully!
📍 View them at: https://github.com/holger-b2c/B2Connect/issues
```

---

## 🔍 What Gets Created

### Each Issue Includes:

✅ **Title:** Story ID + descriptive title  
✅ **Body:** Full user story with acceptance criteria  
✅ **Effort:** Story points (estimated hours)  
✅ **Sprint:** Assignment (S1, S2, S3, S4)  
✅ **Subtasks:** Detailed implementation checklist  
✅ **Dependencies:** Links to related stories  
✅ **Labels:** 
- `P0.6` - Backlog component
- `legal` - Legal/compliance work
- `backend` / `frontend` - Team assignment
- `sprint-1/2/3/4` - Sprint number
- Specific labels: `pricing`, `vat`, `b2b`, `b2c`, `returns`, `email`, `invoices`, etc.

---

## 📱 Example Issue

Here's what one issue looks like after creation:

```
Title: P0.6-US-001: B2C Price Transparency (PAngV Compliance)

Effort: 12 hours | Sprint: Sprint 1

User Story
As a shop owner, I want customers to always see final prices 
including VAT, so that I comply with PAngV and customers know 
actual cost before checkout.

Acceptance Criteria
✓ Product listing displays price as "€99,99 inkl. MwSt"
✓ Product detail page shows VAT breakdown
✓ Cart displays "Subtotal + VAT = Total"
✓ Checkout confirms final price before payment
✓ Invoice shows VAT breakdown per line item
... (more criteria)

Subtasks
☐ Create PriceCalculationService (backend)
☐ Implement VAT rate lookup per country/product
☐ Update ProductCard component to show VAT
... (more subtasks)

Labels: P0.6, legal, pricing, backend, sprint-1
```

---

## ⚙️ After Creation

### Next Steps:

1. **View Issues:**
   ```bash
   gh issue list --label "P0.6"
   ```

2. **Assign to Team Members:**
   ```bash
   gh issue edit <issue-number> --assignee @username
   ```

3. **Create Project (Optional):**
   ```bash
   gh project create --title "P0.6 Store Legal Compliance"
   ```

4. **Link to Project:**
   ```bash
   gh issue edit <issue-number> --add-project "Project Name"
   ```

5. **Verify in GitHub Web UI:**
   - Go to https://github.com/holger-b2c/B2Connect/issues
   - Filter by label: `P0.6`
   - Should see all 10 issues

---

## 🔄 If Issues Already Exist

If you already created some of these issues, the script will fail with:
```
Error: Validation Failed: {"resource":"Issue","field":"title","code":"custom","message":"is invalid"}
```

### Solution:
1. **Delete existing issues** (if desired):
   ```bash
   gh issue delete <issue-number>
   ```

2. **Or modify script** to check for duplicates:
   ```bash
   # Add before gh issue create:
   if gh issue list --label "P0.6" | grep -q "US-001"; then
     echo "Issue already exists, skipping..."
     continue
   fi
   ```

---

## 📝 Customization

### To Modify Issues Before Creation:

Edit `CREATE_BACKLOG_ISSUES.sh`:

```bash
# Change sprint assignment
--label "P0.6,legal,pricing,backend,sprint-1"
# ↓
--label "P0.6,legal,pricing,backend,sprint-2"

# Change effort
**Effort:** 12 hours | **Sprint:** Sprint 1
# ↓
**Effort:** 16 hours | **Sprint:** Sprint 2

# Add/remove subtasks
- [ ] Create PriceCalculationService (backend)
- [ ] ...
```

---

## 🐛 Troubleshooting

### Issue: "gh: command not found"
```bash
# Install GitHub CLI
brew install gh

# Then authenticate
gh auth login
```

### Issue: "Unauthorized: Requires push access"
```bash
# Re-authenticate with proper permissions
gh auth login
# → Choose: Generate new GitHub token
# → Select scopes: repo, workflow
```

### Issue: "Rate limit exceeded"
```bash
# GitHub API rate limit: 60 requests/hour (unauthenticated)
# Wait 1 hour or authenticate properly (5000/hour)

gh auth status
# If shows "Not logged in", do: gh auth login
```

---

## ✨ Success Checklist

After running the script:

- [ ] All 10 issues created (shown in output)
- [ ] No errors in output
- [ ] Can view issues on GitHub: https://github.com/holger-b2c/B2Connect/issues
- [ ] All issues labeled with `P0.6`
- [ ] Issues distributed across Sprint 1-4
- [ ] Each issue has acceptance criteria and subtasks

---

## 📞 Need Help?

If issues fail to create:

```bash
# Check GitHub CLI status
gh auth status

# List existing P0.6 issues
gh issue list --label "P0.6" --limit 20

# Check for rate limiting
gh api rate_limit

# View recent errors
gh issue list --state all --limit 5
```

---

**Ready to create the backlog?** Run:
```bash
./CREATE_BACKLOG_ISSUES.sh
```

