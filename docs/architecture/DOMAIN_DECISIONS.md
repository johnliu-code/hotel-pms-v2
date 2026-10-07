# Domain decisions

This document records the recovered Sprint 0 domain decisions (HPMS-15/HPMS-16) and the implementation decisions for HPMS-20, HPMS-21, and HPMS-22. These are authoritative model boundaries; recording a concept does not implement it. Apply the [HPMS-18 development standards](../development/DEVELOPMENT_STANDARDS.md) and [HPMS-19 security/payment boundaries](../security/SECURITY_AND_PAYMENT_BOUNDARIES.md).

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

## HPMS-21 implementation decisions

- Property, RoomType, and Room are sealed classes with constructor validation and get-only fields. They use reference identity. Persistent identifiers and equality across materialized entities are deferred to the HPMS-24 persistence slice; no audit fields or mutation workflows are introduced.
- Property requires Name, string TimeZoneId, and the existing non-null HPMS-20 Currency value, plus explicit IsActive.
- TimeZoneId rejects null/blank values only and otherwise preserves the supplied string. No TimeZone value object, registry lookup, timezone conversion, DST handling, or external timezone service is introduced. Concrete registry validation is deferred until timezone behavior is implemented.
- Property address fields are optional scalar strings: AddressLine1, AddressLine2, City, Region, PostalCode, CountryCode. No Address value object, country/region registry, or provider-specific address rules are introduced.
- Property contact fields are optional scalar strings: Phone and Email. No ContactInfo value object or complex format validation is introduced. Optional address/contact values default to null and are preserved as supplied.
- RoomType references a non-null Property, requires Code and Name and a positive OccupancyLimit, and accepts optional Description and explicit IsActive.
- Room references a non-null Property and RoomType. Its Property must be the same instance as RoomType.Property; database-style ID comparison is outside HPMS-21.
- Room codes are required strings, preserved as supplied, supporting numeric-looking codes, letters, cabins and chalets. No uniqueness, casing, numeric parsing, or normalization policy is introduced. Optional Floor is a text label; Notes is optional text.
- Room accepts only defined HPMS-20 RoomOperationalStatus values (Ready and OutOfService). IsActive is independent of operational status. No occupancy, availability, housekeeping, or status-transition behavior is added.
- Required names/codes reject null or whitespace. Optional text has no additional formatting rules. Active state and operational status are supplied explicitly rather than choosing business defaults.
- Domain remains persistence-, HTTP-, UI-, and provider-neutral with no additional dependencies.

## HPMS-22 implementation decisions

- Customer is the booking contact / payer / contact profile; Guest is an actual staying person. They are separate sealed classes with get-only fields and reference identity, without inheritance or automatic linking.
- The recovered model specifies roles but not scalar field lists. HPMS-22 uses a minimal implementation choice: Customer requires Name and has optional scalar Phone and Email, plus approved optional scalar AddressLine1, AddressLine2, City, Region, PostalCode, and CountryCode; Guest requires only Name. Names are single strings, permitting single names, international names, and booking-contact organization names without a first/last-name assumption.
- Name rejects null/blank input and otherwise is preserved as supplied. Customer address/contact fields default to null and are preserved as supplied without address format or complex phone/email validation. Region is country-neutral; no Province/State fields, Address value object, country registry, or postal-code registry is introduced. Guest does not duplicate Customer address/contact fields.
- Reservations.ReservationGuest holds a non-null Guest reference and an explicitly supplied IsPrimary designation. It stores no duplicated guest personal data. The designation belongs to the association, not to Guest globally.
- HPMS-22 adds no Reservation aggregate, reservation reference/identifier, collection management, or lifecycle behavior. ReservationGuest is an association to be composed within the reservation context by HPMS-23. Reservation-wide uniqueness (including at most one primary guest) belongs to HPMS-23, not to individual associations.
- Persistent IDs and equality across materialized entities remain deferred to HPMS-24. No audit fields, active-state rules, or mutation workflows are added here.
- Following HPMS-19 data minimization, no birth dates, identity documents, passport/government identifiers, demographic data, free-form personal notes, or raw payment-card credentials are introduced. Full card numbers, CVV/security codes, and similar sensitive payment credentials must not be stored on Customer, Guest, or ReservationGuest. A later payment-domain story may store provider-issued payment references/tokens and justified non-sensitive card metadata such as card brand or last four digits, subject to HPMS-19. No personal data is added to diagnostics or exception messages.
- Domain remains persistence-, UI-, HTTP-, and provider-neutral with no new dependencies. Any further personal/contact fields, format rules, retention policies, and deletion rules require a validated workflow and explicit requirements.
