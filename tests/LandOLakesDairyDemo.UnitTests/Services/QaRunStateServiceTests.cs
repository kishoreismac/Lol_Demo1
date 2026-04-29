using LandOLakesDairyDemo.Models.Qa;
using LandOLakesDairyDemo.Services;

namespace LandOLakesDairyDemo.UnitTests.Services;

[TestClass]
public class QaRunStateServiceTests
{
    [TestMethod]
    public void GetState_ReturnsInitialValuesAsCopy()
    {
        var service = new QaRunStateService();

        var first = service.GetState();
        var second = service.GetState();

        Assert.AreEqual("idle", first.Status);
        Assert.AreEqual("Waiting for a Playwright run to start.", first.Notes);
        Assert.AreNotSame(first, second);
    }

    [TestMethod]
    public void UpdateState_UsesRunningStatus_WhenStatusIsBlank()
    {
        var service = new QaRunStateService();

        var updatedState = service.UpdateState(new QaRunState
        {
            CaseId = "TC-UI-002",
            Title = "Products page loads successfully",
            Status = " ",
            Notes = "Started"
        });

        Assert.AreEqual("TC-UI-002", updatedState.CaseId);
        Assert.AreEqual("Products page loads successfully", updatedState.Title);
        Assert.AreEqual("running", updatedState.Status);
        Assert.AreEqual("Started", updatedState.Notes);
    }

    [TestMethod]
    public void UpdateState_PreservesExplicitStatus()
    {
        var service = new QaRunStateService();

        var updatedState = service.UpdateState(new QaRunState
        {
            CaseId = "TC-UI-004",
            Title = "Category filter narrows the catalog",
            Status = "passed",
            Notes = "Done"
        });

        Assert.AreEqual("passed", updatedState.Status);
        Assert.AreEqual("Done", updatedState.Notes);
        Assert.IsTrue(updatedState.UpdatedUtc <= DateTime.UtcNow);
    }
}