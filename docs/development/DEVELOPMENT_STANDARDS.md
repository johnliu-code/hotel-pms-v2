# Development standards and Definition of Done

These standards apply to Hotel PMS 2.0 development by humans and AI assistants. They establish the engineering controls for Jira work items, implementation, validation, and review.

## Source-control workflow

- Do not develop features directly on `main`.
- Use Jira-linked branches with the convention `feature/HPMS-<issue>-short-description`, for example `feature/HPMS-18-development-standards`. Use `bugfix/` or `chore/` where appropriate, retaining the Jira key and short description.
- Reference the Jira key in commit messages, for example `HPMS-18: document development standards and Definition of Done`.
- Push the working branch and create a Pull Request (PR) into `main`.
- Do not merge before code review and validation are complete.
- Prefer normal merge commits during the current project phase.
- Delete merged feature branches when they are no longer needed.

## Code and architecture standards

- Use .NET 10 LTS and enable nullable reference types.
- Follow standard C#/.NET naming and formatting conventions. Favor clarity over cleverness.
- Preserve the [architecture dependency boundaries](../architecture/README.md):
  - **Domain:** no dependencies on Application, Infrastructure, API, MAUI, Web, EF Core, or database-provider-specific code.
  - **Application:** may depend on Domain.
  - **Infrastructure:** implements persistence and external technical concerns required by Application/Domain.
  - **API:** backend composition root and API entry point.
  - **MAUI/Web:** communicate through HTTP APIs only and never access the database directly.
- Keep provider-specific integration dependencies outside Domain.
- Do not over-engineer or introduce abstractions without a concrete requirement.
- Never commit production secrets, credentials, certificates, signing material, raw payment-card data, or CVV.

## Testing standards

- Testing must be proportional to the change.
- Domain rules and Application use cases require meaningful unit tests.
- API, persistence, infrastructure, and cross-boundary behavior require meaningful integration tests.
- Reservation inventory and concurrency rules require explicit integrity/concurrency tests when implemented.
- Bug fixes should include regression tests where reasonably possible.
- Relevant tests must pass before completion. Do not create meaningless tests merely to increase test count.
- Run relevant client build, lint, and test checks where the environment supports them.
- Explicitly report unavailable workloads or native SDK limitations rather than claiming successful validation.

Use the [root README](../../README.md) for the repository's build, test, and client validation commands.

## Pull Request and review standards

Each PR must:

- Reference its Jira issue.
- Summarize its purpose and material changes.
- Report build, test, and lint results and known environment limitations, including checks that could not run.
- Remain focused on the Jira work item.
- Contain no secrets, generated build output, or unrelated files.
- Receive code review before merge.

Review must consider:

- Acceptance criteria.
- Architecture and dependency boundaries.
- Correctness and edge cases.
- Tests and failure behavior.
- Security and privacy.
- Readability and maintainability.
- Documentation impact.

## AI-assisted development

AI/Codex-generated code follows exactly the same engineering controls as human-written code. The required workflow is:

Jira Story → feature branch → AI/human implementation → developer understanding/review → build/test → commit → push → Pull Request → code review → merge → Jira Done

AI-generated code must be inspected, understood, tested, and reviewed. It must not be merged directly into `main`.

## Documentation standard

Update `README.md` or `docs/` when changes affect architecture, setup, workflow, API contracts, data-model decisions, security boundaries, or developer onboarding.

## Definition of Done

A Jira Story/Task is Done only when all applicable conditions are satisfied:

- [ ] Acceptance criteria are implemented.
- [ ] Architecture and dependency boundaries are respected.
- [ ] There are no known blocking defects.
- [ ] Relevant tests are added or updated.
- [ ] Relevant build, test, and lint checks pass.
- [ ] Validation limitations are explicitly documented.
- [ ] Security and privacy are considered.
- [ ] Required documentation is updated.
- [ ] No secrets or prohibited payment data are committed.
- [ ] Changes are committed and pushed on a Jira-linked branch.
- [ ] The PR references the Jira issue.
- [ ] Code review is completed.
- [ ] Review findings and conflicts are resolved.
- [ ] The PR is merged into `main`.
- [ ] Jira/GitHub development linkage is visible where available.
- [ ] Jira is transitioned to Done only after merge and final verification.

For documentation-only or analysis-only work, code, build, and test requirements apply only when relevant.
