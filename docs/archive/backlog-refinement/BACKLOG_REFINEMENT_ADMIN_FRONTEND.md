# Backlog Refinement: Admin-Frontend Store Management Dashboard

**Epic:** [#18 Admin-Frontend: Store Management Dashboard & Control Panel](https://github.com/HRasch/B2Connect/issues/18)  
**Date:** 28. Dezember 2025  
**Team:** Frontend Developers (2-3 FTE)  
**Total Epic Effort:** ~200-240 hours (6-8 weeks)

---

## 📋 Backlog Refinement Outcomes

### Priority 1: CRITICAL (Foundation) - 4 weeks
These stories are prerequisites for all other features.

#### Story 1.1: Admin Dashboard Layout & Navigation
**Story Points:** 13 | **Effort:** 40 hours | **Owner:** TBD

**Description:**
Create the main admin dashboard layout with responsive navigation sidebar, top header bar, and page structure that will be used by all other components.

**Acceptance Criteria:**
- [ ] Sidebar navigation with collapsible menu (Admin, Catalog, Users, Orders, Settings, Health)
- [ ] Top header with user profile, notifications, logout
- [ ] Breadcrumb navigation showing current location
- [ ] Mobile-responsive (hamburger menu on < 768px)
- [ ] Dark mode toggle (persisted to localStorage)
- [ ] Dashboard homepage with empty state
- [ ] Navigation persists state across page refreshes
- [ ] All elements follow accessibility standards (WCAG 2.1 AA)

**Technical Details:**
- Use Vue Router for navigation
- Pinia store for navigation state
- Tailwind CSS for styling
- ARIA landmarks for screen readers

**Definition of Done:**
- [ ] Component built and tested
- [ ] Story points accurate
- [ ] Accessibility audit passed
- [ ] Lighthouse score > 90

**Blockers:** None

---

#### Story 1.2: Authentication & Authorization Layer
**Story Points:** 13 | **Effort:** 40 hours | **Owner:** TBD

**Description:**
Implement JWT-based authentication, role-based access control (RBAC), and session management for admin users.

**Acceptance Criteria:**
- [ ] Login page with email/password input (beautiful, responsive)
- [ ] JWT token stored securely (httpOnly cookie)
- [ ] Auto-logout after 30min inactivity
- [ ] Role-based route guards (Admin, Manager, Customer Support)
- [ ] User profile display in header
- [ ] Logout functionality
- [ ] "Session expired" modal with re-login option
- [ ] Protected routes return 401 if not authenticated

**Technical Details:**
- Use Vue Router navigation guards
- Pinia store for auth state
- Axios interceptor for JWT injection
- httpOnly cookie for token storage

**Definition of Done:**
- [ ] Component built and tested
- [ ] All routes protected
- [ ] Token refresh working
- [ ] Tests: 8+ test cases

---

#### Story 1.3: Health Status Dashboard
**Story Points:** 13 | **Effort:** 40 hours | **Owner:** TBD

**Description:**
Real-time health monitoring dashboard showing status of all microservices, databases, and system resources.

**Acceptance Criteria:**
- [ ] Display health status of: Identity, Catalog, CMS, Search, Database, Redis
- [ ] Status indicators: Green (healthy), Yellow (degraded), Red (down)
- [ ] Last check timestamp for each service
- [ ] CPU & Memory usage charts (real-time, last 24h)
- [ ] API response time metrics (P50, P95, P99)
- [ ] Database connection pool status
- [ ] Auto-refresh every 30 seconds
- [ ] Historical data visible (24h trend)

**Technical Details:**
- WebSocket for real-time updates (or polling every 30s)
- Chart library: Chart.js or ECharts
- Connect to `/health` endpoints of each service

**Definition of Done:**
- [ ] Dashboard displays all metrics
- [ ] Auto-refresh working
- [ ] Performance acceptable (updates smooth)
- [ ] Tests: 6+ test cases

---

#### Story 1.4: Store State Management Controls
**Story Points:** 8 | **Effort:** 25 hours | **Owner:** TBD

**Description:**
Admin controls to manage cache, restart services, and reload configurations.

**Acceptance Criteria:**
- [ ] "Reset Cache" button with confirmation modal
- [ ] "Restart Service" dropdown to select service
- [ ] "Reload Configuration" button
- [ ] Success/error notifications for each action
- [ ] Audit log entry created for each action
- [ ] Disabled if user doesn't have Admin role

**Technical Details:**
- Confirmation modals (Vue dialog component)
- API calls to Admin Gateway endpoints
- Toast notifications for feedback

**Definition of Done:**
- [ ] All controls functional
- [ ] Confirmation dialogs working
- [ ] Error handling in place
- [ ] Tests: 5+ test cases

---

### Priority 2: HIGH (Business Features) - 3 weeks

#### Story 2.1: Catalog Management - Product CRUD
**Story Points:** 21 | **Effort:** 65 hours | **Owner:** TBD

**Description:**
Full CRUD interface for products with filtering, search, and pagination.

**Acceptance Criteria:**
- [ ] Product listing page with pagination (20 items per page)
- [ ] Search by SKU, name, category
- [ ] Filter by category, price range, stock status
- [ ] Create product form (SKU, name, description, category, price)
- [ ] Edit product form (all fields editable)
- [ ] Delete product with confirmation
- [ ] Bulk actions: Delete multiple, Change category
- [ ] Product detail modal/drawer
- [ ] "Last modified" timestamp for each product

**Technical Details:**
- DataTable component with sorting/filtering
- Form validation with Vuelidate or VeeValidate
- Modal for create/edit
- API integration with Catalog service

**Definition of Done:**
- [ ] Full CRUD working
- [ ] Search & filters functional
- [ ] Form validation complete
- [ ] Tests: 15+ test cases

---

#### Story 2.2: Catalog Management - Categories & Hierarchy
**Story Points:** 13 | **Effort:** 40 hours | **Owner:** TBD

**Description:**
Manage product categories with drag-and-drop hierarchy.

**Acceptance Criteria:**
- [ ] Category listing with drag-and-drop reordering
- [ ] Create category (name, description, parent category)
- [ ] Edit category
- [ ] Delete category (prevent if products assigned)
- [ ] Visual hierarchy tree view
- [ ] Show product count per category
- [ ] SEO fields: slug, meta description

**Technical Details:**
- Drag-and-drop: Vue.Draggable or similar
- Tree view component
- Form validation

**Definition of Done:**
- [ ] Hierarchy management working
- [ ] Drag-and-drop smooth
- [ ] Tests: 10+ test cases

---

#### Story 2.3: User Management
**Story Points:** 13 | **Effort:** 40 hours | **Owner:** TBD

**Description:**
Manage admin and support users with role assignments and permissions.

**Acceptance Criteria:**
- [ ] User listing with roles displayed
- [ ] Create user (email, name, role)
- [ ] Edit user (name, role)
- [ ] Deactivate/activate user
- [ ] Reset user password (email them reset link)
- [ ] Show last login timestamp
- [ ] Login attempt audit trail
- [ ] Filter by role, status

**Technical Details:**
- User list with pagination
- Role selector (Admin, Manager, Customer Support)
- Form validation

**Definition of Done:**
- [ ] User CRUD working
- [ ] Audit trail visible
- [ ] Tests: 10+ test cases

---

#### Story 2.4: Order Management - List & Details
**Story Points:** 13 | **Effort:** 40 hours | **Owner:** TBD

**Description:**
Order listing with filtering, search, and detailed order view.

**Acceptance Criteria:**
- [ ] Order listing with pagination
- [ ] Search by order ID, customer email, date range
- [ ] Filter by status (pending, shipped, delivered, cancelled)
- [ ] Sort by date, amount, customer
- [ ] Order detail view showing:
  - Customer info
  - Line items
  - Totals (subtotal, shipping, tax, total)
  - Status timeline (created → shipped → delivered)
  - Invoice PDF link
- [ ] Audit trail visible (who modified, when)

**Technical Details:**
- DataTable with filters/sorting
- Status timeline component
- PDF viewer integration

**Definition of Done:**
- [ ] Listing and details working
- [ ] Filters/search functional
- [ ] Tests: 12+ test cases

---

#### Story 2.5: Order Management - Status & Returns
**Story Points:** 8 | **Effort:** 25 hours | **Owner:** TBD

**Description:**
Update order status, process returns/refunds.

**Acceptance Criteria:**
- [ ] Change order status (dropdown: pending → shipped → delivered)
- [ ] Add tracking number
- [ ] Process return request (approve/reject)
- [ ] Generate return label
- [ ] Process refund (full/partial)
- [ ] Send status emails to customer
- [ ] Audit log created for changes

**Technical Details:**
- Status dropdown with confirmation
- Modal for refund details
- Email notification API calls

**Definition of Done:**
- [ ] All status changes working
- [ ] Notifications sending
- [ ] Tests: 8+ test cases

---

### Priority 3: MEDIUM (Configuration & Advanced) - 2 weeks

#### Story 3.1: Store Configuration
**Story Points:** 13 | **Effort:** 40 hours | **Owner:** TBD

**Description:**
Comprehensive store settings management (branding, payment, shipping, etc.).

**Acceptance Criteria:**
- [ ] **Branding**: Logo upload, brand colors, store name
- [ ] **Payment**: Stripe/PayPal API key configuration
- [ ] **Shipping**: Providers setup (DHL, UPS, DPD rates)
- [ ] **Email**: SMTP settings for transactional emails
- [ ] **Legal**: Terms, Privacy Policy, Impressum text areas
- [ ] **Regional**: Country-specific tax rates
- [ ] **Languages**: Multi-language configuration
- [ ] Save & preview functionality
- [ ] Validation before save
- [ ] Audit trail for configuration changes

**Technical Details:**
- Large form with multiple sections (tabs or accordion)
- File upload for logos
- Rich text editor for legal documents
- Form validation

**Definition of Done:**
- [ ] All config options working
- [ ] Save/preview functional
- [ ] Tests: 10+ test cases

---

#### Story 3.2: Email Template Management
**Story Points:** 13 | **Effort:** 40 hours | **Owner:** TBD

**Description:**
Editor for transactional email templates with preview and testing.

**Acceptance Criteria:**
- [ ] Template listing (Welcome, Order Confirmation, Shipping, Return, etc.)
- [ ] Template editor with variables reference
- [ ] WYSIWYG editor for HTML emails
- [ ] Multi-language support (DE, EN minimum)
- [ ] Preview email before save
- [ ] Send test email to admin
- [ ] Version history (rollback to previous version)
- [ ] Variables available: {{customer_name}}, {{order_id}}, {{tracking_number}}, etc.

**Technical Details:**
- WYSIWYG editor: TinyMCE or Quill
- Variable insertion toolbar
- Email preview iframe
- Markdown/HTML toggle

**Definition of Done:**
- [ ] Editor fully functional
- [ ] Preview working
- [ ] Test sending working
- [ ] Tests: 8+ test cases

---

#### Story 3.3: Audit Logs Viewer
**Story Points:** 8 | **Effort:** 25 hours | **Owner:** TBD

**Description:**
Read-only audit log viewer for compliance and debugging.

**Acceptance Criteria:**
- [ ] Audit log listing with pagination
- [ ] Filter by: entity type, action (CREATE, UPDATE, DELETE), date range
- [ ] Search by entity ID or user
- [ ] Show: timestamp, user, action, entity, before/after values
- [ ] JSON diff viewer for before/after
- [ ] Export audit logs to CSV
- [ ] Admin-only access

**Technical Details:**
- DataTable with complex filtering
- JSON diff viewer library
- CSV export functionality

**Definition of Done:**
- [ ] All filters working
- [ ] Diff viewer displays correctly
- [ ] Tests: 6+ test cases

---

#### Story 3.4: Dashboard Analytics
**Story Points:** 8 | **Effort:** 25 hours | **Owner:** TBD

**Description:**
Dashboard widgets showing key metrics and trends.

**Acceptance Criteria:**
- [ ] Total Sales (today, this week, this month)
- [ ] New Orders (count, growth %)
- [ ] Conversion Rate (visitors → buyers)
- [ ] Top Products (by revenue, by sales count)
- [ ] Top Customers (by spend)
- [ ] Revenue trend chart (last 30 days)
- [ ] Charts auto-refresh every 5 minutes
- [ ] Period selector (Today, Week, Month, Year, Custom)

**Technical Details:**
- Chart.js or ECharts for visualizations
- WebSocket for real-time updates
- Number formatting with K/M abbreviations

**Definition of Done:**
- [ ] All metrics displaying
- [ ] Charts responsive
- [ ] Tests: 5+ test cases

---

### Priority 4: LOW (Nice-to-Have) - 1 week

#### Story 4.1: Backup & Restore
**Story Points:** 5 | **Effort:** 15 hours | **Owner:** TBD

**Description:**
Manual backup and restore functionality.

**Acceptance Criteria:**
- [ ] "Create Backup" button
- [ ] Backup listing with date, size, status
- [ ] Restore from backup (with confirmation)
- [ ] Download backup file
- [ ] Auto-backup schedule configuration

---

#### Story 4.2: API Key Management
**Story Points:** 5 | **Effort:** 15 hours | **Owner:** TBD

**Description:**
Generate and manage API keys for third-party integrations.

**Acceptance Criteria:**
- [ ] List API keys with creation date
- [ ] Create new API key
- [ ] Revoke API key
- [ ] Copy key to clipboard
- [ ] Show last used timestamp

---

#### Story 4.3: Webhook Configuration
**Story Points:** 5 | **Effort:** 15 hours | **Owner:** TBD

**Description:**
Configure webhooks for external integrations.

**Acceptance Criteria:**
- [ ] Webhook listing
- [ ] Create webhook (URL, events, headers)
- [ ] Test webhook
- [ ] View webhook logs
- [ ] Delete webhook

---

## 📊 Backlog Summary

| Priority | Stories | Points | Effort | Duration |
|----------|---------|--------|--------|----------|
| **P1: CRITICAL** | 4 stories | 47 | 145h | 4-5 weeks |
| **P2: HIGH** | 5 stories | 69 | 215h | 3-4 weeks |
| **P3: MEDIUM** | 4 stories | 42 | 130h | 2-3 weeks |
| **P4: LOW** | 3 stories | 15 | 45h | 1 week |
| **TOTAL** | **16 stories** | **173 points** | **535h** | **10-12 weeks** |

---

## 🔄 Dependencies & Sequencing

### Phase 1: Foundation (Weeks 1-2)
```
Start:
├── 1.1 Dashboard Layout (40h)
├── 1.2 Auth Layer (40h)
└── 1.3 Health Status (40h)

Then:
└── 1.4 Store Controls (25h)
```

**Dependencies:** All subsequent stories depend on Phase 1 completion.

### Phase 2: Business Logic (Weeks 3-5)
```
Start (in parallel):
├── 2.1 Product CRUD (65h)
├── 2.2 Categories (40h)
├── 2.3 Users (40h)
└── 2.4 Orders List (40h)

Then:
└── 2.5 Order Returns (25h)
```

**Dependencies:** 2.1 & 2.2 can be done in parallel; 2.5 depends on 2.4 completion.

### Phase 3: Advanced (Weeks 6-7)
```
├── 3.1 Store Config (40h)
├── 3.2 Email Templates (40h)
├── 3.3 Audit Logs (25h)
└── 3.4 Analytics (25h)
```

**Dependencies:** None; can be done in parallel.

### Phase 4: Polish (Week 8+)
```
├── 4.1 Backup/Restore (15h)
├── 4.2 API Keys (15h)
└── 4.3 Webhooks (15h)
```

**Dependencies:** None; low priority, can be added later.

---

## 👥 Resource Plan

**Team:** 2-3 Frontend Developers  
**Sprint Length:** 2 weeks  
**Velocity Target:** ~60-80 story points per sprint

### Suggested Sprint Allocation

| Sprint | Focus | Stories | Points |
|--------|-------|---------|--------|
| Sprint 1 | Foundation | 1.1, 1.2, 1.3 | 39 points |
| Sprint 2 | Foundation Finish | 1.4, 2.1 (start) | 31 points |
| Sprint 3 | Catalog & Users | 2.1 (finish), 2.2, 2.3 | 66 points |
| Sprint 4 | Orders | 2.4, 2.5, 3.1 | 54 points |
| Sprint 5 | Templates & Analytics | 3.2, 3.3, 3.4 | 46 points |
| Sprint 6+ | Nice-to-Have | 4.1, 4.2, 4.3 | 15 points |

---

## ✅ Definition of Done (Each Story)

- [ ] Code written and committed
- [ ] Unit tests written (90%+ coverage)
- [ ] Code review approved
- [ ] Manual testing completed
- [ ] Accessibility audit passed (WCAG 2.1 AA)
- [ ] Performance acceptable (Lighthouse > 90)
- [ ] Documentation updated
- [ ] Story points accurate
- [ ] Ready for production

---

## 🚀 Success Criteria

### Completion Metrics
- [ ] All P1 & P2 stories completed (100% acceptance criteria)
- [ ] No critical or high-severity bugs
- [ ] Test coverage > 80%
- [ ] Accessibility audit: WCAG 2.1 AA
- [ ] Performance: Lighthouse > 90
- [ ] Security: Penetration test passed

### User Acceptance
- [ ] Admin users can perform all CRUD operations
- [ ] Dashboard is intuitive and responsive
- [ ] No confusing error messages
- [ ] Help/documentation available

---

## 📝 Notes & Considerations

1. **Accessibility First**: Every story must include WCAG 2.1 AA compliance checks
2. **Performance**: Images/PDFs should be lazy-loaded, pagination required for large datasets
3. **Security**: All API calls over HTTPS, JWT tokens in httpOnly cookies, RBAC enforced
4. **Internationalization**: Support DE & EN minimum (Pinia i18n store)
5. **Responsive Design**: Mobile-friendly (test on iPad, iPhone 12, Android)
6. **Error Handling**: User-friendly error messages, retry mechanisms
7. **Loading States**: Skeleton screens during data fetches, disabled buttons during submission

---

## 📞 Questions for Clarification

- [ ] Which payment gateways to support initially? (Stripe, PayPal, others?)
- [ ] Should we integrate with specific ERP systems (SAP, Oracle)?
- [ ] Email service: SMTP relay or API (SendGrid, Mailgun)?
- [ ] Reporting/BI tools: Include analytics, or defer to Phase 2?
- [ ] Multi-language: DE, EN only, or more languages?

---

**Owner:** Frontend Tech Lead  
**Last Updated:** 28. Dezember 2025  
**Next Review:** End of Sprint 1
