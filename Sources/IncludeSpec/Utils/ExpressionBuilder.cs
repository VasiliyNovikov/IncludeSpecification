using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace IncludeSpec.Utils
{
  static class ExpressionBuilder
  {
    public static LambdaExpression KeyListContains(Type entityType, IList<PropertyInfo> keyPropertyList, IEnumerable<object[]> keyValuesList)
    {
      var parameter = Expression.Parameter(entityType);
      var properties = keyPropertyList.Select(pk => Expression.Property(parameter, pk)).ToList();
      Expression body = null;
      foreach (var keyValues in keyValuesList)
      {
        Expression entityCheck = null;
        for (var i = 0; i < keyValues.Length; i++)
        {
          var keyValue = keyValues[i];
          var property = properties[i];
          var valueEq = Expression.Equal(property, Expression.Constant(keyValue, property.Type));
          entityCheck = entityCheck == null ? valueEq : Expression.AndAlso(entityCheck, valueEq);
        }

        if (entityCheck == null)
        {
          throw new ArgumentException();
        }

        body = body == null ? entityCheck : Expression.OrElse(body, entityCheck);
      }

      if (body == null)
      {
        throw new ArgumentException();
      }

      return Expression.Lambda(body, parameter);
    }

    public static Expression<Func<TEntity, bool>> KeyListContains<TEntity>(IList<PropertyInfo> keyPropertyList, IEnumerable<object[]> keyValuesList)
    {
      return (Expression<Func<TEntity, bool>>)KeyListContains(typeof(TEntity), keyPropertyList, keyValuesList);
    }
  }
}
