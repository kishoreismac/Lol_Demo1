namespace LandOLakesDairyDemo.Models.Qa;

public class QaTestCasesViewModel
{
    public IReadOnlyList<QaTestCaseItem> TestCases { get; set; } = Array.Empty<QaTestCaseItem>();

    public QaRunState CurrentState { get; set; } = new();
}