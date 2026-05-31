namespace Infrastructure.Settings;

public class OutboxSetting
{ 
   
    public int BatchSize { get; set; } = 20;
    public int PollingIntervalSeconds { get; set; } = 5;
    public int MaxRetryCount { get; set; } = 3;
}

