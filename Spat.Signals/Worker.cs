// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Domain.Asn1.J2735;
using Econolite.Ode.Monitoring.Events;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.Metrics;
using Econolite.Ode.Status.Signal;
using Status.Signal.Messaging;
using Status.Bsm.Messaging;

namespace Econolite.Ode.Spat.Signals;

public class Worker : BackgroundService
{
    private readonly Guid _tenantId;
    private readonly ISignalStatusProducer _producer;
    private readonly IBsmMessageProducer _bsmProducer;
    private readonly IRawSpatConsumer _consumer;
    private readonly ILogger<Worker> _logger;
    private readonly IAsn1J2735Service _asn1J2735Service;
    private readonly UserEventFactory _userEventFactory;
    private readonly string _topic;
    private readonly IMetricsCounter _loopCounter;

    public Worker(IConfiguration configuration, ISignalStatusProducer producer, IBsmMessageProducer bsmProducer, IRawSpatConsumer consumer, ILogger<Worker> logger, IAsn1J2735Service asn1J2735Service, UserEventFactory userEventFactory, IMetricsFactory metricsFactory)
    {
        _tenantId = configuration.GetValue("Tenant:Id", Guid.Empty);
        _topic = configuration.GetValue("Topics:OdeRawEncodedSPATJsonTopic", "topic.OdeRawEncodedSPATJson")!;
        _producer = producer;
        _bsmProducer = bsmProducer;
        _consumer = consumer;
        _logger = logger;
        _asn1J2735Service = asn1J2735Service;
        _userEventFactory = userEventFactory;

        _loopCounter = metricsFactory.GetMetricsCounter("Ode");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting the main loop");
        try
        {
            _logger.LogInformation("Consuming config response from topic: {@}", _topic);
            await _consumer.ConsumeOn(_topic, async data =>
            {
                try
                {
                    _logger.LogInformation("Received Spat");
                    var spats = data.Value.SpatMessageContent.Select(s => _asn1J2735Service.DecodeSpat(s.Payload)).ToArray();
                    if (spats == null || spats.Length == 0)
                    {
                        _logger.LogInformation("No SPATs found in the message");
                        _loopCounter.Increment(1);
                        return;
                    }
                    var ids = await _consumer.GetMapIdsToSignalIdsAsync(stoppingToken);
                    foreach (var spat in spats)
                    {
                        var signalStatuses = spat?.ToSignalStatus(ids) ?? Array.Empty<SignalStatus>();
                        foreach (var signalStatus in signalStatuses)
                        {
                            var message = spat?.ToJsonDocument();
                            await Task.WhenAll(_producer.ProduceAsync(_tenantId, signalStatus, stoppingToken), _bsmProducer.ProduceAsync(_tenantId, message, stoppingToken));
                        }
                    }

                    _loopCounter.Increment(spats.Length);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to process Spat data from: {@}", data);

                    _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Error, string.Format("Unable to process Spat data from: {0}", data?.Value?.SpatMessageContent?.Length)));
                }
            }, stoppingToken);
        }
        finally
        {
            _logger.LogInformation("Ending the main loop");
        }
    }
}
