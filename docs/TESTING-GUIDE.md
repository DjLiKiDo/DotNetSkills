# Testing Guide

*Comprehensive testing practices and strategies for DotNetSkills*

## Testing Philosophy

**Quality over Quantity**: Focus on meaningful tests that catch real bugs and enable confident refactoring.

**80% Coverage Minimum**: Especially for Domain and Application layers where business logic lives.

**Fast Feedback**: Tests should run quickly and provide immediate feedback during development.

## Test Structure Overview

```
tests/
├── DotNetSkills.Domain.UnitTests/         # Domain logic and business rules
├── DotNetSkills.Application.UnitTests/    # Handlers, behaviors, validation  
├── DotNetSkills.Infrastructure.UnitTests/ # Repository and data access
└── DotNetSkills.API.UnitTests/           # Endpoints and middleware
```

## Essential Testing Commands

### Running Tests
```bash
# Run all tests
make test
# or: dotnet test

# Run specific test project
dotnet test tests/DotNetSkills.Domain.UnitTests
dotnet test tests/DotNetSkills.Application.UnitTests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run tests matching a filter
dotnet test --filter "Category=Unit"
dotnet test --filter "TestMethod=CreateUser*"
dotnet test --filter "FullyQualifiedName~UserTests"
```

### Continuous Testing
```bash
# Watch mode - runs tests on file changes
dotnet watch test --project tests/DotNetSkills.Domain.UnitTests

# Watch specific test
dotnet watch test --filter "CreateUserCommandHandlerTests"
```

## Testing Patterns by Layer

### 🏛️ Domain Layer Testing

**Focus**: Business logic, domain rules, entity behavior

**What to Test**:
- Entity creation and validation
- Business rule enforcement
- Domain event raising
- Value object behavior
- Invariant preservation

**Example - Entity Tests**:
```csharp
public class UserTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = new EmailAddress("john.doe@example.com");

        // Act
        var user = User.Create(firstName, lastName, email);

        // Assert
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.Email.Should().Be(email);
        user.Status.Should().Be(UserStatus.Active);
        user.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserCreatedDomainEvent>();
    }

    [Fact]
    public void Create_WithInvalidEmail_ShouldThrowDomainException()
    {
        // Arrange
        var invalidEmail = "invalid-email";

        // Act & Assert
        var act = () => User.Create("John", "Doe", new EmailAddress(invalidEmail));
        act.Should().Throw<DomainException>()
            .WithMessage("*invalid email format*");
    }
}
```

**Example - Value Object Tests**:
```csharp
public class EmailAddressTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test.email+tag@domain.co.uk")]
    public void Create_WithValidEmail_ShouldSucceed(string validEmail)
    {
        // Act & Assert
        var act = () => new EmailAddress(validEmail);
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("@domain.com")]
    public void Create_WithInvalidEmail_ShouldThrowException(string invalidEmail)
    {
        // Act & Assert
        var act = () => new EmailAddress(invalidEmail);
        act.Should().Throw<DomainException>();
    }
}
```

### 🎯 Application Layer Testing

**Focus**: Use case orchestration, validation, mapping

**What to Test**:
- Command/Query handler logic
- Validation rules
- Mapping configurations
- Pipeline behaviors
- Error handling

**Example - Command Handler Tests**:
```csharp
public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());
        _mapper = config.CreateMapper();
        
        _handler = new CreateUserCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateUser()
    {
        // Arrange
        var command = new CreateUserCommand("John", "Doe", "john.doe@example.com");
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<EmailAddress>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("john.doe@example.com");
        
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldThrowDomainException()
    {
        // Arrange
        var command = new CreateUserCommand("John", "Doe", "existing@example.com");
        var existingUser = UserTestBuilder.Create().WithEmail("existing@example.com").Build();
        
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<EmailAddress>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*email already exists*");
    }
}
```

**Example - Validation Tests**:
```csharp
public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        _validator = new CreateUserCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new CreateUserCommand("John", "Doe", "john.doe@example.com");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("", "LastName is required")]
    [InlineData("J", "FirstName must be at least 2 characters")]
    [InlineData("VeryLongFirstNameThatExceedsMaximumLength", "FirstName must not exceed 50 characters")]
    public void Validate_WithInvalidFirstName_ShouldHaveError(string firstName, string expectedError)
    {
        // Arrange
        var command = new CreateUserCommand(firstName, "Doe", "john.doe@example.com");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage(expectedError);
    }
}
```

### 🔧 Infrastructure Layer Testing

**Focus**: Data access, repository implementations, external integrations

**What to Test**:
- Repository query logic
- Database mapping
- Configuration validation
- External service integrations

**Example - Repository Tests**:
```csharp
public class UserRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new ApplicationDbContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task GetByEmailAsync_WithExistingEmail_ShouldReturnUser()
    {
        // Arrange
        var user = UserTestBuilder.Create()
            .WithEmail("test@example.com")
            .Build();
        
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(new EmailAddress("test@example.com"), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Value.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByEmailAsync_WithNonExistentEmail_ShouldReturnNull()
    {
        // Arrange
        var email = new EmailAddress("nonexistent@example.com");

        // Act
        var result = await _repository.GetByEmailAsync(email, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
```

### 📱 API Layer Testing

**Focus**: HTTP endpoints, middleware, authentication

**What to Test**:
- Endpoint routing and responses
- Authentication/authorization
- Middleware behavior
- Input validation integration

**Example - Endpoint Tests**:
```csharp
public class UserEndpointsTests
{
    [Fact]
    public async Task CreateUser_WithValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var expectedResponse = new UserResponse(UserId.New(), "John", "Doe", "john.doe@example.com", UserStatus.Active);
        
        mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var request = new CreateUserCommand("John", "Doe", "john.doe@example.com");
        var result = await UserEndpoints.CreateUser(request, mediatorMock.Object);

        // Assert
        var okResult = result.Should().BeOfType<Ok<UserResponse>>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResponse);
    }
}
```

## Test Builders Pattern

Use the Builder pattern for creating test data to make tests more maintainable and readable.

**Example - User Test Builder**:
```csharp
public class UserTestBuilder
{
    private UserId _id = UserId.New();
    private string _firstName = "John";
    private string _lastName = "Doe";
    private EmailAddress _email = new("john.doe@example.com");
    private UserRole _role = UserRole.Developer;
    private UserStatus _status = UserStatus.Active;

    public static UserTestBuilder Create() => new();

    public UserTestBuilder WithId(UserId id)
    {
        _id = id;
        return this;
    }

    public UserTestBuilder WithName(string firstName, string lastName)
    {
        _firstName = firstName;
        _lastName = lastName;
        return this;
    }

    public UserTestBuilder WithEmail(string email)
    {
        _email = new EmailAddress(email);
        return this;
    }

    public UserTestBuilder WithRole(UserRole role)
    {
        _role = role;
        return this;
    }

    public UserTestBuilder WithStatus(UserStatus status)
    {
        _status = status;
        return this;
    }

    public User Build()
    {
        return User.Create(_firstName, _lastName, _email, _role, _status);
    }
}

// Usage in tests
var user = UserTestBuilder.Create()
    .WithEmail("admin@example.com")
    .WithRole(UserRole.Admin)
    .Build();
```

## Testing Best Practices

### ✅ Do's

**Test Structure**:
- Follow **Arrange-Act-Assert** pattern consistently
- Use descriptive test method names: `Method_Scenario_ExpectedResult`
- Group related tests in nested classes
- Use `Theory` with `InlineData` for parameterized tests

**Assertions**:
- Use **FluentAssertions** for readable assertions
- Be specific about what you're testing
- Test both success and failure scenarios
- Verify mock calls when relevant

**Test Data**:
- Use Builder pattern for complex object creation
- Prefer explicit test data over random generation
- Use constants for magic strings/numbers
- Create focused test fixtures

### ❌ Don'ts

**Common Anti-Patterns**:
- Don't test framework code (EF Core, MediatR internals)
- Don't test private methods directly
- Don't use real database for unit tests
- Don't test multiple concerns in one test
- Don't ignore flaky tests

**Mocking Guidelines**:
- Mock external dependencies only
- Don't mock the system under test
- Don't over-mock value objects
- Verify behavior, not just state

## Test Categories and Attributes

### Test Categories
```csharp
[Fact]
[Trait("Category", "Unit")]
public void Unit_Test_Example() { }

[Fact]
[Trait("Category", "Integration")]
public void Integration_Test_Example() { }

[Fact]
[Trait("Category", "Smoke")]
public void Smoke_Test_Example() { }
```

### Custom Attributes
```csharp
// Run only unit tests
dotnet test --filter "Category=Unit"

// Run integration tests
dotnet test --filter "Category=Integration"

// Run smoke tests (quick health checks)
dotnet test --filter "Category=Smoke"
```

## Testing Configuration

### Test Project Structure
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" />
    <PackageReference Include="FluentAssertions" />
    <PackageReference Include="Moq" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" />
  </ItemGroup>
</Project>
```

### Global Test Setup
```csharp
// Global usings for all test projects
global using Xunit;
global using FluentAssertions;
global using Moq;
global using Microsoft.Extensions.DependencyInjection;
global using DotNetSkills.Domain.Common.Exceptions;
```

## Coverage and Quality Gates

### Coverage Targets
- **Domain Layer**: 90%+ (critical business logic)
- **Application Layer**: 85%+ (orchestration and validation)
- **Infrastructure Layer**: 70%+ (data access)
- **API Layer**: 60%+ (integration points)

### Quality Metrics
```bash
# Generate coverage report
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage

# Generate HTML report (with reportgenerator tool)
reportgenerator -reports:"coverage/**/coverage.cobertura.xml" -targetdir:"coverage/report" -reporttypes:Html
```

## Integration Testing Strategy

### Database Testing
```csharp
// Use Testcontainers for real database testing
public class IntegrationTestBase : IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithPassword("TestPassword123!")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        // Setup test data
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}
```

### API Testing
```csharp
// Use WebApplicationFactory for integration tests
public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetUsers_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

## Debugging Tests

### Common Test Debugging Techniques
```bash
# Run specific test with debugging
dotnet test --filter "FullyQualifiedName=YourNamespace.YourTestClass.YourTestMethod" --logger "console;verbosity=detailed"

# Debug failing test
dotnet test --filter "YourFailingTest" --logger "console;verbosity=diagnostic"

# Run tests in isolation
dotnet test --no-build --verbosity normal tests/YourSpecificTest.UnitTests
```

### Test Output
```csharp
public class DebugTests
{
    private readonly ITestOutputHelper _output;

    public DebugTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Test_With_Debug_Output()
    {
        // Arrange
        var value = "test";
        
        // Debug output
        _output.WriteLine($"Testing with value: {value}");
        
        // Act & Assert
        value.Should().NotBeNull();
    }
}
```

## Continuous Integration

### Test Pipeline
```yaml
# Example GitHub Actions test step
- name: Run Tests
  run: |
    dotnet test --configuration Release \
      --collect:"XPlat Code Coverage" \
      --results-directory ./coverage \
      --logger trx \
      --no-build
```

### Quality Gates
- All tests must pass
- Code coverage above minimum thresholds
- No flaky tests allowed
- Performance tests within acceptable limits

---

**🧪 Testing Philosophy Summary**:
- **Test Behavior, Not Implementation**: Focus on what the code does, not how
- **Red-Green-Refactor**: Write failing test, make it pass, improve
- **Test First When Possible**: TDD for complex business logic
- **Maintain Test Quality**: Tests are code too - keep them clean and maintainable