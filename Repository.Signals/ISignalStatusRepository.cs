// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Repository;
using Econolite.Ode.Status.Signal;

namespace Econolite.Ode.Repository.Signals
{
    public interface ISignalStatusRepository
    {
        public Task<SignalStatus?> GetAsync(Guid signalId);
        Task<SignalStatus[]> GetAllAsync();
    }
}
