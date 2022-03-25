using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Maps information related to a calendar presenter
    /// </summary>
    public class CalendarPresenterMap : BasePresenterMap
    {
        #region Private Members

        /// <summary>
        /// The member of the <see cref="SearchColumns"/> property
        /// </summary>
        private IEnumerable<PropertyInfo> mSearchColumns;

        #endregion

        #region Public Properties

        /// <summary>
        /// The formula composed by property shortcodes that
        /// when the shortcodes are replaced with actual values
        /// represents the title
        /// </summary>
        public string TitleFormula { get; set; }

        /// <summary>
        /// The formula composed by property shortcodes that
        /// when the shortcodes are replaced with actual values
        /// represents the description
        /// </summary>
        public string DescriptionFormula { get; set; }

        /// <summary>
        /// The property of the root type that represents the start date column.
        /// NOTE: This property is required!
        /// </summary>
        public PropertyInfo DateStartColumn { get; set; }

        /// <summary>
        /// The property of the root type that represents the end date column.
        /// NOTE: This property is optional
        /// </summary>
        public PropertyInfo DateEndColumn { get; set; }

        /// <summary>
        /// The columns used for searching
        /// </summary>
        public IEnumerable<PropertyInfo> SearchColumns { get => mSearchColumns ?? Enumerable.Empty<PropertyInfo>(); set => mSearchColumns = value; }

        /// <summary>
        /// A flag indicating whether the addition of database records is allowed
        /// </summary>
        public bool AllowAdd { get; set; }

        /// <summary>
        /// A flag indicating whether the modification of database records is allowed or not
        /// </summary>
        public bool AllowEdit { get; set; }

        /// <summary>
        /// A flag indicating whether the deletion of database records is allowed or not
        /// </summary>
        public bool AllowDelete { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="queryMap">The query map</param>
        public CalendarPresenterMap(QueryMap queryMap) : base(queryMap)
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates and returns a <see cref="CalendarPresenterMapDataModel"/> from the current <see cref="CalendarPresenterMap"/>
        /// </summary>
        /// <returns></returns>
        public CalendarPresenterMapDataModel ToDataModel() => new CalendarPresenterMapDataModel()
        {
            Id = Id,
            Name = Name,
            Description = Description,
            Color = Color,
            TitleFormula = TitleFormula,
            DescriptionFormula = DescriptionFormula,
            DateStartColumnName = DateStartColumn.Name,
            DateEndColumnName = DateEndColumn?.Name,
            SearchColumnNames = SearchColumns.Select(x => x.Name).ToArray(),
            AllowAdd = AllowAdd,
            AllowEdit = AllowEdit,
            AllowDelete = AllowDelete
        };

        /// <summary>
        /// Creates and returns a <see cref="CalendarPresenterMap"/> from the specified <paramref name="model"/>
        /// </summary>
        /// <param name="queryMap">The query map</param>
        /// <param name="model">The model</param>
        /// <returns></returns>
        public static CalendarPresenterMap FromDataModel(QueryMap queryMap, CalendarPresenterMapDataModel model) => new CalendarPresenterMap(queryMap)
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Color = model.Color,
            TitleFormula = model.TitleFormula,
            DescriptionFormula = model.DescriptionFormula,
            DateStartColumn = queryMap.RootType.GetProperty(model.DateStartColumnName),
            DateEndColumn = model.DateEndColumnName == null ? null : queryMap.RootType.GetProperty(model.DateEndColumnName),
            SearchColumns = queryMap.RootType.GetProperties().Where(x => model.SearchColumnNames.Contains(x.Name)).ToList(),
            AllowAdd = model.AllowAdd,
            AllowEdit = model.AllowEdit,
            AllowDelete = model.AllowDelete
        };

        #endregion
    }

    /// <summary>
    /// Represents a <see cref="CalendarPresenterMap"/>
    /// </summary>
    public class CalendarPresenterMapDataModel : BasePresenterMapDataModel
    {
        #region Public Properties

        /// <summary>
        /// The formula composed by property shortcodes that
        /// when the shortcodes are replaced with actual values
        /// represents the title
        /// </summary>
        public string TitleFormula { get; set; }

        /// <summary>
        /// The formula composed by property shortcodes that
        /// when the shortcodes are replaced with actual values
        /// represents the description
        /// </summary>
        public string DescriptionFormula { get; set; }

        /// <summary>
        /// The name of the property that represents the start date column
        /// </summary>
        public string DateStartColumnName { get; set; }

        /// <summary>
        /// The name of the property that represents the end date column 
        /// </summary>
        public string DateEndColumnName { get; set; }

        /// <summary>
        /// The names of the columns used for searching
        /// </summary>
        public string[] SearchColumnNames { get; set; }

        /// <summary>
        /// A flag indicating whether the addition of database records is allowed
        /// </summary>
        public bool AllowAdd { get; set; }

        /// <summary>
        /// A flag indicating whether the modification of database records is allowed or not
        /// </summary>
        public bool AllowEdit { get; set; }

        /// <summary>
        /// A flag indicating whether the deletion of database records is allowed or not
        /// </summary>
        public bool AllowDelete { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public CalendarPresenterMapDataModel() : base()
        {

        }

        #endregion
    }
}
