using TechMove.GLMS.Web.Models;
using Xunit;

namespace TechMove.GLMS.Tests
{
    public class WorkflowValidationTests
    {
        [Fact]
        public void ServiceRequest_WithActiveAgreement_CanBeCreated()
        {
            // Arrange
            var agreement = new Agreement
            {
                AgreementId = 1,
                Status = AgreementStatus.Active
            };

            // Act
            bool canCreateRequest = agreement.IsValidForServiceRequests;

            // Assert
            Assert.True(canCreateRequest);
        }

        [Fact]
        public void ServiceRequest_WithDraftAgreement_CannotBeCreated()
        {
            // Arrange
            var agreement = new Agreement
            {
                AgreementId = 1,
                Status = AgreementStatus.Draft
            };

            // Act
            bool canCreateRequest = agreement.IsValidForServiceRequests;

            // Assert
            Assert.False(canCreateRequest);
        }

        [Fact]
        public void ServiceRequest_WithExpiredAgreement_CannotBeCreated()
        {
            // Arrange
            var agreement = new Agreement
            {
                AgreementId = 1,
                Status = AgreementStatus.Expired
            };

            // Act
            bool canCreateRequest = agreement.IsValidForServiceRequests;

            // Assert
            Assert.False(canCreateRequest);
        }

        [Fact]
        public void ServiceRequest_WithOnHoldAgreement_CannotBeCreated()
        {
            // Arrange
            var agreement = new Agreement
            {
                AgreementId = 1,
                Status = AgreementStatus.OnHold
            };

            // Act
            bool canCreateRequest = agreement.IsValidForServiceRequests;

            // Assert
            Assert.False(canCreateRequest);
        }

        [Fact]
        public void AgreementStatus_Active_IsTheOnlyValidStatusForServiceRequests()
        {
            // Arrange
            var activeAgreement = new Agreement { Status = AgreementStatus.Active };
            var draftAgreement = new Agreement { Status = AgreementStatus.Draft };
            var expiredAgreement = new Agreement { Status = AgreementStatus.Expired };
            var onHoldAgreement = new Agreement { Status = AgreementStatus.OnHold };

            // Act & Assert
            Assert.True(activeAgreement.IsValidForServiceRequests);
            Assert.False(draftAgreement.IsValidForServiceRequests);
            Assert.False(expiredAgreement.IsValidForServiceRequests);
            Assert.False(onHoldAgreement.IsValidForServiceRequests);
        }
    }
}