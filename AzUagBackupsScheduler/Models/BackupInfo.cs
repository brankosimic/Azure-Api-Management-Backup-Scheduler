namespace AzUagBackupsScheduler
{
    public class BackupInfo
    {
        public string StorageAccount { get; set; }
        public string AccessKey { get; set; }
        public string ContainerName { get; set; }
        public string BackupName { get; set; }
    }
}
