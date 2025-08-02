namespace DotNetSkills.Domain.UnitTests.ProjectManagement.ValueObjects;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "ProjectManagement")]
public class ProjectIdTests : TestBase
{
    #region Constructor Tests

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidGuid_ShouldCreateProjectId()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var projectId = new ProjectId(guid);

        // Assert
        projectId.Value.Should().Be(guid);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithEmptyGuid_ShouldCreateProjectId()
    {
        // Arrange
        var guid = Guid.Empty;

        // Act
        var projectId = new ProjectId(guid);

        // Assert
        projectId.Value.Should().Be(Guid.Empty);
    }

    #endregion

    #region Factory Method Tests

    [Fact]
    [Trait("TestType", "Creation")]
    public void New_ShouldCreateUniqueProjectId()
    {
        // Act
        var projectId1 = ProjectId.New();
        var projectId2 = ProjectId.New();

        // Assert
        projectId1.Should().NotBe(projectId2);
        projectId1.Value.Should().NotBe(Guid.Empty);
        projectId2.Value.Should().NotBe(Guid.Empty);
        projectId1.Value.Should().NotBe(projectId2.Value);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void New_CalledMultipleTimes_ShouldGenerateUniqueGuids()
    {
        // Arrange
        var projectIds = new HashSet<Guid>();

        // Act
        for (int i = 0; i < 1000; i++)
        {
            var projectId = ProjectId.New();
            projectIds.Add(projectId.Value);
        }

        // Assert
        projectIds.Should().HaveCount(1000);
    }

    #endregion

    #region Operator Tests

    [Fact]
    [Trait("TestType", "Conversion")]
    public void ImplicitOperator_ToGuid_ShouldReturnValue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var projectId = new ProjectId(guid);

        // Act
        Guid result = projectId;

        // Assert
        result.Should().Be(guid);
    }

    [Fact]
    [Trait("TestType", "Conversion")]
    public void ExplicitOperator_FromGuid_ShouldCreateProjectId()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var projectId = (ProjectId)guid;

        // Assert
        projectId.Value.Should().Be(guid);
    }

    [Fact]
    [Trait("TestType", "Conversion")]
    public void Operators_Roundtrip_ShouldPreserveValue()
    {
        // Arrange
        var originalGuid = Guid.NewGuid();
        var projectId = new ProjectId(originalGuid);

        // Act
        Guid convertedGuid = projectId;
        var backToProjectId = (ProjectId)convertedGuid;

        // Assert
        backToProjectId.Value.Should().Be(originalGuid);
        backToProjectId.Should().Be(projectId);
    }

    #endregion

    #region Equality Tests

    [Fact]
    [Trait("TestType", "Equality")]
    public void Equality_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var projectId1 = new ProjectId(guid);
        var projectId2 = new ProjectId(guid);

        // Act & Assert
        projectId1.Should().Be(projectId2);
        (projectId1 == projectId2).Should().BeTrue();
        (projectId1 != projectId2).Should().BeFalse();
        projectId1.GetHashCode().Should().Be(projectId2.GetHashCode());
    }

    [Fact]
    [Trait("TestType", "Equality")]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var projectId1 = new ProjectId(Guid.NewGuid());
        var projectId2 = new ProjectId(Guid.NewGuid());

        // Act & Assert
        projectId1.Should().NotBe(projectId2);
        (projectId1 == projectId2).Should().BeFalse();
        (projectId1 != projectId2).Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "Equality")]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var projectId = ProjectId.New();

        // Act & Assert
        projectId.Equals(null).Should().BeFalse();
        (projectId == null).Should().BeFalse();
        (null == projectId).Should().BeFalse();
        (projectId != null).Should().BeTrue();
        (null != projectId).Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "Equality")]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // Arrange
        var projectId = ProjectId.New();
        var guid = projectId.Value;

        // Act & Assert
        projectId.Equals(guid).Should().BeFalse();
        projectId.Equals("string").Should().BeFalse();
        projectId.Equals(42).Should().BeFalse();
    }

    #endregion

    #region ToString Tests

    [Fact]
    [Trait("TestType", "Serialization")]
    public void ToString_ShouldReturnGuidStringRepresentation()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var projectId = new ProjectId(guid);

        // Act
        var result = projectId.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }

    [Fact]
    [Trait("TestType", "Serialization")]
    public void ToString_WithEmptyGuid_ShouldReturnEmptyGuidString()
    {
        // Arrange
        var projectId = new ProjectId(Guid.Empty);

        // Act
        var result = projectId.ToString();

        // Assert
        result.Should().Be(Guid.Empty.ToString());
    }

    #endregion

    #region Interface Compliance Tests

    [Fact]
    [Trait("TestType", "Interface")]
    public void ProjectId_ShouldImplementIStronglyTypedId()
    {
        // Arrange
        var projectId = ProjectId.New();

        // Act & Assert
        projectId.Should().BeAssignableTo<IStronglyTypedId<Guid>>();
    }

    [Fact]
    [Trait("TestType", "Interface")]
    public void Value_Property_ShouldBeAccessible()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var projectId = new ProjectId(guid);

        // Act
        var value = ((IStronglyTypedId<Guid>)projectId).Value;

        // Assert
        value.Should().Be(guid);
    }

    #endregion

    #region Record Features Tests

    [Fact]
    [Trait("TestType", "RecordFeatures")]
    public void Record_ShouldSupportWithExpression()
    {
        // Arrange
        var originalGuid = Guid.NewGuid();
        var newGuid = Guid.NewGuid();
        var originalProjectId = new ProjectId(originalGuid);

        // Act
        var newProjectId = originalProjectId with { Value = newGuid };

        // Assert
        originalProjectId.Value.Should().Be(originalGuid);
        newProjectId.Value.Should().Be(newGuid);
        originalProjectId.Should().NotBe(newProjectId);
    }

    [Fact]
    [Trait("TestType", "RecordFeatures")]
    public void Record_ShouldSupportDeconstruction()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var projectId = new ProjectId(guid);

        // Act
        var value = projectId.Value;

        // Assert
        value.Should().Be(guid);
    }

    #endregion

    #region Hash Code Tests

    [Fact]
    [Trait("TestType", "Equality")]
    public void GetHashCode_WithSameValue_ShouldReturnSameHashCode()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var projectId1 = new ProjectId(guid);
        var projectId2 = new ProjectId(guid);

        // Act
        var hashCode1 = projectId1.GetHashCode();
        var hashCode2 = projectId2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    [Trait("TestType", "Equality")]
    public void GetHashCode_WithDifferentValues_ShouldReturnDifferentHashCodes()
    {
        // Arrange
        var projectId1 = ProjectId.New();
        var projectId2 = ProjectId.New();

        // Act
        var hashCode1 = projectId1.GetHashCode();
        var hashCode2 = projectId2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }

    #endregion

    #region Performance Tests

    [Fact]
    [Trait("TestType", "Performance")]
    public void Creation_ShouldBeFast()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 10000; i++)
        {
            var projectId = ProjectId.New();
        }

        // Assert
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
    }

    [Fact]
    [Trait("TestType", "Performance")]
    public void Conversion_ShouldBeFast()
    {
        // Arrange
        var projectIds = Enumerable.Range(0, 1000)
            .Select(_ => ProjectId.New())
            .ToList();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 1000; i++)
        {
            Guid guid = projectIds[i];
            var backToProjectId = (ProjectId)guid;
        }

        // Assert
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(10);
    }

    #endregion
}