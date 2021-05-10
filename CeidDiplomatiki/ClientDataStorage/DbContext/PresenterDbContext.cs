using Atom.Core;
using Atom.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CeidDiplomatiki
{
    /// <summary>
    /// A <see cref="StandardDbContext"/> that used the Fluent API to configure its
    /// dynamically created <see cref="DbSet{TEntity}"/> properties using the information
    /// provided by the <see cref="QueryMap"/>
    /// </summary>
    public class PresenterDbContext : StandardDbContext
    {
        #region Public Properties
        
        /// <summary>
        /// The query map
        /// </summary>
        public QueryMap QueryMap { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContext"/> class
        /// using the specified options. The <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/>
        /// method will still be called to allow further configuration of the options.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        /// <param name="queryMap">The query map</param>
        public PresenterDbContext(DbContextOptions options, QueryMap queryMap) : base(options)
        {
            QueryMap = queryMap ?? throw new ArgumentNullException(nameof(queryMap));
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

            // For every DbSet...
            foreach(var dbSetProperty in GetType().GetProperties().Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)))
            {
                // Get the table
                var table = QueryMap.Tables.First(x => x.TableName == dbSetProperty.Name);

                // Get the name of the primary key column
                var primaryKeyColumnName = QueryMap.Columns.First(x => x.TableName == table.TableName && x.IsPrimaryKey).ColumnName;

                // Get the generic type of the DbSet
                var modelType = dbSetProperty.PropertyType.GetGenericArguments()[0];

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
                var keyBuilder = hasKeyMethod.Invoke(typeBuilder, new object[] { new string[] { primaryKeyColumnName } });

                // For every join where the current DbSet property represents the principle table...
                foreach(var join in QueryMap.Joins.Where(x => x.Table.TableName == table.TableName))
                {
                    // We need to recreate the following Fluent API command using reflection
                    // modelBuilder.Entity<TModel>
                    //             .HasMany<TRelatedModel>(navigationName)
                    //             .WithOne<TRelatedModel>(navigationName)
                    //             .HasForeignKey<TModel, TRelatedModel>(new string[]{ foreignKeyColumnName })
                    //             .HasPrincipleKey<TModel, TRelatedModel>(new string[]{ principleKeyColumnName })
                    //             .OnDelete<TModel, TRelatedModel>(DeleteBehavior.Cascade);

                    // Get the related DbSet property
                    var relatedDbSetProperty = GetType().GetProperty(join.ReferencedTable.TableName);

                    // Get the related model type
                    var relatedModelType = relatedDbSetProperty.PropertyType.GetGenericArguments()[0];

                    // Get the HasMany method
                    var hasManyMethod = typeBuilder.GetType().GetMethods().First(x => x.Name == nameof(EntityTypeBuilder<object>.HasMany) && x.IsGenericMethod && x.GetParameters()[0].ParameterType == typeof(string));

                    // Call the HasMany method
                    var collectionNavigationBuilder = hasManyMethod.MakeGenericMethod(relatedModelType).Invoke(typeBuilder, new object[] { CeidDiplomatikiHelpers.GetPluralForm(join.ReferencedTable.TableName) });

                    // Get the WithOne method
                    var withOneMethod = collectionNavigationBuilder.GetType().GetMethods().First(x => x.Name == nameof(CollectionNavigationBuilder<object, object>.WithOne) && x.GetParameters()[0].ParameterType == typeof(string));

                    // Call the WithOne method
                    var referenceCollectionBuilder = withOneMethod.Invoke(collectionNavigationBuilder, new object[]{ join.Table.TableName });

                    // Get The HasForeignKey method
                    var hasForeignKeyMethod = referenceCollectionBuilder.GetType().GetMethods().First(x => x.Name == nameof(ReferenceCollectionBuilder<object, object>.HasForeignKey) && x.GetParameters()[0].ParameterType == typeof(string[]));

                    // Call the HasForeignKey method
                    hasForeignKeyMethod.Invoke(referenceCollectionBuilder, new object[] { new string[] { join.ForeignKeyColumn.ColumnName } });

                    // Get the HasPrincipleKey method
                    var hasPrincipleKeyMethod = referenceCollectionBuilder.GetType().GetMethods().First(x => x.Name == nameof(ReferenceCollectionBuilder<object, object>.HasPrincipalKey) && x.GetParameters()[0].ParameterType == typeof(string[]));

                    // Call the HasPrincipleKey method
                    hasPrincipleKeyMethod.Invoke(referenceCollectionBuilder, new object[] { new string[] { join.PrincipleKeyColumn.ColumnName } });

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
