# Unit tests

Focused Domain tests cover Money/Currency validation, normalization, equality, hashing, and exact decimal preservation. Tests also cover Property/RoomType/Room invariants and HPMS-22 Customer/Guest name validation, optional booking-contact fields, reference identity, and ReservationGuest reference/primary designation. Reservation-wide primary-guest uniqueness remains deferred to HPMS-23. Add meaningful tests alongside future use cases and domain rules.

From the repository root, run `dotnet test tests/HotelPms.UnitTests --configuration Release`.
