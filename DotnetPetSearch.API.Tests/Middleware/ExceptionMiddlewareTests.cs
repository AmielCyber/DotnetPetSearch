using DotnetPetSearch.API.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace DotnetPetSearch.API.Tests.Middleware;

public class ExceptionMiddlewareTests
{
    private readonly ILogger<ExceptionMiddleware> _loggerMock;
    private readonly IHostEnvironment _envMock;
    private readonly RequestDelegate _next;
    private readonly HttpContext _context;
    private readonly ExceptionMiddleware _exceptionMiddleware;

    public ExceptionMiddlewareTests()
    {
        _loggerMock = Substitute.For<ILogger<ExceptionMiddleware>>();
        _envMock = Substitute.For<IHostEnvironment>();
        _context = new DefaultHttpContext();
        var responseBody = new MemoryStream();
        _context.Response.Body = responseBody;
        _next = Substitute.For<RequestDelegate>();
        _exceptionMiddleware = new ExceptionMiddleware(_next, _loggerMock, _envMock);
    }

    [Fact]
    public async Task InvokeAsync_DoesNothing_WhenNextIsSuccessful()
    {
        // Action
        await _exceptionMiddleware.InvokeAsync(_context);
        // Assert
        await _next.ReceivedWithAnyArgs(1).Invoke(_context);
        _loggerMock.DidNotReceiveWithAnyArgs();
        _envMock.DidNotReceiveWithAnyArgs();
    }
    [Fact]
    public async Task InvokeAsync_ResponseContentTypeIsProblemJson_WhenExceptionIsThrown()
    {
        // Arrange
        string expectedErrorMessage = "Test error";
        ArgumentException expectedException = new ArgumentException(expectedErrorMessage);
        
        _next.When(x => x.Invoke(Arg.Any<HttpContext>()))
            .Do(x => throw expectedException);
        // Action

        await _exceptionMiddleware.InvokeAsync(_context);
        
        // Assert
        Assert.Contains("problem+json", _context.Response.ContentType);
    }
    
    [Fact]
    public async Task InvokeAsync_ResponseContentStatusCodeIs500_WhenExceptionIsThrown()
    {
        // Arrange
        string expectedErrorMessage = "Test error";
        ArgumentException expectedException = new ArgumentException(expectedErrorMessage);
        
        _next.When(x => x.Invoke(Arg.Any<HttpContext>()))
            .Do(x => throw expectedException);
        // Action

        await _exceptionMiddleware.InvokeAsync(_context);
        
        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, _context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_LogsError_WhenExceptionIsThrown()
    {
        // Arrange
        string expectedErrorMessage = "Test error";
        ArgumentException expectedException = new ArgumentException(expectedErrorMessage);
        
        _next.When(x => x.Invoke(Arg.Any<HttpContext>()))
            .Do(x => throw expectedException);
        // Action

        await _exceptionMiddleware.InvokeAsync(_context);
        
        // Assert
        _loggerMock.Received().LogError(0,expectedException, expectedErrorMessage);
    }
    
    [Fact]
    public async Task InvokeAsync_WritesErrorMessage_WhenExceptionIsThrownAndInDeveloperMode()
    {
        // Arrange
        _envMock.EnvironmentName = Environments.Development; 
        string expectedErrorMessage = "Test error";
        ArgumentException expectedException = new ArgumentException(expectedErrorMessage);
        
        _next.When(x => x.Invoke(Arg.Any<HttpContext>()))
            .Do(x => throw expectedException);
        
        // Action
        await _exceptionMiddleware.InvokeAsync(_context);
        
        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(_context.Response.Body);
        string actualBody = await reader.ReadToEndAsync();
        Assert.Contains(expectedErrorMessage, actualBody);
    }
    
    [Fact]
    public async Task InvokeAsync_DoesNotWritesErrorMessage_WhenExceptionIsThrownAndNotInDeveloperMode()
    {
        // Arrange
        _envMock.EnvironmentName = Environments.Production; 
        string expectedErrorMessage = "Test error";
        ArgumentException expectedException = new ArgumentException(expectedErrorMessage);
        
        _next.When(x => x.Invoke(Arg.Any<HttpContext>()))
            .Do(x => throw expectedException);
        
        // Action
        await _exceptionMiddleware.InvokeAsync(_context);
        
        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(_context.Response.Body);
        string actualBody = await reader.ReadToEndAsync();
        Assert.DoesNotContain(expectedErrorMessage, actualBody);
    }
}