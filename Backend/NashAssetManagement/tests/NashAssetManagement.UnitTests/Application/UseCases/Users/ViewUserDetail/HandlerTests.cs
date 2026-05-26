using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.UseCases.Users.ViewUserDetail;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Users.ViewUserDetail;

public class HandlerTests
{
    private static readonly Guid LocationId = Guid.Parse("a3b7ef5a-bce7-401f-bbe3-94c2f7bf0b94");
    private static readonly Guid OtherLocationId = Guid.Parse("6750e0d3-6e01-42f0-b99a-353519e5d6b1");
    private static readonly Guid ExistingUserId = Guid.Parse("36c29308-4d9c-4e1b-9baf-a5dc11f26001");

    [Fact]
    public async Task Handle_Should_Return_User_Detail_When_Request_Is_Valid()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock(CreateUsers());
        var currentUserMock = CreateCurrentUserMock(LocationId.ToString());
        var request = new Request(ExistingUserId.ToString());

        // Act
        var result = await HandleAsync(userManagerMock, currentUserMock, request);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(ExistingUserId, result.Value.Id);
        Assert.Equal("SD1901", result.Value.StaffCode);
        Assert.Equal("An Tran", result.Value.FullName);
        Assert.Equal("ant", result.Value.UserName);
        Assert.Equal("8/10/2022 12:00:00 AM", result.Value.JoinedDate);
        Assert.Equal(UserType.Staff.ToString(), result.Value.UserType);
        Assert.Equal("1998-02-14", result.Value.DateOfBirth);
        Assert.Equal(Gender.Male.ToString(), result.Value.Gender);
        Assert.Equal("HCM", result.Value.Location);
    }

    [Fact]
    public async Task Handle_Should_Return_Unauthorized_When_Current_User_Has_No_Location()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock(CreateUsers());
        var currentUserMock = CreateCurrentUserMock(null);
        var request = new Request(ExistingUserId.ToString());

        // Act
        var result = await HandleAsync(userManagerMock, currentUserMock, request);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Users.NotFound", result.FirstError.Code);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        // Arrange
        var missingUserId = Guid.Parse("36c29308-4d9c-4e1b-9baf-a5dc11f26999");
        var userManagerMock = CreateUserManagerMock(CreateUsers());
        var currentUserMock = CreateCurrentUserMock(LocationId.ToString());
        var request = new Request(missingUserId.ToString());

        // Act
        var result = await HandleAsync(userManagerMock, currentUserMock, request);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Users.NotFound", result.FirstError.Code);
        Assert.Contains(missingUserId.ToString(), result.FirstError.Description);
    }

    [Fact]
    public async Task Handle_Should_Return_Conflict_When_User_Has_Different_Location()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock(CreateUsers());
        var currentUserMock = CreateCurrentUserMock(OtherLocationId.ToString());
        var request = new Request(ExistingUserId.ToString());

        // Act
        var result = await HandleAsync(userManagerMock, currentUserMock, request);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Users.Conflict", result.FirstError.Code);
        Assert.Contains(ExistingUserId.ToString(), result.FirstError.Description);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_Strings_For_Null_Optional_User_Fields()
    {
        // Arrange
        var userId = Guid.Parse("36c29308-4d9c-4e1b-9baf-a5dc11f26002");
        var userManagerMock = CreateUserManagerMock(CreateUsers());
        var currentUserMock = CreateCurrentUserMock(LocationId.ToString());
        var request = new Request(userId.ToString());

        // Act
        var result = await HandleAsync(userManagerMock, currentUserMock, request);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(userId, result.Value.Id);
        Assert.Equal("", result.Value.UserName);
        Assert.Equal("", result.Value.DateOfBirth);
        Assert.Equal("", result.Value.Location);
    }

    private static async Task<ErrorOr<Response>> HandleAsync(
        Mock<UserManager<User>> userManagerMock,
        Mock<ICurrentUser> currentUserMock,
        Request request)
    {
        var handlerType = typeof(Request).Assembly.GetType(
            "NashAssetManagement.Application.UseCases.Users.ViewUserDetail.Handler",
            throwOnError: true)!;
        var handler = Activator.CreateInstance(
            handlerType,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            args: [userManagerMock.Object, currentUserMock.Object, new Validators()],
            culture: null)!;
        var handleMethod = handlerType.GetMethod(
            "Handle",
            BindingFlags.Instance | BindingFlags.Public)!;
        var task = (Task<ErrorOr<Response>>)handleMethod.Invoke(
            handler,
            [request, CancellationToken.None])!;

        return await task;
    }

    private static Mock<UserManager<User>> CreateUserManagerMock(IEnumerable<User> users)
    {
        var store = new Mock<IUserStore<User>>();
        var userManagerMock = new Mock<UserManager<User>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        userManagerMock
            .Setup(userManager => userManager.Users)
            .Returns(users.AsQueryable().BuildMockAsync());

        return userManagerMock;
    }

    private static Mock<ICurrentUser> CreateCurrentUserMock(string? locationId)
    {
        var currentUserMock = new Mock<ICurrentUser>();

        currentUserMock
            .Setup(currentUser => currentUser.LocationId)
            .Returns(locationId);

        currentUserMock
            .Setup(currentUser => currentUser.UserId)
            .Returns(Guid.NewGuid());

        return currentUserMock;
    }

    private static List<User> CreateUsers()
    {
        return
        [
            new()
            {
                Id = ExistingUserId,
                StaffCode = "SD1901",
                FirstName = "An",
                LastName = "Tran",
                UserName = "ant",
                DateOfBirth = new DateTime(1998, 2, 14, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Male,
                JoinedAtUtc = new DateTime(2022, 8, 10, 0, 0, 0, DateTimeKind.Utc),
                UserType = UserType.Staff,
                LocationId = LocationId,
                Location = new Location
                {
                    Id = LocationId,
                    Name = "HCM",
                    Prefix = "HCM"
                }
            },
            new()
            {
                Id = Guid.Parse("36c29308-4d9c-4e1b-9baf-a5dc11f26002"),
                StaffCode = "SD1902",
                FirstName = "Lan",
                LastName = "Pham",
                UserName = null,
                DateOfBirth = null,
                Gender = Gender.Female,
                JoinedAtUtc = new DateTime(2023, 5, 20, 0, 0, 0, DateTimeKind.Utc),
                UserType = UserType.Admin,
                LocationId = LocationId,
                Location = null
            }
        ];
    }
}

internal static class AsyncQueryableExtensions
{
    public static IQueryable<T> BuildMockAsync<T>(this IQueryable<T> source)
    {
        return new TestAsyncEnumerable<T>(source);
    }
}

internal sealed class TestAsyncQueryProvider<TEntity>(IQueryProvider inner) : IAsyncQueryProvider
{
    public IQueryable CreateQuery(Expression expression)
    {
        return new TestAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
    }

    public object? Execute(Expression expression)
    {
        return inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return inner.Execute<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        var expectedResultType = typeof(TResult).GetGenericArguments()[0];
        var executionResult = typeof(IQueryProvider)
            .GetMethod(
                name: nameof(IQueryProvider.Execute),
                genericParameterCount: 1,
                types: [typeof(Expression)])!
            .MakeGenericMethod(expectedResultType)
            .Invoke(inner, [expression]);

        return (TResult)typeof(Task)
            .GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(expectedResultType)
            .Invoke(null, [executionResult])!;
    }
}

internal sealed class TestAsyncEnumerable<T> :
    EnumerableQuery<T>,
    IAsyncEnumerable<T>,
    IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    {
    }

    public TestAsyncEnumerable(Expression expression)
        : base(expression)
    {
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

internal sealed class TestAsyncEnumerator<T>(IEnumerator<T> inner) : IAsyncEnumerator<T>
{
    public T Current => inner.Current;

    public ValueTask DisposeAsync()
    {
        inner.Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return ValueTask.FromResult(inner.MoveNext());
    }
}
