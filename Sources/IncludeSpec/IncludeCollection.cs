﻿using System.Reflection;

namespace IncludeSpec
{
  public class IncludeCollection : IncludeMember
  {
    public IncludeCollection(MemberInfo member, IncludeSpecification itemSpecification = null, bool loadSeparately = false, int? desiredBatchSize = null)
      : base(member, itemSpecification, loadSeparately, desiredBatchSize)
    {
    }
  }
}