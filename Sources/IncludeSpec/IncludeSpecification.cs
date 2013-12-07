using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IncludeSpec
{
  public class IncludeSpecification
  {
    protected IncludeSpecification(Type type, IEnumerable<IncludeMember> members)
    {
      if (type == null) throw new ArgumentNullException("type");
      Type = type;
      Members = (members ?? Enumerable.Empty<IncludeMember>()).ToList().AsReadOnly();
    }

    public static IncludeSpecification Create(Type type, IEnumerable<IncludeMember> members)
    {
      var specificType = typeof (IncludeSpecification<>).MakeGenericType(type);
      return (IncludeSpecification)Activator.CreateInstance(specificType, members);
    }

    public Type Type { get; private set; }

    public IList<IncludeMember> Members { get; private set; }

    public static IncludeSpecificationBuilder<T> For<T>()
    {
      return new IncludeSpecificationBuilder<T>();
    }

    #region Combine

    private static Delegate CombineConditions(Delegate c1, Delegate c2)
    {
      if (c1 == null || c2 == null)
      {
        return null;
      }

      var objectType = c1.Method.GetParameters()[0].ParameterType;
      var param = Expression.Parameter(objectType);
      return Expression.Lambda(Expression.OrElse(Expression.Invoke(Expression.Constant(c1), param),
                                                 Expression.Invoke(Expression.Constant(c2), param)),
                               param)
                       .Compile();
    }

    private static IncludeMember Combine(IncludeMember m1, IncludeMember m2)
    {
      var combinedSpec = Combine(m1.Specification, m2.Specification);
      var loadSeparately = m1.LoadSeparately || m2.LoadSeparately;
      var desiredBatchSize = m1.DesiredBatchSize == null || m2.DesiredBatchSize == null
                               ? (m1.DesiredBatchSize ?? m2.DesiredBatchSize)
                               : Math.Min(m1.DesiredBatchSize.Value, m2.DesiredBatchSize.Value);
      var includeCondition = CombineConditions(m1.IncludeCondition, m2.IncludeCondition);
      return m1 is IncludeProperty
               ? (IncludeMember)new IncludeProperty(m1.Member, combinedSpec, loadSeparately, desiredBatchSize, includeCondition)
               : new IncludeCollection(m1.Member, combinedSpec, loadSeparately, desiredBatchSize, includeCondition);
    }

    public static IncludeSpecification Combine(IncludeSpecification spec1, IncludeSpecification spec2)
    {
      if (spec1 == null)
      {
        return spec2;
      }
      
      if (spec2 == null)
      {
        return spec1;
      }

      var membersToMerge = spec1.Members.Select(m1 => new { M1 = m1, M2 = spec2.Members.FirstOrDefault(m2 => m1.Member == m2.Member) })
                                        .Where(mm => mm.M2 != null)
                                        .ToList();

      var resultMembers = spec1.Members.Where(m1 => membersToMerge.All(mm => m1 != mm.M1))
                                       .Concat(spec2.Members.Where(m2 => membersToMerge.All(mm => m2 != mm.M2)))
                                       .Concat(membersToMerge.Select(mm => Combine(mm.M1, mm.M2)))
                                       .ToList();
      return Create(spec1.Type, resultMembers);
    }

    public static IncludeSpecification<T> Combine<T>(IncludeSpecification<T> spec1, IncludeSpecification<T> spec2)
    {
      return (IncludeSpecification<T>) Combine((IncludeSpecification)spec1, spec2);
    }

    #endregion
  }

  public class IncludeSpecification<T> : IncludeSpecification
  {
    public IncludeSpecification(IEnumerable<IncludeMember> members)
      : base(typeof (T), members)
    {
    }
  }

  public static class IncludeSpecificationExtenstions
  {
    public static IncludeSpecification<T> CombineWith<T>(this IncludeSpecification<T> lhs, IncludeSpecification<T> rhs)
    {
      return IncludeSpecification.Combine(lhs, rhs);
    }
  }
}