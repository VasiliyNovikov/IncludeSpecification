using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using IncludeSpec.Utils;

namespace IncludeSpec
{
  public class IncludeSpecificationBuilder<T>
  {
    private readonly List<IncludeMember> _members = new List<IncludeMember>();

    public IncludeSpecificationBuilder<T> Include<TProperty>(Expression<Func<T, TProperty>> property, bool loadSeparately, Action<IncludeSpecificationBuilder<TProperty>> propertyBuildCallback, int? desiredBatchSize = null)
    {
      IncludeSpecification<TProperty> propertySpecification = null;
      if (propertyBuildCallback != null)
      {
        var propertyBuilder = new IncludeSpecificationBuilder<TProperty>();
        propertyBuildCallback(propertyBuilder);
        propertySpecification = propertyBuilder;
      }

      return Include(property, loadSeparately, propertySpecification, desiredBatchSize);
    }

    public IncludeSpecificationBuilder<T> Include<TProperty>(Expression<Func<T, TProperty>> member, Action<IncludeSpecificationBuilder<TProperty>> propertyBuildCallback, int? desiredBatchSize = null)
    {
      return Include(member, false, propertyBuildCallback, desiredBatchSize);
    }

    public IncludeSpecificationBuilder<T> Include<TProperty>(Expression<Func<T, TProperty>> member, bool loadSeparately = false, IncludeSpecification<TProperty> propertySpecification = null, int? desiredBatchSize = null)
    {
      _members.Add(new IncludeProperty(member.GetNavigationMember(), propertySpecification, loadSeparately, desiredBatchSize));
      return this;
    }

    public IncludeSpecificationBuilder<T> Include<TProperty>(Expression<Func<T, TProperty>> property, IncludeSpecification<TProperty> propertySpecification, int? desiredBatchSize = null)
    {
      return Include(property, false, propertySpecification, desiredBatchSize);
    }

    public IncludeSpecificationBuilder<T> IncludeIf<TProperty>(Expression<Func<T, TProperty>> member, Func<T, bool> includeCondition, bool loadSeparately = false, IncludeSpecification<TProperty> propertySpecification = null, int? desiredBatchSize = null)
    {
      _members.Add(new IncludeProperty(member.GetNavigationMember(), propertySpecification, loadSeparately, desiredBatchSize));
      return this;
    }

    public IncludeSpecificationBuilder<T> IncludeIf<TProperty>(Expression<Func<T, TProperty>> property, Func<T, bool> includeCondition, bool loadSeparately, Action<IncludeSpecificationBuilder<TProperty>> propertyBuildCallback, int? desiredBatchSize = null)
    {
      IncludeSpecification<TProperty> propertySpecification = null;
      if (propertyBuildCallback != null)
      {
        var propertyBuilder = new IncludeSpecificationBuilder<TProperty>();
        propertyBuildCallback(propertyBuilder);
        propertySpecification = propertyBuilder;
      }

      return IncludeIf(property, includeCondition, loadSeparately, propertySpecification, desiredBatchSize);
    }

    public IncludeSpecificationBuilder<T> IncludeIf<TProperty>(Expression<Func<T, TProperty>> member, Func<T, bool> includeCondition, Action<IncludeSpecificationBuilder<TProperty>> propertyBuildCallback, int? desiredBatchSize = null)
    {
      return IncludeIf(member, includeCondition, false, propertyBuildCallback, desiredBatchSize);
    }

    public IncludeSpecificationBuilder<T> IncludeIf<TProperty>(Expression<Func<T, TProperty>> property, Func<T, bool> includeCondition, IncludeSpecification<TProperty> propertySpecification, int? desiredBatchSize = null)
    {
      return IncludeIf(property, includeCondition, false, propertySpecification, desiredBatchSize);
    }

    public IncludeSpecificationBuilder<T> IncludeCollection<TEntity>(Expression<Func<T, IEnumerable<TEntity>>> property, bool loadSeparately, Action<IncludeSpecificationBuilder<TEntity>> itemBuildCallback, int? desiredBatchSize = null)
    {
      IncludeSpecification<TEntity> itemSpecification = null;
      if (itemBuildCallback != null)
      {
        var itemBuilder = new IncludeSpecificationBuilder<TEntity>();
        itemBuildCallback(itemBuilder);
        itemSpecification = itemBuilder;
      }

      return IncludeCollection(property, loadSeparately, itemSpecification, desiredBatchSize);
    }

    public IncludeSpecificationBuilder<T> IncludeCollection<TEntity>(Expression<Func<T, IEnumerable<TEntity>>> property, Action<IncludeSpecificationBuilder<TEntity>> itemBuildCallback, int? desiredBatchSize = null)
    {
      return IncludeCollection(property, false, itemBuildCallback, desiredBatchSize);
    }

    public IncludeSpecificationBuilder<T> IncludeCollection<TEntity>(Expression<Func<T, IEnumerable<TEntity>>> property, IncludeSpecification<TEntity> itemSpecification, int? desiredBatchSize = null)
    {
      return IncludeCollection(property, false, itemSpecification, desiredBatchSize);
    }

    public IncludeSpecificationBuilder<T> IncludeCollection<TEntity>(Expression<Func<T, IEnumerable<TEntity>>> property, bool loadSeparately = false, IncludeSpecification<TEntity> itemSpecification = null, int? desiredBatchSize = null)
    {
      _members.Add(new IncludeCollection(property.GetNavigationMember(), itemSpecification, loadSeparately, desiredBatchSize));
      return this;
    }

    public IncludeSpecificationBuilder<T> IncludeCollectionIf<TEntity>(Expression<Func<T, IEnumerable<TEntity>>> property, Func<T, bool> includeCondition, bool loadSeparately = false, IncludeSpecification<TEntity> itemSpecification = null, int? desiredBatchSize = null)
    {
      _members.Add(new IncludeCollection(property.GetNavigationMember(), itemSpecification, loadSeparately, desiredBatchSize));
      return this;
    }

    public IncludeSpecificationBuilder<T> IncludeCollectionIf<TEntity>(Expression<Func<T, IEnumerable<TEntity>>> property, Func<T, bool> includeCondition, bool loadSeparately, Action<IncludeSpecificationBuilder<TEntity>> itemBuildCallback, int? desiredBatchSize = null)
    {
      IncludeSpecification<TEntity> itemSpecification = null;
      if (itemBuildCallback != null)
      {
        var itemBuilder = new IncludeSpecificationBuilder<TEntity>();
        itemBuildCallback(itemBuilder);
        itemSpecification = itemBuilder;
      }

      return IncludeCollectionIf(property, includeCondition, loadSeparately, itemSpecification, desiredBatchSize);
    }

    public IncludeSpecificationBuilder<T> IncludeCollectionIf<TEntity>(Expression<Func<T, IEnumerable<TEntity>>> property, Func<T, bool> includeCondition, Action<IncludeSpecificationBuilder<TEntity>> itemBuildCallback, int? desiredBatchSize = null)
    {
      return IncludeCollectionIf(property, includeCondition, false, itemBuildCallback, desiredBatchSize);
    }

    public IncludeSpecificationBuilder<T> IncludeCollectionIf<TEntity>(Expression<Func<T, IEnumerable<TEntity>>> property, Func<T, bool> includeCondition, IncludeSpecification<TEntity> itemSpecification, int? desiredBatchSize = null)
    {
      return IncludeCollectionIf(property, includeCondition, false, itemSpecification, desiredBatchSize);
    }

    public static implicit operator IncludeSpecification<T>(IncludeSpecificationBuilder<T> builder)
    {
      return new IncludeSpecification<T>(builder._members);
    }

    public static implicit operator IncludeSpecification(IncludeSpecificationBuilder<T> builder)
    {
      return (IncludeSpecification<T>)builder;
    }
  }
}