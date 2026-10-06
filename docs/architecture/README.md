# Architecture

Hotel PMS 2.0 is an API-first modular monolith. Future business modules share one backend deployment while preserving domain boundaries; module implementations and cross-module contracts will be introduced with actual requirements.

Dependency direction:

- Domain: independent business model; no EF Core, provider, client, or outer-layer references.
- Application → Domain: use cases and ports as requirements emerge.
- Infrastructure → Application and Domain: persistence and external-system adapters.
- API → Application and Infrastructure: HTTP/JSON entry point and composition root.
- MAUI and Web → HTTP APIs only: no backend project references or direct database access.

Booking platforms, channel managers, and external PMS integrations belong behind application ports with infrastructure adapters. Translate external payloads at the boundary; keep vendor SDKs, schemas, and identifiers out of the core domain. No provider adapters or speculative abstractions are implemented in this skeleton.
