using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace SAPHub;

public class EndpointResolver
{
    private readonly Dictionary<string,string> _endpoints = new();

    public EndpointResolver(IConfiguration configuration)
    {
        configuration.Bind("endpoints",_endpoints);

    }

    public Uri GetEndpoint(string name)
    {
        Uri endpoint = null;
        var isDefault = false;
        if (_endpoints.TryGetValue(name, out var endpointString))
        {
            endpoint = endpointString.StartsWith("http")
                ? new Uri(endpointString, UriKind.Absolute)
                : new Uri(endpointString, UriKind.Relative);
        }

        if (endpoint == null)
        {
            endpoint = new Uri(_endpoints["default"]);
            isDefault = true;
        }

        if (endpoint.IsAbsoluteUri || isDefault) return endpoint;

        var defaultEndpoint = new Uri(_endpoints["default"]);
        return new Uri(defaultEndpoint, endpoint);
    }
}