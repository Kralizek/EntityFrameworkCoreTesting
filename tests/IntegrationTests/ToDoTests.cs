using System.Net.Http.Json;

using Api;

namespace Tests;

[TestFixture]
public class ToDoTests : TestsWithBackend
{
    [Test]
    public async Task GET_should_return_all_items()
    {
        var items = await Http.GetFromJsonAsync<TodoItem[]>("/todos", default);

        Assert.That(items?.Length ?? 0, Is.EqualTo(Database.Items.Count()));
    }

    [Test]
    public async Task POST_should_create_new_item()
    {
        var item = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Description = "Some very important description",
            CreatedAt = DateTime.UtcNow,
            IsComplete = false
        };

        using var response = await Http.PostAsJsonAsync("/todos", item, default);

        Assert.That(response.IsSuccessStatusCode, Is.True);
    }
}