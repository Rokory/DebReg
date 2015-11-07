using DebRegOrchestration.Models;
using DebReg.Models;
using System;

namespace DebRegOrchestration {
    public interface IPaymentManager {
        CalculatePaidSlotsResult CalculatePaidSlots(Guid tournamentId, Guid organizationId, Decimal value);
        void ConfirmSlots(Guid tournamentId, Guid organizationId, int paidTeams, int paidAdjudicators, String paymentPageUrl, User user);
    }
}
