using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using IncludeSpec.Integration;

namespace IncludeSpec.Plan
{
  public class IncludePlan
  {
    public IncludePlan(IEnumerable<string> paths, IEnumerable<LoadPlan> separates)
    {
      Paths = paths.ToList().AsReadOnly();
      Separates = separates.ToList().AsReadOnly();
    }

    public ReadOnlyCollection<string> Paths { get; private set; }

    public ReadOnlyCollection<LoadPlan> Separates { get; private set; }

    public bool HasPaths
    {
      get { return Paths.Count != 0; }
    }

    public bool HasSeparates
    {
      get { return Separates.Count != 0; }
    }

    public bool IsEmpty
    {
      get { return !HasPaths && !HasSeparates; }
    }

    public IQueryable AddIncludes(IIntegrationSource source, IQueryable query)
    {
      foreach (var path in Paths)
      {
        query = source.Include(query, path);
      }

      return query;
    }

    private static IncludePlan BuildMemberPlan(IIntegrationSource source, IncludeMember includeMember)
    {
      return includeMember.Specification == null ? null : BuildPlan(source, includeMember.Specification);
    }

    private static void ProcessLocalMembers(IIntegrationSource source, IEnumerable<IncludeMember> localMembers, ICollection<string> paths, ICollection<LoadPlan> separates)
    {
      foreach (var includeMember in localMembers)
      {
        var memberPlan = BuildMemberPlan(source, includeMember);
        if (memberPlan != null)
        {
          if (memberPlan.Paths.Count != 0)
          {
            foreach (var memberPath in memberPlan.Paths)
            {
              var path = includeMember.Member.Name + "." + memberPath;
              paths.Add(path);
            }
          }
          else
          {
            paths.Add(includeMember.Member.Name);
          }

          foreach (var memberSeparate in memberPlan.Separates)
          {
            var referencingEntityPath = new[] { includeMember };
            var separate = new SeparateLoadPlan(memberSeparate, referencingEntityPath);
            separates.Add(separate);
          }
        }
        else
        {
          paths.Add(includeMember.Member.Name);
        }
      }
    }

    private static void ProcessLocalMembersLoadedSeparately(IIntegrationSource source, IncludeSpecification includeSpecification, IEnumerable<IncludeMember> localMembersLoadedSeparately, ICollection<LoadPlan> separates)
    {
      foreach (var includeMember in localMembersLoadedSeparately)
      {
        var memberPlan = BuildMemberPlan(source, includeMember);
        var navigationProperty = (PropertyInfo)includeMember.Member;
        var navigationPropertyType = navigationProperty.PropertyType;
        var isCollection = includeMember is IncludeCollection;

        var memberEntityType = isCollection ? navigationPropertyType.GetGenericArguments()[0] : navigationPropertyType;
        var referenceKey = source.GetNavigationKey(navigationProperty);
        var externalKey = source.GetOtherNavigationKey(navigationProperty);
        var external = new KeyBasedLoadPlan(memberEntityType, includeSpecification.Type, memberPlan, externalKey, referenceKey, includeMember.DesiredBatchSize);
        separates.Add(external);
      }
    }

    public static IncludePlan BuildPlan(IIntegrationSource source, IncludeSpecification includeSpecification)
    {
      var paths = new List<string>();
      var separates = new List<LoadPlan>();

      var members = includeSpecification.Members;
      ProcessLocalMembers(source, members.Where(m => !m.LoadSeparately), paths, separates);
      ProcessLocalMembersLoadedSeparately(source, includeSpecification, members.Where(m => m.LoadSeparately), separates);

      return new IncludePlan(paths, separates);
    }

    public static readonly IncludePlan Empty = new IncludePlan(Enumerable.Empty<string>(), Enumerable.Empty<LoadPlan>());
  }
}