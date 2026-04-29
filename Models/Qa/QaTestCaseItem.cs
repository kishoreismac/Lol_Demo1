namespace LandOLakesDairyDemo.Models.Qa;

public class QaTestCaseItem
{
    public string Id { get; set; } = string.Empty;

    public string Requirement { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Preconditions { get; set; } = string.Empty;

    public string Steps { get; set; } = string.Empty;

    public string ExpectedResult { get; set; } = string.Empty;

    public string AutomationCoverage { get; set; } = string.Empty;
}