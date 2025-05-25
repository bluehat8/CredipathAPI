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

        public static void UpdateIfNotEmpty(string value, Action<string> updateAction)
        {
            if (!string.IsNullOrEmpty(value))
            {
                updateAction(value);
            }
        }

        public static DateTime CalculateNextPaymentDate(DateTime startDate, int paymentNumber, string frequencyName, int customInterval = 0)
        {
            return frequencyName.ToLower() switch
            {
                "diario" => startDate.AddDays(paymentNumber),
                "semanal" => startDate.AddDays(paymentNumber * 7),
                "quincenal" => startDate.AddDays(paymentNumber * 14),
                "mensual" => startDate.AddMonths(paymentNumber),
                "ingresar manual" when customInterval > 0 => startDate.AddDays(paymentNumber * customInterval),
                _ => throw new Exception("Frecuencia de pago no reconocida.")
            };
        }
    }
}
