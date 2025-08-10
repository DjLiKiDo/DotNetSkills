using System.Security.Claims;
using DotNetSkills.API.Services;
using DotNetSkills.Application.Common.Abstractions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using DotNetSkills.Domain.UserManagement.Enums; // Added for UserRole enum

namespace DotNetSkills.API.UnitTests.Services;

public class CurrentUserServiceTests
{
    private static CurrentUserService CreateService(ClaimsPrincipal? user = null)
    {
        var context = new DefaultHttpContext();
        if (user != null)
        {
            context.User = user;
        }

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(a => a.HttpContext).Returns(context);

        return new CurrentUserService(httpContextAccessor.Object);
    }

    [Fact]
    public void GetCurrentUserId_WhenNotAuthenticated_ReturnsNull()
    {
        var service = CreateService();
        var result = service.GetCurrentUserId();
        Assert.Null(result);
        Assert.False(service.IsAuthenticated());
    }

    [Fact]
    public void GetCurrentUserId_WhenAuthenticated_ReturnsUserId()
    {
        var userId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        }, authenticationType: "Test"));

        var service = CreateService(claimsPrincipal);

        var currentUserId = service.GetCurrentUserId();
        Assert.NotNull(currentUserId);
        Assert.Equal(userId, currentUserId!.Value);
        Assert.True(service.IsAuthenticated());

        var role = service.GetCurrentUserRole();
        Assert.NotNull(role);
        Assert.Equal(UserRole.Admin, role);
    }

    [Fact]
    public void GetCurrentUserRole_WhenRoleInvalid_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "NotARealRole")
        }, authenticationType: "Test"));

        var service = CreateService(claimsPrincipal);
        var role = service.GetCurrentUserRole();
        Assert.Null(role);
    }
}
