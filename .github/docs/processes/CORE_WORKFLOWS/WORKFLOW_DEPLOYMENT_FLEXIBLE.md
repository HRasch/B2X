# WORKFLOW_DEPLOYMENT - Flexible Continuous (No Safe Windows)

**Owner**: @process-assistant  
**Last Updated**: 29. Dezember 2025  
**Version**: 2.0 - AGENT-DRIVEN CONTINUOUS DEPLOYMENT  
**Status**: ACTIVE - Replaces Tue-Thu 2-4 PM UTC window

---

## 🎯 Core Principle

**Deploy when ready and safe, not on a schedule. Monitoring determines if safe, not a calendar window.**

---

## 📋 Deployment Readiness

### **Before Deploy Check** (Immediate Pre-Deployment)

**Owner**: @devops-engineer + code author  
**Checklist**:

- [ ] **Code merged**: PR approved, merged to main
- [ ] **Tests passing**: `dotnet test B2Connect.slnx` = 100%
- [ ] **Build succeeds**: `dotnet build B2Connect.slnx` = 0 errors
- [ ] **Code review approved**: All required reviews complete
- [ ] **Security approved**: @security-engineer signed off (if changes PII/security)
- [ ] **Documentation updated**: User-facing changes have docs
- [ ] **Migration ready**: DB migrations tested (if applicable)
- [ ] **Rollback plan**: Know how to revert if needed

**Exit Criteria**: All checks passing → Ready to deploy

---

## 🚀 Deployment Flow

### **Phase 1: Pre-Deployment Validation** (5-10 minutes)

**Owner**: @devops-engineer  
**Actions**:

1. **Pull latest main branch**
   ```bash
   git pull origin main
   ```

2. **Verify tests passing locally**
   ```bash
   dotnet test B2Connect.slnx -v minimal
   ```

3. **Verify build success**
   ```bash
   dotnet build B2Connect.slnx
   ```

4. **Check deployment target**
   - Staging or Production?
   - Database state?
   - Other services running?

**Duration**: 5-10 min

---

### **Phase 2: Deploy to Staging** (5-15 minutes)

**Owner**: @devops-engineer  
**Process**:

1. **Build Docker image**
   ```bash
   docker build -t b2connect:staging-[commit-sha] .
   ```

2. **Push to staging registry**
   ```bash
   docker push staging-registry/b2connect:staging-[commit-sha]
   ```

3. **Deploy to staging environment**
   ```bash
   kubectl apply -f deploy/staging.yaml
   ```

4. **Verify deployment**
   - Pods running?
   - Services healthy?
   - Database migrations applied?

**Duration**: 10-15 min (depends on deployment method)

---

### **Phase 3: Smoke Tests** (10-20 minutes)

**Owner**: @qa-engineer  
**Process**:

1. **Run automated smoke tests**
   ```bash
   npm run test:e2e -- --smoke
   ```

2. **Manual checks**:
   - [ ] Can users login?
   - [ ] Can create product?
   - [ ] Can checkout?
   - [ ] Can view admin dashboard?
   - [ ] Error handling working?

3. **Check logs**:
   - Any startup errors?
   - Any database issues?
   - Any security warnings?

4. **Monitor metrics**:
   - Response times normal?
   - Error rate < 0.1%?
   - Database queries performing?

**Duration**: 10-20 min

**Exit**: Staging is stable → Ready for production

---

### **Phase 4: Deploy to Production** (5-15 minutes)

**Owner**: @devops-engineer  
**Timing**: 
- **Anytime** when staging is verified stable
- **NOT "only on Tue-Thu 2-4 PM"** - deploy when ready
- **Prefer low-traffic times** if possible (early morning UTC)
- **Don't wait** if urgent fix needed

**Process**:

1. **Blue-Green or Canary Deploy** (if supported)
   - Deploy to canary (5% traffic)
   - Monitor for 5 min
   - Roll out to 50% traffic
   - Monitor for 5 min
   - Roll out to 100% traffic

   OR

   - Deploy to blue, traffic still on green
   - Run production smoke tests
   - Switch traffic to blue
   - Keep green as instant rollback

2. **Direct Deploy** (if critical fix):
   - Deploy directly to production
   - Monitor closely
   - Rollback ready

3. **Verify Production**
   - [ ] Pods running
   - [ ] Services responding
   - [ ] Database queries working
   - [ ] Log aggregation receiving logs
   - [ ] Monitoring showing green metrics

**Duration**: 10-20 min

---

### **Phase 5: Post-Deployment Monitoring** (30 minutes)

**Owner**: @devops-engineer + on-call team  
**Process**:

1. **First 5 minutes**: High alert
   - Watch metrics dashboard
   - Any spike in errors?
   - Any service down?
   - Any database issues?

2. **Next 10 minutes**: Monitor key metrics
   - Response time (P95 should be normal)
   - Error rate (should be < 0.1%)
   - Database connections (normal levels)
   - Memory/CPU (normal)

3. **Next 15 minutes**: Settle in
   - Spot check key features
   - Review error logs
   - Confirm no data corruption

4. **After 30 minutes**: Deployment successful
   - Add deployment to changelog
   - Notify team in Slack #deployments
   - Close any related issues

---

## 🛑 Rollback Procedure (If Issues Found)

**Owner**: @devops-engineer  
**Trigger**: Error rate spike, data corruption, service down

**Immediate Action** (< 1 minute):
```bash
# Option 1: Blue-Green swap
kubectl set selector svc/api blue=inactive blue=active  # Switch traffic to green

# Option 2: Rollback deployment
kubectl rollout undo deployment/b2connect
```

**Post-Rollback** (5-10 min):
1. Verify service recovered
2. Notify team #incident channel
3. Page @tech-lead for root cause analysis
4. Document issue
5. Fix in code, test, re-deploy

---

## 📊 Deployment Cadence (Flexible)

**OLD MODEL**: Only Tue-Thu 2-4 PM UTC
- Limited deployment windows
- Deployments often batched
- Wait times = delayed fixes

**NEW MODEL**: Deploy when ready
```
Example deployment timeline (realistic):

Monday 14:00 UTC: Bug reported + fixed
Monday 14:30 UTC: Code merged + tested
Monday 14:45 UTC: Deploy to staging ✅
Monday 15:00 UTC: Smoke tests pass ✅
Monday 15:15 UTC: Deploy to production ✅
Monday 15:45 UTC: Monitoring confirmed stable ✅
Total: 45 minutes from fix to production

(Old model: Would wait until Tuesday 2 PM = 22+ hours delay!)
```

---

## 🎯 Deployment Safety

### **Monitoring Determines If Safe** (Not Calendar)

**Automated Checks**:
- Tests: 100% passing
- Build: 0 errors
- Coverage: Maintain ≥80%
- Code review: Approved

**Manual Safety Check**:
- Error rate < 0.1% in staging
- Response time < 200ms P95
- Database healthy
- No data inconsistencies
- Logs clean

---

## 📋 Deployment Checklist

### **Before Staging Deploy**:
- [ ] Code merged to main
- [ ] Tests passing (100%)
- [ ] Build succeeding
- [ ] Code review approved
- [ ] Security approved (if sensitive)
- [ ] Database migrations tested

### **Before Production Deploy**:
- [ ] Staging stable (30 min no errors)
- [ ] Smoke tests passing
- [ ] Metrics showing normal
- [ ] Logs clean
- [ ] On-call team aware
- [ ] Rollback plan documented

### **After Production Deploy**:
- [ ] Metrics green for 30 min
- [ ] Customer-facing features verified
- [ ] Error rate normal
- [ ] No data issues
- [ ] Changelog updated
- [ ] Team notified

---

## 🔄 Deployment Frequency

**Expected**:
- Small bug fix: 2-4 per week
- Feature: 2-3 per week
- Hotfix (critical): Same day
- Security patch: Same day

**Goal**: Deploy every business day (but not on fixed schedule)

---

## 🚫 What Blocks Deployment

**Cannot deploy if**:
- Tests failing
- Build errors
- Code review not approved
- Security concerns unresolved
- Data migration not tested
- Customer notification not sent (for major features)

**How to unblock**:
- Fix failing tests
- Resolve code review comments
- Security sign-off obtained
- Test migration in staging

---

## 📞 Deployment Roles

| Role | Responsibility |
|------|-----------------|
| **@devops-engineer** | Execute deployment, monitoring, rollback |
| **Code author** | Verify fix works in staging |
| **@qa-engineer** | Smoke testing, quality verification |
| **@security-engineer** | Security approval (if needed) |
| **@tech-lead** | Approval for critical changes |
| **On-call** | Post-deployment monitoring, escalation |

---

## 🚨 Emergency Deployment (Critical Fix)

**Trigger**: Production down, data corruption, security breach

**Process** (expedited):
1. Fix code (minimal change)
2. Rapid testing in staging
3. Get @tech-lead approval
4. Deploy immediately (no safe window)
5. Monitor closely
6. Document incident

**Normal deployment windows DON'T apply**

---

## 🔗 Related Documents

- [WORKFLOW_CODE_REVIEW_ASYNC.md](./WORKFLOW_CODE_REVIEW_ASYNC.md) - Code must be reviewed before deploy
- [WORKFLOW_INCIDENT_RESPONSE_SEVERITY_BASED.md](./WORKFLOW_INCIDENT_RESPONSE_SEVERITY_BASED.md) - If deployment causes incident

---

**Owner**: @process-assistant  
**Version**: 2.0 (Flexible Continuous, No Safe Windows)  
**Status**: ACTIVE  
**Key Change**: Deploy when ready and safe, not on Tue-Thu 2-4 PM schedule. Monitoring determines safety, not calendar. Staging validation → Production. Rollback ready anytime.
