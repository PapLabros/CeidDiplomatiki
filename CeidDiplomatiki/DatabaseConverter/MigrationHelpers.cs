using Atom.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    public static class MigrationHelpers
    {
        #region Public Methods

        #region Strings

        /// <summary>
        /// Uses the specified <paramref name="s"/> to generate a unique
        /// property name.
        /// NOTE: The <paramref name="s"/> is usually a database column name!
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns></returns>
        public static string GetUniquePropertyName(string s) => Guid.NewGuid().ToString() + "-" + s;

        /// <summary>
        /// Gets the plural form of the specified <paramref name="s"/>
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns></returns>
        public static string GetPluralForm(string s) => s + "s";

        /// <summary>
        /// Return the normalized version of the <paramref name="value"/>
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns></returns>
        public static string NormalizeString(string value) => value?.ToUpper();

        #endregion

        #region Reflection Emit

        /// <summary>
        /// Gets the assembly name used by the <see cref="DynamicAssemblyBuilder"/>
        /// </summary>
        public static AssemblyName DynamicAssemblyName { get; } = new AssemblyName("DynamicAssembly");

        /// <summary>
        /// Gets the assembly builder that creates in memory assemblies
        /// </summary>
        public static AssemblyBuilder DynamicAssemblyBuilder { get; } = AssemblyBuilder.DefineDynamicAssembly(DynamicAssemblyName, AssemblyBuilderAccess.Run);

        /// <summary>
        /// Gets the module builder provided by the <see cref="DynamicAssemblyBuilder"/>
        /// </summary>
        public static ModuleBuilder ModuleBuilder { get; } = DynamicAssemblyBuilder.DefineDynamicModule("Core");

        /// <summary>
        /// Gets a type builder that has defined a type with the specified <paramref name="typeName"/>.
        /// NOTE: If <paramref name="typeName"/> is set to <see cref="null"/> then the name of the type is
        ///       set to a new <see cref="Guid"/>!
        /// </summary>
        /// <param name="typeName">The name of the type</param>
        /// <returns></returns>
        public static TypeBuilder GetTypeBuilder(string typeName = null) => ModuleBuilder.DefineType(typeName.IsNullOrEmpty() ? GenerateUniqueTypeName() : typeName);

        #endregion

        /// <summary>
        /// Creates and returns the <see cref="DbContext"/> type that represents the database
        /// pointed by the specified <paramref name="info"/>
        /// </summary>
        /// <param name="info">The info</param>
        /// <returns></returns>
        public static Failable<DynamicDbContextCreationResult> CreateDbContextType(DatabaseInfo info)
        {
            try
            {
                var dbContext = new DbContext(info.GetDbContextOptions());

                // Get the dbContext relational type mapping service
                var relationalTypeMapping = dbContext.GetInfrastructure().GetRequiredService<IRelationalTypeMappingSource>();

                // Get an analyzer
                var analyzer = info.GetDatabaseAnalyzer();

                // Get the table result
                var tableResults = analyzer.GetTables(info.DatabaseName);

                // If not successful...
                if (!tableResults.Successful)
                    // Return the unsuccessful result
                    return new Failable<DynamicDbContextCreationResult>()
                    {
                        ErrorMessage = tableResults.ErrorMessage,
                        ErrorType = tableResults.ErrorType
                    };

                // Get the tables
                var tables = tableResults.Result;

                var migrationsTableName = NormalizeString(GetMigrationTableName(dbContext));
                var tablesPrefix = NormalizeString(info.TablesPrefix);

                tables = tables.Where(x => NormalizeString(x.TableName) != migrationsTableName).ToList();

                // If there is a tables prefix...
                if (!info.TablesPrefix.IsNullOrEmpty())
                    tables = tables.Where(x => NormalizeString(x.TableName).StartsWith(tablesPrefix)).ToList();

                // The database table columns
                var columns = new List<IDbProviderColumn>();

                // For every table...
                foreach (var table in tables)
                {
                    // Get the columns result
                    var columnsResults = analyzer.GetColumns(info.DatabaseName, table.TableName);

                    // If not successful...
                    if (!columnsResults.Successful)
                        // Return the unsuccessful result
                        return new Failable<DynamicDbContextCreationResult>()
                        {
                            ErrorMessage = columnsResults.ErrorMessage,
                            ErrorType = columnsResults.ErrorType
                        };

                    // Get the columns
                    columns.AddRange(columnsResults.Result);
                }

                // The foreign key columns
                var foreignKeyColumns = new List<IDbProviderForeignKeyColumn>();

                // For every table...
                foreach (var table in tables)
                {
                    // Get the foreign keys result
                    var foreignKeyResults = analyzer.GetForeignKeyColumns(info.DatabaseName, table.TableName);

                    // If not successful...
                    if (!foreignKeyResults.Successful)
                        // Return the unsuccessful result
                        return new Failable<DynamicDbContextCreationResult>()
                        {
                            ErrorMessage = foreignKeyResults.ErrorMessage,
                            ErrorType = foreignKeyResults.ErrorType
                        };

                    // Get the foreign key columns
                    foreignKeyColumns.AddRange(foreignKeyResults.Result);
                }

                #region Tables

                var tableToTypeBuilderMapper = new Dictionary<IDbProviderTable, TypeBuilder>();

                // For every table...
                foreach (var table in tables)
                {
                    // Create the builder
                    var typeBuilder = GetTypeBuilder(GenerateUniqueTypeName() + "-" + table.TableName);

                    // For every table column...
                    foreach (var column in columns.Where(x => x.TableName == table.TableName))
                        // Create a property
                        TypeBuilderHelpers.CreateProperty(typeBuilder, column.ColumnName, column.IsNullable ? GetNullableType(relationalTypeMapping.FindMapping(column.SQLTypeName).ClrType) : relationalTypeMapping.FindMapping(column.SQLTypeName).ClrType);

                    // Map it
                    tableToTypeBuilderMapper.Add(table, typeBuilder);
                }

                #endregion

                #region Navigation Properties

                // For every foreign key column...
                foreach (var foreignKeyColumn in foreignKeyColumns)
                {
                    // Get the principle model type builder
                    var principleModelTypeBuilder = tableToTypeBuilderMapper.First(x => x.Key.TableName == foreignKeyColumn.ReferencedTableName).Value;

                    // Get the referenced model type builder
                    var referencedModelTypeBuilder = tableToTypeBuilderMapper.First(x => x.Key.TableName == foreignKeyColumn.TableName).Value;

                    // Create the principle navigation property type
                    var principleNavigationPropertyType = typeof(IEnumerable<>).MakeGenericType(referencedModelTypeBuilder);

                    // Add it to the principle model
                    TypeBuilderHelpers.CreateProperty(principleModelTypeBuilder, GetPluralForm(foreignKeyColumn.TableName), principleNavigationPropertyType);

                    // Add the foreign navigation property type
                    TypeBuilderHelpers.CreateProperty(referencedModelTypeBuilder, foreignKeyColumn.ReferencedTableName, principleModelTypeBuilder);
                }

                #endregion

                #region DbContext

                // Declare a dictionary for the mapping between table and types
                var tableToTypeMapper = new Dictionary<IDbProviderTable, Type>();

                // For every table to type builder map...
                foreach (var map in tableToTypeBuilderMapper)
                {
                    // Build the type
                    var type = map.Value.CreateTypeInfo();

                    // Map it
                    tableToTypeMapper.Add(map.Key, type);
                }

                // Create the db context type builder
                var dbContextTypeBuilder = GetTypeBuilder(Guid.NewGuid().ToString() + "-DbContext");

                // Set the base type
                var baseType = typeof(DatabaseConverterDbContext);

                // Inherit from the DatabaseConverterDbContext
                dbContextTypeBuilder.SetParent(baseType);

                // For every data model type...
                foreach (var map in tableToTypeMapper)
                {
                    // Create the DbSet type that will be set as a property to the DbContext
                    var dbSetType = typeof(DbSet<>).MakeGenericType(map.Value);

                    // Get the table name
                    var tableName = map.Key.TableName;

                    // If there was a tables prefix...
                    if (!info.TablesPrefix.IsNullOrEmpty())
                        // Clear the prefix
                        tableName = tableName.Replace(info.TablesPrefix, string.Empty);

                    // Add it to the type
                    TypeBuilderHelpers.CreateProperty(dbContextTypeBuilder, tableName, dbSetType);
                }

                // Create the constructors
                TypeBuilderHelpers.CreatePassThroughConstructors(dbContextTypeBuilder, baseType);

                // Create the source db context type
                var sourceDbContextType = dbContextTypeBuilder.CreateTypeInfo();

                // Create the db context type builder
                var destinationDBContextTypeBuilder = GetTypeBuilder(Guid.NewGuid().ToString() + "-DbContext");

                // Inherit from the DatabaseConverterDbContext
                destinationDBContextTypeBuilder.SetParent(dbContextTypeBuilder);

                // Create the constructors
                TypeBuilderHelpers.CreatePassThroughConstructors(destinationDBContextTypeBuilder, baseType);

                // Create the destination db context type
                var destinationDbContextType = destinationDBContextTypeBuilder.CreateTypeInfo();

                #endregion

                // Return the result
                return new Failable<DynamicDbContextCreationResult>(new DynamicDbContextCreationResult(sourceDbContextType, destinationDbContextType, tables, columns, foreignKeyColumns));
            }
            catch (Exception ex)
            {
                return new Failable<DynamicDbContextCreationResult>() { ErrorMessage = ex.Message };
            }
        }

        /// <summary>
        /// Creates and returns an instance of type <paramref name="dbContextType"/> and configures it for the database
        /// specified by the <paramref name="databaseInfo"/>
        /// </summary>
        /// <param name="databaseInfo">The database info</param>
        /// <param name="dbContextType">The type of the db context to create an instance from</param>
        /// <param name="tables">The tables</param>
        /// <param name="columns">The columns</param>
        /// <param name="foreignKeyColumns">The foreign key columns</param>
        /// <returns></returns>
        public static DatabaseConverterDbContext CreateDbContextInstance(DatabaseInfo databaseInfo, Type dbContextType, IEnumerable<IDbProviderTable> tables, IEnumerable<IDbProviderColumn> columns, IEnumerable<IDbProviderForeignKeyColumn> foreignKeyColumns)
        {
            // Create the options
            var options = databaseInfo.GetDbContextOptions();

            // Create and return an instance of the DbContext
            return (DatabaseConverterDbContext)Activator.CreateInstance(dbContextType, new object[] { options, tables, columns, foreignKeyColumns });
        }

        /// <summary>
        /// Creates and returns an instance of type <typeparamref name="TDbContext"/> and configures it for the database
        /// specified by the <paramref name="databaseInfo"/>
        /// </summary>
        /// <param name="databaseInfo">The database info</param>
        /// <returns></returns>
        public static TDbContext CreateDbContextInstance<TDbContext>(DatabaseInfo databaseInfo)
            where TDbContext : DbContext
        {
            // Create the db context type builder
            var dbContextTypeBuilder = GetTypeBuilder(GenerateUniqueTypeName() + "-DbContext");

            // Inherit from the DatabaseConverterDbContext
            dbContextTypeBuilder.SetParent(typeof(TDbContext));

            // Create the constructors
            TypeBuilderHelpers.CreatePassThroughConstructors(dbContextTypeBuilder, typeof(TDbContext));

            // Create the destination db context type
            var dbContextType = dbContextTypeBuilder.CreateTypeInfo();

            var getDbContextOptionsMethodInfo = typeof(DatabaseInfoExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static).First(x => x.GetGenericArguments().Count() == 1).MakeGenericMethod(typeof(TDbContext));

            // Create the options
            var options = (DbContextOptions)getDbContextOptionsMethodInfo.Invoke(null, new object[] { databaseInfo });

            // Create and return an instance of the DbContext
            return (TDbContext)Activator.CreateInstance(dbContextType, new object[] { options });
        }

        /// <summary>
        /// Executes the <see cref="EntityFrameworkQueryableExtensions.ToListAsync{TSource}(IQueryable{TSource}, CancellationToken)"/> method
        /// on the specified <paramref name="queryable"/>
        /// </summary>
        /// <param name="queryable">The queryable</param>
        /// <param name="queryableType">The type of the queryable</param>
        /// <returns></returns>
        public static async Task<Failable<IEnumerable>> ExecuteToListAsync(IQueryable queryable, Type queryableType)
        {
            try
            {
                // Get the ToListAsync method
                var method = typeof(EntityFrameworkQueryableExtensions).GetMethod(nameof(EntityFrameworkQueryableExtensions.ToListAsync)).MakeGenericMethod(queryableType);

                // Get the task
                var task = (Task)method.Invoke(null, new object[] { queryable, new CancellationToken() });

                // Await the task
                await task;

                // Get the result property from the task
                var resultProperty = task.GetType().GetProperty(nameof(Task<object>.Result));

                // Get the result from the property
                var result = (IEnumerable)resultProperty.GetValue(task);

                // Return the result
                return new Failable<IEnumerable>(result);
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable>() { ErrorMessage = ex.Message };
            }
        }

        /// <summary>
        /// Returns the migration table name
        /// </summary>
        /// <returns></returns>
        public static string GetMigrationTableName(DbContext dbContext)
        {
            // Get the dbContext relational options
            var relationalOptions = (RelationalOptionsExtension)dbContext.GetInfrastructure().GetService<IDbContextOptions>().Extensions.First(x => x.GetType().IsSubclassOf(typeof(RelationalOptionsExtension)));

            // Initialize the default migration table name
            var migrationTableName = "__EFMigrationsHistory";

            // If the migration history table name has a custom name...
            if (relationalOptions.MigrationsHistoryTableName != null)
                // Get the custom name
                migrationTableName = relationalOptions.MigrationsHistoryTableName;

            // Return the table name
            return migrationTableName;
        }

        /// <summary>
        /// Copies the data from the specified <paramref name="sourceDbContext"/> to the specified <paramref name="destinationDbContext"/>
        /// </summary>
        /// <param name="dbContextType">The type of the database contexts</param>
        /// <param name="sourceDbContext">The source database context</param>
        /// <param name="destinationDbContext">The destination database context</param>
        /// <param name="columns">The columns</param>
        /// <returns></returns>
        public static async Task<Failable> CopyDataAsync(Type dbContextType, DbContext sourceDbContext, DbContext destinationDbContext, IEnumerable<IDbProviderColumn> columns)
        {
            try
            {
                // Create the destination database if it doesn't already exist
                await destinationDbContext.Database.EnsureCreatedAsync();

                // Get the auto incremented columns that have current value
                var autoIncrementColumns = columns.Where(x => x.IsAutoIncrement && x.NextAutoIncrementValue != null);
                var tablesPrefix = PrefixAccessor.GetPrefixFromDbContextOrEmpty(destinationDbContext);

                // For every column...
                foreach (var column in autoIncrementColumns)
                    // Set the auto increment primary key value
                    await destinationDbContext.SetAutoIncrementPrimaryKeyValueAsync(tablesPrefix + column.TableName, column.ColumnName, column.NextAutoIncrementValue.Value);

                // Disable the foreign key checks of the destination database
                await destinationDbContext.DisableForeignKeyChecksAsync();

                // For every DbSet<> property...
                foreach (var property in dbContextType.GetProperties().Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)))
                {
                    // Get the source database set
                    var sourceDbSet = (IQueryable)property.GetValue(sourceDbContext);

                    // Get all the data
                    var dataResult = await ExecuteToListAsync(sourceDbSet, property.PropertyType.GetGenericArguments()[0]);

                    // If there was an error...
                    if (!dataResult.Successful)
                        // Return
                        return dataResult;

                    // If there are data...
                    if (!dataResult.Result.IsNullOrEmpty())
                        // Add them to the destination database context
                        destinationDbContext.AddRange(dataResult.Result.Cast<object>());
                }

                // Save the changes
                await destinationDbContext.SaveChangesAsync();

                // Re-enable the foreign key checks of the destination database
                await destinationDbContext.EnableForeignKeyChecksAsync();

                return new Failable();
            }
            catch (Exception ex)
            {
                return new Failable() { ErrorMessage = ex.Message };
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates a unique type name
        /// </summary>
        /// <returns></returns>
        private static string GenerateUniqueTypeName() => Guid.NewGuid().ToString("N");

        /// <summary>
        /// Uses the specified <paramref name="type"/> to create the <see cref="Nullable{type}"/>
        /// when the <paramref name="type"/> is a value type, otherwise it returns the <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type whose nullable equivalent to get</param>
        /// <returns></returns>
        public static Type GetNullableType(Type type)
        {
            // Use Nullable.GetUnderlyingType() to remove the Nullable<T> wrapper if type is already nullable.
            type = Nullable.GetUnderlyingType(type) ?? type; // avoid type becoming null
            if (type.IsValueType)
                return typeof(Nullable<>).MakeGenericType(type);
            else
                return type;
        }

        #endregion
    }
}
