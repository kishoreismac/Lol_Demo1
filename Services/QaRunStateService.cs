using LandOLakesDairyDemo.Models.Qa;

namespace LandOLakesDairyDemo.Services;

public class QaRunStateService
{
    private readonly object _syncLock = new();
    private QaRunState _state = new()
    {
        Status = "idle",
        Notes = "Waiting for a Playwright run to start."
    };

    public QaRunState GetState()
    {
        lock (_syncLock)
        {
            return new QaRunState
            {
                CaseId = _state.CaseId,
                Title = _state.Title,
                Status = _state.Status,
                Notes = _state.Notes,
                UpdatedUtc = _state.UpdatedUtc
            };
        }
    }

    public QaRunState UpdateState(QaRunState state)
    {
        lock (_syncLock)
        {
            _state = new QaRunState
            {
                CaseId = state.CaseId,
                Title = state.Title,
                Status = string.IsNullOrWhiteSpace(state.Status) ? "running" : state.Status,
                Notes = state.Notes,
                UpdatedUtc = DateTime.UtcNow
            };

            return GetState();
        }
    }
}