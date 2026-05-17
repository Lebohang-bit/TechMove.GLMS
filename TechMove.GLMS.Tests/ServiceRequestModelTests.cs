using System;
using TechMove.GLMS.Web.Models;
using Xunit;

namespace TechMove.GLMS.Tests
{
    public class ServiceRequestModelTests
    {
        [Fact]
        public void ServiceRequest_NewInstance_HasDefaultPendingStatus()
        {
            var request = new ServiceRequest();
            Assert.Equal(RequestStatusType.Pending, request.Status);
        }

        [Fact]
        public void ServiceRequest_NewInstance_HasTodayRequestDate()
        {
            var request = new ServiceRequest();
            Assert.Equal(DateTime.Today, request.RequestDate);
        }

        [Fact]
        public void ServiceRequest_CanUpdateStatus()
        {
            var request = new ServiceRequest();
            request.Status = RequestStatusType.Approved;
            Assert.Equal(RequestStatusType.Approved, request.Status);
        }

        [Fact]
        public void ServiceRequest_StoresExchangeRateUsed()
        {
            var request = new ServiceRequest();
            decimal expectedRate = 19.50m;
            request.ExchangeRateUsed = expectedRate;
            Assert.Equal(expectedRate, request.ExchangeRateUsed);
        }

        [Fact]
        public void ServiceRequest_AmountZAR_DecimalPrecision()
        {
            var request = new ServiceRequest
            {
                AmountUSD = 100.00m,
                ExchangeRateUsed = 19.50m
            };

            request.AmountZAR = request.AmountUSD * request.ExchangeRateUsed;

            Assert.Equal(1950.00m, request.AmountZAR);
            Assert.IsType<decimal>(request.AmountZAR);
        }
    }
}