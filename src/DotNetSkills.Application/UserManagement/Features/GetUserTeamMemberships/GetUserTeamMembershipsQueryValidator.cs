namespace DotNetSkills.Application.UserManagement.Features.GetUserTeamMemberships;

/// <summary>
/// Validator for GetUserTeamMembershipsQuery.
/// </summary>
public class GetUserTeamMembershipsQueryValidator : AbstractValidator<GetUserTeamMembershipsQuery>
{
    public GetUserTeamMembershipsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        RuleFor(x => x.UserId.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("UserId cannot be empty GUID.");
    }
}
