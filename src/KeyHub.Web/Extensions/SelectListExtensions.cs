using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace KeyHub.Web
{
    /// <summary>
    /// Extension methods to convert a query to a SelectionList
    /// </summary>
    public static class SelectListExtensions
    {
        /// <summary>
        /// Convert a query to SelectionList. 
        /// Value and name of SelectionList item is determined by a value and display expression.
        /// </summary>
        /// <typeparam name="TEntity">Type of the object queried (typed or untyped)</typeparam>
        /// <typeparam name="TValueType">Type of the value expression return type</typeparam>
        /// <typeparam name="TDisplayType">Type of the display expression return type</typeparam>
        /// <param name="query">Query statement</param>
        /// <param name="valueNavigator">Expression to the value member</param>
        /// <param name="displayNavigator">Expression to the display member</param>
        /// <returns>Selectionlist with query results in name, value pairs</returns>
        public static SelectList ToSelectList<TEntity, TValueType, TDisplayType>(this IQueryable<TEntity> query,
            Expression<Func<TEntity, TValueType>> valueNavigator,
            Expression<Func<TEntity, TDisplayType>> displayNavigator)
        {
            return ToSelectList(query.ToList(), valueNavigator, displayNavigator);
        }

        /// <summary>
        /// Convert a list to SelectionList. 
        /// Value and name of SelectionList item is determined by a value and display expression.
        /// </summary>
        /// <typeparam name="TEntity">Type of the object in list (typed or untyped)</typeparam>
        /// <typeparam name="TValueType">Type of the value expression return type</typeparam>
        /// <typeparam name="TDisplayType">Type of the display expression return type</typeparam>
        /// <param name="list">List of objects</param>
        /// <param name="valueNavigator">Expression to the value member</param>
        /// <param name="displayNavigator">Expression to the display member</param>
        /// <returns>Selectionlist with list in name, value pairs</returns>
        public static SelectList ToSelectList<TEntity, TValueType, TDisplayType>(this List<TEntity> list,
            Expression<Func<TEntity, TValueType>> valueNavigator,
            Expression<Func<TEntity, TDisplayType>> displayNavigator)
        {
            string valueMember = valueNavigator.GetPropertyInfo().Name;
            string displayMember = displayNavigator.GetPropertyInfo().Name;

            return new SelectList(list, valueMember, displayMember);
        }

        /// <summary>
        /// Convert a list to MultiSelectionList. 
        /// Value and name of MultiSelectionList item is determined by a value and display expression.
        /// </summary>
        /// <typeparam name="TEntity">Type of the object in list (typed or untyped)</typeparam>
        /// <typeparam name="TValueType">Type of the value expression return type</typeparam>
        /// <typeparam name="TDisplayType">Type of the display expression return type</typeparam>
        /// <param name="list">List of objects</param>
        /// <param name="valueNavigator">Expression to the value member</param>
        /// <param name="displayNavigator">Expression to the display member</param>
        /// <returns>MultiSelectionList with list in name, value pairs</returns>
        public static MultiSelectList ToMultiSelectList<TEntity, TValueType, TDisplayType>(this List<TEntity> list,
            Expression<Func<TEntity, TValueType>> valueNavigator,
            Expression<Func<TEntity, TDisplayType>> displayNavigator)
        {
            string valueMember = valueNavigator.GetPropertyInfo().Name;
            string displayMember = displayNavigator.GetPropertyInfo().Name;

            return new MultiSelectList(list, valueMember, displayMember);
        }

        /// <summary>
        /// Helper function to get propertyinfo of a navigator
        /// </summary>
        /// <typeparam name="TEntity">Type of the object queried (typed or untyped)</typeparam>
        /// <typeparam name="TValueType">Return type of the expression to get propertyinfo of</typeparam>
        /// <param name="expression">The expression to get propertyinfo of</param>
        /// <returns>A PropertyInfo of the naviagor</returns>
        /// <exception cref="ArgumentException">Provided expression is a method, not a property</exception>
        /// <exception cref="ArgumentException">Provided expression's member is a field, not a property</exception>
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
