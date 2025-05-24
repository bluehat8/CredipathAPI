using CredipathAPI.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace CredipathAPI.Model
{
    public class Collaborator : BaseEntity
    {
        public string Identifier { get; set; } 
        public string Phone { get; set; }
        public string Mobile { get; set; }
        
        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; }
        
        public int CreatedById { get; set; }
        
        [ForeignKey("CreatedById")]
        public User CreatedBy { get; set; }
        
        // Permisos específicos del colaborador
        public LoanPermissions Loan { get; set; }
        public PaymentPermissions Payment { get; set; }
        public ModulePermissions Modules { get; set; }
    }

    public class LoanPermissions
    {
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
    }

    public class PaymentPermissions
    {
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
    }

    public class ModulePermissions
    {
        public bool Collaborators { get; set; }
        public bool OverduePayments { get; set; }
        public bool UpcomingPayments { get; set; }
        public bool LoanPayment { get; set; }
        public bool Report { get; set; }
    }
}
