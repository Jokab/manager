namespace ManagerGame.Test.Domain;

public class LeagueTest
{
    [Fact]
    public void CanCreateDraft()
    {
        League sut = League.Empty();

        sut.AdmitTeam(Team.Create(new TeamName("Lag"), Guid.NewGuid(), [], sut));
        sut.CreateDraft();

        Assert.Single(sut.Drafts);
    }

    [Fact]
    public void CanStartDraft()
    {
        League sut = League.Empty();

        sut.AdmitTeam(Team.Create(new TeamName("Lag"), Guid.NewGuid(), [], sut));
        sut.AdmitTeam(Team.Create(new TeamName("Lag2"), Guid.NewGuid(), [], sut));
        sut.CreateDraft();
        sut.Drafts.First().Start();

        Assert.Equal(DraftState.Started, sut.Drafts.First().State);
    }


    [Fact]
    public void CannotStartAlreadyStartedDraft()
    {
        League sut = League.Empty();

        sut.AdmitTeam(Team.Create(new TeamName("Lag"), Guid.NewGuid(), [], sut));
        sut.AdmitTeam(Team.Create(new TeamName("Lag2"), Guid.NewGuid(), [], sut));
        sut.CreateDraft();
        sut.Drafts.First().Start();

        Assert.Throws<InvalidOperationException>(() => sut.Drafts.First().Start());
    }

    [Fact]
    public void CannotCreateDraftWithCreatedDraft()
    {
        League sut = League.Empty();
        sut.AdmitTeam(Team.Create(new TeamName("Lag"), Guid.NewGuid(), [], sut));
        sut.AdmitTeam(Team.Create(new TeamName("Lag2"), Guid.NewGuid(), [], sut));
        sut.CreateDraft();

        Assert.Equal(DraftState.Created, sut.Drafts.Single().State);

        Assert.Throws<InvalidOperationException>(() => sut.CreateDraft());
    }

    [Fact]
    public void CannotCreateDraftWithStartedDraft()
    {
        League sut = League.Empty();
        sut.AdmitTeam(Team.Create(new TeamName("Lag"), Guid.NewGuid(), [], sut));
        sut.AdmitTeam(Team.Create(new TeamName("Lag2"), Guid.NewGuid(), [], sut));
        sut.CreateDraft();
        sut.Drafts.First().Start();

        Assert.Equal(DraftState.Started, sut.Drafts.Single().State);

        Assert.Throws<InvalidOperationException>(() => sut.CreateDraft());
    }
}
