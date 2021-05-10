using Atom.Relational.Analyzers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Maps information related to the specified <see cref="Column"/>
    /// </summary>
    public class PropertyMap
    {
        #region Public Properties

        /// <summary>
        /// The query map
        /// </summary>
        public QueryMap QueryMap { get; }

        /// <summary>
        /// The type that contains the <see cref="PropertyInfo"/>
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The property that's getting map.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// The table that contains the <see cref="Column"/>.
        /// NOTE: The table will be <see cref="null"/> when the <see cref="PropertyInfo"/> is a navigation property!
        /// </summary>
        public IDbProviderTable Table { get; }

        /// <summary>
        /// The column.
        /// NOTE: The column will be <see cref="null"/> when the <see cref="PropertyInfo"/> is a navigation property!
        /// </summary>
        public IDbProviderColumn Column { get; }

        /// <summary>
        /// The name of the column
        /// </summary>
        public string ColumnName => Column?.ColumnName;

        /// <summary>
        /// The name of the table
        /// </summary>
        public string TableName => Table?.TableName;

        /// <summary>
        /// The standardize column value type
        /// </summary>
        public ColumnValueType ValueType => PropertyInfo.PropertyType.ToColumnValueType();

        /// <summary>
        /// The id of the column map
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The custom name of the column
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

        /// <summary>
        /// The default value
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// The presentation order
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The column attributes
        /// </summary>
        public IEnumerable<ColumnAttribute> Attributes { get; set; }

        /// <summary>
        /// A flag indicating whether the column value is editable or not
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// A flag indicating whether the column values is required or not
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// A flag indicating whether the column values should get previewed when possible.
        /// Ex.: A read-only input on create and edit forms.
        /// </summary>
        public bool IsPreview { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="queryMap">The query map</param>
        /// <param name="type">The type that contains the <see cref="PropertyInfo"/></param>
        /// <param name="property">The property that's getting map.</param>
        /// <param name="column">
        /// The column.
        /// NOTE: The column will be <see cref="null"/> when the <see cref="PropertyInfo"/> is a navigation property!
        /// </param>
        public PropertyMap(QueryMap queryMap, Type type, PropertyInfo property, IDbProviderColumn column = null) : base()
        {
            Column = column;
            QueryMap = queryMap ?? throw new ArgumentNullException(nameof(queryMap));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            PropertyInfo = property ?? throw new ArgumentNullException(nameof(property));
            Table = QueryMap.Tables.First(x => x.TableName == queryMap.GetTableName(type));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;

        /// <summary>
        /// Creates and returns a <see cref="PropertyMapDataModel"/> from the current <see cref="PropertyMap"/>
        /// </summary>
        /// <returns></returns>
        public PropertyMapDataModel ToDataModel() => new PropertyMapDataModel()
        {
            Id = Id,
            QueryMapId = QueryMap.Id,
            Name = Name,
            TableName = TableName,
            ColumnName = ColumnName,
            Color = Color,
            DefaultValue = DefaultValue,
            Order = Order,
            Attributes = Attributes.ToArray(),
            Description = Description,
            IsEditable = IsEditable,
            IsRequired = IsRequired,
            IsPreview = IsPreview,
            PropertyName = PropertyInfo.Name
        };

        /// <summary>
        /// Creates and returns a <see cref="PropertyMap"/> from the specified <paramref name="model"/>
        /// </summary>
        /// <param name="queryMap">The query map</param>
        /// <param name="type">The type that contains the <see cref="PropertyInfo"/></param>
        /// <param name="model">The model</param>
        /// <returns></returns>
        public static PropertyMap FromDataModel(QueryMap queryMap, Type type, PropertyMapDataModel model)
        {
            // Create and return the column map
            return new PropertyMap(queryMap, type, type.GetProperty(model.PropertyName), queryMap.Columns.FirstOrDefault(x => x.TableName == model.TableName && x.ColumnName == model.ColumnName)) 
            {
                Id = model.Id,
                Color = model.Color,
                Description = model.Description,
                Name = model.Name,
                Attributes = model.Attributes,
                IsRequired = model.IsRequired,
                IsEditable = model.IsEditable,
                IsPreview = model.IsPreview,
                DefaultValue = model.DefaultValue,
                Order = model.Order
            };
        }

        #endregion
    }

    /// <summary>
    /// Represents a <see cref="PropertyMap"/>
    /// </summary>
    public class PropertyMapDataModel
    {
        #region Public Properties

        /// <summary>
        /// The id of the column map
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The id of the query map that contains this column map
        /// </summary>
        public string QueryMapId { get; set; }

        /// <summary>
        /// The property name
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// The name of the table that contains the column
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// The name of the column that was mapped
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// The custom name of the column
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

        /// <summary>
        /// The default value
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// The presentation order
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The column attributes
        /// </summary>
        public ColumnAttribute[] Attributes { get; set; }

        /// <summary>
        /// A flag indicating whether the column value is editable or not
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// A flag indicating whether the column values is required or not
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// A flag indicating whether the column values should get previewed when possible.
        /// Ex.: A read-only input on create and edit forms.
        /// </summary>
        public bool IsPreview { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public PropertyMapDataModel() : base()
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
}
