using System;
using System.Linq.Expressions;
using System.Reflection;

namespace IncludeSpec
{
  public class IncludeCollection : IncludeMember
  {
    public IncludeCollection(MemberInfo member, IncludeSpecification itemSpecification = null, bool loadSeparately = false, int? desiredBatchSize = null, Delegate includeCondition = null)
      : base(member, itemSpecification, loadSeparately, desiredBatchSize, includeCondition)
    {
    }

    public IncludeCollection(MemberInfo member, LambdaExpression memberPredicate, IncludeSpecification itemSpecification = null, bool loadSeparately = false, int? desiredBatchSize = null, Delegate includeCondition = null) 
      : base(member, itemSpecification, loadSeparately, desiredBatchSize, includeCondition)
    {
      MemberPredicate = memberPredicate;
    }
  }
}