using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IncludeSpec.Integration;

namespace IncludeSpec.EntityFramework.Integration
{
  public class IntegrationSource : IIntegrationSource
  {
    private readonly DbContext _context;
    private readonly MetadataWorkspace _metadata;

    public IntegrationSource(DbContext context)
    {
      _context = context;
      _metadata = (_context as IObjectContextAdapter).ObjectContext.MetadataWorkspace;
    }

    private EntityType GetEdmType(Type entityType)
    {
      var edmClrType = (EntityType)_metadata.GetType(entityType.Name, entityType.Namespace, DataSpace.OSpace);
      return (EntityType)_metadata.GetEdmSpaceType(edmClrType);
    }

    public IEnumerable<PropertyInfo> GetPrimaryKey(Type entityType)
    {
      var edmType = GetEdmType(entityType);
      return edmType.KeyProperties.Select(p => entityType.GetProperty(p.Name));
    }

    public IEnumerable<PropertyInfo> GetNavigationKey(PropertyInfo navigationProperty)
    {
      return GetNavigationKey(navigationProperty, false);
    }

    private IEnumerable<PropertyInfo> GetNavigationKey(PropertyInfo navigationProperty, bool other)
    {
      var entityType = navigationProperty.ReflectedType;
      var edmType = GetEdmType(entityType);
      var edmProp = edmType.NavigationProperties.Single(p => p.Name == navigationProperty.Name);
      var constraint = ((AssociationType) edmProp.RelationshipType).Constraint;
      var isFromRole = constraint.FromRole == edmProp.FromEndMember;
      var navigationProps = (isFromRole && !other || !isFromRole && other)
        ? constraint.FromProperties
        : constraint.ToProperties;
      Type targetEntityType;
      if (other)
      {
        var propertyType = navigationProperty.PropertyType;
        targetEntityType = propertyType.IsGenericType && propertyType.GetGenericArguments().Length == 1 && typeof(IEnumerable<object>).IsAssignableFrom(propertyType.GetGenericTypeDefinition().MakeGenericType(typeof(object)))
          ? propertyType.GetGenericArguments()[0]
          : propertyType;
      }
      else
      {
        targetEntityType = entityType;
      }
      return navigationProps.Select(p => targetEntityType.GetProperty(p.Name));
    }

    public IEnumerable<PropertyInfo> GetOtherNavigationKey(PropertyInfo navigationProperty)
    {
      return GetNavigationKey(navigationProperty, true);
    }

    public IQueryable Include(IQueryable query, string path)
    {
      return query.Include(path);
    }

    public IList ExecuteQuery(IQueryable query)
    {
      return query.OfType<object>().ToList();
    }

    public async Task<IList> ExecuteQueryAsync(IQueryable query)
    {
      return await query.ToListAsync();
    }

    public IQueryable<T> CreateQuery<T>() where T : class
    {
      return _context.Set<T>();
    }
  }
}
