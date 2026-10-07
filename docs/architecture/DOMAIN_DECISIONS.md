# Domain decisions

This document records the recovered Sprint 0 domain decisions (HPMS-15/HPMS-16) and the implementation decisions for HPMS-20, HPMS-21, HPMS-22, and HPMS-23. These are authoritative model boundaries; recording a concept does not implement it. Apply the [HPMS-18 development standards](../development/DEVELOPMENT_STANDARDS.md) and [HPMS-19 security/payment boundaries](../security/SECURITY_AND_PAYMENT_BOUNDARIES.md).

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

Supported currencies, currency-specific precision/rounding, and concrete UTC audit field representation/enforcement remain deferred. Do not infer them from these primitives.

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

## HPMS-23 implementation decisions

- Reservations.Reservation is a sealed aggregate with reference identity and no persistent ID. It requires non-null Property, RoomType, and Customer references. RoomType must reference the same Property instance. It books a RoomType, not a physical Room; RoomAssignment remains outside this story.
- CheckInDate and CheckOutDate use System.DateOnly. Checkout is exclusive and must be strictly later than check-in. Stay dates and occupancy are fixed at construction; no amendment workflow is introduced.
- Adults must be at least one; Children must be non-negative; their total must not exceed RoomType.OccupancyLimit. Summation uses a wider integer to prevent overflow bypassing the limit.
- Guest associations are copied from optional construction input and exposed through a read-only collection. AddGuest accepts a non-null HPMS-22 ReservationGuest and rejects a second primary association before mutation. Zero primary guests is permitted. Customer and Guest remain distinct; the aggregate stores references rather than duplicated personal fields.
- No unstated duplicate-guest, guest-count-versus-occupancy, active-property/type, or lifecycle-based guest editing rules are introduced. These require future approved workflow requirements.
- New reservations start in Draft. TransitionTo allows exactly Draft -> Confirmed, Draft -> Cancelled, Confirmed -> CheckedIn, Confirmed -> Cancelled, CheckedIn -> CheckedOut. CheckedOut and Cancelled are terminal. Same-state, undefined-state, and all other transitions throw without changing state. No NoShow value is added.
- New reservations are always Draft with a null confirmation number; the constructor accepts no confirmation number. Draft -> Confirmed requires a non-null/non-blank confirmation number, assigned during that transition. Confirmation input is accepted only when transitioning to Confirmed; later transitions retain it, including cancellation. No sequence, generator, global uniqueness, or persistence enforcement is introduced; those belong to HPMS-24.
- EntryMethod contains exactly Manual, Import, Integration and rejects undefined enum input. Optional BookingChannelCode is a configurable/provider-neutral string independent of EntryMethod; no commercial-provider enum or registry is introduced. Optional Notes is plain text. Optional strings and confirmation numbers are preserved as supplied.
- Cancellation changes lifecycle state and preserves the booking record and associations. No physical deletion behavior is implemented.
- Persistent IDs/materialized-entity equality, confirmation uniqueness, availability, physical assignments, overlap/concurrency protection, and audit infrastructure remain deferred. HPMS-19 audit requirements apply when audit infrastructure is implemented; this slice adds no actor or timestamp fields. No persistence, HTTP/API, UI, provider SDK, or payment implementation/dependencies are introduced.
