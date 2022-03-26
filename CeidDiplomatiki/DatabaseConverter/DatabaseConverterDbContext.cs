using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;
using System.Collections.Generic;
using System.Linq;

namespace CeidDiplomatiki
{
    /// <summary>
    /// A <see cref="DbContext"/> that is used as the base of the DbContext used for converting a database
    /// </summary>
    public class DatabaseConverterDbContext : BaseDbContext
    {
        #region Public Properties

        /// <summary>
        /// The tables of the database
        /// </summary>
        public IEnumerable<IDbProviderTable> Tables { get; }

        /// <summary>
        /// The columns of the database
        /// </summary>
        public IEnumerable<IDbProviderColumn> Columns { get; }

        /// <summary>
        /// The foreign key columns
        /// </summary>
        public IEnumerable<IDbProviderForeignKeyColumn> ForeignKeyColumns { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DatabaseConverterDbContext(DbContextOptions options, IEnumerable<IDbProviderTable> tables, IEnumerable<IDbProviderColumn> columns, IEnumerable<IDbProviderForeignKeyColumn> foreignKeyColumns) : base(options)
        {
            Tables = tables ?? throw new ArgumentNullException(nameof(tables));
            Columns = columns ?? throw new ArgumentNullException(nameof(columns));
            ForeignKeyColumns = foreignKeyColumns ?? throw new ArgumentNullException(nameof(foreignKeyColumns));
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Override this method to further configure the model that was discovered by convention
        /// from the entity types exposed in Microsoft.EntityFrameworkCore.DbSet`1 properties
        /// on your derived context. The resulting model may be cached and re-used for subsequent
        /// instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">The model builder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Get the prefix
            var prefix = PrefixAccessor.GetPrefixFromDbContextOrEmpty(this);

            // For every database context table...
            foreach (var dbContextTable in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = dbContextTable.GetTableName();
                // Get the table
                var table = Tables.First(x => prefix + x.TableName == tableName);

                // Get the name of the primary key column
                var primaryKeyColumn = Columns.First(x => x.TableName == table.TableName && x.IsPrimaryKey);

                // Get the generic type of the DbSet
                var modelType = dbContextTable.ClrType;

                // We need to recreate the following Fluent API command using reflection
                // modelBuilder.Entity<TModel>().HasKey(new string[]{ IdColumnName });

                // Get the Entity<> method
                var entityMethod = modelBuilder.GetType().GetMethods().First(x => x.Name == nameof(ModelBuilder.Entity) && x.IsGenericMethod)
                    .MakeGenericMethod(modelType);

                // Call the Entity<> method
                var typeBuilder = entityMethod.Invoke(modelBuilder, null);

                // Get the HasKey method
                var hasKeyMethod = typeBuilder.GetType().GetMethods()
                    .First(x => x.Name == nameof(EntityTypeBuilder<object>.HasKey) && x.GetParameters().Any(y => y.ParameterType == typeof(string[])));

                // Call the HasKey method
                var keyBuilder = hasKeyMethod.Invoke(typeBuilder, new object[] { new string[] { primaryKeyColumn.ColumnName } });

                // If it's auto increment...
                if (primaryKeyColumn.IsAutoIncrement)
                {
                    // We need to recreate the following Fluent API command using reflection
                    // modelBuilder.Entity<TModel>().Property(string).ValueGeneratedOnAdd();

                    // Get the Property method
                    var propertyMethod = typeBuilder.GetType().GetMethods().First(x => x.Name == nameof(EntityTypeBuilder<object>.Property) && x.GetGenericArguments().Count() == 0 && x.GetParameters().Any(y => y.ParameterType == typeof(string)));

                    // Call the Property method
                    var propertyBuilder = (PropertyBuilder)propertyMethod.Invoke(typeBuilder, new object[] { primaryKeyColumn.ColumnName });

                    if (Database.IsSqlServer())
                        propertyBuilder.ValueGeneratedNever();
                    else
                        // Set the value generation
                        propertyBuilder.ValueGeneratedOnAdd();
                }

                // For every join where the current DbSet property represents the principle table...
                foreach (var foreignKeyColumn in ForeignKeyColumns.Where(x => x.ReferencedTableName == table.TableName))
                {
                    // We need to recreate the following Fluent API command using reflection
                    // modelBuilder.Entity<TModel>
                    //             .HasMany<TRelatedModel>(navigationName)
                    //             .WithOne<TRelatedModel>(navigationName)
                    //             .HasForeignKey<TModel, TRelatedModel>(new string[]{ foreignKeyColumnName })
                    //             .HasPrincipleKey<TModel, TRelatedModel>(new string[]{ principleKeyColumnName })
                    //             .OnDelete<TModel, TRelatedModel>(DeleteBehavior.Cascade);

                    // Get the related DbSet property
                    var relatedDbSetProperty = GetType().GetProperty(foreignKeyColumn.TableName);

                    // Get the related model type
                    var relatedModelType = relatedDbSetProperty.PropertyType.GetGenericArguments()[0];

                    // Get the HasMany method
                    var hasManyMethod = typeBuilder.GetType().GetMethods().First(x => x.Name == nameof(EntityTypeBuilder<object>.HasMany) && x.IsGenericMethod && x.GetParameters()[0].ParameterType == typeof(string));

                    // Call the HasMany method
                    var collectionNavigationBuilder = hasManyMethod.MakeGenericMethod(relatedModelType).Invoke(typeBuilder, new object[] { MigrationHelpers.GetPluralForm(foreignKeyColumn.TableName) });

                    // Get the WithOne method
                    var withOneMethod = collectionNavigationBuilder.GetType().GetMethods().First(x => x.Name == nameof(CollectionNavigationBuilder<object, object>.WithOne) && x.GetParameters()[0].ParameterType == typeof(string));

                    // Call the WithOne method
                    var referenceCollectionBuilder = withOneMethod.Invoke(collectionNavigationBuilder, new object[] { foreignKeyColumn.ReferencedTableName });

                    // Get The HasForeignKey method
                    var hasForeignKeyMethod = referenceCollectionBuilder.GetType().GetMethods().First(x => x.Name == nameof(ReferenceCollectionBuilder<object, object>.HasForeignKey) && x.GetParameters()[0].ParameterType == typeof(string[]));

                    // Call the HasForeignKey method
                    hasForeignKeyMethod.Invoke(referenceCollectionBuilder, new object[] { new string[] { foreignKeyColumn.ColumnName } });

                    // Get the HasPrincipleKey method
                    var hasPrincipleKeyMethod = referenceCollectionBuilder.GetType().GetMethods().First(x => x.Name == nameof(ReferenceCollectionBuilder<object, object>.HasPrincipalKey) && x.GetParameters()[0].ParameterType == typeof(string[]));

                    // Call the HasPrincipleKey method
                    hasPrincipleKeyMethod.Invoke(referenceCollectionBuilder, new object[] { new string[] { foreignKeyColumn.ReferencedColumnName } });

                    // Get the OnDelete method
                    var onDeleteMethod = referenceCollectionBuilder.GetType().GetMethods().First(x => x.Name == nameof(ReferenceCollectionBuilder<object, object>.OnDelete));

                    // Call the OnDelete method
                    // TODO: We should get the constraint condition from the analyzer and then user it to configure the delete behavior.
                    onDeleteMethod.Invoke(referenceCollectionBuilder, new object[] { DeleteBehavior.Cascade });
                }
            }
        }

        #endregion
    }
}
