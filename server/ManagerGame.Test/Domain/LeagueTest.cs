using ManagerGame.Domain;

namespace ManagerGame.Test.Domain;

public class LeagueTest
{
    // Helper method to set ID for domain tests (since EF Core won't generate them)
    private static void SetId(Entity entity, Guid id)
    {
        entity.Id = id;
    }
    [Fact]
    public void CanCreateDraft()
    {
        var sut = League.Empty();
        SetId(sut, Guid.NewGuid());

        var team = Team.Create(new TeamName("Lag"), Guid.NewGuid(), [], sut.Id);
        SetId(team, Guid.NewGuid());
        sut.AdmitTeam(team);
        sut.CreateDraft();

        Assert.Single(sut.Drafts);
    }

    [Fact]
    public void CanStartDraft()
    {
        var sut = League.Empty();
        SetId(sut, Guid.NewGuid());

        var team1 = Team.Create(new TeamName("Lag"), Guid.NewGuid(), [], sut.Id);
        SetId(team1, Guid.NewGuid());
        var team2 = Team.Create(new TeamName("Lag2"), Guid.NewGuid(), [], sut.Id);
        SetId(team2, Guid.NewGuid());
        sut.AdmitTeam(team1);
        sut.AdmitTeam(team2);
        sut.CreateDraft();
        sut.Drafts.First().Start(Team.PlayerLimit);

        Assert.Equal(DraftState.Started, sut.Drafts.First().State);
    }


    [Fact]
    public void CannotStartAlreadyStartedDraft()
    {
        var sut = League.Empty();
        SetId(sut, Guid.NewGuid());

        var team1 = Team.Create(new TeamName("Lag"), Guid.NewGuid(), [], sut.Id);
        SetId(team1, Guid.NewGuid());
        var team2 = Team.Create(new TeamName("Lag2"), Guid.NewGuid(), [], sut.Id);
        SetId(team2, Guid.NewGuid());
        sut.AdmitTeam(team1);
        sut.AdmitTeam(team2);
        sut.CreateDraft();
        sut.Drafts.First().Start(Team.PlayerLimit);

        Assert.Throws<InvalidOperationException>(() => sut.Drafts.First().Start(Team.PlayerLimit));
    }

    [Fact]
    public void CannotCreateDraftWithCreatedDraft()
    {
        var sut = League.Empty();
        SetId(sut, Guid.NewGuid());

        var team1 = Team.Create(new TeamName("Lag"), Guid.NewGuid(), [], sut.Id);
        SetId(team1, Guid.NewGuid());
        var team2 = Team.Create(new TeamName("Lag2"), Guid.NewGuid(), [], sut.Id);
        SetId(team2, Guid.NewGuid());
        sut.AdmitTeam(team1);
        sut.AdmitTeam(team2);
        sut.CreateDraft();

        Assert.Equal(DraftState.Created, sut.Drafts.Single().State);

        Assert.Throws<InvalidOperationException>(() => sut.CreateDraft());
    }

    [Fact]
    public void CannotCreateDraftWithStartedDraft()
    {
        var sut = League.Empty();
        SetId(sut, Guid.NewGuid());

        var team1 = Team.Create(new TeamName("Lag"), Guid.NewGuid(), [], sut.Id);
        SetId(team1, Guid.NewGuid());
        var team2 = Team.Create(new TeamName("Lag2"), Guid.NewGuid(), [], sut.Id);
        SetId(team2, Guid.NewGuid());
        sut.AdmitTeam(team1);
        sut.AdmitTeam(team2);
        sut.CreateDraft();
        sut.Drafts.First().Start(Team.PlayerLimit);

        Assert.Equal(DraftState.Started, sut.Drafts.Single().State);

        Assert.Throws<InvalidOperationException>(() => sut.CreateDraft());
    }
}
