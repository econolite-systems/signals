// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Domain.Asn1.J2735;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.Signals;
using Econolite.Ode.Monitoring.Events;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.Metrics;
using Econolite.Ode.Repository.Signals;

namespace Econolite.Ode.Srm.Signals;

public class Worker : BackgroundService
{
    private readonly ISignalRequestMessageRepository _signalRequestMessageRepository;
    private readonly IRawSrmConsumer _consumer;
    private readonly ILogger<Worker> _logger;
    private readonly IAsn1J2735Service _asn1J2735Service;
    private readonly UserEventFactory _userEventFactory;
    private readonly Guid _tenantId;
    private readonly string _topic;
    private readonly IMetricsCounter _loopCounter;

    public Worker(IConfiguration configuration, IServiceProvider serviceProvider, IRawSrmConsumer consumer, ILogger<Worker> logger, IAsn1J2735Service asn1J2735Service, UserEventFactory userEventFactory, IMetricsFactory metricsFactory)
    {
        var serviceScope = serviceProvider.CreateScope();
        _tenantId = configuration.GetValue("Tenant:Id", Guid.Empty);
        _topic = configuration.GetValue("Topics:OdeRawEncodedSrmJsonTopic", "topic.OdeRawEncodedSRMJson")!;
        _signalRequestMessageRepository =
            serviceScope.ServiceProvider.GetRequiredService<ISignalRequestMessageRepository>();
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
                    _logger.LogInformation("Received Srm");
                    var messages = data.Value.SrmMessageContent.Select(s => _asn1J2735Service.DecodeSrm(s.Payload)).ToArray();
                    if (messages == null || messages.Length == 0)
                    {
                        _logger.LogInformation("No Srm found in the message");
                        _loopCounter.Increment(1);
                        return;
                    }

                    var ids = await _consumer.GetMapIdsToSignalIdsAsync(stoppingToken);
                    foreach (var message in messages)
                    {
                        var statuses = message?.ToSrmStatus(ids).ToArray() ?? Array.Empty<SrmStatus>();

                        foreach (var status in statuses)
                        {
                            _signalRequestMessageRepository.Add(status);
                        }

                        if (statuses.Any())
                        {
                            var (success, errors) = await _signalRequestMessageRepository.DbContext.SaveChangesAsync();
                            if (!success)
                            {
                                _logger.LogError("Failed to save SRM status messages: {@Errors}", errors);
                            }
                        }
                    }

                    _loopCounter.Increment(messages.Length);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to process Spat data from: {@}", data);

                    _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Error, string.Format("Unable to process SRM data from: {0}", data?.Value?.SrmMessageContent?.Length)));
                }
            }, stoppingToken);
        }
        finally
        {
            _logger.LogInformation("Ending the main loop");
        }
    }
}
