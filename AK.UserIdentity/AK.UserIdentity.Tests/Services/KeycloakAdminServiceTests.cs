using AK.BuildingBlocks.Authentication;
using AK.UserIdentity.API.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace AK.UserIdentity.Tests.Services;

public sealed class KeycloakAdminServiceTests
{
    private readonly KeycloakSettings _settings = new()
    {
        Authority = "http://localhost:8090/realms/antkart",
        Audience = "antkart-client",
        AdminUrl = "http://localhost:8090",
        Realm = "antkart",
        ClientId = "antkart-client",
        ClientSecret = "antkart-secret"
    };

    private IHttpClientFactory CreateHttpClientFactory(params HttpResponseMessage[] responses)
    {
        var queue = new Queue<HttpResponseMessage>(responses);
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => queue.Count > 0 ? queue.Dequeue() : new HttpResponseMessage(HttpStatusCode.OK));

        var client = new HttpClient(handlerMock.Object);
        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
        return factoryMock.Object;
    }

    [Fact]
    public async Task GetUsersAsync_ReturnsListOfUsers()
    {
        var usersJson = JsonSerializer.Serialize(new[]
        {
            new { id = "id1", username = "alice", email = "alice@example.com", firstName = "Alice", lastName = "Smith", enabled = true },
            new { id = "id2", username = "bob", email = "bob@example.com", firstName = "Bob", lastName = "Jones", enabled = true }
        });

        var factory = CreateHttpClientFactory(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(usersJson, System.Text.Encoding.UTF8, "application/json")
        });

        var svc = new KeycloakAdminService(factory, Options.Create(_settings));
        var result = await svc.GetUsersAsync("admin.token");

        result.Should().HaveCount(2);
        result[0].Username.Should().Be("alice");
        result[1].Username.Should().Be("bob");
    }

    [Fact]
    public async Task GetUsersAsync_WhenApiFails_ThrowsInvalidOperationException()
    {
        var factory = CreateHttpClientFactory(new HttpResponseMessage(HttpStatusCode.Forbidden));

        var svc = new KeycloakAdminService(factory, Options.Create(_settings));
        var act = async () => await svc.GetUsersAsync("bad.token");

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task AssignRoleAsync_WithValidRole_Succeeds()
    {
        var roleJson = JsonSerializer.Serialize(new { id = "role-id-1", name = "user" });

        var factory = CreateHttpClientFactory(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(roleJson, System.Text.Encoding.UTF8, "application/json")
            },
            new HttpResponseMessage(HttpStatusCode.NoContent));

        var svc = new KeycloakAdminService(factory, Options.Create(_settings));
        var act = async () => await svc.AssignRoleAsync("user-id", "user", "admin.token");

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AssignRoleAsync_WithNonExistentRole_ThrowsKeyNotFoundException()
    {
        var factory = CreateHttpClientFactory(new HttpResponseMessage(HttpStatusCode.NotFound));

        var svc = new KeycloakAdminService(factory, Options.Create(_settings));
        var act = async () => await svc.AssignRoleAsync("user-id", "nonexistent", "admin.token");

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetAdminTokenAsync_WithValidClientCredentials_ReturnsAccessToken()
    {
        var tokenJson = JsonSerializer.Serialize(new
        {
            access_token = "service.account.token",
            expires_in = 60,
            token_type = "Bearer"
        });

        var factory = CreateHttpClientFactory(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(tokenJson, System.Text.Encoding.UTF8, "application/json")
        });

        var svc = new KeycloakAdminService(factory, Options.Create(_settings));
        var token = await svc.GetAdminTokenAsync();

        token.Should().Be("service.account.token");
    }

    [Fact]
    public async Task AssignRoleAsync_WhenRoleAssignmentPostFails_ThrowsInvalidOperationException()
    {
        // Step 1 (GET role) succeeds; Step 2 (POST role-mapping) returns 500.
        var roleJson = JsonSerializer.Serialize(new { id = "role-id-1", name = "admin" });

        var factory = CreateHttpClientFactory(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(roleJson, System.Text.Encoding.UTF8, "application/json")
            },
            new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var svc = new KeycloakAdminService(factory, Options.Create(_settings));
        var act = async () => await svc.AssignRoleAsync("user-id", "admin", "admin.token");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Failed to assign role*");
    }

    [Fact]
    public async Task GetUsersAsync_WithUsersHavingNoOptionalFields_ReturnsEmptyStringsForMissingFields()
    {
        // Keycloak users registered with only username — no firstName, lastName, or email set.
        var usersJson = JsonSerializer.Serialize(new[]
        {
            new { id = "id-minimal", username = "minimaluser", enabled = false }
        });

        var factory = CreateHttpClientFactory(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(usersJson, System.Text.Encoding.UTF8, "application/json")
        });

        var svc = new KeycloakAdminService(factory, Options.Create(_settings));
        var result = await svc.GetUsersAsync("admin.token");

        result.Should().HaveCount(1);
        result[0].Id.Should().Be("id-minimal");
        result[0].Username.Should().Be("minimaluser");
        result[0].Email.Should().BeEmpty();
        result[0].FirstName.Should().BeEmpty();
        result[0].LastName.Should().BeEmpty();
        result[0].Enabled.Should().BeFalse();
    }
}
