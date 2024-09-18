using System.Security.Principal;

namespace ArooshyStore.Authentication.FormsAuthentication
{
    public class CustomPrincipal:ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role) { return false; }

        public CustomPrincipal(string email)
        {
            this.Identity = new GenericIdentity(email);
        }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
    }
}