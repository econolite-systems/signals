// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.Signals;

public interface IMapStatusDataAccess
{
    Task<IDictionary<string, MapStatusModel>> GetMapPointStatusAsync(Guid tenantId, int changeId);

    Task<int> GetLatestMapPointChangedKeyAsync(Guid tenantId);
}
