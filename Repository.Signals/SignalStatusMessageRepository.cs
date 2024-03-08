// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.Signals;
using Econolite.Ode.Persistence.Common.Repository;
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Repository;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Repository.Signals;

public class SignalStatusMessageRepository : GuidDocumentRepositoryBase<SsmStatus>, ISignalStatusMessageRepository
{
    public SignalStatusMessageRepository(IMongoContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory.CreateLogger<SignalStatusMessageRepository>())
    {
    }
    
}

public interface ISignalStatusMessageRepository: IRepository<SsmStatus, Guid>
{
}
