// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Authorization;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.Signals;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Econolite.Ode.Srm.Signals;

public class RawSrmConsumer : ScalingConsumer<Guid, OdeSrmData>, IRawSrmConsumer 
{
    private readonly ITokenHandler _tokenHandler;
    private readonly HttpClient _client;
    private readonly string _url;

    public RawSrmConsumer(
        IConfiguration configuration,
        ITokenHandler tokenHandler,
        HttpClient client,
        IBuildMessagingConfig<Guid, OdeSrmData> buildMessagingConfig,
        IOptions<ScalingConsumerOptions<Guid, OdeSrmData>> options,
        IConsumeResultFactory<Guid, OdeSrmData> consumeResultFactory,
        ILogger<Consumer<Guid, OdeSrmData>> logger) : base(buildMessagingConfig, options, consumeResultFactory, logger)
    {
        _tokenHandler = tokenHandler;
        _client = client;
        _url = configuration.GetValue("Services:Configuration", "http://localhost:5138")!;
    }
    
    public async Task<IDictionary<int, Guid>> GetMapIdsToSignalIdsAsync(CancellationToken cancellationToken = default)
    {
        var url = $"{_url}/entities/types/Signal";
        _client.DefaultRequestHeaders.Authorization = await _tokenHandler.GetAuthHeaderAsync(cancellationToken);
        var response = await _client.GetAsync(url);
        var content = response.Content.ReadAsStringAsync();
        var entities = JsonSerializer.Deserialize<IEnumerable<EntityNode>>(content.Result, JsonPayloadSerializerOptions.Options);
        var signals = entities?.ToArray() ?? Array.Empty<EntityNode>();
        return signals.Where(s => s.IdMapping.HasValue).ToDictionary(s => s.IdMapping!.Value, s => s.Id);
    }
}

public interface IRawSrmConsumer : IScalingConsumer<Guid, OdeSrmData>
{
    Task<IDictionary<int, Guid>> GetMapIdsToSignalIdsAsync(CancellationToken cancellationToken = default);
}

public static class RawSrmConsumerExtensions
{
    public static IEnumerable<SrmStatus> ToSrmStatus(this Asn1J2735.J2735.SignalRequestMessage srm, IDictionary<int, Guid> mapIdsToSignalIds)
    {
        return srm.Requests.Where(i => mapIdsToSignalIds.Keys.Contains(i.Request.Id.Id)).Select(i => i.ToSrmStatus(mapIdsToSignalIds[i.Request.Id.Id]))!;
    }

    public static SrmStatus ToSrmStatus(this Asn1J2735.J2735.SignalRequestPackage request, Guid id)
    {
        return new SrmStatus()
        {
            Id = id,
            ReferenceId = request.Request.Id.Id,
            Timestamp = DateTime.UtcNow,
        };
    }
}
