namespace ArooshyStore.Authentication.FormsAuthentication
{
    public class CustomPrincipalSerializeModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
    }
}