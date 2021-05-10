using Atom.Core;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Helper methods related to CeidDiplomatiki
    /// </summary>
    public static class CeidDiplomatikiHelpers
    {
        #region General Helpers

        /// <summary>
        /// Uses the specified <paramref name="s"/> to generate a unique
        /// property name.
        /// NOTE: The <paramref name="s"/> is usually a database column name!
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns></returns>
        public static string GetUniquePropertyName(string s) => Guid.NewGuid().ToString() + "-" + s;

        /// <summary>
        /// Gets the plural form of the specified <paramref name="s"/>
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns></returns>
        public static string GetPluralForm(string s) => s + "s";

        #endregion

        #region Reflection Emit

        /// <summary>
        /// Gets the assembly name used by the <see cref="DynamicAssemblyBuilder"/>
        /// </summary>
        public static AssemblyName DynamicAssemblyName { get; } = new AssemblyName("DynamicAssembly");

        /// <summary>
        /// Gets the assembly builder that creates in memory assemblies
        /// </summary>
        public static AssemblyBuilder DynamicAssemblyBuilder { get; } = AssemblyBuilder.DefineDynamicAssembly(DynamicAssemblyName, AssemblyBuilderAccess.Run);

        /// <summary>
        /// Gets the module builder provided by the <see cref="DynamicAssemblyBuilder"/>
        /// </summary>
        public static ModuleBuilder ModuleBuilder { get; } = DynamicAssemblyBuilder.DefineDynamicModule("Core");

        /// <summary>
        /// Gets a type builder that has defined a type with the specified <paramref name="typeName"/>.
        /// NOTE: If <paramref name="typeName"/> is set to <see cref="null"/> then the name of the type is
        ///       set to a new <see cref="Guid"/>!
        /// </summary>
        /// <param name="typeName">The name of the type</param>
        /// <returns></returns>
        public static TypeBuilder GetTypeBuilder(string typeName = null) => ModuleBuilder.DefineType(typeName.IsNullOrEmpty() ? Guid.NewGuid().ToString() : typeName);

        #endregion

        #region Entity Framework

        /// <summary>
        /// Uses the <see cref="Queryable.Skip{TSource}(IQueryable{TSource}, int)"/> method to apply a skip condition
        /// to the specified <paramref name="queryable"/> with the specified <paramref name="count"/>
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="queryableType"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IQueryable AddSkipCondition(IQueryable queryable, Type queryableType, int count)
        {
            // Get the Take method
            var method = typeof(Queryable).GetMethod(nameof(Queryable.Skip)).MakeGenericMethod(queryableType);

            // Call the method
            queryable = (IQueryable)method.Invoke(null, new object[] { queryable, count });

            // Return the queryable
            return queryable;
        }

        /// <summary>
        /// Uses the <see cref="Queryable.Take{TSource}(IQueryable{TSource}, int)"/> method to apply a skip condition
        /// to the specified <paramref name="queryable"/> with the specified <paramref name="count"/>
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="queryableType"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IQueryable AddTakeCondition(IQueryable queryable, Type queryableType, int count)
        {
            // Get the Skip method
            var method = typeof(Queryable).GetMethod(nameof(Queryable.Take)).MakeGenericMethod(queryableType);

            // Call the method
            queryable = (IQueryable)method.Invoke(null, new object[] { queryable, count });

            // Return the queryable
            return queryable;
        }

        /// <summary>
        /// Uses the <see cref="Queryable.OrderByDescending{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/> method
        /// to apply an order by condition to the specified <paramref name="queryable"/>
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="queryableType"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static IQueryable AddOrderByDescendinCondition(IQueryable queryable, Type queryableType, PropertyInfo propertyInfo)
        {
            // Get the Take method
            var method = typeof(Queryable).GetMethods().First(x => x.Name == nameof(Queryable.OrderByDescending) && x.GetParameters().Count() == 2).MakeGenericMethod(queryableType, propertyInfo.PropertyType);

            // Create the expression
            var expression = ExpressionHelpers.CreatePropertySelectorExpression(queryableType, propertyInfo);

            // Call the method
            queryable = (IQueryable)method.Invoke(null, new object[] { queryable, expression });

            // Return the queryable
            return queryable;
        }

        /// <summary>
        /// Uses the <see cref="Queryable.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource, bool}})"/> method
        /// to apply a where condition to the specified <paramref name="queryable"/>
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="queryableType"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IQueryable AddWhereCondition(IQueryable queryable, Type queryableType, Expression expression)
        {
            // Get the Take method
            var method = typeof(Queryable).GetMethods().First(x => x.Name == nameof(Queryable.Where)).MakeGenericMethod(queryableType);

            // Call the method
            queryable = (IQueryable)method.Invoke(null, new object[] { queryable, expression });

            // Return the queryable
            return queryable;
        }

        /// <summary>
        /// Executes the <see cref="EntityFrameworkQueryableExtensions.ToListAsync{TSource}(IQueryable{TSource}, CancellationToken)"/> method
        /// on the specified <paramref name="queryable"/>
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="queryableType"></param>
        /// <returns></returns>
        public async static Task<IEnumerable> ExecuteToListAsync(IQueryable queryable, Type queryableType)
        {
            // Get the ToListAsync method
            var method = typeof(EntityFrameworkQueryableExtensions).GetMethod(nameof(EntityFrameworkQueryableExtensions.ToListAsync)).MakeGenericMethod(queryableType);

            // Get the task
            var task = (Task)method.Invoke(null, new object[] { queryable, new CancellationToken() });

            // Await the task
            await task;

            // Get the result property from the task
            var resultProperty = task.GetType().GetProperty(nameof(Task<object>.Result));

            // Get the result from the property
            var result = (IEnumerable)resultProperty.GetValue(task);

            // Return the result
            return result;
        }

        #endregion
    }
}
