# Phase 2: Identity & Employee Module Expansion

**Status**: Ready to Start  
**Estimated Duration**: 4-6 weeks  
**Dependencies**: Phase 1 ✅ (v1.0.0-alpha)  
**Target Release**: v1.1.0-beta

---

## Phase 2 Objectives

Phase 2 focuses on **enterprise authentication, authorization, and employee module completeness**, moving from in-memory stubs to production-grade implementations.

### Core Goals
1. ✅ Implement authentication pipeline (JWT tokens, refresh tokens, SSO integration)
2. ✅ Database-backed authorization (role/permission management APIs)
3. ✅ Secure document management (Azure Blob Storage or AWS S3 integration)
4. ✅ Real caching layer (Redis integration with invalidation)
5. ✅ Employee module expansion (payroll, attendance, leave)
6. ✅ API security hardening (rate limiting, CORS, input sanitization)
7. ✅ Integration testing (end-to-end workflows, database tests)
8. ✅ Performance optimization (query optimization, caching strategies)

---

## High-Level Architecture Changes

### Phase 1 → Phase 2 Comparison

| Component | Phase 1 | Phase 2 |
|-----------|---------|---------|
| **Authentication** | Placeholder (ICurrentUser) | JWT Pipeline + SSO (Azure AD/Okta) |
| **Authorization** | InMemoryAuthorizationService | EF-based roles/permissions with caching |
| **File Storage** | InMemoryFileStorage | Azure Blob Storage (or AWS S3/GCS) |
| **Caching** | InMemoryCacheService | Redis with TTL & invalidation |
| **Database** | Core entities only | Core + domain expansion (payroll, attendance) |
| **Testing** | Unit (29 tests) | Unit + Integration + E2E |
| **API Security** | Error handling only | Rate limiting + CORS + Signing |
| **Documentation** | ADRs (6) | Additional Phase 2 ADRs (ADR-007+) |

---

## Phase 2 Detailed Workstreams

### 1. Authentication Pipeline (Week 1-2)

**Goal**: Enterprise-grade authentication with JWT and SSO support.

**Tasks**:
- [ ] Create `AuthenticationController` (POST /auth/login, POST /auth/refresh, POST /auth/logout)
- [ ] Implement `JwtTokenService` (generate, validate, refresh tokens)
- [ ] Create `AuthenticationService` implementation (JWT lifecycle)
- [ ] Implement `AzureAdIdentityProvider` for SSO
  - Azure AD OAuth2 integration
  - External user info mapping to Employee
  - Automatic employee creation on first login
- [ ] Add JWT middleware to Program.cs
- [ ] Create `LoginDto`, `TokenResponse`, `RefreshTokenRequest` DTOs
- [ ] Add test cases for token generation, validation, refresh, expiry
- [ ] Update Swagger/OpenAPI to show auth endpoints
- [ ] Create ADR-007: JWT & OAuth2 Strategy

**Deliverables**:
- ✅ Login/logout endpoints functional
- ✅ Token refresh working
- ✅ Azure AD integration option ready
- ✅ Tests for auth pipeline (>80% coverage)

---

### 2. Database-Backed Authorization (Week 2-3)

**Goal**: Move authorization from in-memory to EF Core with permission APIs.

**Tasks**:
- [ ] Create `RoleController` (CRUD roles, assign permissions)
- [ ] Create `PermissionController` (list permissions)
- [ ] Implement `EfAuthorizationService` (database-backed)
  - Replace `InMemoryAuthorizationService`
  - Cache roles/permissions in Redis
  - Support dynamic role assignment
- [ ] Add `UserRole` APIs for admin role management
- [ ] Migrate existing in-memory test data to seed method
- [ ] Create integration tests for authorization scenarios
- [ ] Add audit logging for role/permission changes
- [ ] Create ADR-008: RBAC Management & Caching Strategy

**Deliverables**:
- ✅ Role/permission management endpoints
- ✅ Database-backed RBAC functional
- ✅ Redis caching reducing database hits
- ✅ Integration tests for auth scenarios

---

### 3. Document Management Integration (Week 3-4)

**Goal**: Secure file upload/download for HR documents (offers, contracts, performance reviews).

**Tasks**:
- [ ] Implement `AzureBlobStorageService` (or AWS S3 alternative)
  - Container strategy (org/employee/year)
  - Secure SAS URL generation with time-limited access
  - Virus scanning integration (optional Phase 2+)
  - Content type validation
- [ ] Create `DocumentController` (POST upload, GET download, DELETE)
- [ ] Add document metadata entity (`Document` table)
- [ ] Implement document versioning (keep previous versions)
- [ ] Add access control (who can see which documents)
- [ ] Create integration tests with mock/fake Blob Storage
- [ ] Add file size/type validation
- [ ] Create ADR-009: Document Storage & Access Control Strategy

**Deliverables**:
- ✅ File upload/download secure and working
- ✅ Time-limited URLs generated correctly
- ✅ Document access control enforced
- ✅ Audit trail for document access

---

### 4. Real Caching Layer (Week 3)

**Goal**: Redis integration for performance at scale (1k-10k concurrent users).

**Tasks**:
- [ ] Add StackExchange.Redis NuGet package
- [ ] Implement `RedisCacheService` (replaces in-memory)
- [ ] Configure Redis connection pooling
- [ ] Implement cache Key naming strategy
- [ ] Add cache invalidation patterns
  - User logout → clear user permissions
  - Role change → clear affected users' permissions
  - Permission change → invalidate role cache
- [ ] Implement distributed cache decorators
- [ ] Add cache statistics/monitoring endpoints
- [ ] Create integration tests with TestContainers.Redis
- [ ] Add performance baseline tests

**Deliverables**:
- ✅ Redis cache working for sessions, roles, permissions
- ✅ Cache-aside pattern implemented
- ✅ Invalidation strategies working
- ✅ Performance metrics (hit rate, latency) exposed

---

### 5. Employee Module Expansion (Week 4-5)

**Goal**: Add employee-adjacent features (payroll, attendance, leave) to foundation.

**Tasks**:
- [ ] Create domain entities: `Payroll`, `Attendance`, `LeaveRequest`
- [ ] Create `PayrollController` (GET monthly payroll, audit trail)
- [ ] Create `AttendanceController` (POST check-in/out, GET attendance log)
- [ ] Create `LeaveController` (POST leave request, GET balance, manager approval)
- [ ] Implement approval workflow for leave/payroll
- [ ] Add DTOs and validators for new entities
- [ ] Add soft-delete and audit logging for all
- [ ] Create complex authorization checks (manager approval access)
- [ ] Add integration tests for payroll/attendance/leave workflows
- [ ] Create ADR-010: Employee Expansion Module Architecture

**Deliverables**:
- ✅ Payroll, attendance, leave entities in database
- ✅ APIs exposed with proper RBAC
- ✅ Approval workflows functional
- ✅ Tests covering happy path + edge cases

---

### 6. API Security Hardening (Week 5)

**Goal**: Protect API against common attacks and overload.

**Tasks**:
- [ ] Add rate limiting middleware
  - Per-user limits (100 req/minute)
  - Per-IP limits (1000 req/minute)
  - Configurable via IConfiguration
- [ ] Add CORS policy refinement
  - Only allow specified origins
  - Allow only safe methods (GET, POST)
  - Exclude sensitive headers
- [ ] Add input sanitization
  - SQL injection prevention (via EF parameterization ✅)
  - XSS prevention in error messages
- [ ] Add request signing (optional, if client requires mutual TLS)
- [ ] Add API versioning header
- [ ] Add security headers (HSTS, CSP, X-Frame-Options)
- [ ] Create load testing to validate rate limits
- [ ] Create ADR-011: API Security & Rate Limiting

**Deliverables**:
- ✅ Rate limiting working and enforceable
- ✅ Security headers in all responses
- ✅ CORS correctly configured
- ✅ Security load tests passing

---

### 7. Integration Testing (Weeks 4-5)

**Goal**: Comprehensive end-to-end test coverage for major workflows.

**Tasks**:
- [ ] Create `HRMS.IntegrationTests` project (xUnit + TestContainers)
- [ ] Add database fixture (ephemeral SQL Server container)
- [ ] Add authentication flow tests
  - Login → get token → use token → logout
  - Token refresh → new token works
  - Invalid token → 401 Unauthorized
- [ ] Add authorization flow tests
  - Employee accesses own profile → ✅
  - Employee accesses other profile → ❌
  - Manager accesses direct report → ✅
  - HR accesses any employee → ✅
- [ ] Add document workflow tests
  - Upload → download → verify content
  - Non-owner access → forbidden
  - Expired URL → 404
- [ ] Add leave approval workflow tests
  - Employee requests leave → pending
  - Manager approves → approved
  - Approval emails sent → verified (mock)
- [ ] Add stress tests (concurrent requests, database contention)
- [ ] Add data cleanup between tests (rollback transactions)

**Deliverables**:
- ✅ Integration test suite (50+ tests)
- ✅ >80% code coverage including Auth/Authorization
- ✅ Major workflows validated end-to-end

---

### 8. Performance Optimization (Week 5-6)

**Goal**: Achieve 1k-10k concurrent user target with <200ms p95 latency.

**Tasks**:
- [ ] Query optimization
  - Add missing indexes on frequently-queried fields
  - Implement query batching for reports
  - Use `AsNoTracking()` for read-heavy endpoints
  - Profile N+1 queries with EF Profiler
- [ ] Caching strategy
  - Cache employee list (5min TTL)
  - Cache role/permission lists (10min TTL)
  - Cache organization hierarchy (24hr TTL)
- [ ] Connection pooling
  - Configure EF Core pool size (10-20)
  - Monitor connection pool metrics
- [ ] Async/await everywhere
  - No blocking calls (.Result, .Wait())
  - Measure improvement vs Phase 1
- [ ] Add Application Insights logging
  - Track slow queries (>500ms)
  - Track failed requests
  - Monitor memory/CPU utilization
- [ ] Load test with k6 or JMeter
  - Simulate 1k concurrent users
  - Measure p95, p99 latency
  - Identify bottlenecks
- [ ] Create ADR-012: Performance & Scalability Strategy

**Deliverables**:
- ✅ <200ms p95 latency at 1k concurrent users
- ✅ Identified and optimized N+1 queries
- ✅ Cache hit rate >80% for frequently-accessed data
- ✅ Performance baseline established for Phase 3

---

## New Entities & Migrations

**Phase 2 Database Schema Additions**:

```sql
-- User Authentication
CREATE TABLE UserLogins (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  UserId UNIQUEIDENTIFIER NOT NULL,
  Provider VARCHAR(50) NOT NULL, -- 'AzureAd', 'Custom'
  ProviderKey VARCHAR(200) NOT NULL,
  LastLoginAt DATETIME2,
  CreatedAt DATETIME2 NOT NULL
)

--Document Management
CREATE TABLE Documents (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  EmployeeId UNIQUEIDENTIFIER NOT NULL,
  DocumentType VARCHAR(50) NOT NULL, -- 'Offer', 'Contract', 'Review'
  FileName VARCHAR(255) NOT NULL,
  BlobUri VARCHAR(1000) NOT NULL,
  FileSize BIGINT NOT NULL,
  ContentType VARCHAR(100),
  CreatedAt DATETIME2 NOT NULL,
  CreatedBy VARCHAR(255),
  IsDeleted BIT
)

-- Payroll
CREATE TABLE Payrolls (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  EmployeeId UNIQUEIDENTIFIER NOT NULL,
  PayMonth DATE NOT NULL,
  BaseSalary DECIMAL(10,2) NOT NULL,
  Deductions DECIMAL(10,2),
  NetSalary DECIMAL(10,2),
  Status VARCHAR(20) DEFAULT 'Pending', -- Pending, Approved, Paid
  ApprovedBy UNIQUEIDENTIFIER,
  ApprovedAt DATETIME2,
  CreatedAt DATETIME2 NOT NULL
)

-- Attendance
CREATE TABLE Attendances (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  EmployeeId UNIQUEIDENTIFIER NOT NULL,
  CheckInTime DATETIME2 NOT NULL,
  CheckOutTime DATETIME2,
  WorkHours DECIMAL(5,2),
  Status VARCHAR(20) DEFAULT 'Present', -- Present, Absent, OnLeave
  CreatedAt DATETIME2 NOT NULL
)

-- Leave Management
CREATE TABLE LeaveRequests (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  EmployeeId UNIQUEIDENTIFIER NOT NULL,
  LeaveType VARCHAR(50) NOT NULL, -- 'Vacation', 'Sick', 'Personal'
  StartDate DATE NOT NULL,
  EndDate DATE NOT NULL,
  Reason TEXT,
  Status VARCHAR(20) DEFAULT 'Pending', -- Pending, Approved, Rejected
  ApprovedBy UNIQUEIDENTIFIER,
  ApprovedAt DATETIME2,
  CreatedAt DATETIME2 NOT NULL,
  IsDeleted BIT
)
```

---

## Technology Additions (Phase 2)

| Component | Technology | Version | Purpose |
|-----------|-----------|---------|---------|
| **Caching** | StackExchange.Redis | 2.6.0 | Distributed cache |
| **Cloud Storage** | Azure.Storage.Blobs | 12.0+ | File storage |
| **Azure Identity** | Azure.Identity | 1.10+ | MSI authentication |
| **JWT Handling** | System.IdentityModel.Tokens.Jwt | 7.0+ | Token validation |
| **Testing** | TestContainers | 3.0+ | Container-based test fixtures |
| **Load Testing** | k6 (or JMeter) | Latest | Performance testing |
| **Monitoring** | Application Insights | SDK 2.20+ | APM and logging |

---

## Success Criteria

### Functional
- ✅ JWT authentication pipeline working (login, token refresh, logout)
- ✅ Azure AD SSO integration optional but ready
- ✅ Role/permission management APIs operational
- ✅ Employee, payroll, attendance, leave modules complete
- ✅ File upload/download secure and functional
- ✅ Rate limiting enforced

### Testing
- ✅ Unit test coverage >80% (Phase 1 + Phase 2)
- ✅ Integration tests for major workflows
- ✅ E2E tests covering auth → employee access → document download
- ✅ Load tests validating 1k-10k concurrent users

### Performance
- ✅ API response <200ms p95 latency at 1k concurrency
- ✅ Database queries optimized (no N+1)
- ✅ Cache hit rate >80% for frequently-accessed data
- ✅ Memory/CPU stable under load

### Security & Compliance
- ✅ All endpoints require authentication
- ✅ RBAC enforced on all sensitive endpoints
- ✅ Audit logging for sensitive operations
- ✅ GDPR right-to-deletion implemented
- ✅ Security headers in all responses

### Documentation
- ✅ ADRs for new decisions (ADR-007 through ADR-012)
- ✅ API documentation updated (Swagger)
- ✅ Deployment guide for Phase 2
- ✅ Team onboarding guide

---

## Risk Mitigation

| Risk | Impact | Mitigation |
|------|--------|-----------|
| Azure AD integration delays | High | Plan fallback to custom OAuth2 |
| Redis unavailability crashes app | High | Implement circuit breaker pattern |
| N+1 queries bottleneck performance | High | Query profiling in Week 4 |
| Load test reveals scalability issues | Medium | Run load tests early (Week 5) |
| Authorization logic too complex | Medium | Keep Phase 1 in-memory fallback |
| Document storage compliance needed | Medium | Use Azure managed backups |

---

## Phase 2 → Phase 3 Handoff

### Phase 3 Preview
**Focus**: Payroll Processing, Attendance Automation, Notifications

1. Payroll calculation engine and salary review workflow
2. Biometric/RFID attendance integration
3. Email/SMS notification service (using MessageBus)
4. Mobile app integration (native iOS/Android)
5. Report generation (custom reports, PDF export)
6. Advanced analytics (turnover, salary benchmarking, attrition)

---

## Quick Reference

**Phase 2 Workstream Priorities**:
1. **Week 1-2**: Authentication (JWT + SSO)
2. **Week 2-3**: Database-backed authorization
3. **Week 3**: Document management + caching
4. **Week 4-5**: Employee module expansion + integration tests
5. **Week 5-6**: Security hardening + performance optimization

**Target Deliverable**: v1.1.0-beta with all Phase 2 features, >80% test coverage, and <200ms latency at scale.

---

## Resources

- **Phase 1 Complete**: v1.0.0-alpha https://github.com/AdityaRane97/HRMS/releases/tag/v1.0.0-alpha
- **Development Branch**: https://github.com/AdityaRane97/HRMS/tree/development
- **Existing ADRs**: /docs/adr/ (ADR-001 through ADR-006)
- **Azure Identity Documentation**: https://learn.microsoft.com/azure/identity/
- **JWT Best Practices**: https://tools.ietf.org/html/rfc8949
- **Redis Caching Patterns**: https://redis.io/patterns/
- **Scalability Guidelines**: https://learn.microsoft.com/aspnet/core/performance/

---

**Phase 2 Planning: Complete! Ready to kickoff.** 🚀

For questions or refinements, review this document and the Phase 1 ADRs before starting implementation.
