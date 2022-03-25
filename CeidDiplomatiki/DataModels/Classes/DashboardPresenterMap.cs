using System;
using System.Collections.Generic;
using System.Reflection;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Maps information related to a dashboard presenter
    /// </summary>
    public class DashboardPresenterMap : BasePresenterMap
    {
        #region Private Members

        /// <summary>
        /// The member of the <see cref="ChartMaps"/> property
        /// </summary>
        private readonly List<ChartMap> mChartMaps = new List<ChartMap>();

        #endregion

        #region Public Properties

        /// <summary>
        /// The chart maps
        /// </summary>
        public IEnumerable<ChartMap> ChartMaps { get => mChartMaps; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="queryMap">The query map</param>
        public DashboardPresenterMap(QueryMap queryMap) : base(queryMap)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the specified <paramref name="chartMap"/>
        /// </summary>
        /// <param name="chartMap">The chart map</param>
        public void Add(ChartMap chartMap)
        {
            mChartMaps.Add(chartMap);
        }

        #endregion
    }

    public class ChartMap
    {
        #region Public Properties

        /// <summary>
        /// The presenter map
        /// </summary>
        public DashboardPresenterMap PresenterMap { get; }

        /// <summary>
        /// The property that the group by operation is applied to
        /// </summary>
        public PropertyInfo GroupByProperty { get; set; }

        public bool IsCount => CountProperty != null;

        public PropertyInfo CountProperty { get; set; }

        public bool IsSum => SumProperty != null;

        public PropertyInfo SumProperty { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="presenterMap">The presenter map</param>
        public ChartMap(DashboardPresenterMap presenterMap) : base()
        {
            PresenterMap = presenterMap ?? throw new ArgumentNullException(nameof(presenterMap));
        }

        #endregion
    }
}
