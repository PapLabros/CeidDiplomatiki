using Atom.Core;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// A <see cref="BasePresenterDataStorage{TArgs}"/> for a data grid presenter
    /// </summary>
    public class DataGridPresenterDataStorage : BasePresenterDataStorage<DataGridPresenterArgs>
    {
        #region Public Properties

        /// <summary>
        /// The presenter map
        /// </summary>
        public new DataGridPresenterMap PresenterMap => (DataGridPresenterMap)base.PresenterMap;

        /// <summary>
        /// The map of the data grid that presents the root type models
        /// </summary>
        public DataGridMap DataGridMap { get; }

        /// <summary>
        /// The column that's the primary key of the root type
        /// </summary>
        public IDbProviderColumn PrimaryKeyColumn { get; }

        /// <summary>
        /// The property of the root type that represents the primary key
        /// </summary>
        public PropertyInfo PrimaryKeyProperty { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="presenterMap">The data grid presenter map</param>
        public DataGridPresenterDataStorage(DataGridPresenterMap presenterMap) : base(presenterMap)
        {
            DataGridMap = PresenterMap.DataGrids.First(x => x.Type == QueryMap.RootType);
            PrimaryKeyColumn = QueryMap.Columns.First(x => x.IsPrimaryKey && x.TableName == QueryMap.GetTableName(QueryMap.RootType));
            PrimaryKeyProperty = QueryMap.RootType.GetProperty(PrimaryKeyColumn.ColumnName);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the data using the specified <paramref name="args"/>.
        /// NOTE: Only root types are returned by this method!
        /// </summary>
        /// <param name="args">The args</param>
        /// <returns></returns>
        public override async Task<IFailable<IEnumerable>> GetDataAsync(DataGridPresenterArgs args)
        {
            // Create the result
            var result = new Failable<IEnumerable>();

            try
            {
                var queryable = GetDataQuery;

                // Get the data model type
                var dataModelType = PresenterMap.QueryMap.RootType;

                // If there is a date column...
                if (DataGridMap.DateColumn != null)
                {
                    // If there is an after date...
                    if (args.After != null)
                        // Add a where condition
                        queryable = CeidDiplomatikiHelpers.AddWhereCondition(queryable, dataModelType, ExpressionHelpers.CreatePropertyEqualityExpression(dataModelType, DataGridMap.DateColumn, args.After.Value.DateTime, EqualityOperator.GreaterOrEqualThan));
                    // If there is a before date...
                    if (args.Before != null)
                        // Add a where condition
                        queryable = CeidDiplomatikiHelpers.AddWhereCondition(queryable, dataModelType, ExpressionHelpers.CreatePropertyEqualityExpression(dataModelType, DataGridMap.DateColumn, args.Before.Value.DateTime, EqualityOperator.LessOrEqualThan));
                }

                // If there is a search value...
                if (!args.Search.IsNullOrEmpty())
                {
                    // If there are search column...
                    if (!DataGridMap.SearchColumns.IsNullOrEmpty())
                    {
                        var parameterExp = Expression.Parameter(dataModelType, "x");
                        var methodCallExpressions = new List<MethodCallExpression>();

                        // For every search column...
                        foreach (var searchColumn in DataGridMap.SearchColumns)
                        {
                            var expr = CreateContainsExpression(parameterExp, dataModelType, searchColumn, args.Search);

                            methodCallExpressions.Add(expr);
                        }

                        Expression orExpression = null;
                        foreach (var expr in methodCallExpressions)
                        {
                            if (orExpression == null)
                            {
                                orExpression = expr;

                                continue;
                            }

                            orExpression = Expression.OrElse(orExpression, expr);
                        }

                        var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(dataModelType, typeof(bool)), orExpression, parameterExp);

                        queryable = CeidDiplomatikiHelpers.AddWhereCondition(queryable, dataModelType, lambda);
                    }
                }

                // Add the order by condition
                queryable = CeidDiplomatikiHelpers.AddOrderByDescendinCondition(queryable, dataModelType, PrimaryKeyProperty);

                // Add the skip condition
                queryable = CeidDiplomatikiHelpers.AddSkipCondition(queryable, dataModelType, args.Page * args.PerPage);

                // Add the take condition
                queryable = CeidDiplomatikiHelpers.AddTakeCondition(queryable, dataModelType, args.PerPage);

                // Get the models
                var models = await CeidDiplomatikiHelpers.ExecuteToListAsync(queryable, dataModelType);

                // Set the data
                result.Result = models;
            }
            catch(Exception ex)
            {
                // If there was an error...
                result.ErrorMessage = ex.Message;
            }

            // Return the result
            return result;
        }

        private MethodCallExpression CreateContainsExpression(ParameterExpression parameterExp, Type type, PropertyInfo propertyInfo, string value)
        {
            
            var propertyExp = Expression.Property(parameterExp, propertyInfo.Name);
            var method = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
            var someValue = Expression.Constant(value, typeof(string));
            return Expression.Call(propertyExp, method, someValue);
        }

        #endregion
    }
}
