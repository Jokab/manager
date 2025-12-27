using System.Text.Json.Serialization;

namespace ManagerGame.Domain;

public class League : Entity
{
    [JsonConstructor]
    private League()
    {
        Teams = [];
        Drafts = [];
        MatchResults = [];
        InviteCode = GenerateInviteCode();
        Settings = new LeagueSettings();
        IsGroupStageCompleted = false;
        IsKnockoutDraftCompleted = false;
        IsTournamentCompleted = false;
    }

    public string Name { get; set; } = string.Empty;
    public string InviteCode { get; private set; }
    public LeagueSettings Settings { get; private set; }
    public ICollection<Team> Teams { get; init; }
    public ICollection<Draft> Drafts { get; init; }
    public ICollection<MatchResult> MatchResults { get; private set; }
    public bool IsGroupStageCompleted { get; private set; }
    public bool IsKnockoutDraftCompleted { get; private set; }
    public bool IsTournamentCompleted { get; private set; }

    public static League Empty() => new();

    public void AdmitTeam(Team team)
    {
        if (Teams.Any(x => x.Name == team.Name))
        {
            throw new ArgumentException("Team with name already joined");
        }
        Teams.Add(team);
    }

    public void CreateDraft()
    {
        if (Drafts.Any(x => x.State != DraftState.Finished))
            throw new InvalidOperationException("Cannot create new draft while there unfinished drafts");
        Drafts.Add(Draft.DoubledPeakTraversalDraft(Id, Teams.Select(x => x.Id).ToList()));
    }

    public void CreateKnockoutDraft()
    {
        if (!IsGroupStageCompleted)
            throw new InvalidOperationException("Cannot create knockout draft before group stage is completed");

        if (IsKnockoutDraftCompleted)
            throw new InvalidOperationException("Knockout draft has already been completed");

        // Create a new draft for the knockout phase
        Drafts.Add(Draft.DoubledPeakTraversalDraft(Id, Teams.Select(x => x.Id).ToList()));
        IsKnockoutDraftCompleted = true;
    }

    public void CompleteGroupStage()
    {
        if (IsGroupStageCompleted)
            return;

        // Check if all group stage matches are completed
        var groupStageMatches = MatchResults.Where(m => !m.IsFinished && IsGroupStageMatch(m)).ToList();
        if (groupStageMatches.Any())
            throw new InvalidOperationException("Cannot complete group stage while matches are still in progress");

        IsGroupStageCompleted = true;
    }

    public void CompleteTournament()
    {
        if (IsTournamentCompleted)
            return;

        // Check if all matches are completed
        if (MatchResults.Any(m => !m.IsFinished))
            throw new InvalidOperationException("Cannot complete tournament while matches are still in progress");

        IsTournamentCompleted = true;
    }

    private bool IsGroupStageMatch(MatchResult match)
    {
        // Implement logic to determine if a match is part of the group stage
        // This would depend on how you're organizing matches in your tournament
        // For simplicity, this could be based on match dates or round information
        return true; // Simplified implementation
    }

    private static string GenerateInviteCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
