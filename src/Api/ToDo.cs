using Microsoft.EntityFrameworkCore;

namespace Api;

public class TodoItem
{ 
	public Guid Id { get; set; }
	
	public string Title { get; set; } = default!;
	
	public string Description { get; set; } = default!;
	
	public DateTime CreatedAt { get; set; }
	
	public bool IsComplete { get; set; }
}

public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
	public DbSet<TodoItem> Items => Set<TodoItem>();
}

public class ToDoService(TodoDbContext db)
{
	public async Task<TodoItem[]> GetAll()
	{
		var items = await db.Items.ToArrayAsync();

		return items;
	}

	public async Task Add(TodoItem item)
	{
		db.Items.Add(item);

		await db.SaveChangesAsync();
	}
}