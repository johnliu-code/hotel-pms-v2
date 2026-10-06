# Security and payment-data boundaries

HPMS-19 defines the Hotel PMS 2.0 security baseline for future implementation. These are requirements, not a claim that controls are already implemented. The current solution is a foundation with no authentication or business endpoints. Follow the [architecture boundaries](../architecture/README.md) and [development standards and Definition of Done](../development/DEVELOPMENT_STANDARDS.md) when implementing this baseline.

## 1. Security trust boundary

The ASP.NET Core backend API is the authoritative security boundary. MAUI and React clients are untrusted callers from the server perspective. Client-side visibility, disabled controls, or cached permissions must never substitute for backend authorization. Every protected operation must be authorized by the backend, regardless of how it is invoked.

External PMS systems, booking platforms, payment providers, and future integrations must use explicit Application/API contracts. They must not access the authoritative database directly or bypass backend security and business rules.

## 2. Authentication and authorization

Use established ASP.NET Core authentication and security mechanisms. Do not implement custom password hashing, credential storage, or authentication protocols.

Authentication establishes the caller's identity; authorization determines which operations that identity may perform. Keep these concerns separate and enforce permissions server-side using least privilege. Security-sensitive operations should be attributable to the authenticated user.

The MVP roles are:

- **Owner/Admin:** administrative and operational permissions required by validated workflows.
- **Front Desk:** only the operational permissions required for front-desk workflows.

Role names alone do not grant access: each protected use case must define and enforce its permissions. Preserve extensibility for Manager, Housekeeping, Maintenance, and Read-only roles without relying on client checks or assuming that every staff member has administrative access.

## 3. Customer and Guest data

Customer and Guest information is protected operational/personal data. Apply data minimization: collect only information required for validated hotel workflows, business requirements, and applicable legal requirements. Avoid unnecessary duplication of personal data across records, clients, and integrations.

Do not expose sensitive Customer/Guest information in logs, diagnostics, URLs, exception messages, or client telemetry. Return protected information only when the authorized workflow needs it. Detailed jurisdiction-specific retention and deletion rules require later validation; this baseline does not establish a retention schedule or permission to retain data indefinitely.

## 4. Payment-data boundary

Hotel PMS manages payment business state, not sensitive card credentials. The PMS must never store or intentionally log:

- Full payment-card numbers (PAN).
- CVV/CVC/security codes.
- PINs.
- Magnetic-stripe or track data.
- Raw payment credentials or equivalent sensitive authentication data.

This prohibition applies to persistence, caches, logs, diagnostics, telemetry, and audit records. Do not introduce these fields into PMS data models or API contracts.

When required by a validated workflow, the PMS may store non-sensitive payment/business metadata:

- Amount and currency.
- Payment method category and payment status.
- External transaction/reference identifier and payment provider identifier.
- Timestamps.
- Provider-safe display metadata when explicitly allowed by the provider and the agreed security design.

Do not assume an arbitrary provider payload, reference, or token is safe to persist or log. Keep stored metadata limited to the approved business purpose; credentials remain secrets.

Provider-hosted/tokenized processing is the preferred future integration approach so PCI-sensitive payment data remains outside the Hotel PMS trust boundary whenever possible. Sensitive card capture should occur through the provider's supported flow rather than PMS-owned forms or backend payloads. Tokenization does not by itself establish PCI compliance.

Keep Domain payment-provider neutral. Stripe, Moneris, Square, POS, and other provider-specific SDKs/models must remain outside the core Domain. Translate provider details through Infrastructure adapters and Application contracts; Domain expresses payment business state rather than vendor-specific processing models.

## 5. Secrets and environment configuration

Never commit production or reusable secrets, including:

- Database credentials.
- API keys.
- Authentication/signing secrets.
- Payment-provider credentials.
- Private certificates or keys.
- Production connection strings containing credentials.

Development, test, staging, and production configuration must remain logically separated. Use supported local secret/environment configuration for development, such as ASP.NET Core user secrets, and deployment-platform secret management for hosted environments. Keep server and integration credentials out of client bundles and client configuration. Repository examples/templates must contain placeholders only, never working credentials.

## 6. Transport, API, and validation

Production traffic containing authentication, personal, or business data must use HTTPS. Local development profiles do not establish production transport security.

Backend APIs must validate inputs and enforce authorization independently of clients. Use safe error handling: API responses must not expose internal exception details or secrets. Operational diagnostics must also respect the logging restrictions below.

CORS, token/session settings, cookie settings where applicable, and deployment security controls must be explicitly configured and validated rather than assumed. CORS is a browser access control and does not replace backend authorization.

## 7. Audit and logging

Important actions should be auditable, including:

- Reservation lifecycle changes.
- Check-in and check-out.
- Room-status changes.
- Cancellation.
- Important payment-state changes.
- Security/permission-sensitive operations.

Audit information should capture an appropriate actor, action, target/reference, and timestamp. Associate user-initiated sensitive actions with the authenticated user; distinguish authenticated integration actors where relevant. Audit records must not become a duplicate sensitive-data repository: use references and the minimum information needed to understand the action.

Application logs must not contain secrets, authentication tokens, raw card data, or unnecessary personal information. Apply these restrictions to request/response logging, exception handling, integration diagnostics, and client telemetry as well as ordinary application messages.

## 8. Integration and offline boundaries

Future PMS, channel, and payment integrations must use provider-neutral adapters and Application use cases rather than bypassing security or business rules. Integration credentials are secrets and must follow the configuration rules above.

External events and commands must support appropriate authentication, validation/authorization, idempotency, auditability, and failure handling. External input is untrusted; duplicate delivery, retries, and failures must not bypass reservation integrity or incorrectly repeat payment-state changes.

Future MAUI offline capability must not weaken authentication, authorization, data protection, reservation integrity, or payment boundaries. Any local cache containing protected information requires an explicit security design before implementation, including access protection, data minimization, and safe synchronization with backend authorization and integrity checks. Offline UI state must not grant authority over server operations.

## 9. MVP security scope

The baseline requirements above apply as relevant features are implemented: backend authorization, established authentication mechanisms, least privilege, protected personal data, prohibited card-data storage/logging, secret separation, HTTPS, input validation, safe errors, and appropriate auditability. They are not optional because advanced controls are deferred.

Detailed decisions deferred until implementation/deployment requirements justify them include:

- Payment-provider selection.
- PCI validation level.
- Jurisdiction-specific retention/deletion rules.
- Advanced threat detection.
- Single sign-on (SSO).
- Multi-factor authentication (MFA) policy.
- Enterprise key management.
- Penetration testing.

The architecture must remain compatible with adding these controls later. Reassess the required controls before deploying affected workflows; deferral is not an exemption from applicable obligations. This documentation does not make Hotel PMS PCI compliant or fully compliant with any specific privacy regime.
