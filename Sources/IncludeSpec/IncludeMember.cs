using System;
using System.Linq.Expressions;
using System.Reflection;

namespace IncludeSpec
{
  public abstract class IncludeMember
  {
    protected IncludeMember(MemberInfo member, IncludeSpecification specification, bool loadSeparately, int? desiredBatchSize = null, Delegate includeCondition = null)
    {
      IncludeCondition = includeCondition;
      if (member == null)
      {
        throw new ArgumentNullException("member");
      }

      Member = member;
      Specification = specification;
      LoadSeparately = loadSeparately;
      DesiredBatchSize = desiredBatchSize;
    }

    public MemberInfo Member { get; private set; }

    public IncludeSpecification Specification { get; private set; }

    public bool LoadSeparately { get; private set; }

    public int? DesiredBatchSize { get; private set; }

    public Delegate IncludeCondition { get; private set; }

    public LambdaExpression MemberPredicate { get; protected set; }
  }
}