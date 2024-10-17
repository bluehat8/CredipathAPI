using static CredipathAPI.Constants;

namespace CredipathAPI.Helpers
{
    public class Helper
    {
        public static string GetPaymentStatusText(PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.paid => "Pagado",
                PaymentStatus.pending => "Pendiente",
                PaymentStatus.overdue => "Vencido",
                PaymentStatus.reassigned => "Reasignado",
                _ => "Desconocido"
            };
        }
    }
}
