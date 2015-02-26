using IncludeSpec.Test.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace IncludeSpec.EntityFramework.Test
{
  [TestClass]
  public class TestDatabaseTest
  {
    [TestMethod]
    public async Task CreateDeleteDatabaseTest()
    {
      var database = await TestDatabase.Create(Resources.TestDb);
      try
      {
        using (var connection = new SqlConnection(database.ConnectionString))
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
      finally
      {
        await TestDatabase.Delete(database);
      }
    }
  }
}
