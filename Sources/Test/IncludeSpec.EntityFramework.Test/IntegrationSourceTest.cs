using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using IncludeSpec.Test.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IncludeSpec.EntityFramework.Test
{
  [TestClass]
  public class IntegrationSourceTest
  {
    private TestDatabase _database;

    [TestInitialize]
    public void TestInitialize()
    {
      _database = TestDatabase.Create(Resources.TestDb).Result;
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
  }
}