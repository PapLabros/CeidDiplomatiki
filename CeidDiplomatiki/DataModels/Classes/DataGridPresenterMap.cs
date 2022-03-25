using Atom.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Maps information related to a data grid presenter
    /// </summary>
    public class DataGridPresenterMap : BasePresenterMap, IEquatable<DataGridPresenterMap>
    {
        #region Public Properties

        /// <summary>
        /// The data grids required to present the information related to the <see cref="QueryMap"/>.
        /// NOTE: Usually a data grid is required for every join!
        /// </summary>
        public IEnumerable<DataGridMap> DataGrids { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataGridPresenterMap(QueryMap queryMap) : base(queryMap)
        {
            var dataGridsCollection = new List<DataGridMap>();

            foreach (var type in queryMap.DataModelTypes)
                dataGridsCollection.Add(new DataGridMap(this, type));

            DataGrids = dataGridsCollection;
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Creates and returns a <see cref="DataGridPresenterMapDataModel"/> from the current <see cref="DataGridPresenterMap"/>
        /// </summary>
        /// <returns></returns>
        public DataGridPresenterMapDataModel ToDataModel() => new DataGridPresenterMapDataModel()
        {
            Id = Id,
            QueryMapId = QueryMap.Id,
            Name = Name,
            Description = Description,
            Color = Color,
            DataGrids = DataGrids.Select(x => x.ToDataModel()).ToArray()
        };

        /// <summary>
        /// Creates and returns a <see cref="DataGridPresenterMap"/> from the specified <paramref name="model"/>
        /// </summary>
        /// <param name="queryMap">The query map</param>
        /// <param name="model">The model</param>
        /// <returns></returns>
        public static DataGridPresenterMap FromDataModel(QueryMap queryMap ,DataGridPresenterMapDataModel model)
        {
            // Create the map
            var dataGridPresenterMap = new DataGridPresenterMap(queryMap)
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Color = model.Color,
            };

            foreach (var dataGridMapDataModel in model.DataGrids)
            {
                var dataGridMap = dataGridPresenterMap.DataGrids.First(x => x.Type.Name == dataGridMapDataModel.TypeName);

                dataGridMap.AllowAdd = dataGridMapDataModel.AllowAdd;
                dataGridMap.AllowEdit = dataGridMapDataModel.AllowEdit;
                dataGridMap.AllowDelete = dataGridMapDataModel.AllowDelete;
                dataGridMap.Columns = dataGridMapDataModel.ColumnNames.Select(x => dataGridMap.Type.GetProperty(x)).ToList();
                dataGridMap.SearchColumns = dataGridMapDataModel.SearchColumnNames.Select(x => dataGridMap.Type.GetProperty(x)).ToList();
                if (!dataGridMapDataModel.DateColumnName.IsNullOrEmpty())
                    dataGridMap.DateColumn = dataGridMap.Type.GetProperty(dataGridMapDataModel.DateColumnName);
            }

            // Return the map
            return dataGridPresenterMap;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(DataGridPresenterMap other)
        {
            if (other == null)
                return false;

            return Id == other.Id;
        }

        #endregion
    }

    /// <summary>
    /// Maps information related to the data grid that presents models of the specified <see cref="Type"/>
    /// </summary>
    public class DataGridMap
    {
        #region Private Methods

        /// <summary>
        /// The member of the <see cref="Columns"/> property
        /// </summary>
        private IEnumerable<PropertyInfo> mColumns;

        /// <summary>
        /// The member of the <see cref="SearchColumns"/> property
        /// </summary>
        private IEnumerable<PropertyInfo> mSearchColumns;

        #endregion

        #region Public Properties

        /// <summary>
        /// The query map
        /// </summary>
        public QueryMap QueryMap => PresenterMap.QueryMap;

        /// <summary>
        /// The presenter map that contains this data grid map
        /// </summary>
        public DataGridPresenterMap PresenterMap { get; }

        /// <summary>
        /// The type of the models that the grid presents
        /// </summary>
        public Type Type { get; }

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

        /// <summary>
        /// The properties that represent the columns that the grid should present
        /// </summary>
        public IEnumerable<PropertyInfo> Columns { get => mColumns ?? Enumerable.Empty<PropertyInfo>(); set => mColumns = value; }

        /// <summary>
        /// The properties that represent the search columns if any.
        /// NOTE: The columns used for searching the database!
        /// </summary>
        public IEnumerable<PropertyInfo> SearchColumns { get => mSearchColumns ?? Enumerable.Empty<PropertyInfo>(); set => mSearchColumns = value; }

        /// <summary>
        /// The property that represents the date column if any.
        /// NOTE: The date column is used for day span based search!
        /// </summary>
        public PropertyInfo DateColumn { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="presenterMap">The presenter map that contains this data grid map</param>
        /// <param name="type">The type of the models that the grid presents</param>
        public DataGridMap(DataGridPresenterMap presenterMap, Type type) : base()
        {
            PresenterMap = presenterMap ?? throw new ArgumentNullException(nameof(presenterMap));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates and instance of a model of the same type as the type of models
        /// that the grid presents
        /// </summary>
        /// <returns></returns>
        public object CreateModelInstance()
        {
            // Create the instance
            var instance = Activator.CreateInstance(Type);

            // For every property...
            foreach(var property in Type.GetProperties())
            {
                // If it's a DateTime...
                if (property.PropertyType == typeof(DateTime))
                    // Set the default value to now
                    property.SetValue(instance, DateTime.Now);
                // If it's a DateTimeOffset...
                else if (property.PropertyType == typeof(DateTimeOffset))
                    // Set the default value to now
                    property.SetValue(instance, DateTimeOffset.Now);
            }

            // Return the instance
            return instance;
        }

        /// <summary>
        /// Creates and returns a property mapper
        /// </summary>
        /// <returns></returns>
        public IPropertyMapper CreatePropertyMapper()
        {
            // Create the mapper
            var mapper = (IPropertyMapper)Activator.CreateInstance(typeof(PropertyMapper<>).MakeGenericType(Type));

            // For every property map...
            foreach(var propertyMap in QueryMap.PropertyMaps)
            {
                // Set the custom name
                mapper.UnsafeMap(PropertyMapperExtensions.Title, propertyMap.PropertyInfo, propertyMap.Name);
            }

            // Return the mapper
            return mapper;
        }

        /// <summary>
        /// Creates and returns a <see cref="DataGridMapDataModel"/> from the current <see cref="DataGridMap"/>
        /// </summary>
        /// <returns></returns>
        public DataGridMapDataModel ToDataModel()
        {
            return new DataGridMapDataModel()
            {
                TypeName = Type.Name,
                ColumnNames = Columns.Select(x => x.Name).ToArray(),
                SearchColumnNames = SearchColumns.Select(x => x.Name).ToArray(),
                DateColumnName = DateColumn?.Name,
                AllowAdd = AllowAdd,
                AllowEdit = AllowEdit,
                AllowDelete = AllowDelete
            };
        }

        #endregion
    }

    /// <summary>
    /// Represents a <see cref="DataGridMap"/>
    /// </summary>
    public class DataGridMapDataModel
    {
        #region Public Properties

        /// <summary>
        /// The name of the type of the models that the grid presents
        /// </summary>
        public string TypeName { get; set; }

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

        /// <summary>
        /// The name of the properties that represent the columns that the grid should present
        /// </summary>
        public string[] ColumnNames{ get; set; }

        /// <summary>
        /// The name of the properties that represent the search columns if any.
        /// NOTE: The columns used for searching the database!
        /// </summary>
        public string[] SearchColumnNames { get; set; }

        /// <summary>
        /// The name of the property that represents the date column if any.
        /// NOTE: The date column is used for day span based search!
        /// </summary>
        public string DateColumnName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataGridMapDataModel() : base()
        {

        }

        #endregion
    }

    /// <summary>
    /// The base for all the presenter map data models
    /// </summary>
    public class BasePresenterMapDataModel
    {
        #region Public Properties

        /// <summary>
        /// The id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The id of the query map
        /// </summary>
        public string QueryMapId { get; set; }

        /// <summary>
        /// The name of the presenter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the presenter
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The color of the presenter
        /// </summary>
        public string Color { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public BasePresenterMapDataModel() : base()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;

        #endregion
    }

    /// <summary>
    /// Represents a <see cref="DataGridPresenterMap"/>
    /// </summary>
    public class DataGridPresenterMapDataModel : BasePresenterMapDataModel
    {
        #region Public Properties

        /// <summary>
        /// The data grids
        /// </summary>
        public DataGridMapDataModel[] DataGrids { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataGridPresenterMapDataModel() : base()
        {

        }

        #endregion
    }

    /// <summary>
    /// Represents a <see cref="IDbProviderColumn"/>
    /// </summary>
    public class DbProviderColumnDataModel
    {
        #region Public Properties

        /// <summary>
        /// The name of the table
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// The name of the column
        /// </summary>
        public string ColumnName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DbProviderColumnDataModel() : base()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates and returns a <see cref="DbProviderColumnDataModel"/> from the specified <paramref name="column"/>
        /// </summary>
        /// <param name="column">the column</param>
        /// <returns></returns>
        public static DbProviderColumnDataModel FromDbProviderColumn(IDbProviderColumn column) => new DbProviderColumnDataModel()
        {
            TableName = column.TableName,
            ColumnName = column.ColumnName
        };

        #endregion
    }
}
