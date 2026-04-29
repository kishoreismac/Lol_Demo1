namespace LandOLakesDairyDemo.Models.Qa;

public class QaRunState
{
    public string? CaseId { get; set; }

    public string? Title { get; set; }

    public string Status { get; set; } = "idle";

    public string? Notes { get; set; }

    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}