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
    IEnumerable<PropertyInfo> GetInverseNavigationKey(PropertyInfo navigationProperty);

    List<T> ExecuteQuery<T>(IQueryable<T> query);
    Task<List<T>> ExecuteQueryAsync<T>(IQueryable<T> query);


  }
}
