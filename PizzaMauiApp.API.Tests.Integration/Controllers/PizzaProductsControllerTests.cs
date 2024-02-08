using System.Net;
using System.Net.Http.Headers;
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
public class PizzaProductsControllerTests
{
    IntegrationTestFixture _fixture;
    
    public PizzaProductsControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task GET_should_throw_unauthorized_if_token_is_not_provided_upon_get()
    {
        var response = await _fixture.Client.GetAsync("/api/PizzaProducts");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact, TestPriority(1)]
    public async Task GET_checkGetAllPizzaProducts_OK()
    {
        if(!string.IsNullOrEmpty(_fixture.Token))
            _fixture.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.Token);
        
        var response = await _fixture.Client.GetAndDeserialize<ApiResponse<IReadOnlyList<PizzaProductDto>>>("/api/PizzaProducts");
        response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        response.Success.Should().BeTrue();
    }
    
    [Fact, TestPriority(2)]
    public async Task GET_checkGetAllPizzaProducts_Count()
    {
        if(!string.IsNullOrEmpty(_fixture.Token))
            _fixture.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.Token);
        
        //from seeding; PizzaMauiApp.API.Infrastructure.SeedData.StoreDbContextSeed
        var response = await _fixture.Client.GetAndDeserialize<ApiResponse<IReadOnlyList<PizzaProductDto>>>("/api/PizzaProducts");
        response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        response.Success.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data.Count.Should().Be(12);
    }
}