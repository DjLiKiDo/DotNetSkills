namespace DotNetSkills.Domain.UnitTests.Builders;

/// <summary>
/// Builder for creating test User instances.
/// </summary>
public class UserBuilder
{
    private string _name = "Test User";
    private EmailAddress _email = EmailAddressBuilder.Create().WithValidEmail("test", "example.com").Build();
    private UserRole _role = UserRole.Developer;
    private UserId? _createdBy = null;

    public static UserBuilder Create() => new();

    public UserBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public UserBuilder WithEmail(EmailAddress email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithEmail(string emailValue)
    {
        _email = new EmailAddress(emailValue);
        return this;
    }

    public UserBuilder WithRole(UserRole role)
    {
        _role = role;
        return this;
    }

    public UserBuilder AsAdmin()
    {
        _role = UserRole.Admin;
        return this;
    }

    public UserBuilder AsDeveloper()
    {
        _role = UserRole.Developer;
        return this;
    }

    public UserBuilder AsProjectManager()
    {
        _role = UserRole.ProjectManager;
        return this;
    }

    public UserBuilder AsViewer()
    {
        _role = UserRole.Viewer;
        return this;
    }

    public UserBuilder CreatedBy(UserId createdBy)
    {
        _createdBy = createdBy;
        return this;
    }

    public UserBuilder CreatedBy(User createdByUser)
    {
        _createdBy = createdByUser.Id;
        return this;
    }

    public User Build()
    {
        return new User(_name, _email, _role, _createdBy);
    }

    public User BuildWithCreate(User? createdByUser = null)
    {
        return User.Create(_name, _email, _role, createdByUser);
    }

    public static implicit operator User(UserBuilder builder) => builder.Build();
}

/// <summary>
/// Builder for creating test Team instances.
/// </summary>
public class TeamBuilder
{
    private string _name = "Test Team";
    private string _description = "Test team description";
    private User _createdByUser = UserBuilder.Create().AsAdmin().Build();

    public static TeamBuilder Create() => new();

    public TeamBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public TeamBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public TeamBuilder CreatedBy(User createdByUser)
    {
        _createdByUser = createdByUser;
        return this;
    }

    public Team Build()
    {
        return new Team(_name, _description, _createdByUser);
    }

    public static implicit operator Team(TeamBuilder builder) => builder.Build();
}

/// <summary>
/// Builder for creating test Project instances.
/// </summary>
public class ProjectBuilder
{
    private string _name = "Test Project";
    private string _description = "Test project description";
    private TeamId _teamId = TeamIdBuilder.Create();
    private DateTime? _plannedEndDate = DateTime.UtcNow.AddDays(30);
    private User _createdByUser = UserBuilder.Create().AsProjectManager().Build();

    public static ProjectBuilder Create() => new();

    public ProjectBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProjectBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ProjectBuilder WithTeam(TeamId teamId)
    {
        _teamId = teamId;
        return this;
    }

    public ProjectBuilder WithTeam(Team team)
    {
        _teamId = team.Id;
        return this;
    }

    public ProjectBuilder WithPlannedEndDate(DateTime? plannedEndDate)
    {
        _plannedEndDate = plannedEndDate;
        return this;
    }

    public ProjectBuilder CreatedBy(User createdByUser)
    {
        _createdByUser = createdByUser;
        return this;
    }

    public Project Build()
    {
        return new Project(_name, _description, _teamId, _plannedEndDate, _createdByUser);
    }

    public static implicit operator Project(ProjectBuilder builder) => builder.Build();
}

/// <summary>
/// Builder for creating test Task instances.
/// </summary>
public class TaskBuilder
{
    private string _title = "Test Task";
    private string _description = "Test task description";
    private ProjectId _projectId = ProjectIdBuilder.Create();
    private TaskPriority _priority = TaskPriority.Medium;
    private TaskId? _parentTaskId = null;
    private int? _estimatedHours = null;
    private DateTime? _dueDate = null;
    private User _createdByUser = UserBuilder.Create().AsDeveloper().Build();

    public static TaskBuilder Create() => new();

    public TaskBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public TaskBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public TaskBuilder WithProject(ProjectId projectId)
    {
        _projectId = projectId;
        return this;
    }

    public TaskBuilder WithProject(Project project)
    {
        _projectId = project.Id;
        return this;
    }

    public TaskBuilder WithPriority(TaskPriority priority)
    {
        _priority = priority;
        return this;
    }

    public TaskBuilder WithHighPriority()
    {
        _priority = TaskPriority.High;
        return this;
    }

    public TaskBuilder WithMediumPriority()
    {
        _priority = TaskPriority.Medium;
        return this;
    }

    public TaskBuilder WithLowPriority()
    {
        _priority = TaskPriority.Low;
        return this;
    }

    public TaskBuilder WithEstimatedHours(int? estimatedHours)
    {
        _estimatedHours = estimatedHours;
        return this;
    }

    public TaskBuilder WithDueDate(DateTime? dueDate)
    {
        _dueDate = dueDate;
        return this;
    }

    public TaskBuilder AsSubtask(TaskId parentTaskId)
    {
        _parentTaskId = parentTaskId;
        return this;
    }

    public TaskBuilder AsSubtask(DotNetSkills.Domain.TaskExecution.Entities.Task parentTask)
    {
        _parentTaskId = parentTask.Id;
        return this;
    }

    public TaskBuilder CreatedBy(User createdByUser)
    {
        _createdByUser = createdByUser;
        return this;
    }

    public DotNetSkills.Domain.TaskExecution.Entities.Task Build()
    {
        return new DotNetSkills.Domain.TaskExecution.Entities.Task(_title, _description, _projectId, _priority, _parentTaskId, _estimatedHours, _dueDate, _createdByUser);
    }

    public static implicit operator DotNetSkills.Domain.TaskExecution.Entities.Task(TaskBuilder builder) => builder.Build();
}