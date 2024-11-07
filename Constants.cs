namespace CredipathAPI
{
    public class Constants
    {
        public enum UserType
        {
            admin = 1,  
            collaborator = 2   
        }

        public enum PaymentStatus
        {
            pending = 0,   
            paid = 1,
            overdue = 2,
            reassigned = 3   
        }

        public enum PaymentFrequency
        {
            Diario,
            Semanal,
            Quincenal,
            Mensual,
            Manual
        }
    }
}
