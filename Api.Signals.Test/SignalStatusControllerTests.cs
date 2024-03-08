// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Api.Signals.Controllers;
using Econolite.Ode.Models.Signals;
using Econolite.Ode.Repository.Signals;
using Econolite.Ode.Status.Common;
using Econolite.Ode.Status.Signal;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Api.Signals.Test
{
    public class SignalStatusControllerTests
    {
        private Mock<ISignalStatusRepository> _mockRepository;
        private Mock<IConfiguration> _mockConfiguration;
        private SignalStatusController _controller;

        public SignalStatusControllerTests()
        {
            _mockRepository = new Mock<ISignalStatusRepository>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(c => c.GetValue<int>("SignalTimeout")).Returns(20);
            _controller = new SignalStatusController(_mockRepository.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task FindAsync_ReturnsSignalStatus_WhenStatusIsNotTooOldAsync()
        {
            var signalId = Guid.NewGuid();
            var signalStatus = new SignalStatus
            {
                TimeStamp = DateTime.UtcNow,
                CommStatus = CommStatus.Good
            };

            _mockRepository.Setup(r => r.GetAsync(signalId)).ReturnsAsync(signalStatus);

            var result = await _controller.FindAsync(signalId);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var model = okResult.Value.Should().BeAssignableTo<SignalStatusModel>().Subject;
            model.CommStatus.Should().Be(CommStatus.Good);
        }

        [Fact]
        public async Task FindAsync_ReturnsSignalStatusWithOfflineCommStatus_WhenStatusIsTooOldAsync()
        {
            var signalId = Guid.NewGuid();
            var signalStatus = new SignalStatus
            {
                TimeStamp = DateTime.UtcNow.AddMinutes(-30),
                CommStatus = CommStatus.Good
            };

            _mockRepository.Setup(r => r.GetAsync(signalId)).ReturnsAsync(signalStatus);

            var result = await _controller.FindAsync(signalId);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var model = okResult.Value.Should().BeAssignableTo<SignalStatusModel>().Subject;
            model.CommStatus.Should().Be(CommStatus.Offline);
        }

        [Fact]
        public async Task GetMapPointAsync_ReturnsSignalStatuses_WhenStatusesAreNotTooOldAsync()
        {
            var signalStatuses = new[]
            {
                    new SignalStatus
                {
                TimeStamp = DateTime.UtcNow,
                CommStatus = CommStatus.Good
                },
                new SignalStatus
                {
                    TimeStamp = DateTime.UtcNow,
                    CommStatus = CommStatus.Good
            }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(signalStatuses);

            var result = await _controller.GetMapPointAsync();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var models = okResult.Value.Should().BeAssignableTo<IEnumerable<SignalStatusModel>>().Subject;
            models.Should().HaveCount(signalStatuses.Length);
            models.All(m => m.CommStatus == CommStatus.Good).Should().BeTrue();
        }

        [Fact]
        public async Task GetMapPointAsync_ReturnsSignalStatusesWithOfflineCommStatus_WhenStatusesAreTooOldAsync()
        {
            var signalStatuses = new[]
            {
                new SignalStatus
                {
                    TimeStamp = DateTime.UtcNow.AddMinutes(-30),             CommStatus = CommStatus.Good
                },
                new SignalStatus
                {
                    TimeStamp = DateTime.UtcNow.AddMinutes(-30),             CommStatus = CommStatus.Good
                }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(signalStatuses);

            var result = await _controller.GetMapPointAsync();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var models = okResult.Value.Should().BeAssignableTo<IEnumerable<SignalStatusModel>>().Subject;
            models.Should().HaveCount(signalStatuses.Length);
            models.All(m => m.CommStatus == CommStatus.Offline).Should().BeTrue();
        }

        [Fact]
        public async Task GetMapSignalStateAsync_ReturnsMapSignalStates_WhenStatusesAreNotTooOldAsync()
        {
            var signalStatuses = new[]
{
                new SignalStatus
                {
                    TimeStamp = DateTime.UtcNow,
                    CommStatus = CommStatus.Good
                },
                new SignalStatus
                {
                    TimeStamp = DateTime.UtcNow,
                    CommStatus = CommStatus.Good
                }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(signalStatuses);

            var result = await _controller.GetMapSignalStateAsync();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var models = okResult.Value.Should().BeAssignableTo<IEnumerable<MapSignalState>>().Subject;
            models.Should().HaveCount(signalStatuses.Length);
        }

        [Fact]
        public async Task GetMapSignalStateAsync_ReturnsFewerMapSignalStates_WhenStatusesAreTooOldAsync()
        {
            var signalStatuses = new[]
            {
                new SignalStatus
                {
                    TimeStamp = DateTime.UtcNow.AddMinutes(-30),                 CommStatus = CommStatus.Good
                },
                new SignalStatus
                {
                    TimeStamp = DateTime.UtcNow,
                    CommStatus = CommStatus.Good
                }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(signalStatuses);

            var result = await _controller.GetMapSignalStateAsync();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var models = okResult.Value.Should().BeAssignableTo<IEnumerable<MapSignalState>>().Subject;
            models.Should().HaveCountLessThan(signalStatuses.Length);
        }
    }

}
