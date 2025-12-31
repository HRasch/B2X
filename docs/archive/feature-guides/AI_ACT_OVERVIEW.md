# 🤖 EU AI Act Compliance Overview (Quick Reference)

**Status:** Quick Reference | **Last Updated:** 28. Dezember 2025

---

## What is the EU AI Act?

**EU AI Act** (Regulation 2024/1689) is a **risk-based framework** for AI systems in the EU.

| Aspect | Details |
|--------|---------|
| **Deadline** | 12. Mai 2026 (18 months to comply) |
| **Scope** | All companies deploying AI systems affecting EU citizens |
| **Fines** | Up to €30M or 6% annual revenue (whichever is higher) |
| **Application** | Immediately - compliance window is now open |

---

## AI Risk Levels (4 Categories)

```
┌─────────────────────────────────────────────────┐
│ 1. PROHIBITED LEVEL (Banned entirely)           │
│    - Social credit systems                      │
│    - Biometric mass surveillance                │
│    - Manipulation of vulnerable groups          │
│    B2Connect Impact: ❌ NONE (we don't use these)│
└─────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────┐
│ 2. HIGH-RISK LEVEL (Full compliance required)   │
│    ✅ Fraud Detection (payment systems)          │
│    - Educational/employment decisions           │
│    - Law enforcement decisions                  │
│    B2Connect Action: FULL P0.7 compliance       │
└─────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────┐
│ 3. LIMITED-RISK LEVEL (Transparency required)   │
│    ✅ Duplicate Detection (customer accounts)    │
│    ✅ Content Moderation (user reviews)          │
│    - Chatbots                                   │
│    - Emotion recognition                       │
│    B2Connect Action: Transparency labels        │
└─────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────┐
│ 4. MINIMAL-RISK LEVEL (General compliance)      │
│    ✅ Product Recommendations (browsing history)│
│    ✅ Dynamic Pricing (supply/demand)           │
│    - Spam filters                               │
│    B2Connect Action: Basic audit trail          │
└─────────────────────────────────────────────────┘
```

---

## B2Connect AI Systems Categorization

| AI System | Purpose | Risk Level | B2C Impact | B2B Impact | Compliance Effort |
|-----------|---------|-----------|-----------|-----------|-------------------|
| **Fraud Detection** | Detect payment fraud | 🔴 HIGH-RISK | Payment blocking | Transaction denial | 50+ hours (P0.7) |
| **Duplicate Detection** | Find duplicate customer accounts | 🟡 LIMITED-RISK | Account blocking | Account blocking | Included in P0.7 |
| **Recommendations** | Product recommendations (browsing) | 🟢 MINIMAL-RISK | Browse suggestions | Catalog browsing | Audit logging |
| **Dynamic Pricing** | Price optimization per tenant | 🟢 MINIMAL-RISK | Price changes | Price changes | Audit logging |
| **Content Moderation** | Filter inappropriate reviews | 🟡 LIMITED-RISK | Review blocking | Review blocking | Included in P0.7 |
| **Search Ranking** | Elasticsearch relevance | 🟡 LIMITED-RISK (unless biased) | Browse ranking | Catalog ranking | Bias testing |

---

## Compliance Checklist: What B2Connect Must Do

### ✅ Immediate (Weeks 1-2)

- [ ] **AI Risk Register Created**
  - Document each AI system
  - Assign risk level (HIGH, LIMITED, MINIMAL)
  - Assign responsible person (name + email)
  - Document purpose and scope

- [ ] **Responsible Person Designated** (Art. 22)
  - HIGH-RISK: Security Engineer + AI/ML Expert
  - LIMITED-RISK: Backend Developer + Security
  - MINIMAL-RISK: Product Manager

### ✅ Short-Term (Weeks 3-6)

- [ ] **HIGH-RISK AI: Full Documentation** (Art. 11)
  - [ ] Training data documentation
    - Source (customer transactions from 2023-2024)
    - Size (100K+ samples)
    - Preprocessing (feature engineering, scaling)
  - [ ] Validation results
    - Accuracy: 95%+
    - Precision: 92%+
    - Recall: 94%+
    - ROC-AUC: 0.97+
  - [ ] Limitations
    - May block legitimate transactions
    - Requires human review for amount > 10K
    - Geographic biases possible
  - [ ] Human review procedure
    - Fraud team: 4-hour SLA for review
    - Override allowed with documented reason
    - Escalation: if not reviewed within SLA

- [ ] **Transparency Logs Created**
  - Every AI decision logged
  - User ID, timestamp, confidence score
  - Input data (transaction details)
  - Output (fraud decision)
  - Human review status

### ✅ Medium-Term (Weeks 7-10)

- [ ] **Bias Testing Implemented**
  - Test across: gender, age, region, income
  - Acceptance rate difference < 5% between groups
  - Quarterly retesting
  - Documentation of findings

- [ ] **Performance Monitoring Activated**
  - Monthly accuracy checks
  - Alert if accuracy drops > 5%
  - Drift detection for all metrics
  - Model version tracking

- [ ] **User Rights Implemented** (Art. 22)
  - API: `GET /api/ai-decisions/{id}/explanation`
  - Returns: What decided? Why? Can I dispute?
  - User can request explanation within 30 days
  - Response time: < 24 hours

---

## High-Risk AI: What's Required? (Fraud Detection)

### 1. Technical Documentation (Art. 11)

**What:** Write a 20-50 page document explaining:
- Model architecture (what's inside the black box?)
- Training process (how was it built?)
- Data sources (where did you get the data?)
- Performance metrics (how good is it really?)
- Limitations (what doesn't work well?)

**Example Structure:**
```
1. Executive Summary
2. Model Architecture & Design
3. Training Data & Methodology
   - Data sources & collection
   - Preprocessing & feature engineering
   - Training/validation/test split
4. Model Performance
   - Accuracy, precision, recall
   - ROC curve, confusion matrix
   - Performance by customer segment
5. Limitations & Failure Cases
   - When does it fail?
   - Known biases
   - Edge cases
6. Human Review Procedure
   - Who reviews?
   - When is review required?
   - How are overrides handled?
7. Monitoring & Maintenance
   - How is performance tracked?
   - Retraining frequency?
```

### 2. Risk Assessment (Art. 6)

**What:** Evaluate potential harms:
- Blocking legitimate customers (revenue loss, customer dissatisfaction)
- Discriminating based on protected characteristics (gender, age, ethnicity)
- Reinforcing existing biases (only approves "typical" customers)

**Mitigation:**
- Human review for high-risk decisions
- Quarterly bias testing
- Model retraining if drift detected
- Customer dispute process

### 3. Conformity Assessment (Art. 8)

**What:** Third-party audit that your AI system is compliant

**Options:**
- Internal audit (you audit yourself)
- External audit (hire auditor)
- Notified body (EU-certified auditor)

**B2Connect:** Internal audit for Phase 0.7 ✅

### 4. Transparency & User Rights (Art. 22)

**What:** Users affected by the AI must be informed:
- "Your payment was flagged by our AI fraud detection system"
- "Reasons: Large amount + international transaction + unusual merchant type"
- "You can request more details: fraud-support@b2connect.com"
- "You can dispute this decision: appeal-form.b2connect.com"

**How:**
- Email notification when transaction blocked
- Explanation API for users
- Dispute form on website
- SLA: Respond to disputes within 10 days

---

## Limited-Risk AI: What's Required? (Duplicate Detection)

### Compliance Tasks

1. **Transparency Label** ✅
   - Inform customers: "We check for duplicate accounts using AI"
   - Explain: "Why? To prevent fraud and duplicate registrations"
   - Link: To explanation

2. **Audit Trail** ✅
   - Log every duplicate check
   - Log: Which customer flagged as duplicate?
   - Log: Confidence score
   - Log: Action taken (block/approve)

3. **Override Capability** ✅
   - Support team can override "duplicate" decision
   - Customer can appeal
   - Manual review available

---

## Minimal-Risk AI: What's Required? (Recommendations)

### Compliance Tasks

1. **Audit Trail** ✅
   - Log recommendations shown to users
   - Track: What was recommended? Why?

2. **Optional Opt-Out** ✅
   - Users can disable recommendations
   - Fallback to non-AI method (popular products)

---

## Key Articles in AI Act

| Article | Requirement | B2Connect Impact |
|---------|-------------|-----------------|
| Art. 6 | Risk assessment | HIGH-RISK AI needs full assessment |
| Art. 8 | Conformity assessment | Internal audit required |
| Art. 11 | Technical documentation | 30-50 page document per HIGH-RISK AI |
| Art. 22 | Right to explanation | Users can ask "why?" and get answer |
| Art. 35 | Governance & compliance | Designate responsible persons |

---

## Testing & Validation

### What Tests Do We Need?

| Test Category | HIGH-RISK | LIMITED-RISK | MINIMAL-RISK |
|---------------|-----------|--------------|--------------|
| Risk Register | ✅ | ✅ | ✅ |
| Decision Logging | ✅ | ✅ | ✅ |
| Bias Testing | ✅ | ✅ | — |
| Performance Monitoring | ✅ | — | — |
| Human Oversight | ✅ | — | — |
| User Explanation | ✅ | ✅ | — |
| Quarterly Audits | ✅ | — | — |

**Total Tests: 15 (see P0.7_AI_ACT_TESTS.md)**

---

## Bias Testing: Deep Dive

### Why Test for Bias?

**Scenario:** AI blocks 20% of female customers for fraud, but only 5% of male customers
- This is **discrimination** (violates AI Act Art. 14)
- Fines: Up to €30M
- Loss of customer trust
- Bad publicity

### How to Test?

**Collect test data grouped by demographics:**
```
Gender (M/F), Age (20-30, 30-40, etc.), Region (DE/AT/FR/etc.)
1000 transactions per group = 8000+ test transactions

Run AI on all test data
Calculate: Approval rate per group

Gender:
  - Male: 95% approved
  - Female: 93% approved
  Difference: 2% ✅ (< 5% threshold)

Age:
  - 20-30: 94% approved
  - 50-60: 92% approved
  Difference: 2% ✅ (< 5% threshold)

Region:
  - Germany: 94% approved
  - Eastern Europe: 88% approved
  Difference: 6% ❌ (> 5% threshold!)
  → ACTION: Investigate bias, retrain model
```

### Testing Frequency

- **Initial:** Before deployment to production
- **Quarterly:** Every 3 months
- **Incident-Driven:** If discrimination complaint received

---

## Performance Monitoring: Deep Dive

### What to Monitor?

| Metric | Baseline | Alert Threshold | Action |
|--------|----------|-----------------|--------|
| **Accuracy** | 95% | Drop to 90% | Retraining review |
| **Precision** | 92% | Drop to 87% | Retraining review |
| **Recall** | 94% | Drop to 89% | Retraining review |
| **False Positive Rate** | 8% | Rise to 13% | Retraining review |
| **Decision Distribution** | 5% fraud rate | Change to > 15% | Investigate anomaly |

### Monitoring Frequency

- **Daily:** Count of decisions, distribution of outcomes
- **Weekly:** Spot check of accuracy (sample 100 recent cases)
- **Monthly:** Full accuracy evaluation
- **Quarterly:** Bias testing + documentation update

---

## Human Oversight: Decision Flow

```
HIGH-RISK Decision (confidence > 0.9)
  ↓
Log decision immediately
  ↓
Notify fraud team
  ↓
Fraud analyst reviews (SLA: 4 hours)
  ↓
Decision Options:
  ├─ Approve (allow transaction) → Override log
  ├─ Reject (block transaction) → Confirm AI decision
  └─ Escalate (need more info) → Customer service review

All overrides logged:
  - Who overrode?
  - When?
  - Why?
  - Outcome?
```

---

## Cost & Effort Summary

| Component | Hours | Team | Timeline |
|-----------|-------|------|----------|
| AI Risk Register | 8 | Backend Dev + Security | 1-2 days |
| Decision Logging | 12 | Backend Dev | 2-3 days |
| Bias Testing | 10 | Data/ML Engineer | 2-3 days |
| Performance Monitoring | 8 | DevOps + Backend | 1-2 days |
| Human Oversight Workflow | 6 | Backend Dev | 1 day |
| User Explanation API | 4 | Backend Dev | 1 day |
| Testing & Documentation | 8 | QA + Security | 1-2 days |
| **Total P0.7** | **~50 hours** | **1.5 FTE** | **1.5 weeks** |

---

## Penalties for Non-Compliance

| Violation | Fine | Example |
|-----------|------|---------|
| Missing risk assessment | Up to €10M | Didn't document AI system |
| Missing technical documentation | Up to €10M | Can't explain how fraud AI works |
| Missing human review for HIGH-RISK | Up to €20M | Let AI block transactions without review |
| No bias testing | Up to €15M | Discovered AI discriminates against women |
| Missing user explanation | Up to €8M | Customer blocked, can't explain why |
| **Total Maximum** | **€30M or 6% revenue** | **Non-compliant HIGH-RISK AI** |

---

## B2Connect Action Plan

### Timeline: 10 Weeks (P0.7 in Phase 0)

```
WEEK 1-2: AI Risk Register
  ├─ Document Fraud Detection (HIGH-RISK)
  ├─ Document Duplicate Detection (LIMITED-RISK)
  ├─ Document Recommendations (MINIMAL-RISK)
  └─ Assign responsible persons

WEEK 3-4: Decision Logging & Transparency
  ├─ Implement AiDecisionLog entity
  ├─ Add logging to fraud detection service
  ├─ Create explanation service
  └─ Build user explanation API

WEEK 5-6: Bias Testing Framework
  ├─ Create AiBiasTester
  ├─ Generate test data (1000+)
  ├─ Run initial bias tests
  └─ Document results

WEEK 7-8: Performance Monitoring
  ├─ Create AiPerformanceMonitor service
  ├─ Set up monthly accuracy checks
  ├─ Build drift detection
  └─ Create alerts

WEEK 9-10: Testing & Validation
  ├─ Run 15 comprehensive tests
  ├─ Legal review of documentation
  ├─ Internal compliance audit
  └─ Go/No-Go decision for Phase 1
```

---

## Resources & References

- [EU AI Act Official Text](https://eur-lex.europa.eu/eli/reg/2024/1689/oj)
- [AI Act Article-by-Article Guide](https://www.europarl.europa.eu/topics/en/article/20240206JAG00909/eu-artificial-intelligence-act-explained)
- [P0.7_AI_ACT_TESTS.md](P0.7_AI_ACT_TESTS.md) - Comprehensive test specifications (15 tests)
- [EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md](EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md) - Full Phase 0.7 specification

---

## FAQ

### Q: Does B2Connect use AI?
**A:** Yes, for Fraud Detection (HIGH-RISK), Duplicate Detection (LIMITED-RISK), and Recommendations (MINIMAL-RISK).

### Q: Do we need to comply with AI Act immediately?
**A:** Yes. The compliance window is open now (18 months until May 12, 2026). We should complete Phase 0.7 within 10 weeks to be ahead of schedule.

### Q: Who is responsible for compliance?
**A:** Designated responsible persons per AI system:
- Fraud Detection: Security Engineer (P0.7 lead)
- Duplicate Detection: Backend Developer
- Recommendations: Product Manager

### Q: What if we fail an audit?
**A:** Penalties up to €30M. To avoid:
1. Complete P0.7 testing
2. Pass internal audit
3. Document everything
4. Monitor continuously

### Q: Can we just delete our AI systems to avoid compliance?
**A:** Yes, but then you lose fraud detection capability, competitor advantage in recommendations, duplicate prevention. Better to comply.

### Q: Is this different from GDPR?
**A:** Yes. GDPR covers data privacy. AI Act covers AI system governance. Both apply simultaneously.

---

**Document Owner:** Security & Architecture Team  
**Last Updated:** 28. Dezember 2025  
**Next Review:** 15. Januar 2026
