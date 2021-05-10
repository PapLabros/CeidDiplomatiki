using Atom.Windows.Controls;
using System;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The base for all the presenter pages
    /// </summary>
    public abstract class BasePresenterPage : BaseInitializablePage
    {
        #region Public Properties

        /// <summary>
        /// The query map
        /// </summary>
        public QueryMap QueryMap => PresenterMap.QueryMap;

        /// <summary>
        /// The presenter map
        /// </summary>
        public BasePresenterMap PresenterMap { get; }

        /// <summary>
        /// The page map related to this page
        /// </summary>
        public PageMap PageMap { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="pageMap">The page map</param>
        /// <param name="presenterMap">The presenter map</param>
        public BasePresenterPage(BasePresenterMap presenterMap, PageMap pageMap) : base()
        {
            PresenterMap = presenterMap ?? throw new ArgumentNullException(nameof(presenterMap));
            PageMap = pageMap ?? throw new ArgumentNullException(nameof(pageMap));
        }

        #endregion
    }

    /// <summary>
    /// A <see cref="BasePresenterPage"/> that provides type safety for the type of the presenter map
    /// </summary>
    /// <typeparam name="TPresenterMap"></typeparam>
    public abstract class BasePresenterPage<TPresenterMap> : BasePresenterPage
        where TPresenterMap : BasePresenterMap
    {
        #region Public Properties

        /// <summary>
        /// The presenter map
        /// </summary>
        public new TPresenterMap PresenterMap => (TPresenterMap)base.PresenterMap;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="pageMap">The page map</param>
        /// <param name="presenterMap">The presenter map</param>
        public BasePresenterPage(TPresenterMap presenterMap, PageMap pageMap) : base(presenterMap, pageMap)
        {
        }

        #endregion
    }
}
