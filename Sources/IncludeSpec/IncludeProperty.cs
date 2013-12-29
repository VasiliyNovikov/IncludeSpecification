using System.Reflection;

namespace IncludeSpec
{
  public class IncludeProperty : IncludeMember
  {
    public IncludeProperty(MemberInfo member, IncludeSpecification propertySpecification = null, bool loadSeparately = false, int? desiredBatchSize = null)
      : base(member, propertySpecification, loadSeparately, desiredBatchSize)
    {
    }
  }
}