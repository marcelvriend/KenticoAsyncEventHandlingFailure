using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder()
    .ConfigureServices((_, _) => { CMSApplication.Init(); })
    .Build();

using var context = new CMSActionContext();
context.DisableAll();

var foo = new Something();
foo.SomeEvent += async void (_, _) =>
{
    async Task<List<TreeNode>> Retrieve() =>
        (await new MultiDocumentQuery()
            .WhereEquals("NodeID",
                1) // Must be an existing NodeID, with a non-existing NodeID and culture it doesn't throw exceptions.
            .Culture("en-US")
            .Published()
            .WithCoupledColumns()
            .GetEnumerableTypedResultAsync().ConfigureAwait(false))
        .ToList();

    var result = await Retrieve();
    Console.WriteLine(result.Count);
};

for (var i = 0; i < 10; i++)
{
    foo.DoSomething();
}

internal class Something
{
    public delegate void EventHandler(object sender, EventArgs e);

    public event EventHandler? SomeEvent;

    public void DoSomething()
    {
        SomeEvent?.Invoke(this, EventArgs.Empty);
    }
}