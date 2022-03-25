using Atom.Core;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// A <see cref="BasePresenterDataStorage{TArgs}"/> for a calendar presenter
    /// </summary>
    public class CalendarPresenterDataStorage : BasePresenterDataStorage<CalendarPresenterArgs>
    {
        #region Public Properties

        /// <summary>
        /// The presenter map
        /// </summary>
        public new CalendarPresenterMap PresenterMap => (CalendarPresenterMap)base.PresenterMap;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="presenterMap">The calendar presenter map</param>
        public CalendarPresenterDataStorage(CalendarPresenterMap presenterMap) : base(presenterMap)
        {

        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the data using the specified <paramref name="args"/>.
        /// NOTE: Only root types are returned by this method!
        /// </summary>
        /// <param name="args">The args</param>
        /// <returns></returns>
        public override async Task<IFailable<IEnumerable>> GetDataAsync(CalendarPresenterArgs args)
        {
            // Create the result
            var result = new Failable<IEnumerable>();

            try
            {
                var queryable = GetDataQuery;

                // Get the data model type
                var dataModelType = PresenterMap.QueryMap.RootType;

                // Add the where conditions
                queryable = CeidDiplomatikiHelpers.AddWhereCondition(queryable, dataModelType, ExpressionHelpers.CreatePropertyEqualityExpression(dataModelType, PresenterMap.DateStartColumn, args.After.DateTime, EqualityOperator.GreaterOrEqualThan));
                queryable = CeidDiplomatikiHelpers.AddWhereCondition(queryable, dataModelType, ExpressionHelpers.CreatePropertyEqualityExpression(dataModelType, PresenterMap.DateStartColumn, args.Before.DateTime, EqualityOperator.LessOrEqualThan));

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

        #endregion
    }
}
