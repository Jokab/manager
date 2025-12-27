using OneOf;
using OneOf.Types;

namespace ManagerGame.Domain;

public class Draft : Entity
{
    private Draft(Guid id,
        Guid leagueId,
        DraftOrder draftOrder,
        IReadOnlyList<Guid> participantTeamIds) : base(id)
    {
        LeagueId = leagueId;
        DraftOrder = draftOrder;
        Participants = participantTeamIds
            .Select((teamId, seat) => DraftParticipant.Create(id, teamId, seat))
            .ToList();
        Picks = [];
        State = DraftState.Created;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Draft(Guid id) : base(id)
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public Guid LeagueId { get; private init; }
    public League League { get; private init; } = null!;
    public DraftState State { get; private set; }
    private DraftOrder DraftOrder { get; } = new();

    public ICollection<DraftParticipant> Participants { get; private init; } = [];
    public ICollection<DraftPick> Picks { get; private init; } = [];

    private Guid[] ParticipantTeamIdsOrdered =>
        Participants.OrderBy(x => x.Seat).Select(x => x.TeamId).ToArray();

    public static Draft DoubledPeakTraversalDraft(Guid leagueId, IReadOnlyList<Guid> participantTeamIds)
    {
        if (participantTeamIds.Count == 0) throw new ArgumentException("No teams in league", nameof(participantTeamIds));

        return new Draft(
            Guid.NewGuid(),
            leagueId,
            new DraftOrder(new DoubledPeakTraversalDraftOrder()),
            participantTeamIds);
    }

    public Guid? PeekNextTeamId()
    {
        if (State is DraftState.Created)
        {
            return null;
        }

        return DraftOrder.PeekNextTeamId(ParticipantTeamIdsOrdered);
    }

    public Guid AdvanceAndGetNextTeamId()
    {
        if (State is DraftState.Created)
            throw new InvalidOperationException("Draft is not started");
        return DraftOrder.AdvanceAndGetNextTeamId(ParticipantTeamIdsOrdered);
    }

    public DraftPick RecordPick(Guid teamId, Guid playerId)
    {
        if (State is DraftState.Created)
            throw new InvalidOperationException("Draft is not started");

        var expectedTeamId = PeekNextTeamId();
        if (expectedTeamId is null)
            throw new InvalidOperationException("No next team available");
        if (expectedTeamId.Value != teamId)
            throw new InvalidOperationException("It's not your turn to draft");

        var pick = DraftPick.Create(Id, Picks.Count + 1, teamId, playerId);
        Picks.Add(pick);
        AdvanceAndGetNextTeamId();
        return pick;
    }

    public void Start()
    {
        if (State is DraftState.Started) throw new InvalidOperationException("Draft is already started");
        const int minimumTeamCount = 2;
        if (Participants.Count < minimumTeamCount)
            throw new ArgumentException($"Too few teams to draft, needs at least {minimumTeamCount}");
        State = DraftState.Started;
    }
}

public enum DraftState
{
    Created,
    Started,
    Finished
}

public class StartedDraft
{

}
