using Atom.Core;
using Atom.Windows.Controls;

using System;
using System.Linq;

using static Atom.Core.Personalization;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Helper methods related to CeidDiplomatiki data models
    /// </summary>
    public static class CeidDiplomatikiDataModelHelpers
    {
        #region SQLite Options Data Model

        /// <summary>
        /// Creates and returns a <see cref="DataGrid{TClass}"/> used for presenting
        /// <see cref="SQLiteOptionsDataModel"/>s that has pre-selected the default columns
        /// </summary>
        public static DataGrid<SQLiteOptionsDataModel> CreateDefaultSQLiteOptionsDataModelDataGrid()
        {
            return new DataGrid<SQLiteOptionsDataModel>()
                .ShowData(x => x.DatabaseName)
                .ShowData(x => x.DirectoryPath);
        }

        #endregion

        #region MySQL Options Data Model

        /// <summary>
        /// Creates and returns a <see cref="DataGrid{TClass}"/> used for presenting
        /// <see cref="MySQLOptionsDataModel"/>s that has pre-selected the default columns
        /// </summary>
        public static DataGrid<MySQLOptionsDataModel> CreateDefaultMySQLOptionsDataModelDataGrid()
        {
            return new DataGrid<MySQLOptionsDataModel>()
                .ShowData(x => x.DatabaseName)
                .ShowData(x => x.Server)
                .ShowData(x => x.Port)
                .ShowData(x => x.UserId)
                .ShowData(x => x.Password);
        }

        #endregion

        #region SQLServer Options Data Model

        /// <summary>
        /// Creates and returns a <see cref="DataGrid{TClass}"/> used for presenting
        /// <see cref="SQLServerOptionsDataModel"/>s that has pre-selected the default columns
        /// </summary>
        public static DataGrid<SQLServerOptionsDataModel> CreateDefaultSQLServerOptionsDataModelDataGrid()
        {
            return new DataGrid<SQLServerOptionsDataModel>()
                .ShowData(x => x.DatabaseName)
                .ShowData(x => x.Server)
                .ShowData(x => x.Port)
                .ShowData(x => x.UserId)
                .ShowData(x => x.Password)
                .ShowData(x => x.IntegratedSecurity)

                .SetBooleanUIElement(x => x.IntegratedSecurity);
        }

        #endregion

        #region PostgreSQL Options Data Model

        /// <summary>
        /// Creates and returns a <see cref="DataGrid{TClass}"/> used for presenting
        /// <see cref="PostgreSQLOptionsDataModel"/>s that has pre-selected the default columns
        /// </summary>
        public static DataGrid<PostgreSQLOptionsDataModel> CreateDefaultPostgreSQLOptionsDataModelDataGrid()
        {
            return new DataGrid<PostgreSQLOptionsDataModel>()
                .ShowData(x => x.DatabaseName)
                .ShowData(x => x.Host)
                .ShowData(x => x.Port)
                .ShowData(x => x.UserName)
                .ShowData(x => x.Password);
        }

        #endregion

        #region Database query map

        /// <summary>
        /// Maps the properties of the <see cref="QueryMap"/> to custom values
        /// </summary>
        public static Lazy<PropertyMapper<QueryMap>> QueryMapMapper { get; } = new Lazy<PropertyMapper<QueryMap>>(() =>
        {
            // Create the mapper
            var mapper = new PropertyMapper<QueryMap>()
                .MapTitle(x => x.DbContextType, "Database context type")
                .MapTitle(x => x.DataModelTypes, "Data model types")
                .MapTitle(x => x.DatabaseOptions, "Database options")
                .MapTitle(x => x.Database, "Database")
                .MapTitle(x => x.Tables, "Tables")
                .MapTitle(x => x.Columns, "Columns")
                .MapTitle(x => x.PropertyMaps, "Property maps")
                .MapTitle(x => x.Joins, "Joins")
                .MapTitle(x => x.PropertyShortcodes, "Property shortcodes")
                .MapTitle(x => x.Id, "Id")
                .MapTitle(x => x.Name, "Name")
                .MapTitle(x => x.Description, "Description")
                .MapTitle(x => x.Color, "Color")
                .MapTitle(x => x.RootType, "Root type")
                .MapTitle(x => x.PresenterMaps, "Presenter maps")
                .MapTitle(x => x.DataGridPresenterMaps, "Data grid presenter maps")
                .MapTitle(x => x.CalendarPresenterMaps, "Calendar presenter maps")
                .MapTitle(x => x.CustomShortcodes, "Custom shortcodes");

            // Validate the mapper
            mapper.Validate();

            // Return the mapper
            return mapper;
        });

        #endregion

        #region Property Map

        /// <summary>
        /// Maps the properties of the <see cref="PropertyMap"/> to custom values
        /// </summary>
        public static Lazy<PropertyMapper<PropertyMap>> PropertyMapMapper { get; } = new Lazy<PropertyMapper<PropertyMap>>(() =>
        {
            // Create the mapper
            var mapper = new PropertyMapper<PropertyMap>()
                .MapTitle(x => x.QueryMap, "Query map")
                .MapTitle(x => x.Type, "Type")
                .MapTitle(x => x.PropertyInfo, "Property map")
                .MapTitle(x => x.Table, "Table")
                .MapTitle(x => x.Column, "Column")
                .MapTitle(x => x.DefaultValue, "Default value")
                .MapTitle(x => x.Order, "Order")
                .MapTitle(x => x.ColumnName, "Column name")
                .MapTitle(x => x.TableName, "Table name")
                .MapTitle(x => x.ValueType, "Value type")
                .MapTitle(x => x.Id, "Id")
                .MapTitle(x => x.Name, "Name")
                .MapTitle(x => x.Description, "Description")
                .MapTitle(x => x.Color, "Color")
                .MapTitle(x => x.Attributes, "Attributes")
                .MapTitle(x => x.IsEditable, "Editable")
                .MapTitle(x => x.IsRequired, "Required")
                .MapTitle(x => x.IsPreview, "Preview");

            // Validate the mapper
            mapper.Validate();

            // Return the mapper
            return mapper;
        });

        #endregion

        #region Join Map

        /// <summary>
        /// Maps the properties of the <see cref="JoinMap"/> to custom values
        /// </summary>
        public static Lazy<PropertyMapper<JoinMap>> JoinMapMapper { get; } = new Lazy<PropertyMapper<JoinMap>>(() =>
        {
            // Create the mapper
            var mapper = new PropertyMapper<JoinMap>()
                .MapTitle(x => x.PrincipleKeyColumn, "Principle key column")
                .MapTitle(x => x.Table, "Principle table")
                .MapTitle(x => x.ForeignKeyColumn, "Foreign key column")
                .MapTitle(x => x.ReferencedTable, "Referenced table")
                .MapTitle(x => x.Index, "Index")
                .MapTitle(x => x.IsRightJoin, "Is right join");

            // Validate the mapper
            mapper.Validate();

            // Return the mapper
            return mapper;
        });

        /// <summary>
        /// Gets the <see cref="Translator{TClass}"/> required for translating the 
        /// values of a <see cref="JoinMap"/>
        /// </summary>
        public static Lazy<Translator<JoinMap>> JoinMapTranslator { get; } = new Lazy<Translator<JoinMap>>(() =>
        {
            return new Translator<JoinMap>()
                .SetTranslator(x => x.ForeignKeyColumn, x => x.ColumnName)
                .SetTranslator(x => x.PrincipleKeyColumn, x => x.ColumnName)
                .SetTranslator(x => x.Table, x => x.TableName)
                .SetTranslator(x => x.ReferencedTable, x => x.TableName);
        });

        #endregion

        #region Base Presenter Map

        /// <summary>
        /// Maps the properties of the <see cref="BasePresenterMap"/> to custom values
        /// </summary>
        public static Lazy<PropertyMapper<BasePresenterMap>> BasePresenterMapMapper { get; } = new Lazy<PropertyMapper<BasePresenterMap>>(() =>
        {
            // Create the mapper
            var mapper = new PropertyMapper<BasePresenterMap>()
                .MapTitle(x => x.QueryMap, "query map")
                .MapTitle(x => x.Id, "Id")
                .MapTitle(x => x.Name, "Name")
                .MapTitle(x => x.Description, "Description")
                .MapTitle(x => x.Color, "Color");

            // Validate the mapper
            mapper.Validate();

            // Return the mapper
            return mapper;
        });

        #endregion

        #region Data Grid Presenter Map

        /// <summary>
        /// Maps the properties of the <see cref="DataGridPresenterMap"/> to custom values
        /// </summary>
        public static Lazy<PropertyMapper<DataGridPresenterMap>> DataGridPresenterMapMapper { get; } = new Lazy<PropertyMapper<DataGridPresenterMap>>(() =>
        {
            // Create the mapper
            var mapper = new PropertyMapper<DataGridPresenterMap>()
                .MapTitle(x => x.DataGrids, "Data grids");

            mapper.CopyMapsForm(BasePresenterMapMapper.Value);

            // Validate the mapper
            mapper.Validate();

            // Return the mapper
            return mapper;
        });

        #endregion

        #region Data Grid Map

        /// <summary>
        /// Maps the properties of the <see cref="DataGridMap"/> to custom values
        /// </summary>
        public static Lazy<PropertyMapper<DataGridMap>> DataGridMapMapper { get; } = new Lazy<PropertyMapper<DataGridMap>>(() =>
        {
            // Create the mapper
            var mapper = new PropertyMapper<DataGridMap>()
                .MapTitle(x => x.PresenterMap, "Presenter map")
                .MapTitle(x => x.QueryMap, "Query map")
                .MapTitle(x => x.Type, "Type")
                .MapTitle(x => x.AllowAdd, "Allow add")
                .MapTitle(x => x.AllowEdit, "Allow edit")
                .MapTitle(x => x.AllowDelete, "Allow delete")
                .MapTitle(x => x.Columns, "Columns")
                .MapTitle(x => x.SearchColumns, "Search columns")
                .MapTitle(x => x.DateColumn, "Date column");


            // Validate the mapper
            mapper.Validate();

            // Return the mapper
            return mapper;
        });

        /// <summary>
        /// Gets the <see cref="Translator{TClass}"/> required for translating the 
        /// values of a <see cref="DataGridMap"/>
        /// </summary>
        public static Lazy<Translator<DataGridMap>> DataGridMapTranslator { get; } = new Lazy<Translator<DataGridMap>>(() =>
        {
            return new Translator<DataGridMap>().SetTranslator(x => x.Type, t => t.Name.Split("-").Last());
        });

        #endregion

        #region Calendar Presenter Map

        /// <summary>
        /// Maps the properties of the <see cref="CalendarPresenterMap"/> to custom values
        /// </summary>
        public static Lazy<PropertyMapper<CalendarPresenterMap>> CalendarPresenterMapMapper { get; } = new Lazy<PropertyMapper<CalendarPresenterMap>>(() =>
        {
            // Create the mapper
            var mapper = new PropertyMapper<CalendarPresenterMap>()
                .MapTitle(x => x.TitleFormula, "Title formula")
                .MapTitle(x => x.DescriptionFormula, "Description formula")
                .MapTitle(x => x.DateStartColumn, "Date start column")
                .MapTitle(x => x.DateEndColumn, "Date end column")
                .MapTitle(x => x.SearchColumns, "Search columns")
                .MapTitle(x => x.AllowAdd, "Allow add")
                .MapTitle(x => x.AllowEdit, "Allow edit")
                .MapTitle(x => x.AllowDelete, "Allow delete");

            mapper.CopyMapsForm(BasePresenterMapMapper.Value);

            // Validate the mapper
            mapper.Validate();

            // Return the mapper
            return mapper;
        });

        /// <summary>
        /// Gets the <see cref="Translator{TClass}"/> required for translating the 
        /// values of a <see cref="CalendarPresenterMap"/>
        /// </summary>
        public static Lazy<Translator<CalendarPresenterMap>> CalendarPresenterMapTranslator { get; } = new Lazy<Translator<CalendarPresenterMap>>(() =>
        {
            return new Translator<CalendarPresenterMap>()
                .SetTranslator(x => x.DateStartColumn, x => x.Name)
                .SetTranslator(x => x.DateEndColumn, x => x?.Name)
                
                .SetEnumerableAggragationTranslator(x => x.SearchColumns, x => x.Name)

                .SetBooleanTranslator(x => x.AllowDelete);
        });

        #endregion

        #region Column Value Type

        /// <summary>
        /// Returns a localized string that represents the specified <paramref name="columnValueType"/>
        /// </summary>
        /// <param name="columnValueType">The column value type</param>
        /// <returns></returns>
        public static string ToLocalizedString(this ColumnValueType columnValueType)
        {
            if (columnValueType == ColumnValueType.Text)
                return "Text";
            else if (columnValueType == ColumnValueType.Number)
                return "Number";
            else if (columnValueType == ColumnValueType.DateTime)
                return "Date time";
            else
                return "Other";
        }

        /// <summary>
        /// Returns the color hex value that represents the specified <paramref name="columnValueType"/>
        /// </summary>
        /// <param name="columnValueType">The column value type</param>
        /// <returns></returns>
        public static string ToColorHex(this ColumnValueType columnValueType)
        {
            if (columnValueType == ColumnValueType.Text)
                return Blue;
            else if (columnValueType == ColumnValueType.Number)
                return Green;
            else if (columnValueType == ColumnValueType.DateTime)
                return Red;
            else
                return RoyalPurple;
        }

        #endregion

        #region Page Map

        /// <summary>
        /// Maps the properties of the <see cref="PageMap"/> to custom values
        /// </summary>
        public static Lazy<PropertyMapper<PageMap>> PageMapMapper { get; } = new Lazy<PropertyMapper<PageMap>>(() =>
        {
            // Create the mapper
            var mapper = new PropertyMapper<PageMap>()
                .MapTitle(x => x.Id, "Id")
                .MapTitle(x => x.Name, "Name")
                .MapTitle(x => x.Description, "Description")
                .MapTitle(x => x.PathData, "Icon")
                .MapTitle(x => x.Color, "Color")
                .MapTitle(x => x.Presenter, "Presenter")
                .MapTitle(x => x.Category, "Category")
                .MapTitle(x => x.Order, "Order")
                .MapTitle(x => x.Parent, "Parent")
                .MapTitle(x => x.IsRoot, "Is root")
                .MapTitle(x => x.Pages, "Pages");

            // Validate the mapper
            mapper.Validate();

            // Return the mapper
            return mapper;
        });

        /// <summary>
        /// Creates and returns a <see cref="DataForm{TClass}"/> for a <see cref="PageMap"/>
        /// </summary>
        /// <returns></returns>
        public static DataForm<PageMap> CreatePageMapDataForm()
        {
            return new DataForm<PageMap>() { Mapper = CeidDiplomatikiDataModelHelpers.PageMapMapper.Value }
                    .ShowInput(x => x.Name, null, true)
                    .ShowInput(x => x.Description)
                    .ShowIconFormInput(x => x.PathData)
                    .ShowStringColorFormInput(x => x.Color)
                    .ShowInput(x => x.Category)
                    .ShowNumericInput(x => x.Order)
                    .ShowSelectSingleOptionInput<BasePresenterMap>(x => x.Presenter, (form, propertyInfo) => new DropDownMenuOptionsFormInput<BasePresenterMap>(form, propertyInfo, CeidDiplomatikiDI.GetCeidDiplomatikiManager.Presenters, x => x.Name));
        }

        #endregion
    }
}
