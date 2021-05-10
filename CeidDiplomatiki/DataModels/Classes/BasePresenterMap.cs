using System;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The base for all the presenter maps
    /// </summary>
    public abstract class BasePresenterMap
    {
        #region Public Properties

        /// <summary>
        /// The query map related to the data presenter
        /// </summary>
        public QueryMap QueryMap { get; }

        /// <summary>
        /// The id
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

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
        /// <param name="queryMap">The query map</param>
        public BasePresenterMap(QueryMap queryMap) : base()
        {
            QueryMap = queryMap ?? throw new System.ArgumentNullException(nameof(queryMap));
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
