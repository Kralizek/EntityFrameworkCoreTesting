using Api;

namespace Tests;

[TestFixture]
public class ToDoServiceTests : TestsWithDatabase
{
  [Test]
  public async Task GetAll_should_return_empty_array()
  {
    var sut = new ToDoService(Database);

    var items = await sut.GetAll();

    Assert.That(items, Is.Empty);
  }

  [Test]
  public async Task Add_adds_item_successfully()
  {
    var item = new TodoItem
    {
      Id = Guid.NewGuid(),
      Title = "Test",
      Description = "Some very important description",
      CreatedAt = DateTime.UtcNow,
      IsComplete = false
    };

    var sut = new ToDoService(Database);

    await sut.Add(item);

    Assert.That(Database.Items.Count(c => c.Id == item.Id), Is.EqualTo(1));
  }
}