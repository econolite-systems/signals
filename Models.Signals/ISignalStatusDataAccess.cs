// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.Signals;

public interface ISignalStatusDataAccess
{
    Task<IEnumerable<SignalStatusModel>> GetAllAsync(Guid tenantId, IEnumerable<Guid> signalIds, bool getPhaseData);

    Task<SignalStatusModel> GetOneAsync(Guid tenantId, Guid signalId, bool getPhaseData);
}
