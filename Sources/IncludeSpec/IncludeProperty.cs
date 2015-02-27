using System;
using System.Reflection;

namespace IncludeSpec
{
  public class IncludeProperty : IncludeMember
  {
    public IncludeProperty(MemberInfo member, IncludeSpecification propertySpecification = null, bool loadSeparately = false, int? desiredBatchSize = null)
      : base(member, propertySpecification, loadSeparately, desiredBatchSize)
    {
    }

    public IncludeProperty(MemberInfo member, Lazy<IncludeSpecification> propertySpecification, bool loadSeparately = false, int? desiredBatchSize = null)
      : base(member, propertySpecification, loadSeparately, desiredBatchSize)
    {
    }
  }
}