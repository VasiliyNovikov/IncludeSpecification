using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IncludeSpec.Integration;
using IncludeSpec.Plan;

namespace IncludeSpec
{
  public static class IncludeSpecificationExecutor
  {
    public static IEnumerable<T> Execute<T>(IIntegrationSource source, IQueryable<T> query, IncludeSpecification<T> includeSpec)
    {
      return Execute(source, query, IncludePlan.BuildPlan(source, includeSpec));
    }

    private static IEnumerable<T> Execute<T>(IIntegrationSource source, IQueryable<T> query, IncludePlan plan)
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

    private static void Execute(IIntegrationSource source, IQueryable query, IncludePlan plan)
    {
      ExecuteMethodDefinition.MakeGenericMethod(query.ElementType).Invoke(null, new object[] { source, query, plan });
    }

// ReSharper disable UnusedMember.Local
    private static void ExecuteHelper<T>(IIntegrationSource source, IQueryable<T> query, IncludePlan plan)
// ReSharper restore UnusedMember.Local
    {
      Execute(source, query, plan);
    }

    private static readonly MethodInfo ExecuteMethodDefinition = typeof(IncludeSpecificationExecutor).GetMethod("ExecuteHelper", BindingFlags.Static | BindingFlags.NonPublic);

    public static Task<IEnumerable<T>> ExecuteAsync<T>(IIntegrationSource source, IQueryable<T> query, IncludeSpecification<T> includeSpec, bool tryExecuteConcurrently = false)
    {
      return ExecuteAsync(source, query, IncludePlan.BuildPlan(source, includeSpec), tryExecuteConcurrently);
    }

    private static async Task<IEnumerable<T>> ExecuteAsync<T>(IIntegrationSource source, IQueryable<T> query, IncludePlan plan, bool tryExecuteConcurrently)
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

    private static Task ExecuteAsync(IIntegrationSource source, IQueryable query, IncludePlan plan, bool tryExecuteConcurrently)
    {
      var executeAsyncHelper = ExecuteAsyncMethodDefinition.MakeGenericMethod(query.ElementType);
      return (Task)executeAsyncHelper.Invoke(null, new object[] { source, query, plan, tryExecuteConcurrently });
    }

// ReSharper disable UnusedMember.Local
    private static async Task ExecuteAsyncHelper<T>(IIntegrationSource source, IQueryable<T> query, IncludePlan plan, bool tryExecuteConcurrently)
// ReSharper restore UnusedMember.Local
    {
      await ExecuteAsync(source, query, plan, tryExecuteConcurrently);
    }

    private static readonly MethodInfo ExecuteAsyncMethodDefinition = typeof(IncludeSpecificationExecutor).GetMethod("ExecuteAsyncHelper", BindingFlags.Static | BindingFlags.NonPublic);
  }
}
