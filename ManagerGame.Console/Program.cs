using ManagerGame.Core;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
using Microsoft.EntityFrameworkCore;

var dbContext = ConnectDb();
var manager = await CreateManager(dbContext);

Console.WriteLine("Manager med ID " + manager.Id + " skapad");
Console.WriteLine("Namn från db " + dbContext.Managers.First(x => x.Id == manager.Id).Name.Name);

var team = await CreateTeam(dbContext);

Console.WriteLine("Managers lag" + dbContext.Managers.Include(manager => manager.Teams).First(x => x.Id == manager.Id).Teams.First().Name.Name);

return;

ApplicationDbContext ConnectDb()
{
    var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
    builder.UseNpgsql("Host=localhost;Database=test;User Id=postgres;Password=1234");
    var applicationDbContext = new ApplicationDbContext(builder.Options);
    return applicationDbContext;
}

async Task<Manager> CreateManager(ApplicationDbContext dbContext1)
{
    Console.WriteLine("Manager namn");
    var name = Console.ReadLine()!;
    Console.WriteLine("Manager mejl");
    var email = Console.ReadLine()!;
    var createManagerHandler = new CreateManagerCommandHandler(dbContext1);
    var manager1 = (await createManagerHandler.Handle(new CreateManagerRequest
        { Email = new Email(email), Name = new ManagerName(name) })).Value;
    return manager1;
}

async Task<Team> CreateTeam(ApplicationDbContext dbContext1)
{
    Console.WriteLine("Lagnamn");
    var name = Console.ReadLine()!;
    var createTeamHandler = new CreateTeamCommandHandler(dbContext1);
    var team = (await createTeamHandler.Handle(new CreateTeamRequest
            { Name = new TeamName(name) },
        CancellationToken.None)).Value;
    return team;
}