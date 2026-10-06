# Domain decisions

This document records the recovered Sprint 0 domain decisions (HPMS-15/HPMS-16) and the implementation decisions for HPMS-20. These are authoritative model boundaries; recording a concept does not implement it. Apply the [HPMS-18 development standards](../development/DEVELOPMENT_STANDARDS.md) and [HPMS-19 security/payment boundaries](../security/SECURITY_AND_PAYMENT_BOUNDARIES.md).

## Recovered Sprint 0 decisions

- Customer is the reservation contact/customer profile. Guest is an actual occupant. ReservationGuest associates guests with a reservation; these remain separate concepts.
- A Reservation is made against a RoomType. A specific physical Room is associated through Reservation → RoomAssignment → Room.
- Reservation lifecycle includes Draft, Confirmed, CheckedIn, CheckedOut, and Cancelled. Cancellation retains the reservation as a business record rather than deleting it.
- Reservation lifecycle status and physical room operational status are separate concepts.
- Hotel stay dates use `System.DateOnly`. Audit timestamps use UTC semantics. Do not introduce date/time wrappers.
- Availability is server-authoritative. Valid RoomAssignments must eventually prevent overlapping stays for the same Room through server/database transaction and concurrency controls. HPMS-20 implements no availability or concurrency behavior.
- BookingChannel, SourceCategory, EntryMethod, and IntegrationProvider are distinct concepts. They are outside HPMS-20.

## HPMS-20 implementation decisions

- `Reservations.ReservationStatus` defines the five lifecycle values above. No transition rules are introduced by the enum.
- `Rooms.RoomOperationalStatus` contains only Ready and OutOfService. Occupied is not a physical operational status; occupancy will be derived from reservation/room-assignment data. Dirty/Cleaning and other housekeeping workflows are deferred.
- `Financials.Money` is an immutable value consisting only of a decimal Amount and a non-null Currency. Equality includes both values. It preserves the supplied decimal without rounding or a sign restriction; use cases may impose their own amount constraints later.
- `Financials.Currency` is an immutable three-ASCII-letter code normalized to uppercase with invariant casing. Null, whitespace, digits, punctuation, and non-ASCII letters are rejected. Equality uses the normalized code. Validation checks ISO 4217-style syntax only; it does not verify membership in an ISO registry or restrict the application's supported currencies.
- Money/Currency are sealed records with get-only properties. They expose no arithmetic, conversion, exchange rates, payment processing, or provider-specific behavior.
- No entity or audit fields are added in this story. Future stay fields must use DateOnly, and future audit fields must enforce UTC. The choice of audit timestamp storage type and enforcement belongs to the slice that introduces those fields.
- Domain remains independent of EF Core, HTTP, UI, payment providers, and external SDKs. HPMS-20 adds no dependencies.

## Deferred questions

Money itself permits negative, zero, and positive decimal amounts; this sign behavior is decided by HPMS-20. Whether a specific domain use such as a room rate, deposit, refund, or adjustment permits negative values is a contextual invariant to be defined by the story introducing that concept.

Supported currencies, currency-specific precision/rounding, reservation transition rules, and concrete UTC audit field representation/enforcement remain deferred. Do not infer them from these primitives.
