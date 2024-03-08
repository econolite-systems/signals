// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Interfaces;

namespace Econolite.Ode.Models.Signals
{
    public class SsmStatus : IIndexedEntity<Guid>
    {
        public Guid Id { get; set; }
        public int ReferenceId { get; set; }
        public DateTime Timestamp { get; set; }
        public int SequenceNumber { get; set; }
    }
}
