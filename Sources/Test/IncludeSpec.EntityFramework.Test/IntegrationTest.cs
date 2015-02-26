using System;
using System.Linq;
using System.Threading.Tasks;
using IncludeSpec.EntityFramework.Integration;
using IncludeSpec.Test.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IncludeSpec.EntityFramework.Test
{
  [TestClass]
  public class IntegrationTest
  {
    private TestDatabase _database;
    private TestDbEntities _context;
    private IntegrationSource _integrationSource;

    [TestInitialize]
    public void TestInitialize()
    {
      _database = TestDatabase.Create(Resources.TestDb).Result;
      _context = new TestDbEntities(_database.ConnectionString);
      _integrationSource = new IntegrationSource(_context);
    }

    [TestCleanup]
    public void TestCleanup()
    {
      TestDatabase.Delete(_database).Wait();
    }

    private async Task<TestDbEntities> InitializeDbEntities()
    {
      var context = new TestDbEntities(_database.ConnectionString);
      var group1 = context.Groups.Create();
      group1.Id = Guid.NewGuid();
      group1.Name = "Group 1";
      context.Groups.Add(group1);

      var group11 = context.Groups.Create();
      group11.ParentGroupId = group1.Id;
      group11.Id = Guid.NewGuid();
      group11.Name = "Group 1.1";
      context.Groups.Add(group11);

      var group12 = context.Groups.Create();
      group12.ParentGroupId = group1.Id;
      group12.Id = Guid.NewGuid();
      group12.Name = "Group 1.2";
      context.Groups.Add(group12);

      var group121 = context.Groups.Create();
      group121.ParentGroupId = group12.Id;
      group121.Id = Guid.NewGuid();
      group121.Name = "Group 1.2.1";
      context.Groups.Add(group121);

      var group122 = context.Groups.Create();
      group122.ParentGroupId = group12.Id;
      group122.Id = Guid.NewGuid();
      group122.Name = "Group 1.2.2";
      context.Groups.Add(group122);

      var group13 = context.Groups.Create();
      group13.ParentGroupId = group1.Id;
      group13.Id = Guid.NewGuid();
      group13.Name = "Group 1.3";
      context.Groups.Add(group13);

      var group2 = context.Groups.Create();
      group2.Id = Guid.NewGuid();
      group2.Name = "Group 2";
      context.Groups.Add(group2);

      var group21 = context.Groups.Create();
      group21.ParentGroupId = group2.Id;
      group21.Id = Guid.NewGuid();
      group21.Name = "Group 2.1";
      context.Groups.Add(group21);

      var group22 = context.Groups.Create();
      group22.ParentGroupId = group2.Id;
      group22.Id = Guid.NewGuid();
      group22.Name = "Group 2.2";
      context.Groups.Add(group22);

      var group23 = context.Groups.Create();
      group23.ParentGroupId = group2.Id;
      group23.Id = Guid.NewGuid();
      group23.Name = "Group 2.3";
      context.Groups.Add(group23);

      await context.SaveChangesAsync();

      return new TestDbEntities(_database.ConnectionString);
    }

    [TestMethod]
    public async Task IncludeSpec_Integration_EntityFramework_Query_With_No_Includes_Test()
    {
      var context = await InitializeDbEntities();
      var query = context.Groups.Where(g => g.Name == "Group 1.2");

      var groups = await context.QueryAsync(query, null);
      Assert.AreEqual(1, groups.Count);
      foreach (var group in groups)
      {
        Assert.IsNull(group.ParentGroup);
        Assert.AreEqual(0, group.ChildGroups.Count);
      }
    }

    [TestMethod]
    public async Task IncludeSpec_Integration_EntityFramework_Query_With_Includes_Test()
    {
      var context = await InitializeDbEntities();
      var query = context.Groups.Where(g => g.Name == "Group 1.2");
      var includes = IncludeSpecification
        .For<Group>()
        .Include(g => g.ParentGroup)
        .IncludeCollection(g => g.ChildGroups);

      var groups = await context.QueryAsync(query, includes);
      Assert.AreEqual(1, groups.Count);
      foreach (var group in groups)
      {
        Assert.IsNotNull(group.ParentGroup);
        Assert.AreEqual(2, group.ChildGroups.Count);
      }
    }

    [TestMethod]
    public async Task IncludeSpec_Integration_EntityFramework_Query_With_Includes_Loaded_Separately_Test()
    {
      var context = await InitializeDbEntities();
      var query = context.Groups.Where(g => g.Name == "Group 1.2");
      var includes = IncludeSpecification
        .For<Group>()
        .Include(g => g.ParentGroup, true)
        .IncludeCollection(g => g.ChildGroups, true);

      var groups = await context.QueryAsync(query, includes);
      Assert.AreEqual(1, groups.Count);
      foreach (var group in groups)
      {
        Assert.IsNotNull(group.ParentGroup);
        Assert.AreEqual(2, group.ChildGroups.Count);
      }
    }
  }
}
