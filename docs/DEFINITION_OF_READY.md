# Definition of Ready (DoR)

**Project:** Smart Profit Sharing (Demoulas)

## Purpose

The Definition of Ready (DoR) defines the **minimum inputs** required for a story/task/bug to begin development with low risk.

- Applies to **stories, tasks, bugs**, and **tech debt**.
- Items may be marked **N/A** with a brief justification.
- If the prerequisites cannot be met, either split the work, add a spike, or clarify requirements.

## Sources of truth (must-follow)

This DoR is grounded in the repository’s authoritative guidance:

- .github/CODE_REVIEW_CHECKLIST.md
- .github/instructions/all.security.instructions.md
- .github/instructions/all.code-review.instructions.md
- copilot-instructions.md
- src/ui/public/docs/TELEMETRY_GUIDE.md
- VALIDATION_PATTERNS.md
- ai-templates/ (frontend patterns, testing, navigation, validation)

If this DoR conflicts with any of the above, **follow the sources of truth**.

---

## DoR Checklist

### 1) Problem Statement and Scope

- [ ] **Clear title and summary**: What problem are we solving and why now?
- [ ] **In-scope / out-of-scope**: Explicit boundaries to prevent scope creep.
- [ ] **Success definition**: What observable behavior changes when the work is complete?

### 2) Acceptance Criteria (Testable)

- [ ] **Acceptance criteria written and testable**: Each AC is verifiable.
- [ ] **Edge cases listed**: At least the obvious boundary/negative cases.
- [ ] **Non-functional requirements captured** (as applicable): performance, reliability, auditability, UX.

### 3) UX / UI Requirements (when UI changes)

- [ ] **UX expectations defined**: Layout/flow changes are described (mockups if needed).
- [ ] **Accessibility considerations**: WCAG expectations noted where relevant.
- [ ] **Design system constraints known**: Use existing `smart-ui-library` components and established patterns.
- [ ] **No prohibited UI behavior**: e.g., no client-side role elevation; no frontend age calculation.

### 4) Architecture and Data Impact

- [ ] **Approach agreed**: Endpoint/service/data flow approach identified.
- [ ] **Data impact assessed**: Tables/fields touched, expected read/write volume, and any reporting impact.
- [ ] **History/audit implications**: Confirm whether changes require history patterns (close current → insert new).
- [ ] **Oracle EF constraints understood**: Query patterns avoid known translation pitfalls.

### 5) Security and Compliance Pre-Check (OWASP / FISMA-aligned)

- [ ] **AuthZ model identified**: Who is allowed to do this and where is enforcement?
- [ ] **Sensitive fields identified**: SSN, names, email, phone, bank info, DOB/age, etc.
- [ ] **PII handling specified**: masking expectations for logs/telemetry and UI display.
- [ ] **Input validation plan**: Ranges/lengths/enums/degenerate guards are called out.
- [ ] **Threat/abuse cases noted** (lightweight): what could a malicious user try?

### 6) Observability / Telemetry Requirements

- [ ] **Telemetry plan**: What business metric(s) or record counts should be tracked (if applicable)?
- [ ] **Sensitive field access declaration**: Which fields must be declared in telemetry for auditing?
- [ ] **Error scenarios**: Expected failure modes identified (validation, not found, unauthorized).

### 7) Testing Strategy

- [ ] **Test approach stated**: unit vs integration vs UI vs E2E.
- [ ] **Test data ready**: sample data exists or a generation approach is documented.
- [ ] **Regression risk identified**: What could break and how we’ll verify it.

### 8) Dependencies and Delivery

- [ ] **Dependencies identified**: upstream/downstream work, environment dependencies, feature flags.
- [ ] **Migration/rollout plan** (if DB/config changes): safe sequencing and rollback/disable path.
- [ ] **Ready for estimation**: Team can size the work without major unknowns.

---

## Not Ready (Common Triggers)

The work should be considered **not ready** if any of these are true:

- Acceptance criteria are unclear or not testable.
- Authorization is not defined (or relies on client-side enforcement).
- Sensitive fields are involved but masking/telemetry expectations are undefined.
- Data impact is unknown (tables/volume/history implications).
- There is no feasible test plan or test data strategy.
- UI behavior depends on frontend age calculation.
