using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace IncludeSpec.Test.Utility
{
  public class TestDatabase
  {
    public string ConnectionString { get; private set; }

    private TestDatabase(string connectionString)
    {
      ConnectionString = connectionString;
    }

    public static async Task<TestDatabase> Create(byte[] database)
    {
      var fileName = Path.GetTempFileName() + ".mdf";
      File.WriteAllBytes(fileName, database);
      var connectionBuilder = new SqlConnectionStringBuilder
      {
        DataSource = @"(LocalDB)\v12.0",
        AttachDBFilename = fileName,
        IntegratedSecurity = true
      };

      var connectionString = connectionBuilder.ConnectionString;

      using (var connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync();
      }

      return new TestDatabase(connectionString);
    }

    public static async Task Delete(TestDatabase database)
    {
      SqlConnection.ClearAllPools();
      var connectionBuilder = new SqlConnectionStringBuilder(database.ConnectionString);
      var fileName = connectionBuilder.AttachDBFilename;

      using (var conn = new SqlConnection(connectionBuilder.ConnectionString))
      {
        using (var cmd = new SqlCommand("drop database [" + fileName + "]", conn))
        {
          await conn.OpenAsync();
          conn.ChangeDatabase("master");
          await cmd.ExecuteNonQueryAsync();
        }
      }
    }
  }
}