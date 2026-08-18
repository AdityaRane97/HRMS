# ADR-002: Cloud-Neutral Service Abstractions Strategy

## Status
ACCEPTED

## Context
The HRMS enterprise application requires integration with multiple cloud and third-party services:
- **Identity & Authentication**: Azure AD, Okta, or custom OAuth providers
- **File Storage**: Azure Blob Storage, AWS S3, or GCS
- **Messaging**: Azure Service Bus, AWS SQS/SNS, or RabbitMQ
- **Caching**: Redis, Azure Cache for Redis, or in-memory

The application must remain cloud-agnostic and not be locked into a single vendor. This enables:
1. Cost optimization by comparing vendors
2. Flexibility to migrate between providers
3. Support for client-specific infrastructure choices

## Decision
Implement a **cloud-neutral abstraction layer** using the **Repository/Strategy pattern** over explicit service interfaces:

1. **Core Abstractions** (in `HRMS.Application/Contracts/`):
   - `IIdentityProvider`: SSO and external user authentication
   - `IAuthenticationService`: JWT and token lifecycle
   - `IAuthorizationService`: RBAC and permission checks
   - `IFileStorage`: Secure document upload/download/delete
   - `ICacheService`: Optional distributed caching
   - `IMessageBus`: Async publish-subscribe messaging

2. **Phase 1 In-Memory Implementations** (in `HRMS.Infrastructure/Services/`):
   - `InMemoryAuthorizationService`
   - `InMemoryCacheService`
   - `InMemoryFileStorage`

3. **Phase 2+ Concrete Implementations**:
   - `AzureAdIdentityProvider`
   - `AzureAppConfigCacheService`
   - `AzureBlobStorageService`
   - `AzureServiceBusMessageBus`
   - (And AWS/GCP alternatives as needed)

## Rationale
- **Vendor Independence**: No dependency on specific cloud SDK until implementation time
- **Testability**: Easy to mock and unit test with in-memory stubs
- **Gradual Adoption**: Client-specific providers can be added incrementally
- **Cost Optimization**: Enables competitive evaluation of vendors
- **Team Flexibility**: Different teams can implement different providers

## Consequences
- Additional abstraction layer adds slight complexity upfront
- Every cloud-dependent feature must have a corresponding interface contract
- Runtime implementation choice must be externalized to configuration
- Future migrations require new implementation but no contract changes

## Implementation Notes
- All abstractions include comprehensive XML documentation
- Return types use DTOs (results/options) to avoid framework dependencies
- Cancellation tokens supported for async operations
- All exceptions mapped to `HrmsException` hierarchy for consistency
