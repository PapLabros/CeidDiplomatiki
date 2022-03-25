using Atom.Core;
using Atom.Windows.Controls;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using static Atom.Core.Personalization;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The client data storage options page
    /// </summary>
    public class ClientDataStorageOptionsPage : BaseControl
    {
        #region Public Properties

        /// <summary>
        /// The existing database options
        /// </summary>
        public DatabaseProviderOptionsDataModel Options { get; }

        /// <summary>
        /// The directory path as well as the Json name of the file
        /// </summary>
        public string FileName { get; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// The grid that contains all the content
        /// </summary>
        protected Grid ContentGrid { get; private set; }

        /// <summary>
        /// The scroll viewer that contains the <see cref="ContentSection"/>
        /// </summary>
        protected ScrollViewer ContentScrollViewer { get; private set; }

        /// <summary>
        /// The section that contains the <see cref="OptionsContainer"/>
        /// </summary>
        protected Section<DatabaseOptionComponentsContainer> ContentSection { get; private set; }

        /// <summary>
        /// The options container
        /// </summary>
        protected DatabaseOptionComponentsContainer OptionsContainer { get; private set; }

        /// <summary>
        /// The save button
        /// </summary>
        protected IconButton SaveButton { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="fileName">The directory path as well as the json file name</param>
        /// <param name="directoryPath">The directory path of the default directory where the SQLite database is stored</param>
        /// <param name="databaseName">The default database name used when generating the options file</param>
        public ClientDataStorageOptionsPage(string fileName, string directoryPath, string databaseName) : base()
        {
            FileName = fileName.NotNullOrEmpty();

            // Get the options
            Options = DatabaseProviderHelpers.GetDatabaseProviderOptions(fileName, directoryPath, databaseName);

            OptionsContainer.Add(new SQLiteOptionsComponent(Options.SQLite));
            OptionsContainer.Add(new MySQLOptionsComponent(Options.MySQL));
            OptionsContainer.Add(new SQLServerOptionsComponent(Options.SQLServer));
            OptionsContainer.Add(new PostgreSQLOptionsComponent(Options.PostgreSQL));

            // Show the options of the selected provider
            Show(Options.Provider);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the selected database provider
        /// </summary>
        public SQLDatabaseProvider SelectedProvider => OptionsContainer.SelectedOptions.Provider;

        /// <summary>
        /// Shows the component that presents the options of the specified <paramref name="provider"/>
        /// </summary>
        /// <param name="provider">The provider</param>
        public void Show(SQLDatabaseProvider provider)
        {
            if (provider == SQLDatabaseProvider.SQLite)
                OptionsContainer.Show(Options.SQLite);
            else if (provider == SQLDatabaseProvider.SQLServer)
                OptionsContainer.Show(Options.SQLServer);
            else
                OptionsContainer.Show(Options.MySQL);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates and returns the GUI in a form of a <see cref="FrameworkElement"/>
        /// </summary>
        /// <returns></returns>
        protected override FrameworkElement CreateBaseGUIElement()
        {
            // Create the content grid
            ContentGrid = new Grid();

            // Create options container
            OptionsContainer = new DatabaseOptionComponentsContainer() { Margin = new Thickness(NormalUniformMargin) };

            // Create the content section
            ContentSection = ControlsFactory.WrapInSection(OptionsContainer, "Database provider");

            // Wrap it in a scroll viewer
            ContentScrollViewer = ControlsFactory.WrapInScrollViewer(ContentSection);

            ContentScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

            // Add the scroll viewer to the content grid
            ContentGrid.Children.Add(ContentScrollViewer);

            // Create the save button
            SaveButton = ControlsFactory.CreateStandardSaveCircularIconButton();

            SaveButton.Command = new RelayCommand(async () =>
            {
                // Disable the button
                SaveButton.IsEnabled = false;

                // Validate the data
                var isValid = OptionsContainer.Validate();

                // If the data aren't valid...
                if (!isValid)
                {
                    // Re-enable the button
                    SaveButton.IsEnabled = true;

                    // Show an error dialog
                    await DialogHelpers.ShowErrorHintDialogAsync(this, $"Invalid options!");

                    // Return
                    return;
                }

                // Keep the existing options
                var existingOptions = Options.DeepClone();

                // Update the options
                OptionsContainer.UpdateOptionData();

                // Set the selected provider option
                Options.Provider = SelectedProvider;

                // Save the changes
                var result = await SaveChangesAsync(Options, existingOptions, FileName);

                // If there was an error...
                if (!result.Successful)
                {
                    // Re-enable the button
                    SaveButton.IsEnabled = true;

                    // Show the error
                    await result.ShowDialogAsync(this);

                    // Return
                    return;
                }

                // Re-enable the button
                SaveButton.IsEnabled = true;

                // Show a success dialog
                await DialogHelpers.ShowChangesSavedHintDialogAsync(this);
            });

            // Add it to the content grid
            ContentGrid.Children.Add(SaveButton);

            // Return the content grid
            return ContentGrid;
        }

        /// <summary>
        /// Saves the changes
        /// </summary>
        /// <param name="newOptions">The new options</param>
        /// <param name="oldOptions">The old options</param>
        /// <param name="fileName">The file name</param>
        /// <returns></returns>
        protected virtual Task<IFailable> SaveChangesAsync(DatabaseProviderOptionsDataModel newOptions, DatabaseProviderOptionsDataModel oldOptions, string fileName)
        {
            DatabaseProviderHelpers.SaveDatabaseProviderOptions(newOptions, fileName);

            return Task.FromResult<IFailable>(new Failable());
        }

        #endregion
    }

    public static class ObjectExtensions
    {
        #region Private Members

        /// <summary>
        /// The <see cref="MethodInfo"/> that describes the <see cref="object.MemberwiseClone"/> method
        /// </summary>
        private static readonly MethodInfo mCloneMethod = typeof(object).GetMethod(nameof(MemberwiseClone), BindingFlags.NonPublic | BindingFlags.Instance);

        #endregion

        /// <summary>
        /// Creates and returns a deep clone of the specified <paramref name="obj"/>
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="obj">The object</param>
        /// <returns></returns>
        public static T DeepClone<T>(this T obj) => (T)DeepClone((object)obj);

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="type"/> is a primitive type.
        /// The primitive types include all the structs, the enums and the <see cref="string"/>.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns></returns>
        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(string))
                return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        /// <summary>
        /// One-line for each for arrays
        /// </summary>
        /// <param name="array">The source</param>
        /// <param name="action">The action to run</param>
        public static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.LongLength == 0) return;
            var walker = new ArrayTraverse(array);
            do action(array, walker.Position);
            while (walker.Step());
        }

        /// <summary>
        /// Deep clones the specified <paramref name="originalObject"/>
        /// </summary>
        /// <param name="originalObject">The original object</param>
        /// <returns></returns>
        private static object DeepClone(object originalObject)
            => InternalCopy(originalObject, new Dictionary<object, object>(ReferenceEqualityComparer.Instance));

        /// <summary>
        /// Copies the <paramref name="originalObject"/>
        /// </summary>
        /// <param name="originalObject">The original object</param>
        /// <param name="visited">The visited fields</param>
        /// <returns></returns>
        private static object InternalCopy(object originalObject, IDictionary<object, object> visited)
        {
            // If the original object is null...
            if (originalObject == null)
                return null;

            // Get the type of the object
            var typeToReflect = originalObject.GetType();

            // If the type is primitive...
            if (typeToReflect.IsPrimitive())
                // Return the original object
                return originalObject;

            // If the object is already copied...
            if (visited.ContainsKey(originalObject))
                // Return the cashed object
                return visited[originalObject];

            // If the object type is a delegate...
            if (typeof(Delegate).IsAssignableFrom(typeToReflect))
                // Return null
                return null;

            // Invoke the cloning method
            var cloneObject = mCloneMethod.Invoke(originalObject, null);

            // If the object type is an array...
            if (typeToReflect.IsArray)
            {
                // Get the element type
                var arrayType = typeToReflect.GetElementType();

                // If the element type is not primitive...
                if (!arrayType.IsPrimitive())
                {
                    // Get the cloned array
                    var clonedArray = (Array)cloneObject;

                    // Copy each element
                    clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }
            }

            // Add the cloned object
            visited.Add(originalObject, cloneObject);

            // Copy the object fields
            CopyFields(originalObject, visited, cloneObject, typeToReflect);

            // Copy the current type private fields
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);

            // Return the cloned object
            return cloneObject;
        }

        /// <summary>
        /// Copies the private fields of the <paramref name="originalObject"/> to the <paramref name="cloneObject"/>
        /// </summary>
        /// <param name="originalObject">The original object</param>
        /// <param name="visited">The visited objects</param>
        /// <param name="cloneObject">The cloned object</param>
        /// <param name="typeToReflect">The type of the object</param>
        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            // If there is a base type...
            if (typeToReflect.BaseType != null)
            {
                // Copy the base type private fields recursively
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);

                // Copy the current type private fields
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        /// <summary>
        /// Copies the fields of the <paramref name="originalObject"/> to the <paramref name="cloneObject"/>, using the <paramref name="filter"/> to filter the fields
        /// </summary>
        /// <param name="originalObject">The original object</param>
        /// <param name="visited">The visited objects</param>
        /// <param name="cloneObject">The cloned object</param>
        /// <param name="typeToReflect">The type of the object</param>
        /// <param name="bindingFlags">The binding flags</param>
        /// <param name="filter">The method for ignoring a field</param>
        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            // For every field...
            foreach (var fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                // If the field has to be excluded by the filter...
                if (filter != null && filter(fieldInfo) == false)
                    continue;

                // If the field type is primitive...
                if (fieldInfo.FieldType.IsPrimitive())
                    continue;

                // Get the field value
                var originalFieldValue = fieldInfo.GetValue(originalObject);

                // Copy the filed value
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);

                // Set the copied field value
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }

        #region Private Classes

        private class ArrayTraverse
        {
            public int[] Position;
            private readonly int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (var i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (var i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (var j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion
    }

    /// <summary>
    /// An <see cref="EqualityComparer{T}"/> that provides references based comparison between two objects
    /// </summary>
    public class ReferenceEqualityComparer : EqualityComparer<object>
    {
        #region Public Properties

        /// <summary>
        /// The single instance of the <see cref="ReferenceEqualityComparer"/>
        /// </summary>
        public static ReferenceEqualityComparer Instance { get; } = new ReferenceEqualityComparer();

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        protected ReferenceEqualityComparer() : base()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// When overridden in a derived class, determines whether two objects of type T
        /// are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns></returns>
        public override bool Equals(object x, object y) => ReferenceEquals(x, y);

        /// <summary>
        /// When overridden in a derived class, serves as a hash function for the specified
        /// object for hashing algorithms and data structures, such as a hash table.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int GetHashCode(object obj) => obj.GetHashCode();

        #endregion
    }

    /// <summary>
    /// The default relational services that should be available anywhere in the app
    /// </summary>
    public static class DatabaseProviderHelpers
    {
        /// <summary>
        /// Saves the specified <paramref name="options"/>
        /// </summary>
        /// <param name="options">The options</param>
        /// <param name="fileName">The directory path as well as the json file name</param>
        public static void SaveDatabaseProviderOptions(DatabaseProviderOptionsDataModel options, string fileName)
        {
            // Serialize it
            var json = JsonConvert.SerializeObject(options);

            // Save it
            WriteTextToFile(json, fileName);
        }

        /// <summary>
        /// Gets the database provider options.
        /// NOTE: If the options haven't yet been initialized, then they get initialized using the default values!
        /// </summary>
        /// <param name="fileName">The directory path as well as the json file name</param>
        /// <param name="directoryPath">The directory path of the default directory where the SQLite database is stored</param>
        /// <param name="databaseName">The default database name</param>
        /// <returns></returns>
        public static DatabaseProviderOptionsDataModel GetDatabaseProviderOptions(string fileName, string directoryPath, string databaseName)
        {
            // Declare the options
            DatabaseProviderOptionsDataModel options;

            // If the file doesn't exist...
            if (!File.Exists(fileName))
            {
                // Create a new instance
                options = new DatabaseProviderOptionsDataModel()
                {
                    Provider = SQLDatabaseProvider.SQLite,
                    SQLite = new SQLiteOptionsDataModel()
                    {
                        DirectoryPath = directoryPath,
                        DatabaseName = databaseName
                    },
                    MySQL = new MySQLOptionsDataModel(),
                    SQLServer = new SQLServerOptionsDataModel(),
                };

                // Serialize it
                var json = JsonConvert.SerializeObject(options);

                // Save it
                WriteTextToFile(json, fileName);
            }
            // Else
            else
            {
                // Read the file
                var json = File.ReadAllText(fileName);

                // Parse the file and get the options
                options = JsonConvert.DeserializeObject<DatabaseProviderOptionsDataModel>(json);
            }

            // Return the options
            return options;
        }

        /// <summary>
        /// Writes the text to the specified file
        /// </summary>
        /// <param name="text">The text to write</param>
        /// <param name="path">The path of the file to write to</param>
        /// <param name="append">If true, writes the text to the end of the file, otherwise overrides any existing file</param>
        /// <returns></returns>
        public static void WriteTextToFile(string text, string path, bool append = false)
        {
            WriteTextToFile(text, path, Encoding.UTF8, append);
        }

        /// <summary>
        /// Writes the text to the specified file
        /// </summary>
        /// <param name="text">The text to write</param>
        /// <param name="path">The path of the file to write to</param>
        /// <param name="encoding">The encoding</param>
        /// <param name="append">If true, writes the text to the end of the file, otherwise overrides any existing file</param>
        /// <returns></returns>
        public static void WriteTextToFile(string text, string path, Encoding encoding, bool append = false)
        {
            // Normalize path
            path = NormalizePath(path);

            // Resolve to absolute path
            path = ResolvePath(path);

            // Write the log message to file
            using (var fileStream = (TextWriter)new StreamWriter(File.Open(path, append ? FileMode.Append : FileMode.Create), encoding))
                fileStream.Write(text);
        }

        /// <summary>
        /// Resolves any relative elements of the path to absolute
        /// </summary>
        /// <param name="path">The path to resolve</param>
        /// <returns></returns>
        public static string ResolvePath(string path)
        {
            // Resolve the path to absolute
            return Path.GetFullPath(path);
        }

        /// <summary>
        /// Normalizing a path based on the current operating system
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <returns></returns>
        public static string NormalizePath(string path)
        {
            // If on Windows...
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                // Replace any / with \
                return path?.Replace('/', '\\').Trim();
            // If on Linux/Mac
            else
                // Replace any \ with /
                return path?.Replace('\\', '/').Trim();
        }
    }
}
