using PowerArgs;

namespace AzUagBackupsScheduler
{
    [TabCompletion]
    public class Context
    {
        [HelpHook, ArgShortcut("-?"), ArgDescription("There are two modes: backupservice and restore")]
        public bool Help { get; set; }

        [ArgRequired(IfNot = "RestoreMode")]
        public bool BackupServiceMode { get; set; }

        [ArgRequired(IfNot = "BackupServiceMode")]
        public bool RestoreMode { get; set; }

        [ArgRequired(PromptIfMissing = true, If = "RestoreMode")]
        public string ServiceName { get; set; }

        [ArgRequired(PromptIfMissing = true, If = "RestoreMode")]
        public string BackupName { get; set; }
    } 
}
