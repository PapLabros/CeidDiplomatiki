using Atom.Communications;
using Atom.Core;
using Atom.Core.Controls.Calendar;
using Atom.Windows.Controls;
using Atom.Windows.Controls.Calendar;
using Atom.Windows.PlugIns.Communications;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using static Atom.Core.Personalization;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The calendar presenter page
    /// </summary>
    public class CalendarPresenterPage : BasePresenterPage<CalendarPresenterMap>
    {
        #region Public Properties
        
        /// <summary>
        /// The data storage
        /// </summary>
        public CalendarPresenterDataStorage DataStorage { get; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// The calendar
        /// </summary>
        protected Calendar Calendar { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="calendarPresenterMap">The presenter map</param>
        /// <param name="pageMap">The page map related to this page</param>
        public CalendarPresenterPage(CalendarPresenterMap calendarPresenterMap, PageMap pageMap) : base(calendarPresenterMap, pageMap)
        {
            DataStorage = new CalendarPresenterDataStorage(calendarPresenterMap);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initializes the page
        /// </summary>
        /// <returns></returns>
        protected override Task<IFailable> InitializeAsync() => Task.FromResult<IFailable>(new Failable());

        /// <summary>
        /// Handles the successful initialization. 
        /// NOTE: Usually used for creating the UI elements after the initialization succeeded!
        /// </summary>
        /// <returns></returns>
        protected override Task OnSuccessfulInitialization()
        {
            // Create the calendar
            Calendar = new Calendar();

            Calendar.AddPresenter(new HourBasedCalendarPresenter("Hours", Calendar));
            Calendar.AddPresenter(new DayBasedCalendarPresenter("Days", Calendar));
            Calendar.AddPresenter(new ScheduleBasedCalendarPresenter("Schedule", Calendar));

            // Add it to the content grid
            ContentGrid.Children.Add(Calendar);

            // Add the calendar group
            Calendar.Add<CalendarPresenterEvent>(
                new SubCalendarGroup<CalendarPresenterEvent>(
                    subCalendars: new List<SubCalendar>()
                    {
                        new SubCalendar(PresenterMap.Id, "Calendars", PresenterMap.Name, PresenterMap.Color)
                    },
                    eventsRetrieverAsync: async (calendar, calendarGroup, subCalendars, daySpan) =>
                    {
                        // Get the data
                        var result = await DataStorage.GetDataAsync(new CalendarPresenterArgs() { After = daySpan.StartingDate, Before = daySpan.EndingDate });

                        // If there was an error..
                        if (!result.Successful)
                            // Return failed
                            return new Failable<IDictionary<SubCalendar, List<CalendarPresenterEvent>>>() { ErrorMessage = result.ErrorMessage };

                        // Create the events
                        var events = result.Result.SelectBase<CalendarPresenterEvent>(model =>
                        {
                            // Get the title
                            var title = QueryMap.ReplaceFormula(PresenterMap.TitleFormula, model);

                            // Get the description
                            var description = PresenterMap.DescriptionFormula.IsNullOrEmpty() ? null : QueryMap.ReplaceFormula(PresenterMap.DescriptionFormula, model);

                            // Get the date start value
                            var dateStartValue = (DateTime)PresenterMap.DateStartColumn.GetValue(model);

                            return new CalendarPresenterEvent(dateStartValue, model)
                            {
                                Name = title,
                                Description = description,
                                Color = PresenterMap.Color,
                                IsEditable = PresenterMap.AllowEdit
                            };
                        }).ToList();

                        // Return OK
                        return new Failable<IDictionary<SubCalendar, List<CalendarPresenterEvent>>>() 
                        { 
                            Result = new Dictionary<SubCalendar, List<CalendarPresenterEvent>>() 
                            { 
                                { subCalendars.First(), events } 
                            } 
                        };
                    },
                    eventContentElementFactory: (calendar, subCalendarGroup, subCalendar, calendarEvent) => 
                    {
                        return new CalendarPresenterEventContentElement(Calendar, PresenterMap, DataStorage, calendarEvent);
                    },
                    eventPreviewElementFactory: null,
                    subCalendarOptionsAction: null,
                    subCalendarContextMenuFactory: null,
                    eventContextMenuFactory: (calendar, subCalendarGroup, subCalendar, calendarEvent) => 
                    {
                        // Create the context menu
                        var contextMenu = new StandardContextMenu();

                        // If edit is allowed...
                        if (PresenterMap.AllowEdit)
                        {
                            // Add the edit option
                            contextMenu.AddEditOption(new RelayCommand(async () =>
                            {
                                // Create the form type
                                var formType = typeof(DataForm<>).MakeGenericType(QueryMap.RootType);

                                // Create the form
                                var form = (IDataForm)Activator.CreateInstance(formType);

                                form.Model = calendarEvent.Instance;

                                // For every property map related to this data grid...
                                foreach (var propertyMap in QueryMap.PropertyMaps.Where(x => x.Type == QueryMap.RootType && (x.IsEditable || x.IsPreview)).OrderBy(x => x.Order))
                                {
                                    if (propertyMap.Attributes.Any(x => x.Equals(ColumnAttributes.HEXColor)))
                                    {
                                        // Show a string color input
                                        form.UnsafeShowCustomFormInput(propertyMap.PropertyInfo, (form, propertyInfo) => new StringColorFormInput(form, propertyInfo), false, propertyMap.Name, propertyMap.IsRequired, propertyMap.Description, propertyMap.IsPreview, null, propertyMap.Color);

                                        // Continue
                                        continue;
                                    }

                                    // Show a standard input
                                    form.UnsafeShowInput(propertyMap.PropertyInfo, propertyMap.Name, propertyMap.IsRequired, propertyMap.Description, propertyMap.IsPreview, null, propertyMap.Color);
                                }

                                // Show a dialog
                                var dialogResult = await DialogHelpers.ShowValidationDialogAsync(this, "Item modification", null, form.Element, element => form.Validate(), PageMap.PathData, overlay => overlay.PositiveFeedbackText = "Update");

                                // If we didn't get positive feedback...
                                if (!dialogResult.Feedback)
                                    // Return
                                    return;

                                // Update the values of the model
                                form.UpdateModelValues();

                                // Update the item
                                var result = await DialogHelpers.ShowUpdateLoadingHintDialogAsync(this, () => DataStorage.UpdateDataAsync(form.Model));

                                // If there was an error...
                                if (!result.Successful)
                                {
                                    // Show the error
                                    await result.ShowDialogAsync(this);

                                    // Return
                                    return;
                                }

                                // Set the title
                                calendarEvent.Name = QueryMap.ReplaceFormula(PresenterMap.TitleFormula, calendarEvent.Instance);

                                // Set the description
                                calendarEvent.Description = PresenterMap.DescriptionFormula.IsNullOrEmpty() ? null : QueryMap.ReplaceFormula(PresenterMap.DescriptionFormula, calendarEvent.Instance);

                                // Update the calendar
                                calendar.Update(calendarEvent);

                                // Show a success dialog
                                await DialogHelpers.ShowChangesSavedHintDialogAsync(this);
                            }));
                        }

                        // If both edit and delete is supported...
                        if (PresenterMap.AllowEdit && PresenterMap.AllowDelete)
                            // Add a separator
                            contextMenu.AddSeparator();

                        // If delete is a allowed...
                        if (PresenterMap.AllowDelete)
                        {
                            // Add the delete option
                            contextMenu.AddDeleteOption(new RelayCommand(async () =>
                            {
                                // Show a delete dialog
                                var dialogResult = await DialogHelpers.ShowDeletionTransitionalDialogAsync(this, "Item deletion", null, IconPaths.TruckFastPath);

                                // If we got a negative feed back...
                                if (!dialogResult.Feedback)
                                    // Return
                                    return;

                                // Call the server
                                var response = await DataStorage.DeleteDataAsync(calendarEvent.Instance);

                                // If we failed...
                                if (!response.Successful)
                                {
                                    // Show the error to the user
                                    await response.ShowDialogAsync(this);

                                    // Return
                                    return;
                                }

                                // Remove it from the calendar
                                Calendar.Remove(calendarEvent);

                                // Invalidate
                                Calendar.Invalidate();
                            }));
                        }

                        // Return it
                        return contextMenu;
                    },
                    searchRule: (searchValue, calendarEvent) => 
                    {
                        // For every search column...
                        foreach(var searchColumn in PresenterMap.SearchColumns)
                        {
                            // Get the value
                            var value = searchColumn.GetValue(calendarEvent.Instance).ToPotentialNullString();

                            // If there isn't a value...
                            if (value.IsNullOrEmpty())
                                // Continue
                                continue;

                            // If the search value is contained in the value...
                            if (value.ToLower().Contains(searchValue.ToLower()))
                                // Return
                                return true;
                        }

                        return false;
                    }));

            return Task.CompletedTask;
        }

        #endregion
    }

    /// <summary>
    /// A <see cref="CalendarEvent"/> used by the <see cref="CalendarPresenterPage"/>
    /// </summary>
    public class CalendarPresenterEvent : CalendarEvent
    {
        #region Public Properties

        /// <summary>
        /// The instance of the model that the event represents
        /// </summary>
        public object Instance { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="initDateTime">The initialization date time</param>
        /// <param name="instance">The instance of the model that the event represents</param>
        public CalendarPresenterEvent(DateTime initDateTime, object instance) : base(initDateTime)
        {
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        #endregion
    }

    /// <summary>
    /// A <see cref="StandardEventPreviewElement"/> used for presenting a <see cref="CalendarPresenterEvent"/>
    /// </summary>
    public class CalendarPresenterEventContentElement : StandardEventPreviewElement
    {
        #region Public Properties

        /// <summary>
        /// The calendar
        /// </summary>
        public ICalendar Calendar { get; }

        /// <summary>
        /// The calendar presenter map
        /// </summary>
        public CalendarPresenterMap CalendarPresenterMap { get; }

        /// <summary>
        /// The query map
        /// </summary>
        public QueryMap QueryMap => CalendarPresenterMap.QueryMap;

        /// <summary>
        /// The data storage
        /// </summary>
        public CalendarPresenterDataStorage DataStorage { get; }

        /// <summary>
        /// The event this preview element represents
        /// </summary>
        public new CalendarPresenterEvent Event => (CalendarPresenterEvent)base.Event;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="calendar">The calendar</param>
        /// <param name="calendarPresenterMap">The calendar presenter map</param>
        /// <param name="dataStorage">The data storage</param>
        /// <param name="calendarEvent">The event this preview element represents</param>
        public CalendarPresenterEventContentElement(ICalendar calendar, CalendarPresenterMap calendarPresenterMap, CalendarPresenterDataStorage dataStorage, CalendarPresenterEvent calendarEvent) : base(calendarEvent)
        {
            Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));
            CalendarPresenterMap = calendarPresenterMap ?? throw new ArgumentNullException(nameof(calendarPresenterMap));
            DataStorage = dataStorage ?? throw new ArgumentNullException(nameof(dataStorage));
            CreateGUI();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates and adds the required GUI elements
        /// </summary>
        private void CreateGUI()
        {
            // If deletion is allowed...
            if (CalendarPresenterMap.AllowDelete)
            {
                // Add the delete option
                Add(IconPaths.DeletePath, Red, "Delete", async (button, calendarEvent) =>
                {
                    // Show a delete dialog
                    var dialogResult = await DialogHelpers.ShowDeletionTransitionalDialogAsync(this, "Item deletion", null, IconPaths.TruckFastPath);

                    // If we got a negative feed back...
                    if (!dialogResult.Feedback)
                        // Return
                        return;

                    // Call the server
                    var response = await DataStorage.DeleteDataAsync(Event.Instance);

                    // If we failed...
                    if (!response.Successful)
                    {
                        // Show the error to the user
                        await response.ShowDialogAsync(this);

                        // Return
                        return;
                    }

                    // Remove it from the calendar
                    Calendar.Remove(calendarEvent);

                    // Invalidate
                    Calendar.Invalidate();
                });
            }

            // Get the phone number property map if any
            var phoneNumberPropertyMap = QueryMap.PropertyMaps.FirstOrDefault(x => x.Attributes.Any(y => y.Equals(ColumnAttributes.PhoneNumber)));

            // Get the email property map if any
            var emailPropertyMap = QueryMap.PropertyMaps.FirstOrDefault(x => x.Attributes.Any(y => y.Equals(ColumnAttributes.Email)));

            // Get the first name property map if any
            var firstNamePropertyMap = QueryMap.PropertyMaps.FirstOrDefault(x => x.Attributes.Any(y => y.Equals(ColumnAttributes.FirstName)));

            // Get the last name property map if any
            var lastNamePropertyMap = QueryMap.PropertyMaps.FirstOrDefault(x => x.Attributes.Any(y => y.Equals(ColumnAttributes.LastName)));

            #region Communication Plug Ins

            // For every communication plug in...
            foreach (var communicationPlugIn in DI.GetServices<ICommunicationPlugIn>())
            {
                // The property map that will be used
                var propertyMap = default(PropertyMap);

                // If this is an SMS plug in...
                if (communicationPlugIn.CommunicationMean == CommunicationMean.SMS)
                {
                    // If there isn't a phone number property map...
                    if (phoneNumberPropertyMap == null)
                        // Continue
                        continue;

                    // Set the property map
                    propertyMap = phoneNumberPropertyMap;
                }
                // If this is an Email plug in
                else if (communicationPlugIn.CommunicationMean == CommunicationMean.Email)
                {
                    // If there isn't an email property map...
                    if (emailPropertyMap == null)
                        // Continue
                        continue;

                    // Set the property map
                    propertyMap = emailPropertyMap;
                }

                // Add an option
                Add(communicationPlugIn.PathData, communicationPlugIn.Color, communicationPlugIn.Name, async (button, calendarEvent) =>
                {
                    // Disable the button
                    button.IsEnabled = false;

                    // If the plug in isn't configured...
                    if (!communicationPlugIn.HasRegisteredPoint(Event.Instance.GetType().Name))
                    {
                        // Show the error
                        await DialogHelpers.ShowErrorDialogAsync(this, $"A communication point for the order model: {Event.Instance.GetType().Name}, hasn't be set!");

                        // Re-enable the button
                        button.IsEnabled = true;

                        // Return
                        return;
                    }

                    // Get the receiver
                    var receiverString = propertyMap.PropertyInfo.GetValue(Event.Instance).ToPotentialNullString();

                    // Check if the receiver is valid...
                    var isValidReceiver = communicationPlugIn.ValidateReceiver(receiverString, out var error);

                    // If the receiver isn't valid...
                    if (!isValidReceiver)
                    {
                        // Show an error dialog
                        await DialogHelpers.ShowErrorDialogAsync(this, error);

                        // Re-enable the button
                        button.IsEnabled = true;

                        // Return
                        return;
                    }

                    // Get the export method
                    var method = typeof(PlugInHelpers).GetMethod(nameof(PlugInHelpers.CallCommunicationPlugInSendMessageMethodAsync), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).MakeGenericMethod(QueryMap.RootType);

                    // Call the export method
                    var task = (Task)method.Invoke(this, new object[] { communicationPlugIn, this, receiverString, Event.Instance, $"{firstNamePropertyMap?.PropertyInfo.GetValue(Event.Instance)} {lastNamePropertyMap?.PropertyInfo.GetValue(Event.Instance)}" });
                    await task;

                    // Get the result property
                    var resultProperty = task.GetType().GetProperty(nameof(Task<object>.Result));

                    // Get the result
                    var result = (IFailable)resultProperty.GetValue(task);

                    // If there was an error...
                    if (!result.Successful)
                    {
                        // Show the error
                        await result.ShowDialogAsync(this);

                        // Re-enable the button
                        button.IsEnabled = true;

                        // Return
                        return;
                    }

                    // Re-enable the button
                    button.IsEnabled = true;

                    // Show a success dialog
                    await DialogHelpers.ShowSuccessHintDialogAsync(this, "Message sent!");
                });
            }

            #endregion
        }

        #endregion
    }
}
