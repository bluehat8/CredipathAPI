using CredipathAPI.Base;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Identity.Client;
using static CredipathAPI.Constants;

namespace CredipathAPI.Model
{
    public class Loans : BaseEntity
    {
        public int client_id { get; set; }  
        public decimal amount { get; set; } 
        public int interest_type_id { get; set; }   
        public decimal interst_rate { get; set; }   
        public int installments {  get; set; }  
        public int frecuency_id { get; set; }   
        public int custom_payment_interval { get; set; }
        public DateTime loan_date { get; set; } 
        public string notes { get; set; }   
    }
}
