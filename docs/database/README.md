# Database direction

A relational SQL database will be selected later. Infrastructure references EF Core without a database provider. There is no DbContext, connection string, schema, migration, or database prerequisite yet. Future persistence integration tests belong in HotelPms.IntegrationTests and should exercise the selected provider. Clients must never access the database directly.
