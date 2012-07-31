using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KeyHub
{
    public static class SelectListExtensions
    {
        public static SelectList ToSelectList<TEntity, TValueType, TDisplayType>(this IQueryable<TEntity> query,
            Expression<Func<TEntity, TValueType>> valueNavigator,
            Expression<Func<TEntity, TDisplayType>> displayNavigator)
        {
            string displayMember = displayNavigator.GetPropertyInfo().Name;
            string valueMember = valueNavigator.GetPropertyInfo().Name;

            return new SelectList(query.ToList(), valueMember, displayMember);
        }

        public static PropertyInfo GetPropertyInfo<TEntity, TValueType>(this Expression<Func<TEntity, TValueType>> expression)
        {
            MemberExpression member = expression.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    expression.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    expression.ToString()));

            return propInfo;
        }
    }
}
