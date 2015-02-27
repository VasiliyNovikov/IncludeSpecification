using System;
using System.Reflection;

namespace IncludeSpec
{
  public abstract class IncludeMember
  {
    private readonly Lazy<IncludeSpecification> _specification;

    protected IncludeMember(MemberInfo member, Lazy<IncludeSpecification> specification, bool loadSeparately, int? desiredBatchSize)
    {
      if (member == null)
      {
        throw new ArgumentNullException("member");
      }

      Member = member;
      _specification = specification;
      LoadSeparately = loadSeparately;
      DesiredBatchSize = desiredBatchSize;
    }

    protected IncludeMember(MemberInfo member, IncludeSpecification specification, bool loadSeparately, int? desiredBatchSize)
      : this(member, new Lazy<IncludeSpecification>(() => specification), loadSeparately, desiredBatchSize)
    {
      
    }

    public MemberInfo Member { get; private set; }

    public IncludeSpecification Specification
    {
      get { return _specification.Value; }
    }

    public bool LoadSeparately { get; private set; }

    public int? DesiredBatchSize { get; private set; }
  }
}