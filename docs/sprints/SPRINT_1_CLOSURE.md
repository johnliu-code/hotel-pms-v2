# Sprint 1 Closure — Core Domain Foundation

## Sprint summary

Sprint 1 established the first executable and tested Domain foundation for Hotel PMS 2.0.

Committed scope:
- HPMS-20 — Implement core domain primitives and shared value types
- HPMS-21 — Implement Property, RoomType, and Room domain model
- HPMS-22 — Implement Customer, Guest, and ReservationGuest domain model
- HPMS-23 — Implement Reservation aggregate and lifecycle invariants

Committed Story Points: 21  
Completed Story Points: 21

Sprint result: Completed

## Completed domain baseline

### Core primitives

Implemented:
- Money
- Currency
- ReservationStatus
- RoomOperationalStatus
- EntryMethod

Key decisions:
- Money preserves exact decimal values and does not impose contextual sign rules.
- Currency uses normalized three-letter codes.
- Reservation lifecycle status is separate from room operational status.
- EntryMethod is provider-neutral: Manual, Import, Integration.

### Property and room inventory

Implemented:
- Property
- RoomType
- Room

Key decisions:
- Property includes Name, TimeZoneId, Currency, active state, and optional address/contact scalar fields.
- RoomType belongs to a Property.
- Room belongs to a Property and RoomType.
- RoomType must belong to the same Property as Room.
- Room/unit codes are strings and support non-numeric accommodation units.
- Occupancy is not stored as Room operational state.
- RoomOperationalStatus currently contains Ready and OutOfService.

### Customer and guest model

Implemented:
- Customer
- Guest
- ReservationGuest

Key decisions:
- Customer represents the booking contact / payer / contact profile.
- Guest represents an actual staying person.
- Customer and Guest remain separate concepts.
- ReservationGuest carries the Guest reference and IsPrimary designation.
- Customer may contain optional address/contact information.
- Guest remains minimal and does not duplicate Customer data.
- Raw payment-card credentials are not stored in these entities.

### Reservation aggregate

Implemented:
- Reservation aggregate
- Date invariants
- Occupancy invariants
- Guest association management
- Lifecycle transitions
- Confirmation-number lifecycle

Reservation relationships:
- Reservation belongs to a Property.
- Reservation references a Customer.
- Reservation books a RoomType, not a physical Room.
- RoomType must belong to the same Property.

Stay rules:
- CheckInDate and CheckOutDate use DateOnly.
- CheckOutDate is exclusive.
- CheckOutDate must be later than CheckInDate.

Occupancy rules:
- Adults >= 1
- Children >= 0
- Adults + Children <= RoomType.OccupancyLimit

Guest rules:
- Multiple ReservationGuest associations are supported.
- At most one primary guest is allowed.
- Zero primary guests is currently allowed.

Lifecycle:
- Draft -> Confirmed
- Draft -> Cancelled
- Confirmed -> CheckedIn
- Confirmed -> Cancelled
- CheckedIn -> CheckedOut

Terminal states:
- CheckedOut
- Cancelled

Confirmation number:
- New reservations begin in Draft with no confirmation number.
- Confirmation number is assigned during Draft -> Confirmed.
- Confirmation number is retained through later lifecycle states.

## Explicitly deferred

The following are intentionally not implemented in Sprint 1:

- Persistent entity IDs
- EF Core mappings
- DbContext
- Database migrations
- Confirmation-number uniqueness enforcement
- Physical RoomAssignment
- Availability engine
- Reservation overlap/concurrency protection
- Audit infrastructure
- NoShow lifecycle state
- Housekeeping workflow states such as Dirty/Cleaning
- Payment processing
- Raw payment-card storage
- API endpoints
- MAUI workflows
- React workflows
- Provider-specific SDK integrations

## Validation baseline

At Sprint 1 completion:

- Release build passed with zero warnings/errors.
- 144 unit tests passed.
- 2 integration tests passed.
- No tests were skipped or failing.
- git diff checks passed before merge.
- HPMS-20 through HPMS-23 were reviewed and merged through pull requests.

## Repository documentation baseline

Authoritative domain decisions are documented in:

- docs/architecture/DOMAIN_DECISIONS.md
- docs/development/DEVELOPMENT_STANDARDS.md
- docs/security/SECURITY_AND_PAYMENT_BOUNDARIES.md

Future implementation must preserve these decisions unless a later Jira story explicitly changes them.

## Sprint conclusion

Sprint 1 successfully converted the Sprint 0 architecture/domain design into tested C# Domain code.

The project is ready to begin persistence work.

Recommended next phase:

Sprint 2 — Persistence Foundation

Primary candidates:
- HPMS-24 — EF Core persistence foundation and initial migration
- HPMS-25 — Core domain and persistence integration test baseline
