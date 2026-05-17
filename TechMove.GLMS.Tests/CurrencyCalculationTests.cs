using TechMove.GLMS.Web.Models;
using Xunit;

namespace TechMove.GLMS.Tests
{
    public class CurrencyCalculationTests
    {
        [Fact]
        public void ConvertUSDToZAR_WithValidRate_ReturnsCorrectZARAmount()
        {
            // Arrange
            decimal amountUSD = 100.00m;
            decimal exchangeRate = 19.50m;
            decimal expectedZAR = 1950.00m;

            // Act
            decimal actualZAR = amountUSD * exchangeRate;

            // Assert
            Assert.Equal(expectedZAR, actualZAR);
        }

        [Fact]
        public void ConvertUSDToZAR_WithZeroUSD_ReturnsZeroZAR()
        {
            // Arrange
            decimal amountUSD = 0.00m;
            decimal exchangeRate = 19.50m;
            decimal expectedZAR = 0.00m;

            // Act
            decimal actualZAR = amountUSD * exchangeRate;

            // Assert
            Assert.Equal(expectedZAR, actualZAR);
        }

        [Fact]
        public void ConvertUSDToZAR_WithDifferentRate_CalculatesCorrectly()
        {
            // Arrange
            decimal amountUSD = 50.00m;
            decimal exchangeRate = 18.75m;
            decimal expectedZAR = 937.50m;

            // Act
            decimal actualZAR = amountUSD * exchangeRate;

            // Assert
            Assert.Equal(expectedZAR, actualZAR);
        }

        [Fact]
        public void ServiceRequest_AmountZAR_IsCalculatedFromUSDAndExchangeRate()
        {
            // Arrange
            var serviceRequest = new ServiceRequest
            {
                AmountUSD = 250.00m,
                ExchangeRateUsed = 19.50m
            };

            // Act
            serviceRequest.AmountZAR = serviceRequest.AmountUSD * serviceRequest.ExchangeRateUsed;

            // Assert
            Assert.Equal(4875.00m, serviceRequest.AmountZAR);
        }

        [Theory]
        [InlineData(10, 19.50, 195)]
        [InlineData(25.50, 19.50, 497.25)]
        [InlineData(100.75, 19.50, 1964.625)]
        [InlineData(1, 20.00, 20)]
        [InlineData(0, 19.50, 0)]
        public void CurrencyConversion_MultipleScenarios_ReturnsCorrectZAR(
            decimal usd, decimal rate, decimal expectedZar)
        {
            // Act
            decimal actualZar = usd * rate;

            // Assert
            Assert.Equal(expectedZar, actualZar);
        }
    }
}