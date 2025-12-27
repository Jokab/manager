namespace ManagerGame.Test.Domain;

public class DraftTest
{
    [Fact]
    public void DoublePeakDraftOrderIsCorrect()
    {
        var league = League.Empty();
        var team1 = TestData.TeamWithValidFullSquad("Lag 1");
        var team2 = TestData.TeamWithValidFullSquad("Lag 2");
        var team3 = TestData.TeamWithValidFullSquad("Lag 3");
        var team4 = TestData.TeamWithValidFullSquad("Lag 4");
        league.AdmitTeam(team1);
        league.AdmitTeam(team2);
        league.AdmitTeam(team3);
        league.AdmitTeam(team4);
        var draft = Draft.DoubledPeakTraversalDraft(league);
        draft.Start();

        Assert.Equal(team1, GetNext(draft));
        Assert.Equal(team2, GetNext(draft));
        Assert.Equal(team3, GetNext(draft));
        Assert.Equal(team4, GetNext(draft));
        Assert.Equal(team4, GetNext(draft));
        Assert.Equal(team3, GetNext(draft));
        Assert.Equal(team2, GetNext(draft));
        Assert.Equal(team1, GetNext(draft));
        Assert.Equal(team1, GetNext(draft));
        Assert.Equal(team2, GetNext(draft));
        return;

        Team GetNext(Draft d) => d.GetNext().Match(team => team, _ => null!);
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

    [Fact]
    public void GetNext_WhenDraftNotStarted_DoesNothing()
    {
        var team1 = TestData.TeamWithValidFullSquad();
        var league = League.Empty();
        league.AdmitTeam(team1);
        var draft = Draft.DoubledPeakTraversalDraft(league);

        var next = draft.GetNext().Match(
            team => team,
            _ => null!);

        Assert.Null(next);
    }
}
