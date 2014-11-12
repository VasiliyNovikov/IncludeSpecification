using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using IncludeSpec.EntityFramework.Integration;
using IncludeSpec.Test.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IncludeSpec.EntityFramework.Test
{
  [TestClass]
  public class IntegrationSourceTest
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

    [TestMethod]
    public async Task CreateDeleteDatabaseTest()
    {
      using (var connection = new SqlConnection(_database.ConnectionString))
      {
        await connection.OpenAsync();
        using (var command = new SqlCommand("select GETUTCDATE()", connection))
        {
          var dateTime = (DateTime)await command.ExecuteScalarAsync();
          var diff = Math.Abs((dateTime - DateTime.UtcNow).TotalMilliseconds);
          Assert.IsTrue(diff < 500);
        }
      }
    }

    [TestMethod]
    public void IntegrationSource_GetPrimaryKey_Test()
    {
      var primaryKey = _integrationSource.GetPrimaryKey(typeof (User)).ToList();
      Assert.AreEqual(1, primaryKey.Count);
      Assert.AreEqual("Id", primaryKey[0].Name);
      Assert.AreEqual(typeof(Guid), primaryKey[0].PropertyType);
      Assert.AreEqual(typeof(User), primaryKey[0].ReflectedType);

      primaryKey = _integrationSource.GetPrimaryKey(typeof (Group)).ToList();
      Assert.AreEqual(1, primaryKey.Count);
      Assert.AreEqual("Id", primaryKey[0].Name);
      Assert.AreEqual(typeof(Guid), primaryKey[0].PropertyType);
      Assert.AreEqual(typeof(Group), primaryKey[0].ReflectedType);
    }

    [TestMethod]
    public void IntegrationSource_GetNavigationKey_For_Single_Reference_Test()
    {
      var parentGroupProperty = typeof (Group).GetProperty("ParentGroup");
      var navigationKey = _integrationSource.GetNavigationKey(parentGroupProperty).ToList();
      Assert.AreEqual(1, navigationKey.Count);
      Assert.AreEqual("ParentGroupId", navigationKey[0].Name);
      Assert.AreEqual(typeof(Guid?), navigationKey[0].PropertyType);
      Assert.AreEqual(typeof(Group), navigationKey[0].ReflectedType);
    }

    [TestMethod]
    public void IntegrationSource_GetNavigationKey_For_Collection_Test()
    {
      var childGroupsProperty = typeof(Group).GetProperty("ChildGroups");
      var navigationKey = _integrationSource.GetNavigationKey(childGroupsProperty).ToList();
      Assert.AreEqual(1, navigationKey.Count);
      Assert.AreEqual("Id", navigationKey[0].Name);
      Assert.AreEqual(typeof(Guid), navigationKey[0].PropertyType);
      Assert.AreEqual(typeof(Group), navigationKey[0].ReflectedType);
    }
  }
}