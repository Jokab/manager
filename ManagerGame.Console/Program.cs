using ManagerGame;
using ManagerGame.Core;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
using Microsoft.EntityFrameworkCore;

var dbContext = ConnectDb();

var exit = false;
while (!exit)
{
    Console.WriteLine();
    Console.WriteLine("Vad vill du göra?");
    Console.WriteLine("1. Skapa manager");
    Console.WriteLine("2. Logga in");
    Console.WriteLine("3. Avsluta");

    var selection = Console.ReadLine();
    if (selection == "3")
    {
        exit = true;
    }
    
    await Loop(selection, dbContext);
}

return;

async Task Loop(string? selection,
    ApplicationDbContext dbContext)
{
    switch (selection)
    {
        case "1":
            var manager = await CreateManager(dbContext);
            break;
        case "2":
            Console.Write("Namn: ");
            var managerName = Console.ReadLine()!;
            var token = await Login(new ManagerName(managerName), dbContext);
            Console.WriteLine("Du är nu inloggad tjoho!");

            break;
    }
    
    // Console.WriteLine("Manager med ID " + manager.Id + " skapad");
    // Console.WriteLine("Namn från db " + dbContext.Managers.First(x => x.Id == manager.Id).Name.Name);
    //
    // var team = await CreateTeam(manager, dbContext);
    // if (!team.IsSuccess)
    // {
    //     Console.WriteLine("Failed to create team");
    // }
    //
    // Console.WriteLine("Managers lag" + dbContext.Managers.Include(manager => manager.Teams).First(x => x.Id == manager.Id).Teams.First().Name.Name);
    //
    //

}

static async Task<Result<LoginResponse>> Login(ManagerName managerName, ApplicationDbContext applicationDbContext)
{
    var managerWithName = await applicationDbContext.Managers.FirstOrDefaultAsync(x => x.Name == managerName);
    if (managerWithName is null)
    {
        throw new Exception("Managern finns inte");
    }
    var loginHandler = new LoginCommandHandler(applicationDbContext);

    var request = new LoginRequest {ManagerId = managerWithName.Id};
    var loginResult = await loginHandler.Handle(request, CancellationToken.None);
    return loginResult;
}

ApplicationDbContext ConnectDb()
{
    var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
    builder.UseNpgsql("Host=localhost;Database=test;User Id=postgres;Password=1234");
    var applicationDbContext = new ApplicationDbContext(builder.Options);
    return applicationDbContext;
}

async Task<Result<Team>> CreateTeam(Manager manager, ApplicationDbContext dbContext1)
{
    Console.WriteLine("Lagnamn: ");
    var name = Console.ReadLine()!;
    var createTeamHandler = new CreateTeamCommandHandler(dbContext1);
    var team = (await createTeamHandler.Handle(new CreateTeamRequest
            { ManagerId = manager.Id, Name = new TeamName(name) },
        CancellationToken.None));
    return team;
}

async Task<Manager> CreateManager(ApplicationDbContext dbContext1)
{
    Console.Write("Manager namn: ");
    var name = Console.ReadLine()!;
    Console.Write("Manager mejl: ");
    var email = Console.ReadLine()!;
    var createManagerHandler = new CreateManagerCommandHandler(dbContext1);
    var manager1 = (await createManagerHandler.Handle(new CreateManagerRequest
        { Email = new Email(email), Name = new ManagerName(name) })).Value;
    return manager1;
}

