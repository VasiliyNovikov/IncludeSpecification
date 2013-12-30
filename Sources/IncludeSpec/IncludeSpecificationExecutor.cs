using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using IncludeSpec.Integration;
using IncludeSpec.Plan;

namespace IncludeSpec
{
  public static class IncludeSpecificationExecutor
  {
    public static IEnumerable Execute(IIntegrationSource source, IQueryable query, IncludeSpecification includeSpec)
    {
      return Execute(source, query, IncludePlan.BuildPlan(source, includeSpec));
    }

    private static IEnumerable Execute(IIntegrationSource source, IQueryable query, IncludePlan plan)
    {
      var includedQuery = plan.AddIncludes(source, query);
      var entities = source.ExecuteQuery(includedQuery);
      foreach (var separate in plan.Separates)
      {
        var queries = separate.BuildQueries(source, entities);
        var includePlan = separate.IncludePlan ?? IncludePlan.Empty;
        foreach (var separateQuery in queries)
        {
          Execute(source, separateQuery, includePlan);
        }
      }

      return entities;
    }

    public static Task<IEnumerable> ExecuteAsync(IIntegrationSource source, IQueryable query, IncludeSpecification includeSpec, bool tryExecuteConcurrently = false)
    {
      return ExecuteAsync(source, query, IncludePlan.BuildPlan(source, includeSpec), tryExecuteConcurrently);
    }

    private static async Task<IEnumerable> ExecuteAsync(IIntegrationSource source, IQueryable query, IncludePlan plan, bool tryExecuteConcurrently)
    {
      var includedQuery = plan.AddIncludes(source, query);
      var entities = await source.ExecuteQueryAsync(includedQuery);
      foreach (var separate in plan.Separates)
      {
        var queries = separate.BuildQueries(source, entities);
        var includePlan = separate.IncludePlan ?? IncludePlan.Empty;
        if (tryExecuteConcurrently)
        {
          await Task.WhenAll(queries.Select(separateQuery => ExecuteAsync(source, separateQuery, includePlan, true)));
        }
        else
        {
          foreach (var separateQuery in queries)
          {
            await ExecuteAsync(source, separateQuery, includePlan, false);
          }
        }
      }

      return entities;
    }
  }
}
