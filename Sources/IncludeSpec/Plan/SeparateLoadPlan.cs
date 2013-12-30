using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IncludeSpec.Integration;
using IncludeSpec.Utils;

namespace IncludeSpec.Plan
{
  public class SeparateLoadPlan : LoadPlan
  {
    public SeparateLoadPlan(LoadPlan basePlan, IEnumerable<IncludeMember> referencingEntityPath)
      : base(basePlan.EntityType, basePlan.ReferencingEntityType, basePlan.IncludePlan, basePlan.DesiredBatchSize)
    {
      if (basePlan == null) throw new ArgumentNullException("basePlan");
      if (referencingEntityPath == null) throw new ArgumentNullException("referencingEntityPath");

      BasePlan = basePlan;
      ReferencingEntityPath = referencingEntityPath.ToList().AsReadOnly();
    }

    public LoadPlan BasePlan { get; private set; }

    public IList<IncludeMember> ReferencingEntityPath { get; private set; }

    public override List<IQueryable> BuildQueries(IIntegrationSource source, IEnumerable sourceEntities)
    {
      var referencingEntitiesQuery = sourceEntities.OfType<object>();
      foreach (var includeMember in ReferencingEntityPath)
      {
        var includeProperty = includeMember as IncludeProperty;
        if (includeProperty != null)
        {
          referencingEntitiesQuery = referencingEntitiesQuery
            .Select(e => includeProperty.Member.GetMavigationMemberValue<object>(e))
            .Where(e => e != null);
        }
        else
        {
          var includeCollection = (IncludeCollection)includeMember;
          referencingEntitiesQuery = referencingEntitiesQuery.SelectMany(e => includeCollection.Member.GetMavigationMemberValue<IEnumerable>(e).OfType<object>());
        }
      }

      return BasePlan.BuildQueries(source, referencingEntitiesQuery);
    }
  }
}