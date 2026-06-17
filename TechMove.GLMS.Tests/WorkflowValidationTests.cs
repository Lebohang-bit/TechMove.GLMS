using TechMove.GLMS.Web.Models;
using Xunit;

namespace TechMove.GLMS.Tests
{
    public class WorkflowValidationTests
    {
        [Fact]
        public void ServiceRequest_WithActiveAgreement_CanBeCreated()
        {
            
            var agreement = new Agreement
            {
                AgreementId = 1,
                Status = AgreementStatus.Active
            };

            
            bool canCreateRequest = agreement.IsValidForServiceRequests;

            
            Assert.True(canCreateRequest);
        }

        [Fact]
        public void ServiceRequest_WithDraftAgreement_CannotBeCreated()
        {
            
            var agreement = new Agreement
            {
                AgreementId = 1,
                Status = AgreementStatus.Draft
            };

            
            bool canCreateRequest = agreement.IsValidForServiceRequests;

           
            Assert.False(canCreateRequest);
        }

        [Fact]
        public void ServiceRequest_WithExpiredAgreement_CannotBeCreated()
        {
            
            var agreement = new Agreement
            {
                AgreementId = 1,
                Status = AgreementStatus.Expired
            };

           
            bool canCreateRequest = agreement.IsValidForServiceRequests;

            
            Assert.False(canCreateRequest);
        }

        [Fact]
        public void ServiceRequest_WithOnHoldAgreement_CannotBeCreated()
        {
            
            var agreement = new Agreement
            {
                AgreementId = 1,
                Status = AgreementStatus.OnHold
            };


            bool canCreateRequest = agreement.IsValidForServiceRequests;

            
            Assert.False(canCreateRequest);
        }

        [Fact]
        public void AgreementStatus_Active_IsTheOnlyValidStatusForServiceRequests()
        {
            
            var activeAgreement = new Agreement { Status = AgreementStatus.Active };
            var draftAgreement = new Agreement { Status = AgreementStatus.Draft };
            var expiredAgreement = new Agreement { Status = AgreementStatus.Expired };
            var onHoldAgreement = new Agreement { Status = AgreementStatus.OnHold };

            
            Assert.True(activeAgreement.IsValidForServiceRequests);
            Assert.False(draftAgreement.IsValidForServiceRequests);
            Assert.False(expiredAgreement.IsValidForServiceRequests);
            Assert.False(onHoldAgreement.IsValidForServiceRequests);
        }
    }
}