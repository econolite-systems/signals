// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Authorization;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.Signals;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Econolite.Ode.Ssm.Signals;

public class RawSsmConsumer : ScalingConsumer<Guid, OdeSsmData>, IRawSsmConsumer 
{
    private readonly ITokenHandler _tokenHandler;
    private readonly HttpClient _client;
    private readonly string _url;

    public RawSsmConsumer(
        IConfiguration configuration,
        ITokenHandler tokenHandler,
        HttpClient client,
        IBuildMessagingConfig<Guid, OdeSsmData> buildMessagingConfig,
        IOptions<ScalingConsumerOptions<Guid, OdeSsmData>> options,
        IConsumeResultFactory<Guid, OdeSsmData> consumeResultFactory,
        ILogger<Consumer<Guid, OdeSsmData>> logger) : base(buildMessagingConfig, options, consumeResultFactory, logger)
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

public interface IRawSsmConsumer : IScalingConsumer<Guid, OdeSsmData>
{
    Task<IDictionary<int, Guid>> GetMapIdsToSignalIdsAsync(CancellationToken cancellationToken = default);
}

public static class RawSsmConsumerExtensions
{
    public static IEnumerable<SsmStatus> ToSsmStatus(this Asn1J2735.J2735.SignalStatusMessage ssm, IDictionary<int, Guid> mapIdsToSignalIds)
    {
        return ssm.Status.Where(i => mapIdsToSignalIds.Keys.Contains(i.Id.Id)).Select(i => i.ToSsmStatus(mapIdsToSignalIds[i.Id.Id]))!;
    }

    public static SsmStatus ToSsmStatus(this Asn1J2735.J2735.SignalStatus status, Guid id)
    {
        return new SsmStatus()
        {
            Id = id,
            ReferenceId = status.Id.Id,
            Timestamp = DateTime.UtcNow,
            SequenceNumber = status.SequenceNumber,
        };
    }
}
