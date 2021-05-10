using Atom.Core;

using System;
using System.Collections.Generic;
using System.Linq;

using static Atom.Personalization.Constants;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Maps information related to a page and its sub pages
    /// </summary>
    public class PageMap : IIdentifiable<string>, INameable
    {
        #region Public Properties

        /// <summary>
        /// The id
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The name of the page
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The icon of the page
        /// </summary>
        public string PathData { get; set; } = IconPaths.GridPath;

        /// <summary>
        /// The color of the page
        /// </summary>
        public string Color { get; set; } = White;

        /// <summary>
        /// The name of the category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The order
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The presenter that is used.
        /// NOTE: If the <see cref="Pages"/> are set then the <see cref="Presenter"/> gets overridden!
        /// </summary>
        public BasePresenterMap Presenter { get; set; }

        /// <summary>
        /// The parent page if any
        /// </summary>
        public PageMap Parent { get; set; }

        /// <summary>
        /// A flag indicating whether this map is a root
        /// </summary>
        public bool IsRoot => Parent != null;

        /// <summary>
        /// The sub pages.
        /// NOTE: If the <see cref="Pages"/> are set then the <see cref="Presenter"/> gets overridden!
        /// </summary>
        public List<PageMap> Pages { get; private set; } = new List<PageMap>();

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public PageMap() : base()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;

        /// <summary>
        /// Creates and returns a <see cref="PageMap"/> from the specified <paramref name="dataModel"/>
        /// </summary>
        /// <param name="dataModel">The data model</param>
        /// <param name="parent">The parent page</param>
        /// <returns></returns>
        public static PageMap FromDataModel(PageMapDataModel dataModel, PageMap parent)
        {
            // Create the map
            var pageMap = new PageMap()
            {
                Id = dataModel.Id,
                Color = dataModel.Color,
                PathData = dataModel.PathData,
                Name = dataModel.Name,
                Description = dataModel.Description,
                Category = dataModel.Category,
                Order = dataModel.Order,
                Parent = parent,
                Presenter = CeidDiplomatikiDI.GetCeidDiplomatikiManager.Presenters.FirstOrDefault(x => x.Id == dataModel.PresenterId)
            };

            // If there are pages...
            if (!dataModel.Pages.IsNullOrEmpty())
                // Set them
                pageMap.Pages = dataModel.Pages.Select(x => FromDataModel(x, pageMap)).ToList();

            // Return the page map
            return pageMap;
        }

        /// <summary>
        /// Creates and returns a <see cref="PageMapDataModel"/> from the current <see cref="PageMap"/>
        /// </summary>
        /// <returns></returns>
        public PageMapDataModel ToDataModel() => new PageMapDataModel()
        {
            Id = Id,
            Name = Name,
            Description = Description,
            Color = Color,
            Category = Category,
            Order = Order,
            PathData = PathData,
            PresenterId = Presenter?.Id,
            Pages = Pages.Select(x => x.ToDataModel()).ToArray(),
        };

        #endregion
    }

    /// <summary>
    /// Represents a <see cref="PageMap"/>
    /// </summary>
    public class PageMapDataModel
    {
        #region Public Properties

        public string Id { get; set; }

        /// <summary>
        /// The name of the page
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The icon of the page
        /// </summary>
        public string PathData { get; set; } = IconPaths.GridPath;

        /// <summary>
        /// The color of the page
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// The name of the category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The order
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The id of the presenter that is used.
        /// </summary>
        public string PresenterId { get; set; }

        /// <summary>
        /// The sub pages.
        /// </summary>
        public PageMapDataModel[] Pages { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public PageMapDataModel() : base()
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
