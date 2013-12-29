using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IncludeSpec.Integration
{
  public interface IIntegrationSource
  {
    IEnumerable<PropertyInfo> GetPrimaryKey(Type entityType);
    IEnumerable<PropertyInfo> GetNavigationKey(PropertyInfo navigationProperty);
    IEnumerable<PropertyInfo> GetOtherNavigationKey(PropertyInfo navigationProperty);

    IQueryable Include(IQueryable query, string path);

    IList ExecuteQuery(IQueryable query);
    Task<IList> ExecuteQueryAsync(IQueryable query);

    IQueryable<T> CreateQuery<T>() where T : class;
  }
}
