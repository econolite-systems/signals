// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Net;
using System.Net.Http.Json;
using Econolite.Ode.Extensions.Service;
using Econolite.Ode.Models.DeviceManager.Db;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Econolite.Ode.Domain.Signals;

public interface ISignalS2S
{
    Task<Rsu> GetRsuAsync(Guid signalId);
}

public class SignalS2S : ISignalS2S
{
    private readonly IServiceAuthentication _serviceAuthentication;
    private readonly HttpClient _client;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SignalS2S> _logger;
    private readonly string _uri;
    private readonly int _retries;

    public SignalS2S(IHttpClientFactory httpClientFactory, IServiceAuthentication serviceAuthentication, ILogger<SignalS2S> logger, IOptions<SignalS2SOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _serviceAuthentication = serviceAuthentication;
        _logger = logger;
        _uri = options.Value.Uri.Trim('/');
        _retries = options.Value.Retries;
        _client = _httpClientFactory.CreateClient("serviceAuth");
    }

    public async Task<Rsu> GetRsuAsync(Guid signalId)
    {
        var result = new Rsu();
        //var response = await this.GetSignalAsync(signalId);
        return result;
    }

    // public async Task<SignalDto> GetSignalAsync(Guid signalId, bool resetToken = false, int times = 0)
    // {
    //     var result = new SignalDto();
    //     try
    //     {
    //         var url = $"${_uri}/signal/{signalId}";
    //         _client.DefaultRequestHeaders.Authorization = await _serviceAuthentication.GetAuthHeaderAsync(resetToken);
    //         var response = await _client.GetFromJsonAsync<SignalDto>(url);
    //         return response ?? result;
    //     }
    //     catch (HttpRequestException ex)
    //     {
    //         if (ex.StatusCode == HttpStatusCode.Unauthorized && times > _retries)
    //         {
    //             return await GetSignalAsync(signalId, true, times++);
    //         }
    //         else
    //         {
    //             return result;
    //         }
    //     }
    // }
}
