# HTTP API

The API exposes HTTP/JSON with ASP.NET Core OpenAPI. In Development, GET /openapi/v1.json provides the generated specification. GET /health returns {"status":"Healthy"}; this checks process availability, not database readiness. No business endpoints exist yet.

Production OpenAPI is disabled. Browser authentication, CORS policy, and integration contracts will be selected with real requirements rather than permissive defaults. Clients communicate only through the API.
