using ManagerGame.Core.Domain;

namespace ManagerGame.Test.Domain;

public class LeagueTest
{
    [Fact]
    public void CanCreateDraft()
    {
        var sut = new League(Guid.NewGuid());

        sut.AdmitTeam(Team.Create(new TeamName("Lag"), Guid.NewGuid(), [], sut.Id));
        sut.CreateDraft();

        Assert.Single(sut.Drafts);
    }

    [Fact]
    public void CanStartDraft()
    {
        var sut = new League(Guid.NewGuid());

        sut.AdmitTeam(Team.Create(new TeamName("Lag"), Guid.NewGuid(), [], sut.Id));
        sut.AdmitTeam(Team.Create(new TeamName("Lag2"), Guid.NewGuid(), [], sut.Id));
        sut.CreateDraft();
        sut.Drafts.First().Start();

        Assert.Equal(State.Started, sut.Drafts.First().State);
    }


    [Fact]
    public void CannotStartAlreadyStartedDraft()
    {
        var sut = new League(Guid.NewGuid());

        sut.AdmitTeam(Team.Create(new TeamName("Lag"), Guid.NewGuid(), [], sut.Id));
        sut.AdmitTeam(Team.Create(new TeamName("Lag2"), Guid.NewGuid(), [], sut.Id));
        sut.CreateDraft();
        sut.Drafts.First().Start();

        Assert.Throws<InvalidOperationException>(() => sut.Drafts.First().Start());
    }

    [Fact]
    public void CannotCreateDraftWithCreatedDraft()
    {
        var sut = new League(Guid.NewGuid());
        sut.AdmitTeam(Team.Create(new TeamName("Lag"), Guid.NewGuid(), [], sut.Id));
        sut.AdmitTeam(Team.Create(new TeamName("Lag2"), Guid.NewGuid(), [], sut.Id));
        sut.CreateDraft();

        Assert.Equal(State.Created, sut.Drafts.Single().State);

        Assert.Throws<InvalidOperationException>(() => sut.CreateDraft());
    }

    [Fact]
    public void CannotCreateDraftWithStartedDraft()
    {
        var sut = new League(Guid.NewGuid());
        sut.AdmitTeam(Team.Create(new TeamName("Lag"), Guid.NewGuid(), [], sut.Id));
        sut.AdmitTeam(Team.Create(new TeamName("Lag2"), Guid.NewGuid(), [], sut.Id));
        sut.CreateDraft();
        sut.Drafts.First().Start();

        Assert.Equal(State.Started, sut.Drafts.Single().State);

        Assert.Throws<InvalidOperationException>(() => sut.CreateDraft());
    }
}
