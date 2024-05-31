# Demoulas Accounts Receivable project #

## Project Links ##
- [Blue QA](http://appa84d:8080/home.seam?cid=4413)
- [Confluence](https://docs.demoulasmarketbasket.net/pages/viewpage.action?spaceKey=NGDS&title=SMART+Accounts+Receivable)
- [Jira](https://jira.demoulasmarketbasket.net/projects/SAR/issues/SAR-35?filter=allopenissues)
- [Bamboo](https://bamboo.demoulasmarketbasket.net/browse/NGA-SAR0)
- [Stash](https://stash.demoulasmarketbasket.net/projects/NGA/repos/smart-accounts-receivable/browse)


## Definition of Ready ##
Definition of Ready (DoR) Document
Purpose
This defines the prerequisites for any user story, task, or bug fix to be considered ready for development, emphasizing clarity, testability, and quality in our software engineering processes.

Criteria
1. Clearly Defined User Stories
   - Title and Description: Each story must have a clear title and a detailed description of the expected outcome
   - Acceptance Criteria: Comprehensive acceptance criteria outlining the conditions for story completion
   - Auditing requirements, if any
2. UI/UX Designs
   - Design Mockups: Complete UI/UX designs for frontend changes, compatible with React when possible/practical
   - Design Review: Approval by the product owner, UI/UX designer, and stakeholders when possible/practical
3. Dependencies Identified
   - Internal Dependencies: Documentation of dependencies within the project     
   - External Dependencies: Documentation of third-party service dependencies
     - External dependencies should be defined in user tickets and documented in [Confluence](https://docs.demoulasmarketbasket.net/pages/viewpage.action?spaceKey=NGDS&title=SMART+Accounts+Receivable)
4. Data Readiness
   - Test Data: Availability or generation of test data for feature validation
6. Estimation
   - Level of effort: Consensus on the level of effort, (points), estimation by the development team
   - Capacity Planning: Consideration of team capacity and workload for task allocation

## Code Review and Pull Request Guidlines ##

1. Code Correctness and Functionality
   - Verify the code achieves its intended functionality.
   - Check for logic errors, edge cases, and potential bugs.
   - Ensure there are tests covering new changes or functionalities.
2. Code Quality
   - Assess the readability and simplicity of the code.
   - Review naming conventions for consistency and clarity.
   - Consider if the code follows SOLID principles (Single Responsibility, Open-Closed, Liskov Substitution, Interface Segregation, Dependency Inversion).
   - Look for duplicated code and suggest abstractions or refactoring if necessary.
3. Performance
   - Identify any potential performance issues.
   - Suggest optimizations for any inefficient code paths.
   - Review the usage of data structures and algorithms for suitability and efficiency.
4. Security
   - Check for common security vulnerabilities (e.g., SQL injection, cross-site scripting).
   - Ensure sensitive data is properly encrypted and secured.
   - Review authentication and authorization mechanisms for adequacy.
5. Testing
   - Ensure sufficient and meaningful tests are written (unit, integration, end-to-end).
   - Review test coverage for new code and critical paths.
   - Evaluate the use of test doubles (mocks, stubs) for appropriateness and effectiveness.
6. Documentation
   - Check if new methods, classes, or functionalities are properly documented if necessary.
   - Review inline comments for clarity and necessity.
   - Ensure READMEs and any external documentation are updated.
7. Design and Architecture
   - Consider the overall design of the changes within the system's architecture.
   - Assess the scalability and maintainability of the code.
   - Evaluate the use of design patterns and architectural principles.
8. Compatibility and Dependencies
   - Review changes for backward compatibility.
   - Check for dependency updates and their impacts.
   - Ensure compatibility across different environments or platforms.
9. Compliance and Standards
   - Ensure the code adheres to industry standards and best practices.
   - Verify compliance with legal and regulatory requirements if applicable.
10. Team and Project Practices    
   - Review adherence to team or project coding standards and guidelines.
   - Consider the impact on the project timeline or milestones.
   - Encourage knowledge sharing and constructive feedback within the team.

## Project Estimation ##
![Workload](./readme_images/estimation.png)
