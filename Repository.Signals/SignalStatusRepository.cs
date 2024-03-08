// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Buffers;
using Econolite.Ode.Status.Signal;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Econolite.Ode.Repository.Signals
{
    public class SignalStatusRepository : ISignalStatusRepository
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _redisDb;
        private readonly Guid _tenantId = Guid.Empty;

        public SignalStatusRepository(IConfiguration configuration)
        {
            _redis = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis") ?? throw new NullReferenceException("ConnectionStrings:Redis missing from config."));
            _redisDb = _redis.GetDatabase();
        }

        public async Task<SignalStatus?> GetAsync(Guid signalId)
        {
            var fullKey = GetFullKey(_tenantId);
            var result = await _redisDb.HashGetAsync(fullKey, signalId.ToString());
            if (!result.HasValue || result.IsNullOrEmpty)
            {
                return null;
            }
            var controllerStatus = Serializer.Deserialize((byte[])result);
            return controllerStatus;
        }

        public async Task<SignalStatus[]> GetAllAsync()
        {
            var key = GetFullKey(_tenantId);
            var result = await _redisDb.HashGetAllAsync(key);
            return result.Select(v => Serializer.Deserialize((byte[])v.Value)).ToArray();
        }

        private string GetFullKey(Guid tenantId)
        {
            return tenantId.ToString().ToUpper() + ":SignalStatus";
        }

    }
}
