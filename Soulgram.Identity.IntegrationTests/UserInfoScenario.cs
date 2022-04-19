using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using soulgram.identity;
using soulgram.identity.Models;
using Xunit;

namespace Soulgram.Identity.IntegrationTests;

public class UserInfoScenario : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;

    public UserInfoScenario(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetUserToken_successfully_return_token()
    {
        //Arrange
        //Act
        var responseToken = await GetTestToken();

        // Assert
        responseToken.AccessToken.Should().NotBeNullOrEmpty();
        responseToken.TokenType.Should().Be("Bearer");
    }

    [Fact]
    public async Task GetUser_returns_user_info()
    {
        // Arrange
        var token = await GetTestToken();
        var client = _factory.CreateClient();

        var authHeader = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
        client.DefaultRequestHeaders.Authorization = authHeader;

        // Act
        var response = await client.GetAsync("api/account");

        //Assert
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var user = JsonConvert.DeserializeObject<ApplicationUser>(responseString);
        user.Email.Should().NotBeNull();
    }

    private async Task<TestTokenModel> GetTestToken()
    {
        var client = _factory.CreateClient();

        var queryParams = new Dictionary<string, string>
        {
            { "client_id", "test_human_client" },
            { "grant_type", "password" },
            { "password", "DimaPassword123#" },
            { "username", "dimanoit" }
        };

        var response = await client.PostAsync("connect/token", new FormUrlEncodedContent(queryParams));
        var responseString = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<TestTokenModel>(responseString);
    }
}