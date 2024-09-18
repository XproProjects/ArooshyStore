using System.Security.Principal;

namespace ArooshyStore.Authentication.FormsAuthentication
{
    interface ICustomPrincipal : IPrincipal
    {
        int UserId { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string UserType { get; set; }
        int CompanyId { get; set; }
        int BranchId { get; set; }
        string BranchName { get; set; }
    }
}
