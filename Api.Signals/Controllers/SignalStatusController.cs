// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Api.Signals.Extensions;
using Econolite.Ode.Authorization;
using Econolite.Ode.Models.Signals;
using Econolite.Ode.Repository.Signals;
using Econolite.Ode.Status.Common;
using Econolite.Ode.Status.Signal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Api.Signals.Controllers
{
    [Route("signal-status")]
    [ApiController]
    [AuthorizeOde(MoundRoadRole.ReadOnly)]
    public class SignalStatusController : ControllerBase
    {
        private readonly ISignalStatusRepository _signalStatusRepository;
        private readonly int _signalTimeout;

        public SignalStatusController(ISignalStatusRepository signalStatusRepository, IConfiguration configuration)
        {
            _signalStatusRepository = signalStatusRepository;
            _signalTimeout = configuration.GetValue<int>("StaleSignalTime");
        }

        [HttpGet("get")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SignalStatusModel))]
        public async Task<IActionResult> FindAsync([FromQuery][BindRequired] Guid id)
        {
            var signalStatus = await _signalStatusRepository.GetAsync(id);

            if (signalStatus is null)
            {
                return NotFound();
            }

            signalStatus.CheckTimeout(_signalTimeout);

            return Ok(signalStatus.ConvertToSignalStatus(true, 16));
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SignalStatusModel[]))]
        public async Task<IActionResult> GetMapPointAsync()
        {
            var signalStatuses = await _signalStatusRepository.GetAllAsync();

            if (signalStatuses is null)
            {
                return NotFound();
            }

            foreach (var signalStatus in signalStatuses)
            {
                signalStatus.CheckTimeout(_signalTimeout);
            }

            return Ok(signalStatuses.Select(s => s.ConvertToSignalStatus(true, 16)));
        }

        [HttpGet("point")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MapSignalState[]))]
        public async Task<IActionResult> GetMapSignalStateAsync()
        {
            var signalStatuses = await _signalStatusRepository.GetAllAsync();

            if (signalStatuses is null)
            {
                return NotFound();
            }

            foreach (var signalStatus in signalStatuses)
            {
                signalStatus.CheckTimeout(_signalTimeout);
            }

            return Ok(signalStatuses.Select(s => s.ConvertToMapSignalState()));
        }
    }
}
