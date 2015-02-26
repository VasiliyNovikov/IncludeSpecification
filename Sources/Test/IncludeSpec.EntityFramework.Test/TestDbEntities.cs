using System.Configuration;
using System.Data.Entity.Core.EntityClient;

namespace IncludeSpec.EntityFramework.Test
{
  partial class TestDbEntities
  {
    private static string BuildConnectionString(string sqlConnectionString)
    {
      var builder = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["TestDbEntities"].ConnectionString);
      builder.ProviderConnectionString = sqlConnectionString;
      return builder.ToString();
    }

    public TestDbEntities(string connectionString)
      : base(BuildConnectionString(connectionString))
    {
      Configuration.LazyLoadingEnabled = false;
    }
  }
}
