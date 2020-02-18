namespace AzUagBackupsScheduler
{
    public class Config
    {
        public string CronSchedule { get; set; }
        public string TenantId { get; set; }
        public string ApplicationClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthApi { get; set; }
        public string ResourceScopeApi { get; set; }
        public string SubscriptionId { get; set; }
        public string ServiceName { get; set; }
        public string ApiVersion { get; set; }
        public string AccessKey { get; set; }
        public string ContainerName { get; set; }
        public string StorageAccount { get; set; }
        public string BackupApi { get; set; }
        public string ResourceGroupName { get; set; }
        public string RestoreApi { get; set; }
    }
}
