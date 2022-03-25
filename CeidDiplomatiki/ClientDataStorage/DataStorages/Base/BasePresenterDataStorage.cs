using Atom.Core;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The base for all the presenter data storages
    /// </summary>
    /// <typeparam name="TArgs">The type of the arguments used for retrieving the data</typeparam>
    public abstract class BasePresenterDataStorage<TArgs>
    {
        #region Private Members

        /// <summary>
        /// The member of the <see cref="GetDataQuery"/> property
        /// </summary>
        private IQueryable mGetDataQuery;

        #endregion

        #region Public Properties

        /// <summary>
        /// The query map
        /// </summary>
        public QueryMap QueryMap => PresenterMap.QueryMap;

        /// <summary>
        /// The presenter map
        /// </summary>
        public BasePresenterMap PresenterMap { get; }

        /// <summary>
        /// The database context
        /// </summary>
        public PresenterDbContext DbContext { get; }

        /// <summary>
        /// The table
        /// </summary>
        public IDbProviderTable Table { get; }

        /// <summary>
        /// The joins that should be performed on the <see cref="Table"/>
        /// </summary>
        public IEnumerable<JoinMap> Joins { get; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// The get data query
        /// </summary>
        public IQueryable GetDataQuery
        {
            get
            {
                // If the get data query has already been created...
                if (mGetDataQuery != null)
                    // Return it
                    return mGetDataQuery;

                // Get the DbSet property
                var dbSetProperty = DbContext.GetType().GetProperty(Table.TableName);

                // Get the value of the property
                mGetDataQuery = (IQueryable)dbSetProperty.GetValue(DbContext);

                // Get the initial table
                mGetDataQuery = PerformIncludes(mGetDataQuery, Table);

                // Return the query
                return mGetDataQuery;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="presenterMap">The presenter map</param>
        public BasePresenterDataStorage(BasePresenterMap presenterMap) : base()
        {
            PresenterMap = presenterMap ?? throw new ArgumentNullException(nameof(presenterMap));

            DbContext = presenterMap.QueryMap.GetDbContext();
            Table = presenterMap.QueryMap.Joins.Count() == 0 ? presenterMap.QueryMap.Tables.First() : presenterMap.QueryMap.Joins.First(x => x.Index == 0).Table;
            Joins = presenterMap.QueryMap.Joins ?? Enumerable.Empty<JoinMap>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the data using the specified <paramref name="args"/>.
        /// NOTE: Only root types are returned by this method!
        /// </summary>
        /// <param name="args">The args</param>
        /// <returns></returns>
        public abstract Task<IFailable<IEnumerable>> GetDataAsync(TArgs args);

        /// <summary>
        /// Gets the data using the specified <paramref name="args"/>
        /// when the type of the models is known
        /// </summary>
        /// <typeparam name="T">The type of the models</typeparam>
        /// <param name="args">The args</param>
        /// <returns></returns>
        public async Task<IFailable<IEnumerable<T>>> GetDataAsync<T>(TArgs args)
        {
            var result = (await GetDataAsync(args));

            var castedResult = new Failable<IEnumerable<T>>()
            {
                Result = result.Result.SelectBase(x => (T)x).ToList(),
                ErrorMessage = result.ErrorMessage
            };

            return castedResult;
        }

        /// <summary>
        /// Adds the specified <paramref name="model"/>
        /// </summary>
        /// <param name="model">The model</param>
        /// <returns></returns>
        public async Task<IFailable<object>> AddDataAsync(object model)
        {
            // Create the result
            var result = new Failable<object>();

            try
            {
                DbContext.Add(model);

                await DbContext.SaveChangesAsync();

                result.Result = model;
            }
            catch (Exception ex)
            {
                // If there was an error...
                result.ErrorMessage = ex.Message;
            }

            // Return the result
            return result;
        }

        /// <summary>
        /// Adds the specified <paramref name="models"/>
        /// </summary>
        /// <param name="models">The models</param>
        /// <returns></returns>
        public async Task<IFailable<IEnumerable<object>>> AddDataRangeAsync(IEnumerable<object> models)
        {
            // Create the result
            var result = new Failable<IEnumerable<object>>();

            try
            {
                await DbContext.AddRangeAsync(models);

                result.Result = models;
            }
            catch (Exception ex)
            {
                // If there was an error...
                result.ErrorMessage = ex.Message;
            }

            // Return the result
            return result;
        }

        /// <summary>
        /// Updates the specified <paramref name="model"/>
        /// </summary>
        /// <param name="model">The model</param>
        /// <returns></returns>
        public async Task<IFailable<object>> UpdateDataAsync(object model)
        {
            // Create the result
            var result = new Failable<object>();

            try
            {
                // Get the type of the model
                var modelType = model.GetType();

                // Get the name of the table
                var tableName = QueryMap.GetTableName(modelType);

                // Get the primary key column
                var primaryKeyColumn = QueryMap.Columns.First(x => x.TableName == tableName && x.IsPrimaryKey);

                // Get the primary key property
                var primaryKeyProperty = modelType.GetProperty(primaryKeyColumn.ColumnName);

                // Get the existing model
                var existingModel = await DbContext.FindAsync(modelType, primaryKeyProperty.GetValue(model));

                // For every property of the model...
                // TODO: Better implementation by using AutoMapper!
                foreach (var property in modelType.GetProperties())
                {
                    // Update the property value of the existing model
                    property.SetValue(existingModel, property.GetValue(model));
                }

                // Mark the existing model for update
                DbContext.Update(existingModel);

                // Save the changes
                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Set the error
                result.ErrorMessage = ex.Message;
            }

            // Return the result
            return result;
        }

        /// <summary>
        /// Deletes the specified <paramref name="model"/>
        /// </summary>
        /// <param name="model">The model</param>
        /// <returns></returns>
        public async Task<IFailable<object>> DeleteDataAsync(object model)
        {
            // Create the result
            var result = new Failable<object>();

            try
            {
                DbContext.Remove(model);

                await DbContext.SaveChangesAsync();

                result.Result = model;
            }
            catch (Exception ex)
            {
                // If there was an error...
                result.ErrorMessage = ex.Message;
            }

            // Return the result
            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Traverses the join path till it reached the join that's the root of the chained joins
        /// that end with the specified <paramref name="joinMap"/>
        /// </summary>
        /// <param name="joinMap">The join map</param>
        /// <returns></returns>
        private IEnumerable<JoinMap> ComputePath(JoinMap joinMap)
        {
            // Declare the result
            var result = new List<JoinMap>();

            // Declare the current map
            var current = joinMap;

            while(current != null)
            {
                result.Add(current);

                current = Joins.FirstOrDefault(x => x.ReferencedTable == current.Table);
            }

            // Invert the list
            result.Reverse();

            // Return the result
            return result;
        }

        /// <summary>
        /// Performs the required includes on the specified <paramref name="queryable"/>
        /// </summary>
        /// <param name="queryable">The queryable</param>
        /// <param name="initialTable">The initial table</param>
        /// <returns></returns>
        private IQueryable PerformIncludes(IQueryable queryable, IDbProviderTable initialTable)
        {
            // For every join...
            foreach(var join in Joins)
            {
                var joinsPath = ComputePath(join);

                // For every related join...
                foreach(var j in joinsPath)
                {
                    // Get the type of the model
                    var modelType = queryable.GetType().GetGenericArguments()[0];

                    // If it's the first join...
                    if (joinsPath.ElementAt(0) == j)
                    {
                        // Get the navigation property name
                        var navigationPropertyName = CeidDiplomatikiHelpers.GetPluralForm(j.ReferencedTable.TableName);

                        // Get the navigation property
                        var navigationProperty = modelType.GetProperty(navigationPropertyName);

                        // Create the expression
                        var expression = ExpressionHelpers.CreatePropertySelectorExpression(modelType, navigationProperty);

                        // Get the Include<TEntity, TProperty> method
                        var includeMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods().First(x => x.Name == nameof(EntityFrameworkQueryableExtensions.Include) && x.GetParameters()[1].ParameterType != typeof(string));

                        // Set the generic arguments (TEntity and TProperty)
                        includeMethod = includeMethod.MakeGenericMethod(modelType, navigationProperty.PropertyType);

                        // Call the Include method
                        queryable = (IQueryable)includeMethod.Invoke(null, new object[] { queryable, expression });

                        // Continue
                        continue;
                    }

                    queryable = PerformThenInclude(queryable, j);
                }
            }

            // Return the queryable
            return queryable;
        }

        /// <summary>
        /// Performs the required includes on the specified <paramref name="queryable"/>
        /// </summary>
        /// <param name="queryable">The queryable</param>
        /// <param name="joinMap">The join map</param>
        /// <returns></returns>
        private IQueryable PerformThenInclude(IQueryable queryable, JoinMap joinMap)
        {
            // Get the ThenInclude<TEntity, TPreviousProperty, TProperty> method
            var thenIncludeMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods().Where(x => x.Name == nameof(EntityFrameworkQueryableExtensions.ThenInclude)).ElementAt(0);

            // Get the type of the model
            var modelType = queryable.GetType().GetGenericArguments()[0];

            // Get the previous property type
            var previousPropertyType = TypeHelpers.GetEnumerableType(queryable.GetType().GetGenericArguments()[1]);

            // Get the navigation property name
            var navigationPropertyName = CeidDiplomatikiHelpers.GetPluralForm(joinMap.ReferencedTable.TableName);

            // Get the navigation property
            var navigationProperty = previousPropertyType.GetProperty(navigationPropertyName);

            // Create the expression
            var expression = ExpressionHelpers.CreatePropertySelectorExpression(previousPropertyType, navigationProperty);

            // Set the generic arguments (TEntity, TPreviousProperty, TProperty)
            thenIncludeMethod = thenIncludeMethod.MakeGenericMethod(modelType, previousPropertyType, navigationProperty.PropertyType);

            // Call the ThenInclude method
            queryable = (IQueryable)thenIncludeMethod.Invoke(null, new object[] { queryable, expression });

            // Return the queryable
            return queryable;
        }

        #endregion
    }

    public class DashboardPresenterDataStorage : BasePresenterDataStorage<DashboardPresenterArgs>
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="presenterMap">The presenter map</param>
        public DashboardPresenterDataStorage(BasePresenterMap presenterMap) : base(presenterMap)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the data using the specified <paramref name="args"/>.
        /// NOTE: Only root types are returned by this method!
        /// </summary>
        /// <param name="args">The args</param>
        /// <returns></returns>
        public override Task<IFailable<IEnumerable>> GetDataAsync(DashboardPresenterArgs args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Args used by the <see cref="DashboardPresenterDataStorage"/>
    /// </summary>
    public class DashboardPresenterArgs
    {
        #region Public Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DashboardPresenterArgs() : base()
        {

        }

        #endregion
    }
}
