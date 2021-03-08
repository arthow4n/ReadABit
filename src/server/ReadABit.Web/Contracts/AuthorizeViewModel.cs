using System.ComponentModel.DataAnnotations;

namespace ReadABit.Web.Contracts
{
    public class AuthorizeViewModel
    {
        [Display(Name = "Application")]
        public string ApplicationName { get; init; } = "";

        [Display(Name = "Scope")]
        public string Scope { get; init; } = "";
    }
}
