using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IncludeSpec.Integration;

namespace IncludeSpec.Plan
{
  public abstract class LoadPlan
  {
    public const int DefaultBatchSize = 256;

    protected LoadPlan(Type entityType, Type referencingEntityType, IncludePlan includePlan, int? desiredBatchSize)
    {
      EntityType = entityType;
      ReferencingEntityType = referencingEntityType;
      IncludePlan = includePlan;
      DesiredBatchSize = desiredBatchSize;
    }

    public Type EntityType { get; private set; }
    public Type ReferencingEntityType { get; private set; }
    public IncludePlan IncludePlan { get; private set; }
    public int? DesiredBatchSize { get; private set; }

    public int BatchSize
    {
      get { return DesiredBatchSize ?? DefaultBatchSize; }
    }

    public abstract List<IQueryable> BuildQueries(IIntegrationSource source, IEnumerable referencingEntities);
  }
}
