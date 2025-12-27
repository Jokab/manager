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
        var draft = Draft.DoubledPeakTraversalDraft(league.Id, league.Teams.Select(x => x.Id).ToList());
        draft.Start(1);

        Assert.Equal(team1.Id, draft.AdvanceAndGetNextTeamId());
        Assert.Equal(team2.Id, draft.AdvanceAndGetNextTeamId());
        Assert.Equal(team3.Id, draft.AdvanceAndGetNextTeamId());
        Assert.Equal(team4.Id, draft.AdvanceAndGetNextTeamId());
        Assert.Equal(team4.Id, draft.AdvanceAndGetNextTeamId());
        Assert.Equal(team3.Id, draft.AdvanceAndGetNextTeamId());
        Assert.Equal(team2.Id, draft.AdvanceAndGetNextTeamId());
        Assert.Equal(team1.Id, draft.AdvanceAndGetNextTeamId());
        Assert.Equal(team1.Id, draft.AdvanceAndGetNextTeamId());
        Assert.Equal(team2.Id, draft.AdvanceAndGetNextTeamId());
        return;
    }

    [Fact]
    public void StartWithTooFewTeamsThrows()
    {
        var team1 = TestData.TeamWithValidFullSquad();
        var league = League.Empty();
        league.AdmitTeam(team1);
        var draft = Draft.DoubledPeakTraversalDraft(league.Id, league.Teams.Select(x => x.Id).ToList());

        Assert.Throws<ArgumentException>(() => draft.Start(22));
    }

    [Fact]
    public void GetNext_WhenDraftNotStarted_DoesNothing()
    {
        var team1 = TestData.TeamWithValidFullSquad();
        var league = League.Empty();
        league.AdmitTeam(team1);
        var draft = Draft.DoubledPeakTraversalDraft(league.Id, league.Teams.Select(x => x.Id).ToList());

        Assert.Null(draft.PeekNextTeamId());
    }

    [Fact]
    public void DraftFinishesAfterRequiredPicks()
    {
        var league = League.Empty();
        var team1 = TestData.TeamWithValidFullSquad("Lag 1");
        var team2 = TestData.TeamWithValidFullSquad("Lag 2");
        league.AdmitTeam(team1);
        league.AdmitTeam(team2);
        var draft = Draft.DoubledPeakTraversalDraft(league.Id, league.Teams.Select(x => x.Id).ToList());
        draft.Start(1);

        draft.RecordPick(team1.Id, Guid.NewGuid());
        Assert.Equal(DraftState.Started, draft.State);
        draft.RecordPick(team2.Id, Guid.NewGuid());

        Assert.Equal(DraftState.Finished, draft.State);
        Assert.Throws<InvalidOperationException>(() => draft.RecordPick(team1.Id, Guid.NewGuid()));
    }

    [Fact]
    public void CannotExceedPicksPerTeam()
    {
        var league = League.Empty();
        var team1 = TestData.TeamWithValidFullSquad("Lag 1");
        var team2 = TestData.TeamWithValidFullSquad("Lag 2");
        league.AdmitTeam(team1);
        league.AdmitTeam(team2);
        var draft = Draft.DoubledPeakTraversalDraft(league.Id, league.Teams.Select(x => x.Id).ToList());
        draft.Start(1);

        draft.RecordPick(team1.Id, Guid.NewGuid());
        Assert.Throws<InvalidOperationException>(() => draft.RecordPick(team1.Id, Guid.NewGuid()));
    }
}
