using ManagerGame.Core.Domain;

namespace ManagerGame.Test.Domain;

public class DraftTest
{
    [Fact]
    public void DoublePeakDraftOrderIsCorrect()
    {
        var league = new League(Guid.NewGuid());
        var team1 = TestData.TeamWithValidFullSquad();
        var team2 = TestData.TeamWithValidFullSquad();
        var team3 = TestData.TeamWithValidFullSquad();
        var team4 = TestData.TeamWithValidFullSquad();
        league.AddTeam(team1);
        league.AddTeam(team2);
        league.AddTeam(team3);
        league.AddTeam(team4);
        var draft = Draft.DoubledPeakTraversalDraft(league);

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
    //
    // [Fact]
    // public void TooFewTeamsThrows()
    // {
    //     var team1 = TestData.TeamWithValidFullSquad();
    //     Assert.Throws<ArgumentException>(() => Draft.DoubledPeakTraversalDraft([team1]));
    // }
}
