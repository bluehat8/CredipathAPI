using CredipathAPI.Base;
using static CredipathAPI.Constants;

namespace CredipathAPI.Model
{
    public class Bank: BaseEntity
    {
        public string? name { get; set; }
        public string? code { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }

    }
}
