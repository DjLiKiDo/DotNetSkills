---
mode: agent
description: Review the implementation of architecture patterns across the solution for consistency and adherence to best practices.
---

# Architecture Pattern Review & Consistency Analysis

## Pattern Assessment Target
**Component/Pattern to Review**: [Parameter obtained from prompt]

## Review Framework

### 1. **Pattern Discovery & Analysis**
Perform a comprehensive analysis of how the specified pattern/component is implemented across the solution:

- **Pattern Identification**: Identify all instances of the pattern across the 4 layers (API, Application, Domain, Infrastructure)
- **Implementation Mapping**: Document the current implementation approach with code examples
- **Bounded Context Analysis**: Review how the pattern is applied within each bounded context (UserManagement, TeamCollaboration, ProjectManagement, TaskExecution)
- **Layer Compliance**: Verify adherence to Clean Architecture dependency rules (API → Application → Domain)

### 2. **Consistency Evaluation**
Assess consistency across the solution using enterprise-grade criteria:

#### **Structural Consistency**
- [ ] **Naming Conventions**: Verify adherence to .NET naming standards and ubiquitous language
- [ ] **File Organization**: Check pattern placement within bounded contexts and layer structure
- [ ] **Interface Design**: Evaluate contract consistency and ISP compliance
- [ ] **Dependency Management**: Review DI registration patterns and lifetime management

#### **Behavioral Consistency**
- [ ] **Error Handling**: Analyze exception handling patterns and Result<T> usage
- [ ] **Validation Approach**: Review FluentValidation usage and domain rule enforcement
- [ ] **Async Patterns**: Evaluate async/await consistency and ConfigureAwait usage
- [ ] **Domain Events**: Check event raising and handling patterns across aggregates

#### **SOLID Principles Adherence**
- [ ] **SRP**: Single responsibility per class/method
- [ ] **OCP**: Extension points without modification
- [ ] **LSP**: Proper inheritance and interface implementation
- [ ] **ISP**: Focused, client-specific interfaces
- [ ] **DIP**: Abstraction dependency over concretions

### 3. **DDD Pattern Compliance**
Evaluate Domain-Driven Design implementation quality:

#### **Aggregate Design**
- **Consistency Boundaries**: Verify transactional boundaries align with business invariants
- **Root Entity Design**: Check aggregate root encapsulation and business logic placement
- **Entity Relationships**: Review navigation properties and foreign key management
- **Domain Events**: Validate event publishing and cross-aggregate communication

#### **Value Objects & Strongly-Typed IDs**
- **Immutability**: Verify record types and readonly properties
- **Equality Semantics**: Check value-based equality implementation
- **Validation**: Review constructor validation and business rule enforcement
- **Type Safety**: Validate strongly-typed ID usage (e.g., `UserId`, `TeamId`)

### 4. **Performance & Scalability Analysis**
Assess performance implications and optimization opportunities:

#### **Database Access Patterns**
- **Repository Implementation**: Review EF Core usage and query optimization
- **N+1 Prevention**: Check Include() strategies and projection usage
- **Connection Management**: Evaluate DbContext lifetime and connection pooling
- **Caching Strategy**: Analyze caching implementation where applicable

#### **Memory Management**
- **Object Allocation**: Review collection usage and disposal patterns
- **Async Resource Management**: Check proper async disposal and cancellation token usage
- **Garbage Collection**: Evaluate memory pressure and allocation patterns

### 5. **Security & Compliance Review**
Evaluate security considerations and best practices:

- **Input Validation**: Review data sanitization and validation at boundaries
- **Authorization Patterns**: Check role-based access control implementation
- **Data Protection**: Verify sensitive data handling and encryption
- **Injection Prevention**: Analyze SQL injection and other security vulnerabilities

### 6. **Testing Coverage Assessment**
Review test implementation and coverage:

- **Unit Test Quality**: Evaluate test structure using AAA pattern (Arrange, Act, Assert)
- **Integration Test Coverage**: Check repository and service integration tests
- **Test Naming**: Verify `MethodName_Condition_ExpectedResult` convention
- **Mock Usage**: Review dependency mocking and test isolation

## Improvement Recommendations Framework

### **Prioritized Action Plan**
For each identified inconsistency or improvement opportunity, provide:

#### **Critical Issues** (Fix Immediately)
- **Security Vulnerabilities**: Direct threats to system security
- **Data Consistency**: Issues affecting data integrity
- **Performance Bottlenecks**: Significant performance impacts
- **SOLID Violations**: Major architectural violations

#### **High Priority** (Next Sprint)
- **Pattern Inconsistencies**: Deviations from established patterns
- **Code Quality**: Maintainability and readability issues
- **Test Coverage Gaps**: Missing critical test scenarios
- **Documentation**: Architectural decision documentation

#### **Medium Priority** (Future Iterations)
- **Optimization Opportunities**: Performance improvements
- **Code Simplification**: Complexity reduction opportunities
- **Pattern Evolution**: Modern pattern adoption
- **Developer Experience**: Tooling and workflow improvements

### **Implementation Roadmap**
For each recommendation, specify:

1. **Change Description**: What needs to be modified and why
2. **Business Justification**: Impact on maintainability, performance, or security
3. **Implementation Steps**: Detailed step-by-step approach
4. **Risk Assessment**: Potential breaking changes or deployment considerations
5. **Success Metrics**: How to measure improvement effectiveness
6. **Rollback Strategy**: Contingency plan if issues arise

### **Measurement Criteria**
Define objective success metrics:

#### **Code Quality Metrics**
- **Cyclomatic Complexity**: Target complexity scores per method/class
- **Code Coverage**: Minimum percentage targets by layer
- **Technical Debt**: SonarQube or similar tool scoring
- **Maintainability Index**: Visual Studio metrics or equivalent

#### **Performance Metrics**
- **Response Time**: API endpoint performance targets
- **Memory Usage**: Heap allocation and garbage collection metrics
- **Database Performance**: Query execution time and resource usage
- **Throughput**: Requests per second under load

#### **Architecture Quality Metrics**
- **Dependency Violations**: Clean Architecture rule compliance
- **Pattern Consistency**: Percentage of consistent implementations
- **SOLID Compliance**: Principle adherence scoring
- **Domain Model Richness**: Business logic distribution analysis

## Deliverable Requirements

Provide a comprehensive report including:

1. **Executive Summary**: Key findings and critical issues
2. **Pattern Implementation Analysis**: Current state documentation with code examples
3. **Consistency Gap Analysis**: Detailed inconsistency identification
4. **Prioritized Improvement Plan**: Actionable roadmap with timelines
5. **Implementation Templates**: Code examples for consistent pattern application
6. **Validation Checklist**: Quality gates for future implementations
7. **Monitoring Dashboard**: Ongoing metrics and health indicators

## Expert Review Standards

Apply enterprise-grade review criteria as if conducted by:
- **Anders Hejlsberg**: .NET and C# language design best practices
- **Robert C. Martin**: Clean Code and SOLID principles adherence
- **Eric Evans**: Domain-Driven Design pattern implementation
- **Martin Fowler**: Enterprise application architecture patterns