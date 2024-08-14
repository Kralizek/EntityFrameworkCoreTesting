using Api;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace Tests;

[SetUpFixture]
public class Tests
{
    private static readonly PostgreSqlContainer DatabaseContainer = new PostgreSqlBuilder()
      .WithImage("postgres:latest")
      .WithDatabase("mydatabase")
      .WithUsername("myusername")
      .WithPassword("mypassword")
      .WithWaitStrategy(Wait
        .ForUnixContainer()
        .UntilMessageIsLogged("database system is ready to accept connections"))
      .Build();

    private static Respawner respawner = default!;

    private static RespawnerOptions respawnerOptions = new() { DbAdapter = DbAdapter.Postgres };

    public static WebApplicationFactory<Program> WebApplicationFactory = default!;

    public static string ConnectionString => DatabaseContainer.GetConnectionString();

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await DatabaseContainer.StartAsync();

        WebApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(web => web.UseSetting("ConnectionStrings:pgdb", ConnectionString));

        _ = WebApplicationFactory.CreateDefaultClient();

        await using var connection = new NpgsqlConnection(ConnectionString);

        await connection.OpenAsync();

        respawner = await Respawner.CreateAsync(connection, respawnerOptions);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await WebApplicationFactory.DisposeAsync();
        await DatabaseContainer.StopAsync();
        await DatabaseContainer.DisposeAsync();
    }

    public static async Task ResetDatabase()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);

        await connection.OpenAsync();

        await respawner.ResetAsync(connection);
    }
}

[TestFixture]
public abstract class TestsWithBackend
{
  protected TodoDbContext Database = default!;
  protected HttpClient Http = default!;

  [SetUp]
  public void SetUp()
  {
    var options = new DbContextOptionsBuilder<TodoDbContext>()
      .UseNpgsql(Tests.ConnectionString)
      .Options;

    Database = new TodoDbContext(options);

    Http = Tests.WebApplicationFactory.CreateDefaultClient();
  }

  [TearDown]
  public async Task TearDown()
  {
    await Tests.ResetDatabase();

    Http.Dispose();
    Database.Dispose();
  }
}