using IncludeSpec.EntityFramework.Integration;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace IncludeSpec.EntityFramework
{
  public static class IncludeSpecIntegration
  {
    public static IList<T> Query<T>(this DbContext context, IQueryable<T> query, IncludeSpecification<T> includeSpec)
    {
      var integrationSource = new IntegrationSource(context);
      return IncludeSpecificationExecutor.Execute(integrationSource, query, includeSpec);
    }

    public static async Task<IList<T>> QueryAsync<T>(this DbContext context, IQueryable<T> query, IncludeSpecification<T> includeSpec)
    {
      var integrationSource = new IntegrationSource(context);
      return await IncludeSpecificationExecutor.ExecuteAsync(integrationSource, query, includeSpec);
    }
  }
}
