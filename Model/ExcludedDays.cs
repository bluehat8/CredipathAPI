using CredipathAPI.Base;
using static CredipathAPI.Constants;

namespace CredipathAPI.Model
{
    public class ExcludedDays : BaseEntity
    {
     public string excludes_day_name { get; set; }
     public int loan_id { get; set; }        
    }
}
