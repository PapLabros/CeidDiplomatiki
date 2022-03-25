using Atom.Core;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using static Atom.Core.Personalization;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Maps information to a query that may or may not contain multiple <see cref="Tables"/>
    /// </summary>
    public class QueryMap
    {
        #region Private Members

        /// <summary>
        /// The registered query maps
        /// </summary>
        private readonly List<PropertyMap> mPropertyMaps = new List<PropertyMap>();

        /// <summary>
        /// The registered data grid presenter maps
        /// </summary>
        private readonly List<DataGridPresenterMap> mDataGridPresenterMaps = new List<DataGridPresenterMap>();

        /// <summary>
        /// The registered calendar presenter maps
        /// </summary>
        private readonly List<CalendarPresenterMap> mCalendarPresenterMaps = new List<CalendarPresenterMap>();

        /// <summary>
        /// The member of the <see cref="CustomShortcodes"/> property
        /// </summary>
        private IEnumerable<CeidDiplomatikiPropertyShortcodeDataModel> mCustomShortcodes;

        #endregion

        #region Public Properties

        /// <summary>
        /// The database options
        /// </summary>
        public BaseDatabaseOptionsDataModel DatabaseOptions { get; }

        /// <summary>
        /// The database that contains the mapped table
        /// </summary>
        public IDbProviderDatabase Database { get; }

        /// <summary>
        /// The tables.
        /// NOTE: multiple tables may be contained in the case of joins!
        /// </summary>
        public IEnumerable<IDbProviderTable> Tables { get; }

        /// <summary>
        /// The columns of the <see cref="Tables"/>
        /// </summary>
        public IEnumerable<IDbProviderColumn> Columns { get; }

        /// <summary>
        /// The joins
        /// </summary>
        public IEnumerable<JoinMap> Joins { get; }

        /// <summary>
        /// The property maps
        /// </summary>
        public IEnumerable<PropertyMap> PropertyMaps => mPropertyMaps.ToList();

        /// <summary>
        /// The presenter maps
        /// </summary>
        public IEnumerable<BasePresenterMap> PresenterMaps => mDataGridPresenterMaps.Concat<BasePresenterMap>(CalendarPresenterMaps).ToList();

        /// <summary>
        /// The data grid presenter maps
        /// </summary>
        public IEnumerable<DataGridPresenterMap> DataGridPresenterMaps => mDataGridPresenterMaps.ToList();

        /// <summary>
        /// The calendar presenter maps
        /// </summary>
        public IEnumerable<CalendarPresenterMap> CalendarPresenterMaps => mCalendarPresenterMaps.ToList();

        /// <summary>
        /// The property shortcodes related to the root type
        /// </summary>
        public Lazy<IEnumerable<PropertyShortcode>> PropertyShortcodes { get; }

        /// <summary>
        /// The custom shortcodes
        /// </summary>
        public IEnumerable<CeidDiplomatikiPropertyShortcodeDataModel> CustomShortcodes
        {
            get => mCustomShortcodes ?? Enumerable.Empty<CeidDiplomatikiPropertyShortcodeDataModel>();
            set => mCustomShortcodes = value;
        }

        /// <summary>
        /// The id of the query map
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The color
        /// </summary>
        public string Color { get; set; } = White;

        #region Reflection Emit

        /// <summary>
        /// The types that represents the data models
        /// </summary>
        public IEnumerable<Type> DataModelTypes { get; private set; }

        /// <summary>
        /// The type that's represents the root table.
        /// NOTE: The root table is the root of the joins!
        /// </summary>
        public Type RootType { get; private set; }

        /// <summary>
        /// The type of the DbContext used for reading and manipulating the database
        /// </summary>
        public Type DbContextType { get; private set; }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="databaseOptions">The database options</param>
        /// <param name="database">The database that contains the mapped table</param>
        /// <param name="columns">The columns of the tables</param>
        /// <param name="joins">The joins</param>
        /// <param name="tables">The tables</param>
        protected QueryMap(BaseDatabaseOptionsDataModel databaseOptions, IDbProviderDatabase database, IEnumerable<IDbProviderTable> tables, IEnumerable<IDbProviderColumn> columns, IEnumerable<JoinMap> joins) : base()
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            Joins = joins ?? Enumerable.Empty<JoinMap>();
            DatabaseOptions = databaseOptions ?? throw new ArgumentNullException(nameof(databaseOptions));
            Tables = tables.NotNullOrEmpty();
            Columns = columns.NotNullOrEmpty();

            PropertyShortcodes = new Lazy<IEnumerable<PropertyShortcode>>(() =>
            {
                return RootType.GetProperties().Select(x => new PropertyShortcode(null, x, null, x.Name, Blue, "Shortcodes")).ToList();
            });
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the type that represents the data of the database table with the specified <paramref name="tableName"/>
        /// </summary>
        /// <param name="tableName">The database table name</param>
        /// <returns></returns>
        public Type GetTableType(string tableName) 
            => DataModelTypes.First(x => GetTableName(x) == tableName);

        /// <summary>
        /// Gets the name of the table from the data model <paramref name="type"/>
        /// that represents it
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns></returns>
        public string GetTableName(Type type)
            => type.Name.Split("-").Last();

        /// <summary>
        /// Gets the property info that represents the database column with the specified <paramref name="columnName"/>
        /// of the database table with the specified <paramref name="tableName"/>.
        /// </summary>
        /// <param name="tableName">The database table name</param>
        /// <param name="columnName">The database column name</param>
        /// <returns></returns>
        public PropertyInfo GetProperty(string tableName, string columnName)
            => GetTableType(tableName).GetProperty(columnName);

        /// <summary>
        /// Gets the database column that related to the specified <paramref name="propertyInfo"/>.
        /// NOTE: If the specified property is a navigation property then <see cref="null"/> is returned!
        /// </summary>
        /// <param name="propertyInfo">The property info</param>
        /// <returns></returns>
        public IDbProviderColumn GetColumnOrNull(PropertyInfo propertyInfo)
            => Columns.FirstOrDefault(x => x.ColumnName == propertyInfo.Name && x.TableName == propertyInfo.DeclaringType.Name.Split("-").Last());

        /// <summary>
        /// Adds the specified <paramref name="map"/>
        /// </summary>
        /// <param name="map">The map</param>
        public void Add(PropertyMap map) => mPropertyMaps.Add(map);

        /// <summary>
        /// Removes the specified <paramref name="map"/>
        /// </summary>
        /// <param name="map">The map</param>
        public void Remove(PropertyMap map) => mPropertyMaps.Remove(map);

        /// <summary>
        /// Adds the specified <paramref name="map"/>
        /// </summary>
        /// <param name="map">The map</param>
        public void Add(DataGridPresenterMap map) => mDataGridPresenterMaps.Add(map);

        /// <summary>
        /// Removes the specified <paramref name="map"/>
        /// </summary>
        /// <param name="map">The map</param>
        public void Remove(DataGridPresenterMap map) => mDataGridPresenterMaps.Remove(map);

        /// <summary>
        /// Adds the specified <paramref name="map"/>
        /// </summary>
        /// <param name="map">The map</param>
        public void Add(CalendarPresenterMap map) => mCalendarPresenterMaps.Add(map);

        /// <summary>
        /// Removes the specified <paramref name="map"/>
        /// </summary>
        /// <param name="map">The map</param>
        public void Remove(CalendarPresenterMap map) => mCalendarPresenterMaps.Remove(map);

        /// <summary>
        /// Replaces the shortcodes of the specified <paramref name="formula"/> with the values
        /// of the specified root <paramref name="instance"/>
        /// </summary>
        /// <param name="formula">The formula</param>
        /// <param name="instance">The root instance</param>
        /// <returns></returns>
        public string ReplaceFormula(string formula, object instance)
        {
            // Get the shortcodes
            var shortcodes = PropertyShortcodes.Value;

            // Set the instance
            shortcodes.ForEach(x => x.Instance = instance);

            formula = Shortcode.ReplaceShortcodes(formula, shortcodes);

            // Clear the instance
            shortcodes.ForEach(x => x.Instance = null);

            // Return the formula
            return formula;
        }

        /// <summary>
        /// Creates and returns a <see cref="QueryMap"/> from the specified <paramref name="dataModel"/>.
        /// NOTE: If the model is not set, then the default values are used!
        /// </summary>
        /// <param name="databaseOptions">The database options</param>
        /// <param name="database">The database that contains the mapped table</param>
        /// <param name="columns">The columns of the tables</param>
        /// <param name="tables">The tables</param>
        /// <param name="joins">The joins</param>
        /// <param name="dataModel">The data model</param>
        /// <returns></returns>
        public static QueryMap FromDataModel(BaseDatabaseOptionsDataModel databaseOptions, IDbProviderDatabase database, IEnumerable<IDbProviderTable> tables, IEnumerable<IDbProviderColumn> columns, IEnumerable<JoinMap> joins, QueryMapDataModel dataModel = null)
        {
            // Create the map
            var queryMap = new QueryMap(databaseOptions, database, tables, columns, joins);

            // If there is a data model...
            if (dataModel != null)
            {
                queryMap.Id = dataModel.Id;
                queryMap.Color = dataModel.Color;
                queryMap.Description = dataModel.Description;
                queryMap.Name = dataModel.Name;
            }

            var tableToTypeBuilderMapper = new Dictionary<IDbProviderTable, TypeBuilder>();

            // For every table...
            foreach(var table in tables)
            {
                // Create the builder
                var typeBuilder = CeidDiplomatikiHelpers.GetTypeBuilder(queryMap.Id + "-" + table.TableName);

                // For every table column...
                foreach (var column in columns.Where(x => x.TableName == table.TableName))
                {
                    // Create a property // Warning
                    TypeBuilderHelpers.CreateProperty(typeBuilder, column.ColumnName, column.DataType);
                }

                // Map it
                tableToTypeBuilderMapper.Add(table, typeBuilder);
            }

            // For every join...
            foreach(var join in joins)
            {
                // Get the principle model type builder
                var principleModelTypeBuilder = tableToTypeBuilderMapper.First(x => x.Key == join.Table).Value;

                // Get the referenced model type builder
                var referencedModelTypeBuilder = tableToTypeBuilderMapper.First(x => x.Key == join.ReferencedTable).Value;

                // Create the principle navigation property type
                var principleNavigationPropertyType = typeof(IEnumerable<>).MakeGenericType(referencedModelTypeBuilder);

                // Add it to the principle model
                TypeBuilderHelpers.CreateProperty(principleModelTypeBuilder, CeidDiplomatikiHelpers.GetPluralForm(join.ReferencedTable.TableName), principleNavigationPropertyType);

                // Add the foreign navigation property type
                TypeBuilderHelpers.CreateProperty(referencedModelTypeBuilder, join.Table.TableName, principleModelTypeBuilder);
            }

            var tableToTypeMapper = new Dictionary<IDbProviderTable, Type>();

            // For every table to type builder map...
            foreach (var map in tableToTypeBuilderMapper)
            {
                // Build the type
                var type = map.Value.CreateType();

                if (joins.Any(x => x.Index == 0 && x.Table.TableName == map.Key.TableName))
                    queryMap.RootType = type;
                else if (joins.Count() == 0)
                    queryMap.RootType = type;

                // Map it
                tableToTypeMapper.Add(map.Key, type);
            }

            // Set the types to the query
            queryMap.DataModelTypes = tableToTypeMapper.Select(x => x.Value).ToList();

            // Create the db context type builder
            var dbContextTypeBuilder = CeidDiplomatikiHelpers.GetTypeBuilder(Guid.NewGuid().ToString() + "-DbContext");

            // Set the base type
            var baseType = typeof(PresenterDbContext);

            // Inherit from the PresenterDbContext
            dbContextTypeBuilder.SetParent(baseType);

            // For every data model type...
            foreach (var map in tableToTypeMapper)
            {
                // Create the DbSet type that will be set as a property to the DbContext
                var dbSetType = typeof(DbSet<>).MakeGenericType(map.Value);

                // Add it to the type
                TypeBuilderHelpers.CreateProperty(dbContextTypeBuilder, map.Key.TableName, dbSetType);
            }

            // Create the constructors
            TypeBuilderHelpers.CreatePassThroughConstructors(dbContextTypeBuilder, baseType);

            // Create the db context type
            queryMap.DbContextType = dbContextTypeBuilder.CreateType();

            // If there is a data model...
            // NOTE: We create the column and the data presenter maps at the end of the initialization
            //       because some properties of the query map are required to get set!
            if (dataModel != null)
            {
                dataModel.PropertyMaps.ForEach(model => queryMap.Add(PropertyMap.FromDataModel(queryMap,queryMap.DataModelTypes.First(x => queryMap.GetTableName(x) == model.TableName), dataModel.PropertyMaps.First(x => x.TableName == model.TableName && x.ColumnName == model.ColumnName))));
                dataModel.DataGridPresenterMaps.ForEach(model => queryMap.Add(DataGridPresenterMap.FromDataModel(queryMap, model)));
                dataModel.CalendarPresenterMaps.ForEach(model => queryMap.Add(CalendarPresenterMap.FromDataModel(queryMap, model)));
            }

            // Return the map
            return queryMap;
        }

        /// <summary>
        /// Creates and returns a new <see cref="DbContext"/>
        /// based on the <see cref="DbContextType"/>
        /// </summary>
        /// <returns></returns>
        public PresenterDbContext GetDbContext()
        {
            // Get the connection string
            DatabaseOptions.TryGetConnectionString(out var connectionString);

            // Create the options builder
            var optionsBuilder = new DbContextOptionsBuilder();

            // Use MySQL
            optionsBuilder.UseMySql(connectionString);

            // Get the options
            var options = optionsBuilder.Options;

            // Create an instance of the DbContext
            var instance = (PresenterDbContext)Activator.CreateInstance(DbContextType, new object[] { options, this });

            // Return the instance
            return instance;
        }

        /// <summary>
        /// Creates a string that represents the current object
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name + " - " + RootType.Name.Split("-").Last();

        /// <summary>
        /// Creates and returns a <see cref="QueryMapDataModel"/> from the current <see cref="QueryMap"/>
        /// </summary>
        /// <returns></returns>
        public QueryMapDataModel ToDataModel() => new QueryMapDataModel()
        {
            Id = Id,
            DatabaseId = DatabaseOptions.Id,
            TableNames = Tables.Select(x => x.TableName).ToArray(),
            Joins = Joins.Select(x => x.ToDataModel()).ToArray(),
            Color = Color,
            Description = Description,
            Name = Name,

            // Set the column maps
            PropertyMaps = PropertyMaps.Select(x => x.ToDataModel()).ToArray(),

            // Set the data grid presenter maps
            DataGridPresenterMaps = DataGridPresenterMaps.Select(x => x.ToDataModel()).ToArray(),

            // Set the calendar presenter maps
            CalendarPresenterMaps = CalendarPresenterMaps.Select(x => x.ToDataModel()).ToArray()
        };

        #endregion
    }

    /// <summary>
    /// Represents a <see cref="QueryMap"/>
    /// </summary>
    public class QueryMapDataModel
    {
        #region Public Properties

        /// <summary>
        /// The id of the query map
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The id of the registered database that
        /// contains this query map
        /// </summary>
        public string DatabaseId { get; set; }

        /// <summary>
        /// The tables
        /// </summary>
        public string[] TableNames { get; set; }
        
        /// <summary>
        /// The joins
        /// </summary>
        public JoinMapDataModel[] Joins { get; set; }

        /// <summary>
        /// The property maps
        /// </summary>
        public PropertyMapDataModel[] PropertyMaps { get; set; }

        /// <summary>
        /// The data grid presenter maps
        /// </summary>
        public DataGridPresenterMapDataModel[] DataGridPresenterMaps { get; set; }

        /// <summary>
        /// The calendar presenter maps
        /// </summary>
        public CalendarPresenterMapDataModel[] CalendarPresenterMaps { get; set; }

        /// <summary>
        /// The name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The color
        /// </summary>
        public string Color { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public QueryMapDataModel() : base()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a string that represents the current object
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;

        #endregion
    }
}
