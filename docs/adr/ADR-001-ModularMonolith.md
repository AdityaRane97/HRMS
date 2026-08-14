# ADR-001: Modular Monolith Architecture

Status: Accepted

Context:

We require a system that is modular, maintainable, and able to evolve into microservices if necessary. Initial deployment should be a modular monolith.

Decision:

Use a modular monolith with clear module boundaries and Clean Architecture layers (API, Application, Domain, Infrastructure).

Consequences:

- Simpler deployment initially
- Stronger internal module boundaries enable future extraction
- Avoids distributed complexity early
