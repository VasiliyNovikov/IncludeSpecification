using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace IncludeSpec.Utils
{
  public static class NavigationMemberExtensions
  {
    public static MemberInfo GetNavigationMember<TObj, T>(this Expression<Func<TObj, T>> navigationMemberSelector)
    {
      return GetNavigationMember<Expression<Func<TObj, T>>>(navigationMemberSelector);
    }

    public static MemberInfo GetNavigationMember<T>(this Expression<Func<T>> navigationMemberSelector)
    {
      return GetNavigationMember<Expression<Func<T>>>(navigationMemberSelector);
    }

    public static MemberInfo GetNavigationMember<TExpression>(this TExpression navigationMemberSelector)
      where TExpression : LambdaExpression
    {
      var body = navigationMemberSelector.Body;
      var memberExpression = body as MemberExpression;
      if (memberExpression != null)
      {
        return memberExpression.Member;
      }
      var methodCallExpression = body as MethodCallExpression;
      if (methodCallExpression != null)
      {
        return methodCallExpression.Method;
      }

      throw new ArgumentException(@"Specified expression is neither MemberExpression nor MethodCallExpression", "navigationMemberSelector");
    }

    public static Type GetNavigatingEntityType(this MemberInfo navigationMember)
    {
      Type memberReturnType;
      var navigationProperty = navigationMember as PropertyInfo;
      if (navigationProperty != null)
      {
        memberReturnType = navigationProperty.PropertyType;
      }
      else
      {
        var navigationMethod = navigationMember as MethodInfo;
        if (navigationMethod != null)
        {
          memberReturnType = navigationMethod.ReturnType;
        }
        else
        {
          throw new ArgumentException(@"Specified member is neither PropertyInfo nor MethodInfo", "navigationMember");
        }
      }

      var enumerableReturnType = new []{ memberReturnType}
                                                 .Concat(memberReturnType.GetInterfaces())
                                                 .SingleOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEnumerable<>));
      if (enumerableReturnType != null)
      {
        return enumerableReturnType.GetGenericArguments()[0];
      }

      return memberReturnType;
    }

    public static Type GetReferencingEntityType(this MemberInfo navigationMember)
    {
      var navigationProperty = navigationMember as PropertyInfo;
      if (navigationProperty != null)
      {
        return navigationProperty.DeclaringType;
      }

      var navigationMethod = navigationMember as MethodInfo;
      if (navigationMethod != null)
      {
        return navigationMethod.GetParameters()[0].ParameterType;
      }

      throw new ArgumentException(@"Specified member is neither PropertyInfo nor MethodInfo", "navigationMember");
    }

    public static T GetMavigationMemberValue<T>(this MemberInfo navigationMember, object @object)
    {
      var navigationProperty = navigationMember as PropertyInfo;
      if (navigationProperty != null)
      {
        return (T)navigationProperty.GetValue(@object, null);
      }

      var navigationMethod = navigationMember as MethodInfo;
      if (navigationMethod != null)
      {
        return (T)navigationMethod.Invoke(null, new[] {@object});
      }

      throw new ArgumentException(@"Specified member is neither PropertyInfo nor MethodInfo", "navigationMember");
    }
  }
}
