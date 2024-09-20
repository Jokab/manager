using ManagerGame.Core.Domain;

namespace ManagerGame.Test.Domain;

public class DraftTest
{
    [Fact]
    public void DoublePeakDraftOrderIsCorrect()
    {
        var team1 = TestData.TeamWithValidFullSquad();
        var team2 = TestData.TeamWithValidFullSquad();
        var team3 = TestData.TeamWithValidFullSquad();
        var team4 = TestData.TeamWithValidFullSquad();
        var draft = Draft.DoublePeakTraversalDraft([team1, team2, team3, team4]);

        Assert.Equal(team1, draft.GetNext());
        Assert.Equal(team2, draft.GetNext());
        Assert.Equal(team3, draft.GetNext());
        Assert.Equal(team4, draft.GetNext());
        Assert.Equal(team4, draft.GetNext());
        Assert.Equal(team3, draft.GetNext());
        Assert.Equal(team2, draft.GetNext());
        Assert.Equal(team1, draft.GetNext());
        Assert.Equal(team1, draft.GetNext());
        Assert.Equal(team2, draft.GetNext());
    }
}
