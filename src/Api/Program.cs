using Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddNpgsqlDbContext<TodoDbContext>("pgdb");

builder.Services.AddTransient<ToDoService>();

var app = builder.Build();

app.UseHttpsRedirection();

var todos = app.MapGroup("todos");

todos.MapGet("/", (ToDoService svc) => svc.GetAll());

todos.MapPost("/", async (ToDoService svc, TodoItem item) => {
    item.Id = Guid.NewGuid();
    item.CreatedAt = DateTime.UtcNow;

    await svc.Add(item);

    return Results.StatusCode(StatusCodes.Status201Created);
});

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    using var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

    await db.Database.EnsureCreatedAsync();
}

app.Run();
