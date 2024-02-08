using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using PizzaMauiApp.API.Dtos;
using PizzaMauiApp.API.Helpers.API;
using PizzaMauiApp.API.Tests.Integration.Fixtures;
using PizzaMauiApp.API.Tests.Integration.Helpers;
using PizzaMauiApp.API.Tests.Integration.Utils;

namespace PizzaMauiApp.API.Tests.Integration;


[Collection("integrationTest collection")]
[TestCaseOrderer(
    ordererTypeName: "PizzaMauiApp.API.Tests.Integration.Helpers.PriorityOrderer",
    ordererAssemblyName: "PizzaMauiApp.API.Tests.Integration")]
public class AccountControllerTests
{
    IntegrationTestFixture _fixture;
    private readonly string _email;
    private readonly string _password;
    
    public AccountControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _email = "test@test.com";
        _password = "1234e5YUR$%^&6";
    }
    
    [Fact, TestPriority(1)]
    public async Task POST_checkLoginWithoutRegisterUser_Endpoint()
    {
        _fixture.Client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        
        var response = await _fixture.Client.PostAsJsonAsync("api/Accounts/login", 
            new UserLoginDto
            {
                Email = _email, 
                Password = _password
            } );
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseStream = await response.Content.ReadAsStreamAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var responseData = await JsonSerializer.DeserializeAsync<ApiResponse<UserIdentityDto?>>(responseStream, options);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseData.Should().NotBeNull();
        responseData!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        responseData!.Data.Should().BeNull();
    }
    
    [Fact, TestPriority(2)]
    public async Task POST_checkRegisterUser_Endpoint()
    {
        _fixture.Client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        
        var response = await _fixture.Client.PostAsJsonAsync("api/Accounts/register", 
            new UserRegisterDto
            {
                Email = _email, 
                Password = _password
            } );
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseStream = await response.Content.ReadAsStreamAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var responseData = await JsonSerializer.DeserializeAsync<ApiResponse<UserIdentityDto?>>(responseStream, options);

        responseData.Should().NotBeNull();
        responseData!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        responseData!.Data.Should().NotBeNull();
        responseData!.Data.Token.Should().NotBeNullOrEmpty();
        responseData!.Data.RefreshToken.Should().NotBeNullOrEmpty();
    }
    
    [Fact,  TestPriority(3)]
    public async Task GET_checkIfPreviousEmailCanBeRetrieved()
    {
        var response = await _fixture.Client.GetAndDeserialize<ApiResponse<bool?>>($"api/Accounts/emailexists?email={_email}");
        response?.StatusCode.Should().Be((int)HttpStatusCode.OK);
        response.Should().NotBeNull();
        response!.Data.Should().BeTrue();
    }
    
    [Fact, TestPriority(4)]
    public async Task POST_checkLoginUser_EndpointOK()
    {
        _fixture.Client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        
        var response = await _fixture.Client.PostAsJsonAsync("api/Accounts/login", 
            new UserLoginDto
            {
                Email = _email, 
                Password = _password
            } );
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseStream = await response.Content.ReadAsStreamAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var responseData = await JsonSerializer.DeserializeAsync<ApiResponse<UserIdentityDto?>>(responseStream, options);

        responseData.Should().NotBeNull();
        responseData!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        responseData!.Data.Should().NotBeNull();
        responseData!.Data.Token.Should().NotBeNullOrEmpty();
        responseData!.Data.RefreshToken.Should().NotBeNullOrEmpty();
    }
}