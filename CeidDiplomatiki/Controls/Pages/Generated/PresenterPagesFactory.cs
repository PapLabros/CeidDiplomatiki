using Atom.Windows.Controls;
using System;
using System.Windows;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Creates and returns presenter pages
    /// </summary>
    public static class PresenterPagesFactory
    {
        /// <summary>
        /// Creates and returns a presenter page that represents the specified <paramref name="presenterMap"/>.
        /// </summary>
        /// <param name="presenterMap">The presenter map</param>
        /// <param name="pageMap">The page map</param>
        /// <returns></returns>
        public static FrameworkElement CreatePresenterPage(BasePresenterMap presenterMap, PageMap pageMap)
        {
            return new DataGridPresenterPage(presenterMap as DataGridPresenterMap, pageMap);
        }
    }
}
