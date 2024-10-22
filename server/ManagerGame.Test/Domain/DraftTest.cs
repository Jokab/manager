namespace ManagerGame.Test.Domain;

public class DraftTest
{
    [Fact]
    public void DoublePeakDraftOrderIsCorrect()
    {
        var league = League.Empty();
        var team1 = TestData.TeamWithValidFullSquad();
        var team2 = TestData.TeamWithValidFullSquad();
        var team3 = TestData.TeamWithValidFullSquad();
        var team4 = TestData.TeamWithValidFullSquad();
        league.AdmitTeam(team1);
        league.AdmitTeam(team2);
        league.AdmitTeam(team3);
        league.AdmitTeam(team4);
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

    [Fact]
    public void StartWithTooFewTeamsThrows()
    {
        var team1 = TestData.TeamWithValidFullSquad();
        var league = League.Empty();
        league.AdmitTeam(team1);
        var draft = Draft.DoubledPeakTraversalDraft(league);

        Assert.Throws<ArgumentException>(() => draft.Start());
    }
}
