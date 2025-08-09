using System.Net;
using System.Text.Json;
using DotNetSkills.API.Middleware;
using DotNetSkills.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace DotNetSkills.API.UnitTests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private static ExceptionHandlingMiddleware CreateMiddleware(Exception toThrow, out DefaultHttpContext context)
    {
        Task NextDelegate(HttpContext _) => throw toThrow;
        var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var env = new Mock<IWebHostEnvironment>();
        env.Setup(e => e.IsDevelopment()).Returns(true);
        context = new DefaultHttpContext();
        context.Request.Path = "/test";
        return new ExceptionHandlingMiddleware(NextDelegate, logger.Object, env.Object);
    }

    private static async Task<(ProblemDetails problem, int status)> InvokeAsync(Exception ex)
    {
        var middleware = CreateMiddleware(ex, out var ctx);
        await middleware.InvokeAsync(ctx);
        ctx.Response.Body.Seek(0, System.IO.SeekOrigin.Begin);
        var problem = await JsonSerializer.DeserializeAsync<ProblemDetails>(ctx.Response.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return (problem!, ctx.Response.StatusCode);
    }

    [Fact]
    public async Task NotFoundException_MapsTo404()
    {
        var (problem, status) = await InvokeAsync(new NotFoundException("Entity", 1));
        Assert.Equal((int)HttpStatusCode.NotFound, status);
        Assert.Equal("Not Found", problem.Title);
    }

    [Fact]
    public async Task BusinessRuleViolation_MapsTo409()
    {
        var (problem, status) = await InvokeAsync(new BusinessRuleViolationException("rule broken"));
        Assert.Equal((int)HttpStatusCode.Conflict, status);
        Assert.Equal("Business Rule Violation", problem.Title);
    }

    [Fact]
    public async Task DomainException_MapsTo400()
    {
        var (problem, status) = await InvokeAsync(new DomainException("domain failure"));
        Assert.Equal(StatusCodes.Status400BadRequest, status);
        Assert.Equal("Domain Rule Violation", problem.Title);
    }

    [Fact]
    public async Task ValidationException_MapsTo400()
    {
        var failures = new [] { new FluentValidation.Results.ValidationFailure("Name", "Name required") };
        var fvEx = new FluentValidation.ValidationException(failures);
        var (problem, status) = await InvokeAsync(fvEx);
        Assert.Equal(StatusCodes.Status400BadRequest, status);
        Assert.Equal("Validation Failed", problem.Title);
    }
}
