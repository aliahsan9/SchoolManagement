namespace SchoolManagement.Infrastructure.MultiTenancy
{
    public static class TenantScope
    {
        private static readonly AsyncLocal<Guid?> Current = new();

        public static Guid? SchoolId
        {
            get => Current.Value;
            set => Current.Value = value;
        }
    }
}