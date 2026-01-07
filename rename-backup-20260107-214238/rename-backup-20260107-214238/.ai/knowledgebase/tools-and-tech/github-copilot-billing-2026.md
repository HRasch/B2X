---
docid: KB-022
title: GitHub Copilot Billing, Plans & Rate Limits (2026)
owner: GitHub Copilot
status: Active
last_updated: 2026-01-02
---

# GitHub Copilot Billing, Plans & Rate Limits

**Version**: 1.0 (Updated January 2026)  
**Last Updated**: 2026-01-02  
**Maintained By**: GitHub Copilot  
**Status**: ✅ Current

---

## Quick Reference

| Plan | Cost | Best For | Rate Limit |
|------|------|----------|-----------|
| **Copilot Free** | $0/month | Individual exploration | Limited monthly allowance |
| **Copilot Pro** | $10/month or $100/year | Individual developers | Higher monthly allowance |
| **Copilot Pro+** | $39/month or $390/year | Individual power users | Highest allowance + features |
| **Copilot Business** | $19/user/month | Organization teams | Enterprise-grade limits |
| **Copilot Enterprise** | Custom pricing | Large enterprises | Custom limits + security |

---

## Personal Account Plans

### Copilot Free
- **Cost**: $0/month
- **Best For**: Individual developers exploring Copilot
- **Includes**: Limited monthly allowance of completions and premium requests
- **Note**: Intended for individual use only (not suitable for organizations)
- **When Limit Exceeded**: Get "rate_limited" error - must upgrade or wait for monthly reset

### Copilot Pro
- **Cost**: $10 USD/month or $100 USD/year
- **Best For**: Individual developers with regular Copilot use
- **Includes**: Higher monthly allowance than Free
- **Billing**: Choose monthly or yearly; canceling monthly keeps access until end of cycle
- **Trial**: 30-day free trial available (one-time use)

### Copilot Pro+
- **Cost**: $39 USD/month or $390 USD/year
- **Best For**: Individual developers needing premium features and higher limits
- **Includes**:
  - Highest monthly allowance
  - Advanced features (PR descriptions, code generation)
  - Can create PRs and work on code changes
- **Billing**: Choose monthly or yearly; canceling yearly runs until end of term

### Special Eligibility (Free Pro Access)
Copilot Pro is **free** for:
- ✅ Verified students
- ✅ Verified teachers
- ✅ Maintainers of popular open source projects

See [GitHub Copilot educational access documentation](https://docs.github.com/en/copilot/managing-copilot/managing-copilot-as-an-individual-subscriber/getting-started-with-copilot-on-your-personal-account/getting-free-access-to-copilot-pro-as-a-student-teacher-or-maintainer)

---

## Organization & Enterprise Plans

### Copilot Business (Organizations)
- **Cost**: $19 USD/user/month (billed monthly)
- **Best For**: Organization teams needing consistent Copilot access
- **Billing Model**: 
  - Billed at end of each monthly cycle
  - Additional seats billed immediately on prorated basis
  - If user has seats in multiple org within same enterprise, enterprise billed once
- **Seat Assignment**: Flexible assignment by organization owners

### Copilot Enterprise
- **Cost**: Custom pricing (higher than Business)
- **Best For**: Large enterprises with security/compliance needs
- **Includes**: All Business features plus:
  - Enterprise-grade authentication
  - Advanced security features
  - Custom limits
  - Dedicated support

---

## Rate Limits & Usage Limits

### What Triggers "rate_limited" Error?
```
Error: Sorry, you have exceeded your Copilot token usage. 
Please review our Terms of Service. Error Code: rate_limited
```

**Causes**:
- ❌ Monthly token allowance exhausted (based on your plan)
- ❌ Premium request quota exceeded
- ❌ Usage reset hasn't occurred yet

### When Limits Reset
- **Monthly Reset**: Same calendar date each month
- **Check Your Billing Cycle**: [github.com/settings/billing/overview](https://github.com/settings/billing/overview)

### Plan Changes Take Effect Immediately
- Upgrade/downgrade: Effective immediately
- Prorated billing applied where relevant
- No service interruption during transition

---

## Budget Management

### Personal Accounts

**Available Features**:
- ✅ Set budget limits in settings
- ✅ Email alerts at thresholds:
  - 75% of budget
  - 90% of budget
  - 100% of budget (cap exceeded)

**Important**: Budgets are **monitoring-only**
- Alerts notify you but don't block usage
- Set budgets at: [github.com/settings/billing/overview](https://github.com/settings/billing/overview)

### Organizations & Enterprises

**Available Features**:
- ✅ Set budgets at organization level
- ✅ Set budgets at enterprise level
- ✅ Set budgets by cost center
- ✅ Email alerts for threshold breaches

**Important**: Like personal accounts, budgets are **monitoring-only**
- Spending can exceed budget
- Alerts notify owners when thresholds reached
- Configure in organization billing settings

---

## Billing & Payments

### How Licenses Are Measured

**Personal Accounts**:
- Measured by number of active seats (one per account)
- Individual plan = single account binding
- Cannot add additional seats to personal plan

**Organizations/Enterprises**:
- Measured by number of assigned seats
- Each user = one seat
- Multiple seats = higher cost
- Additional seats billed immediately on prorated basis

### Payment Method
- Set payment method at: [github.com/settings/billing/payment-info](https://github.com/settings/billing/payment-info)
- All charges billed to this method

### Automatic Plan Conversion
If you have Copilot Pro and are assigned a Copilot Business/Enterprise seat:
- ✅ Your personal plan automatically cancels
- ✅ You receive prorated refund for remaining balance
- ✅ Access continues under organization plan
- ✅ Company policies now apply to your usage

---

## Troubleshooting Rate Limits

### If You Hit the Limit

**Immediate Actions**:
1. ✅ Check when your monthly reset occurs: [github.com/settings/billing/overview](https://github.com/settings/billing/overview)
2. ✅ Upgrade to a higher plan for immediate access
3. ✅ If in organization, request additional seat or higher-tier plan
4. ✅ Wait for monthly reset (if no budget for upgrade)

**During Waiting Period**:
- Write code manually
- Use documentation/search
- Request code review from team
- Use git history for reference

### Verify Your Plan
```bash
# Check your plan via GitHub CLI
gh auth status  # Shows your account

# Visit settings page
open https://github.com/settings/copilot
```

### Check Billing Cycle
Visit: [github.com/settings/billing/overview](https://github.com/settings/billing/overview)
- Shows current plan
- Shows billing date
- Shows usage/spending

---

## Cost Comparison

**Monthly Spend for Individual Developer**:
| Scenario | Free | Pro | Pro+ |
|----------|------|-----|------|
| Light use (< 100 requests/mo) | ✅ $0 | ❌ $10 | ❌ $39 |
| Regular use (< 1000 requests/mo) | ⚠️ May hit limit | ✅ $10 | ❌ $39 |
| Heavy use (> 5000 requests/mo) | ❌ Will hit limit | ⚠️ May hit limit | ✅ $39 |
| Multiple projects + team work | ❌ No | ❌ Single user | ✅ Best value |

**Organization Spend** (10 developers):
| Plan | Monthly Cost | Per Developer | 
|------|--------------|-----------------|
| Free (no Copilot) | $0 | $0 |
| Mixed (5 Pro users) | $50 | $5 average |
| Business (10 seats) | $190 | $19 per seat |
| Enterprise (10 seats) | $250+ | $25+ per seat |

---

## Key URLs

| Resource | Link |
|----------|------|
| **Settings & Plans** | https://github.com/settings/copilot |
| **Billing Overview** | https://github.com/settings/billing/overview |
| **Payment Methods** | https://github.com/settings/billing/payment-info |
| **Official Docs** | https://docs.github.com/en/copilot |
| **Licensing Docs** | https://docs.github.com/en/billing/concepts/product-billing/github-copilot-licenses |
| **Pricing Page** | https://github.com/features/copilot/pricing |

---

## B2X Implementation

Your project's Copilot configuration:

**Current Setup**:
- Budget monitoring via `scripts/ai-cost-monitor.py`
- Configured monthly limit: $500
- Alert thresholds: 80% ($400) and 95% ($475)

**Actual User Holger's Status** (as of Jan 2, 2026):
- ✅ **Plan**: Copilot Pro+ ($39/month)
- 📊 **Premium Requests Used**: 30% of monthly allowance
- ⚠️ **Note**: Rate limit error received despite 30% usage
  - Suggests potential issue beyond premium requests
  - May be completions limit or temporary service issue
  - Recommend checking error details and GitHub status

**Upgrade Path if Limit Exceeded**:

1. **Immediate** (Continue today):
   - Upgrade to Copilot Pro ($10/month)
   - Or Copilot Pro+ ($39/month) for highest limits

2. **Wait for Reset** (Free option):
   - Check reset date: [github.com/settings/billing/overview](https://github.com/settings/billing/overview)
   - Will be same calendar date next month
   - No code work possible until reset

3. **Organizational** (If available):
   - Request Copilot Business seat from your organization
   - $19/user/month, billed by organization
   - Immediate access after assignment

---

## Related Documentation

- [AI Cost Monitoring & Optimization](./ai-cost-optimization.md)
- [GitHub Copilot Models (KB-016)](./github-copilot-models.md)
- [B2X Copilot Instructions](./.github/copilot-instructions.md)

---

## Summary

**Rate Limited? Here's what happened**:
1. You exceeded your monthly Copilot token allowance
2. This is plan-specific (Free/Pro/Pro+/Business/Enterprise)
3. Either upgrade immediately or wait for monthly reset

**Recommended Action**:
- 🚀 **Quick Fix**: Upgrade to [Copilot Pro](https://github.com/settings/copilot) ($10/month)
- ⏰ **Free Alternative**: Wait for reset date in [billing settings](https://github.com/settings/billing/overview)
- 🏢 **Organization**: Request Copilot Business from your org owner

**Official Support**:
- Docs: https://docs.github.com/en/copilot
- Support: https://support.github.com/

---

**Maintained By**: GitHub Copilot  
**Last Updated**: 2026-01-02  
**Next Review**: 2026-02-02 (monthly billing reset tracking)
