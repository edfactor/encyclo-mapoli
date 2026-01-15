# Profit Sharing Documentation

## Overview

This documentation repository is organized by audience to help you quickly find relevant information. Whether you're a business leader evaluating security posture, a developer implementing features, or a technical lead reviewing architecture decisions, this index will guide you to the right resources.

---

## For Leadership & Executives

Strategic and high-level documentation for CTO, VP Engineering, business leaders, and stakeholders.

### Security & Compliance

- **[SECURITY_POSTURE.md](SECURITY_POSTURE.md)** - Comprehensive security overview covering OWASP Top 10 alignment, threat modeling (STRIDE), security controls, and compliance frameworks. Essential for security audits and executive reviews.

  - _Target Audience:_ CTO, CISO, Security Auditors, Compliance Officers
  - _Last Updated:_ January 13, 2026

- **[SECURITY_REVIEWS/](SECURITY_REVIEWS/)** - Periodic security review documentation and findings
  - [2025-12.md](SECURITY_REVIEWS/2025-12.md) - December 2025 security review notes and action items
  - _Target Audience:_ Security Team, Engineering Leadership
  - _Last Updated:_ December 2025

### Business & Process

- **[ARCHITECTURE_DECISIONS.md](ARCHITECTURE_DECISIONS.md)** - Architecture Decision Records (ADRs) documenting key technology choices, patterns, and rationale. Explains why we chose .NET Aspire, Oracle, FastEndpoints, and other architectural components.

  - _Target Audience:_ CTO, Engineering Managers, Architects
  - _Last Updated:_ January 13, 2026

- **[DEFINITION_OF_DONE_EXECUTIVE.md](DEFINITION_OF_DONE_EXECUTIVE.md)** - Executive summary of quality gates and completion criteria. High-level view of what "done" means for features and releases.

  - _Target Audience:_ VP Engineering, Product Management, Business Stakeholders
  - _Last Updated:_ January 13, 2026

- **[DEFINITION_OF_READY.md](DEFINITION_OF_READY.md)** - Criteria for work to enter development. Ensures stories are properly scoped, designed, and ready for implementation.

  - _Target Audience:_ Product Owners, Engineering Managers, Scrum Masters
  - _Last Updated:_ January 13, 2026

- **[BALANCE_REPORTS_CROSS_REFERENCE_MATRIX.md](BALANCE_REPORTS_CROSS_REFERENCE_MATRIX.md)** - Cross-reference matrix mapping balance reports between legacy COBOL system and new Smart Profit Sharing application. Critical for migration validation.
  - _Target Audience:_ Business Analysts, QA Leadership, Product Management
  - _Last Updated:_ January 13, 2026

### Case Studies & Success Stories

- **[CASE_STUDY_ENTERPRISE_RETAIL_AI_DEVELOPMENT.md](CASE_STUDY_ENTERPRISE_RETAIL_AI_DEVELOPMENT.md)** - Detailed case study of AI-assisted development approach, productivity metrics, and lessons learned. Demonstrates ROI and effectiveness of AI tooling integration.
  - _Target Audience:_ CTO, VP Engineering, Innovation Teams
  - _Last Updated:_ January 13, 2026
  - _Related:_ [PowerPoint version](CASE_STUDY_ENTERPRISE_RETAIL_AI_DEVELOPMENT.pptx) available for presentations

---

## For Developers & Technical Teams

Implementation guides, patterns, and technical documentation for day-to-day development work.

### Quality Standards

- **[DEFINITION_OF_DONE.md](DEFINITION_OF_DONE.md)** - Detailed Definition of Done (DoD) for stories, tasks, and bugs. Technical checklist including unit tests, code review, telemetry, security validation, and documentation requirements.
  - _Target Audience:_ Developers, QA Engineers, Tech Leads
  - _Last Updated:_ January 13, 2026

### Frontend Development

- **[technical/patterns/FRONT_END_PATTERNS.md](technical/patterns/FRONT_END_PATTERNS.md)** - React/TypeScript implementation patterns, Redux Toolkit conventions, component design, grid usage, and UI library integration with smart-ui-library.
  - _Target Audience:_ Frontend Developers, UI/UX Engineers
  - _Last Updated:_ January 13, 2026

### CI/CD & Testing

- **[technical/ci-cd/SELECTIVE_BACKEND_TEST_EXECUTION.md](technical/ci-cd/SELECTIVE_BACKEND_TEST_EXECUTION.md)** - Strategy for optimizing backend test execution in CI/CD pipeline. Explains selective test execution based on file changes to reduce build times.
  - _Target Audience:_ DevOps Engineers, Backend Developers, CI/CD Maintainers
  - _Last Updated:_ January 13, 2026

### Database Schema

- **[ProfitSharing_Database.png](ProfitSharing_Database.png)** - Entity relationship diagram (ERD) showing database schema structure, relationships, and key entities.
  - _Target Audience:_ Database Administrators, Backend Developers, Data Architects
  - _Last Updated:_ January 13, 2026

---

## Archive

Historical documentation, closed ticket investigations, and legacy materials.

### Closed Ticket Documentation

- **[archive/tickets/](archive/tickets/)** - Documentation for completed tickets that required significant investigation or have historical value
  - [PS-2424/FOLLOW_UP_INVESTIGATION.md](archive/tickets/PS-2424/FOLLOW_UP_INVESTIGATION.md) - Follow-up investigation and findings for PS-2424
  - _Purpose:_ Preserve institutional knowledge from complex tickets
  - _Target Audience:_ Developers researching similar issues

---

## Document Maintenance

### Maintenance Schedule

- **Review Cadence:** Quarterly (March, June, September, December)
- **Last Comprehensive Review:** January 13, 2026
- **Next Scheduled Review:** March 2026

### Ownership & Updates

- **Primary Maintainers:** Engineering Leadership Team
- **Update Process:**
  - Create/update documentation as part of feature work
  - Link from commit messages and PRs
  - Move completed investigations to archive/
  - Update this index when adding new documents

### Contact

- **Questions about technical documentation:** Development Team via Slack #profit-sharing-dev
- **Questions about security documentation:** Security Team Lead
- **Questions about business documentation:** Product Owner / Business Analysts

### Contributing

When adding new documentation:

1. Place documents in appropriate folders based on audience
2. Update this README.md with entry including title, description, target audience, and date
3. Use Markdown format with clear headings and structure
4. Link from related code, PRs, or Jira tickets
5. Move ticket-specific documentation to archive/ when ticket is closed and inactive

---

## Quick Reference

**Looking for:**

- **Security review?** → [SECURITY_POSTURE.md](SECURITY_POSTURE.md)
- **Why did we choose X technology?** → [ARCHITECTURE_DECISIONS.md](ARCHITECTURE_DECISIONS.md)
- **Frontend coding standards?** → [technical/patterns/FRONT_END_PATTERNS.md](technical/patterns/FRONT_END_PATTERNS.md)
- **How do we define "done"?** → [DEFINITION_OF_DONE.md](DEFINITION_OF_DONE.md) or [DEFINITION_OF_DONE_EXECUTIVE.md](DEFINITION_OF_DONE_EXECUTIVE.md)
- **Report migration mapping?** → [BALANCE_REPORTS_CROSS_REFERENCE_MATRIX.md](BALANCE_REPORTS_CROSS_REFERENCE_MATRIX.md)
- **Database schema?** → [ProfitSharing_Database.png](ProfitSharing_Database.png)

---

_Last Updated: January 13, 2026_
