using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IncludeSpec.Integration;
using IncludeSpec.Utils;

namespace IncludeSpec.Plan
{
  public class KeyBasedLoadPlan : LoadPlan
  {
    public KeyBasedLoadPlan(Type entityType, Type referencingEntityType, IncludePlan includePlan, IEnumerable<PropertyInfo> primaryKey, IEnumerable<PropertyInfo> referencingEntityReferenceKey, int? desiredBatchSize)
      : base(entityType, referencingEntityType, includePlan, desiredBatchSize)
    {
      PrimaryKey = primaryKey.ToList().AsReadOnly();
      ReferencingEntityReferenceKey = referencingEntityReferenceKey.ToList().AsReadOnly();
    }

    public IList<PropertyInfo> ReferencingEntityReferenceKey { get; private set; }

    public IList<PropertyInfo> PrimaryKey { get; private set; }

    public override List<IQueryable> BuildQueries(IIntegrationSource source, IEnumerable referencingEntities)
    {
      var entityKeys = referencingEntities.OfType<object>()
        .Distinct()
        .Select(e => ReferencingEntityReferenceKey.Select(p => p.GetValue(e, null)).ToArray())
        .Where(ek => ek.All(val => val != null))
        .Distinct(ArrayEqualityComparer<object>.Instance)
        .ToList();

      var result = new List<IQueryable>();
      var fullCount = entityKeys.Count;
      var whereHelper = WhereMethodDefinition.MakeGenericMethod(EntityType);
      for (var i = 0; i < fullCount; i += BatchSize)
      {
        var itemsCount = Math.Min(BatchSize, fullCount - i);
        var entityKeyList = Enumerable.Range(i, itemsCount).Select(j => entityKeys[j]).ToList();
        var predicate = ExpressionBuilder.KeyListContains(EntityType, PrimaryKey, entityKeyList);
        var query = (IQueryable)whereHelper.Invoke(null, new object[] { source, predicate });
        result.Add(query);
      }

      return result;
    }

    // ReSharper disable UnusedMember.Local
    private static IQueryable<T> WhereHelper<T>(IIntegrationSource source, Expression<Func<T, bool>> predicate)
      where T : class
    {
      return source.CreateQuery<T>().Where(predicate);
    }
    // ReSharper restore UnusedMember.Local

    private static readonly MethodInfo WhereMethodDefinition = typeof(KeyBasedLoadPlan).GetMethod("WhereHelper", BindingFlags.Static | BindingFlags.NonPublic);
  }

  class ArrayEqualityComparer<T> : IEqualityComparer<T[]>
  {
    public bool Equals(T[] x, T[] y)
    {
      if (ReferenceEquals(x, y))
      {
        return true;
      }

      if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
      {
        return false;
      }

      if (x.Length != y.Length)
      {
        return false;
      }

      return !x.Where((t, i) => !t.Equals(y[i])).Any();
    }

    public int GetHashCode(T[] arr)
    {
      if (ReferenceEquals(arr, null))
      {
        return 0;
      }

      return arr.Aggregate(13, (current, i) => (current * 7) + i.GetHashCode());
    }

    private static readonly Lazy<ArrayEqualityComparer<T>> _instance = new Lazy<ArrayEqualityComparer<T>>(() => new ArrayEqualityComparer<T>());
    public static ArrayEqualityComparer<T> Instance
    {
      get { return _instance.Value; }
    }
  }
}