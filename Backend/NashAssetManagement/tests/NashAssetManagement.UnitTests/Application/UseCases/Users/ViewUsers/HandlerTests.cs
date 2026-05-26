using System.Linq.Expressions;
using System.Reflection;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.UseCases.Users.ViewUsers;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Users.ViewUsers;

public class HandlerTests
{
    private static readonly Guid LocationId = Guid.Parse("a3b7ef5a-bce7-401f-bbe3-94c2f7bf0b94");

    [Fact]
    public async Task Handle_Should_Return_Paged_User_List_When_Request_Is_Valid()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock(CreateUsers());
        var currentUserMock = CreateCurrentUserMock(LocationId.ToString());
        var request = new Request(1, 10, null, null, null, null);

        // Act
        var result = await HandleAsync(userManagerMock, currentUserMock, request);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(3, result.Value.TotalCount);
        Assert.Equal(1, result.Value.PageNumber);
        Assert.Equal(10, result.Value.PageSize);
        Assert.Equal(3, result.Value.Items.Count);

        var firstUser = result.Value.Items[0];
        Assert.Equal("SD1903", firstUser.StaffCode);
        Assert.Equal("Binh Nguyen", firstUser.FullName);
        Assert.Equal("binhn", firstUser.UserName);
        Assert.Equal("1/15/2024 12:00:00 AM", firstUser.JoinedDate);
        Assert.Equal(UserType.Staff.ToString(), firstUser.UserType);
        Assert.False(firstUser.CanBeDisabled);
    }

    [Fact]
    public async Task Handle_Should_Return_Unauthorized_When_Current_User_Has_No_Location()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock(CreateUsers());
        var currentUserMock = CreateCurrentUserMock(null);
        var request = new Request(1, 10, null, null, null, null);

        // Act
        var result = await HandleAsync(userManagerMock, currentUserMock, request);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Users.NotFound", result.FirstError.Code);
    }

    [Fact]
    public async Task Handle_Should_Filter_Users_By_Type_When_Type_Is_Valid()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock(CreateUsers());
        var currentUserMock = CreateCurrentUserMock(LocationId.ToString());
        var request = new Request(1, 10, null, UserType.Admin.ToString(), null, null);

        // Act
        var result = await HandleAsync(userManagerMock, currentUserMock, request);

        // Assert
        Assert.False(result.IsError);
        Assert.Single(result.Value.Items);
        Assert.Equal("SD1902", result.Value.Items[0].StaffCode);
        Assert.Equal(UserType.Admin.ToString(), result.Value.Items[0].UserType);
    }

    [Fact]
    public async Task Handle_Should_Filter_Users_By_SearchTerm_When_SearchTerm_Is_Provided()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock(CreateUsers());
        var currentUserMock = CreateCurrentUserMock(LocationId.ToString());
        var request = new Request(1, 10, "binh", null, null, null);

        // Act
        var result = await HandleAsync(userManagerMock, currentUserMock, request);

        // Assert
        Assert.False(result.IsError);
        Assert.Single(result.Value.Items);
        Assert.Equal("SD1903", result.Value.Items[0].StaffCode);
        Assert.Equal("Binh Nguyen", result.Value.Items[0].FullName);
    }

    [Fact]
    public async Task Handle_Should_Ignore_SearchTerm_When_SearchTerm_Is_Whitespace()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock(CreateUsers());
        var currentUserMock = CreateCurrentUserMock(LocationId.ToString());
        var request = new Request(1, 10, "   ", null, null, null);

        // Act
        var result = await HandleAsync(userManagerMock, currentUserMock, request);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(3, result.Value.TotalCount);
        Assert.Equal(3, result.Value.Items.Count);
    }

    [Fact]
    public async Task Handle_Should_Not_Filter_Users_When_Type_Is_Invalid()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock(CreateUsers());
        var currentUserMock = CreateCurrentUserMock(LocationId.ToString());
        var request = new Request(1, 10, null, "Unknown", null, null);

        // Act
        var result = await HandleAsync(userManagerMock, currentUserMock, request);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(3, result.Value.TotalCount);
        Assert.Equal(3, result.Value.Items.Count);
    }

    [Fact]
    public async Task Handle_Should_Sort_Users_By_StaffCode_Ascending_When_SortBy_Is_StaffCode()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock(CreateUsers());
        var currentUserMock = CreateCurrentUserMock(LocationId.ToString());
        var request = new Request(1, 10, null, null, "staffCode", false);

        // Act
        var result = await HandleAsync(userManagerMock, currentUserMock, request);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(
            ["SD1901", "SD1902", "SD1903"],
            result.Value.Items.Select(user => user.StaffCode));
    }

    [Fact]
    public async Task Handle_Should_Sort_Users_By_FullName_Descending_When_SortDesc_Is_True()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock(CreateUsers());
        var currentUserMock = CreateCurrentUserMock(LocationId.ToString());
        var request = new Request(1, 10, null, null, "fullName", true);

        // Act
        var result = await HandleAsync(userManagerMock, currentUserMock, request);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(
            ["Lan Pham", "Binh Nguyen", "An Tran"],
            result.Value.Items.Select(user => user.FullName));
    }

    [Fact]
    public async Task Handle_Should_Mark_User_CanBeDisabled_When_User_Has_No_Active_Assignments()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock(CreateUsers());
        var currentUserMock = CreateCurrentUserMock(LocationId.ToString());
        var request = new Request(1, 10, null, null, "staffCode", false);

        // Act
        var result = await HandleAsync(userManagerMock, currentUserMock, request);

        // Assert
        Assert.False(result.IsError);
        Assert.True(result.Value.Items[0].CanBeDisabled);
        Assert.True(result.Value.Items[1].CanBeDisabled);
        Assert.False(result.Value.Items[2].CanBeDisabled);
    }

    [Fact]
    public async Task Handle_Should_Return_Requested_Page_When_PageNumber_And_PageSize_Are_Set()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock(CreateUsers());
        var currentUserMock = CreateCurrentUserMock(LocationId.ToString());
        var request = new Request(2, 1, null, null, "staffCode", false);

        // Act
        var result = await HandleAsync(userManagerMock, currentUserMock, request);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(3, result.Value.TotalCount);
        Assert.Equal(2, result.Value.PageNumber);
        Assert.Equal(1, result.Value.PageSize);
        Assert.Single(result.Value.Items);
        Assert.Equal("SD1902", result.Value.Items[0].StaffCode);
    }

    private static async Task<ErrorOr<PagedList<Response>>> HandleAsync(
        Mock<UserManager<User>> userManagerMock,
        Mock<ICurrentUser> currentUserMock,
        Request request)
    {
        var handlerType = typeof(Request).Assembly.GetType(
            "NashAssetManagement.Application.UseCases.Users.ViewUsers.Handler",
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
        var task = (Task<ErrorOr<PagedList<Response>>>)handleMethod.Invoke(
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
                Id = Guid.Parse("36c29308-4d9c-4e1b-9baf-a5dc11f26001"),
                StaffCode = "SD1901",
                FirstName = "An",
                LastName = "Tran",
                UserName = "ant",
                JoinedAtUtc = new DateTime(2022, 8, 10, 0, 0, 0, DateTimeKind.Utc),
                UserType = UserType.Staff,
                CreatedAtUtc = new DateTime(2022, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                LocationId = LocationId,
                ReceivedAssignments = []
            },
            new()
            {
                Id = Guid.Parse("36c29308-4d9c-4e1b-9baf-a5dc11f26002"),
                StaffCode = "SD1902",
                FirstName = "Lan",
                LastName = "Pham",
                UserName = "lanp",
                JoinedAtUtc = new DateTime(2023, 5, 20, 0, 0, 0, DateTimeKind.Utc),
                UserType = UserType.Admin,
                CreatedAtUtc = new DateTime(2023, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                LocationId = LocationId,
                ReceivedAssignments =
                [
                    new Assignment
                    {
                        State = AssignmentState.Declined
                    }
                ]
            },
            new()
            {
                Id = Guid.Parse("36c29308-4d9c-4e1b-9baf-a5dc11f26003"),
                StaffCode = "SD1903",
                FirstName = "Binh",
                LastName = "Nguyen",
                UserName = "binhn",
                JoinedAtUtc = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                UserType = UserType.Staff,
                CreatedAtUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                LocationId = LocationId,
                ReceivedAssignments =
                [
                    new Assignment
                    {
                        State = AssignmentState.Accepted
                    }
                ]
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
        return new TestAsyncEnumerable<TEntity>(TestExpressionRewriter.Rewrite(expression));
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(TestExpressionRewriter.Rewrite(expression));
    }

    public object? Execute(Expression expression)
    {
        return inner.Execute(TestExpressionRewriter.Rewrite(expression));
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return inner.Execute<TResult>(TestExpressionRewriter.Rewrite(expression));
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
            .Invoke(inner, [TestExpressionRewriter.Rewrite(expression)]);

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
        : base(TestExpressionRewriter.Rewrite(expression))
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

internal sealed class TestExpressionRewriter : ExpressionVisitor
{
    private static readonly MethodInfo LikeMethod = typeof(TestEfFunctions)
        .GetMethod(nameof(TestEfFunctions.Like))!;

    public static Expression Rewrite(Expression expression)
    {
        return new TestExpressionRewriter().Visit(expression);
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.DeclaringType == typeof(DbFunctionsExtensions) &&
            node.Method.Name == nameof(DbFunctionsExtensions.Like) &&
            node.Arguments.Count == 3)
        {
            return Expression.Call(
                LikeMethod,
                Visit(node.Arguments[1]),
                Visit(node.Arguments[2]));
        }

        return base.VisitMethodCall(node);
    }
}

internal static class TestEfFunctions
{
    public static bool Like(string? matchExpression, string? pattern)
    {
        if (matchExpression is null || pattern is null)
        {
            return false;
        }

        var value = matchExpression.ToLowerInvariant();
        var normalizedPattern = pattern.ToLowerInvariant();

        if (normalizedPattern.StartsWith('%') && normalizedPattern.EndsWith('%'))
        {
            return value.Contains(normalizedPattern.Trim('%'));
        }

        if (normalizedPattern.StartsWith('%'))
        {
            return value.EndsWith(normalizedPattern.TrimStart('%'));
        }

        if (normalizedPattern.EndsWith('%'))
        {
            return value.StartsWith(normalizedPattern.TrimEnd('%'));
        }

        return value == normalizedPattern;
    }
}
