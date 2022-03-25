using Atom.Communications;
using Atom.Core;
using Atom.Windows.PlugIns.Communications;
using Atom.Windows.PlugIns.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Helper methods related to plug ins
    /// </summary>
    public static class PlugInHelpers
    {
        #region Data

        /// <summary>
        /// Calls the <see cref="IExportPlugIn.ExportAsync"/> method
        /// </summary>
        /// <typeparam name="TClass">The type of the models</typeparam>
        /// <typeparam name="TArgs">The type of the arguments</typeparam>
        /// <param name="element">The element used for determining the parent of the dialogs</param>
        /// <param name="plugIn">The export plug in</param>
        /// <param name="mapper">The mapper</param>
        /// <param name="dataStorage">The data storage</param>
        /// <param name="args">The args used by the <paramref name="dataStorage"/></param>
        /// <returns></returns>
        public static async Task<IFailable> CallDataPlugInExportMethodAsync<TClass, TArgs>(IExportPlugIn plugIn, UIElement element, PropertyMapper<TClass> mapper, BasePresenterDataStorage<TArgs> dataStorage, TArgs args)
           where TClass : class
        {
            return await plugIn.ExportAsync(element, mapper, Enumerable.Empty<PropertyInfo>(), async () =>
            {
                return await dataStorage.GetDataAsync<TClass>(args);
            }, null, null);
        }

        /// <summary>
        /// Calls the <see cref="IImportPlugIn.ImportAsync"/> method
        /// </summary>
        /// <typeparam name="TClass">The type of the models</typeparam>
        /// <param name="plugIn">The plug in</param>
        /// <param name="element">The element used for determining the parent of the dialogs</param>
        /// <param name="mapper">The mapper</param>
        /// <returns></returns>
        public static async Task<IFailable<IEnumerable<TClass>>> CallDataPlugInImportMethodAsync<TClass>(IImportPlugIn plugIn, UIElement element, PropertyMapper<TClass> mapper)
            where TClass : class
        {
            return await plugIn.ImportAsync(element, () => (TClass)Activator.CreateInstance(typeof(TClass)), mapper);
        }

        #endregion

        #region Communications

        /// <summary>
        /// Calls the <see cref="ICommunicationPlugIn.RegisterPointAsync"/> method
        /// </summary>
        /// <typeparam name="TClass">The type of the models</typeparam>
        /// <param name="plugIn">The plug in</param>
        /// <returns></returns>
        public static async Task CallCommunicationPlugInRegisterPointMethodAsync<TClass>(ICommunicationPlugIn plugIn)
            where TClass : class
        {
            await plugIn.RegisterPointAsync<TClass>(typeof(TClass).Name, (model) =>
            {
                return new CommunicationHistoryDataModel();
            }, null, null, null);
        }

        /// <summary>
        /// Calls the <see cref="CommunicationPoint{TDataModel}.SendMessageAsync"/> method
        /// </summary>
        /// <typeparam name="TClass">The type of the models</typeparam>
        /// <param name="plugIn">The plug in</param>
        /// <param name="element">The element used for determining the parent of the dialog</param>
        /// <param name="receiver">The receiver</param>
        /// <param name="model">The model</param>
        /// <param name="fullName">The full name of the receiver</param>
        /// <returns></returns>
        public static async Task<IFailable> CallCommunicationPlugInSendMessageMethodAsync<TClass>(ICommunicationPlugIn plugIn, UIElement element, string receiver,TClass model, string fullName)
            where TClass : class
        {
            // Get the point
            var point = plugIn.GetPoint<TClass>(typeof(TClass).Name);

            // Localization
            return await point.SendMessageAsync(element, new List<MessageDataModel<TClass>>() { new MessageDataModel<TClass>(model, receiver, fullName) }, typeof(TClass).Name, null);
        }

        #endregion
    }
}
