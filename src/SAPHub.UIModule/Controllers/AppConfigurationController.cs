using Microsoft.AspNetCore.Mvc;
using System;
using SAPHub.UI.Shared;

namespace SAPHub.UI.Controllers;

[ApiController]
[Route("configuration")]
public class AppConfigurationController : Controller
{
    private readonly EndpointResolver _endpointResolver;

    public AppConfigurationController(EndpointResolver endpointResolver)
    {
        _endpointResolver = endpointResolver;
    }

    [HttpGet]
    public ClientConfiguration Get()
    {
        var apiEndpoint = _endpointResolver.GetEndpoint("api");
        if(!apiEndpoint.AbsolutePath.EndsWith('/'))
            apiEndpoint = new Uri(apiEndpoint.OriginalString+"/");

        return new ClientConfiguration
        {
            ApiEndpoint = apiEndpoint.ToString()
        };
    }
}