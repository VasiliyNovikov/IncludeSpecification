using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IncludeSpec.Integration
{
  public interface IIntegrationSource
  {
    IEnumerable<PropertyInfo> GetPrimaryKey(Type entityType);
    IEnumerable<PropertyInfo> GetNavigationKey(PropertyInfo navigationProperty);
    IEnumerable<PropertyInfo> GetOtherNavigationKey(PropertyInfo navigationProperty);

    IQueryable<T> Include<T>(IQueryable<T> query, string path);

    IList<T> ExecuteQuery<T>(IQueryable<T> query);
    Task<IList<T>> ExecuteQueryAsync<T>(IQueryable<T> query);

    IQueryable<T> CreateQuery<T>() where T : class;
  }
}
