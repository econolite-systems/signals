// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Asn1J2735.J2735;
using Econolite.Ode.Authorization;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.Signals;
using Econolite.Ode.Models.Signals.Utilities;
using Econolite.Ode.Status.Common;
using Econolite.Ode.Status.Signal;
using Microsoft.Extensions.Options;
using System.Text.Json;
using SignalStatus = Econolite.Ode.Status.Signal.SignalStatus;

namespace Econolite.Ode.Spat.Signals;

public class RawSpatConsumer : IRawSpatConsumer
{
    private readonly HttpClient _client;
    private readonly string? _url;
    private readonly ITokenHandler _tokenHandler;
    private readonly ISource<OdeSpatData> _source;

    public RawSpatConsumer(
        IConfiguration configuration,
        ITokenHandler tokenHandler,
        HttpClient client,
        ISource<OdeSpatData> source)
    {
        _client = client;
        _url = configuration.GetValue<string>("Services:Configuration");
        if (_url == null)
        {
            throw new Exception("Configuration value for 'Services:Configuration' not found");
        }
        _tokenHandler = tokenHandler;
        _source = source;
    }

    public async Task<IDictionary<int, Guid>> GetMapIdsToSignalIdsAsync(CancellationToken cancellationToken = default)
    {
        var url = $"{_url}/entities/types/Signal";
        _client.DefaultRequestHeaders.Authorization = await _tokenHandler.GetAuthHeaderAsync(cancellationToken);
        var response = await _client.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var entities = JsonSerializer.Deserialize<IEnumerable<EntityNode>>(content, JsonPayloadSerializerOptions.Options);
        var signals = entities?.ToArray() ?? Array.Empty<EntityNode>();
        return signals.Where(s => s.IdMapping.HasValue).ToDictionary(s => s.IdMapping!.Value, s => s.Id);
    }

    public async Task ConsumeOn(string topic, Func<ConsumeResult<Guid, OdeSpatData>, Task> consumeFunc, CancellationToken cancellationToken)
    {
        await _source.ConsumeOnAsync(topic, consumeFunc, cancellationToken);
    }
}


public interface IRawSpatConsumer : IScalingConsumer<Guid, OdeSpatData>
{
    Task<IDictionary<int, Guid>> GetMapIdsToSignalIdsAsync(CancellationToken cancellationToken = default);
}

public static class RawSpatConsumerExtensions
{
    public static IEnumerable<SignalStatus> ToSignalStatus(this Asn1J2735.J2735.Spat spat, IDictionary<int, Guid> mapIdsToSignalIds)
    {
        return spat.Intersections.Where(i => mapIdsToSignalIds.Keys.Contains(i.Id.Id)).Select(i => i.ToSignalStatus(mapIdsToSignalIds[i.Id.Id]))!;
    }

    public static JsonDocument ToJsonDocument(this Asn1J2735.J2735.Spat spat)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(spat, options);
        JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
        var jsonObject = new Dictionary<string, object>(jsonDocument.ToObject<Dictionary<string, object>>())
        {
            { "bsm", "j2735" }
        };
        jsonString = JsonSerializer.Serialize(jsonObject, options);
        jsonDocument = JsonDocument.Parse(jsonString);

        return jsonDocument;
    }
    public static T ToObject<T>(this JsonDocument document)
    {
        var json = document.RootElement.GetRawText();
        return JsonSerializer.Deserialize<T>(json);
    }

    public static T ToObject<T>(this JsonElement element)
    {
        var json = element.GetRawText();
        return JsonSerializer.Deserialize<T>(json);
    }



    public static SignalStatus? ToSignalStatus(this IntersectionState intersectionState, Guid signalId)
    {
        var status = new SignalStatus()
        {
            DeviceId = signalId,
            PhaseGreen = intersectionState.ToPhaseGreen(),
            PhaseYellow = intersectionState.ToPhaseYellow(),
            PhaseRed = intersectionState.ToPhaseRed(),
            PhaseFlash = intersectionState.ToPhaseFlash(),
            PedCalls = intersectionState.ToPedCalls(),
            CommStatus = intersectionState.ToCommStatus(),
            CommSuccessRate = 100,
            TimeStamp = DateTime.UtcNow,
            SignalStatusSource = SignalStatusSource.SPaTMessage,
            CoordPattern = 1
        };
        return status;
    }

    private static CommStatus ToCommStatus(this IntersectionState state)
    {
        var result = state.Status.Values.Any(v => v) ? CommStatus.Unknown : CommStatus.Good;

        if (result == CommStatus.Good)
        {
            return result;
        }

        var status = state.Status.Where(kv => kv.Value).Select(kv => Enum.Parse<IntersectionStatusObject>(kv.Key).ToCommStatus()).ToList();

        result = status.Contains(CommStatus.Good) ? CommStatus.Good : status.First();

        return result;
    }

    private static CommStatus ToCommStatus(this IntersectionStatusObject status)
    {
        switch (status)
        {
            case IntersectionStatusObject.Off:
                return CommStatus.Offline;
            case IntersectionStatusObject.StandbyOperation:
                return CommStatus.Standby;
            case IntersectionStatusObject.FailureFlash:
            case IntersectionStatusObject.FailureMode:
            case IntersectionStatusObject.NoValidSPATisAvailableAtThisTime:
                return CommStatus.Bad;
            case IntersectionStatusObject.ManualControlIsEnabled:
            case IntersectionStatusObject.StopTimeIsActivated:
            case IntersectionStatusObject.PreemptIsActive:
            case IntersectionStatusObject.FixedTimeOperation:
            case IntersectionStatusObject.TrafficDependentOperation:
            case IntersectionStatusObject.RecentMAPmessageUpdate:
            case IntersectionStatusObject.RecentChangeInMAPassignedLanesIDsUsed:
            case IntersectionStatusObject.NoValidMAPisAvailableAtThisTime:
                return CommStatus.Good;
            case IntersectionStatusObject.SignalPriorityIsActive:
            default:
                return CommStatus.Unknown;
        }
    }

    private static ulong ToPhaseData(this IntersectionState state, Func<MovementPhaseState, bool> predicate)
    {
        ulong phaseData = 0;
        state.States.ToList().ForEach(s =>
        {
            BitMapFlag.SetBit(ref phaseData, (s.SignalGroup - 1),
                s.StateTimeSpeed.Any(m => predicate(m.EventState)));
        });
        return phaseData;
    }

    private static ulong ToPhaseGreen(this IntersectionState state)
    {
        return state.ToPhaseData((movement) => movement is MovementPhaseState.ProtectedMovementAllowed or MovementPhaseState.PermissiveMovementAllowed);
    }

    private static ulong ToPhaseYellow(this IntersectionState state)
    {
        return state.ToPhaseData((movement) => movement is MovementPhaseState.ProtectedClearance or MovementPhaseState.PermissiveClearance);
    }

    private static ulong ToPhaseRed(this IntersectionState state)
    {
        return state.ToPhaseData((movement) => movement is MovementPhaseState.StopAndRemain);
    }

    private static ulong ToPhaseFlash(this IntersectionState state)
    {
        return state.ToPhaseData((movement) =>
            movement is MovementPhaseState.StopThenProceed or MovementPhaseState.CautionConflictingTraffic);
    }

    private static ulong ToPedCalls(this IntersectionState state)
    {
        ulong pedData = 0;
        state.ManeuverAssistList.Where(m => m.PedBicycleDetect ?? false).ToList().ForEach(m =>
        {
            BitMapFlag.SetBit(ref pedData, (m.ConnectionID - 1), true);
        });
        return pedData;
    }
}
