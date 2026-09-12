using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.Storage.Pickers;
using MilLeadershipBoard.Config;
using MilLeadershipBoard.Resources;
using MilLeadershipBoard.UI.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MilLeadershipBoard.UI.ViewModels
{
    /// <summary>
    /// ViewModel class used for all add pages for schedule images.
    /// </summary>
    public class AddSchedulePageViewModel : INotifyPropertyChanged, IDisposable
    {
        //   ---   Private Constants   ---

        /// <summary>
        /// ERROR_WRONG_PASSWORD HRESULT constant.
        /// </summary>
        private const int ERROR_WRONG_PASSWORD = unchecked((int)0x8007052B);

        /// <summary>
        /// E_FAIL HRESULT constant.
        /// </summary>
        const int GENERIC_FAIL = unchecked((int)0x80004005);

        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the <see cref="RelayCommand"/> instance used for the <see cref="BrowseScheduleFileCommand"/> property.
        /// </summary>
        private RelayCommand _browseScheduleFileCommand;

        /// <summary>
        /// Field containing the value of the <see cref="CanAdd"/> property.
        /// </summary>
        private bool _canAdd = false;

        /// <summary>
        /// Field containing the value of the <see cref="DateFormatter"/> property.
        /// </summary>
        private Func<DateOnly, DateOnly> _dateFormatter = (d) => d;

        /// <summary>
        /// Field containing the value of the <see cref="FileStatusMessageText"/> property.
        /// </summary>
        private string _fileStatusMessageText = string.Empty;

        /// <summary>
        /// Field containing the value of the <see cref="IsPdfRendering"/> property.
        /// </summary>
        private bool _isPdfRendering = false;

        /// <summary>
        /// Field containing the value of the <see cref="PdfOptionsPanelVisibility"/> property.
        /// </summary>
        private Visibility _pdfOptionsPanelVisibility = Visibility.Collapsed;

        /// <summary>
        /// Field containing the value of the <see cref="PdfPageCount"/> property.
        /// </summary>
        private int _pdfPageCount = 1;

        /// <summary>
        /// Field containing the value of the <see cref="ScheduleDateTimeOffset"/> property.
        /// </summary>
        private DateTimeOffset _scheduleDateTimeOffset = DateTimeOffset.Now;

        /// <summary>
        /// Field containing the value of the <see cref="SchedulePath"/> property.
        /// </summary>
        private string _schedulePath = string.Empty;

        /// <summary>
        /// Field containing the value of the <see cref="SelectedPdfPage"/> property.
        /// </summary>
        private int _selectedPdfPage = 1;

        //   ---   Private Properties   ---

        /// <summary>
        /// Sets or gets the <see cref="StorageFile"/> of the currently selected file or <see langword="null"/> if the current file is invalid.
        /// </summary>
        private StorageFile? CurrentScheduleFile { set; get; }

        /// <summary>
        /// Sets or gets the <see cref="PdfDocument"/> instance containing the loaded pdf document if the selected file is a pdf document and 
        /// </summary>
        private PdfDocument? SchedulePdfFile { set; get; }

        /// <summary>
        /// Gets the <see cref="DispatcherQueueTimer"/> used to delay the validation of the currently set file path.
        /// </summary>
        private DispatcherQueueTimer ValidationTimer { get; }

        /// <summary>
        /// Gets the <see cref="UIElement"/> which is the View this ViewModel instance is assigned to.
        /// </summary>
        private UIElement View { get; }

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the command to invoke the browsing for a 
        /// </summary>
        public ICommand BrowseScheduleFileCommand => _browseScheduleFileCommand;

        /// <summary>
        /// Gets if the schedule can be added.
        /// </summary>
        public bool CanAdd
        {
            private set
            {
                if (value == _canAdd)
                {
                    return;
                }

                _canAdd = value;

                OnPropertyChanged();
                OnCanAddChanged();
            }
            get => _canAdd;
        }

        /// <summary>
        /// Sets or gets a <see cref="Func{T, TResult}"/> used to format the user selected date before the dated resource is created.
        /// </summary>
        public Func<DateOnly, DateOnly> DateFormatter
        {
            set
            {
                if (value == _dateFormatter)
                {
                    return;
                }

                _dateFormatter = value;

                OnPropertyChanged();
            }
            get => _dateFormatter;
        }

        /// <summary>
        /// Gets the status message text of the file status message.
        /// </summary>
        public string FileStatusMessageText
        {
            private set
            {
                if (value == _fileStatusMessageText)
                {
                    return;
                }

                _fileStatusMessageText = value;

                OnPropertyChanged();
            }
            get => _fileStatusMessageText;
        }

        /// <summary>
        /// Gets whether a file status message is active.
        /// </summary>
        public bool IsFileStatusMessageActive { private set; get; }

        /// <summary>
        /// Sets or gets whehter the pdf preview image is currently getting rendered.
        /// </summary>
        public bool IsPdfRendering
        {
            private set
            {
                if (value == _isPdfRendering)
                {
                    return;
                }

                _isPdfRendering = value;

                OnPropertyChanged();
            }
            get => _isPdfRendering;
        }

        /// <summary>
        /// Gets the <see cref="BitmapImage"/> instance containing a preview of the pdf page.
        /// </summary>
        public BitmapImage PdfPagePreviewSource { get; } = new BitmapImage();

        /// <summary>
        /// Gets the <see cref="Visibility"/> of the pdf options panel.
        /// </summary>
        public Visibility PdfOptionsPanelVisibility
        {
            private set
            {
                if (value == _pdfOptionsPanelVisibility)
                {
                    return;
                }

                _pdfOptionsPanelVisibility = value;

                OnPropertyChanged();
            }
            get => _pdfOptionsPanelVisibility;
        }

        /// <summary>
        /// Gets the number of pdf pages in the selected pdf document.
        /// </summary>
        public int PdfPageCount
        {
            set
            {
                if (value == _pdfPageCount)
                {
                    return;
                }

                _pdfPageCount = value;

                OnPropertyChanged();
            }
            get => _pdfPageCount;
        }

        /// <summary>
        /// Sets or gets the <see cref="DateTimeOffset"/> of the date of the schedule.
        /// </summary>
        public DateTimeOffset ScheduleDateTimeOffset
        {
            set
            {
                if (value == _scheduleDateTimeOffset)
                {
                    return;
                }

                _scheduleDateTimeOffset = value;

                OnPropertyChanged();
            }
            get => _scheduleDateTimeOffset;
        }

        /// <summary>
        /// Sets or gets the file path for the schedule image or pdf file.
        /// </summary>
        public string SchedulePath
        {
            set
            {
                if (value == _schedulePath)
                {
                    return;
                }

                _schedulePath = value;

                OnPropertyChanged();
                InvokeFilePathValidation();
            }
            get => _schedulePath;
        }

        /// <summary>
        /// Sets or gets which page of a pdf page is used for the schedule image.
        /// </summary>
        public int SelectedPdfPage
        {
            set
            {
                if (value == _selectedPdfPage)
                {
                    return;
                }

                _selectedPdfPage = value;

                OnPropertyChanged();
                InvokeRefreshPdfFilePreview();
            }
            get => _selectedPdfPage;
        }

        //   ---   Public Events   ---

        /// <summary>
        /// Raised when the value of the <see cref="CanAdd"/> property changed.
        /// </summary>
        public event TypedEventHandler<AddSchedulePageViewModel, bool>? CanAddChanged;

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="AddSchedulePageViewModel"/> class.
        /// </summary>
        /// <param name="view">The View instance this <see cref="AddSchedulePageViewModel"/> ViewModel instance is assgined to.</param>
        public AddSchedulePageViewModel(UIElement view)
        {
            View = view;

            _browseScheduleFileCommand = new RelayCommand(BrowseFile);

            // Create the validation timer
            ValidationTimer = View.DispatcherQueue.CreateTimer();

            // Subscribe to events
            ConfigManager.Config.PropertyChanged += Config_PropertyChanged;

            // Configure the validation timer
            ValidationTimer.Interval = TimeSpan.FromMilliseconds(ConfigManager.Config.FilePathValidationDelay);
            ValidationTimer.IsRepeating = false;
            ValidationTimer.Tick += async (s, e) => await ValidateFilePath();
        }

        //   ---   Private Methods (callbacks)   ---

        private void Config_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Check if the file path validation delay has changed
            if (e.PropertyName != nameof(ConfigData.FilePathValidationDelay))
            {
                return;
            }

            bool validationTimerRunning = ValidationTimer.IsRunning;

            if (validationTimerRunning)
            {
                ValidationTimer.Stop();
            }

            ValidationTimer.Interval = TimeSpan.FromMilliseconds(ConfigManager.Config.FilePathValidationDelay);
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Method used to browse for a schedule file.
        /// </summary>
        private async void BrowseFile()
        {
            FileOpenPicker picker = new FileOpenPicker(View.XamlRoot.ContentIslandEnvironment.AppWindowId)
            {
                Title = ResourceManager.GetString("AddScheduleViewModel/SelectScheduleFileOpenPicker/Title"),
                CommitButtonText = ResourceManager.GetString("AddScheduleViewModel/SelectScheduleFileOpenPicker/CommitButtonText")
            };

            if (Path.Exists(SchedulePath))
            {
                picker.SuggestedFolder = Path.GetDirectoryName(SchedulePath);
                picker.SuggestedStartFolder = Path.GetDirectoryName(SchedulePath);
            }

            PickFileResult? result = await picker.PickSingleFileAsync();

            if (result is null)
            {
                return;
            }

            SchedulePath = result.Path;
        }

        /// <summary>
        /// Method used to clear the file status message.
        /// </summary>
        private void ClearFileStatusMessage()
        {
            IsFileStatusMessageActive = false;
            OnPropertyChanged(nameof(IsFileStatusMessageActive));
        }

        /// <summary>
        /// Method used to get the formatted <see cref="DateOnly"/> representing the set resource date.
        /// </summary>
        /// <returns></returns>
        private DateOnly GetFormattedResourceDate()
        {
            return DateFormatter(Util.Util.DateTimeOffsetToDateOnly(ScheduleDateTimeOffset));
        }

        /// <summary>
        /// Method used to invoke a delayed file path validation.
        /// </summary>
        private void InvokeFilePathValidation() => ValidationTimer.Start();

        /// <summary>
        /// Method used to dispatch the refreshing of the pdf file preview to the <see cref="DispatcherQueue"/> of the View.
        /// </summary>
        private void InvokeRefreshPdfFilePreview()
        {
            View.DispatcherQueue.TryEnqueue(async () => await RefreshPdfFilePreview());
        }

        /// <summary>
        /// Method used to refresh the preview of the pdf file.
        /// </summary>
        private async Task RefreshPdfFilePreview()
        {
            if (SchedulePdfFile is null)
            {
                // Ensure the rendering visuals are deactivated
                IsPdfRendering = false;

                return;
            }

            // Activate the rendering visuals
            IsPdfRendering = true;

            uint pageIndex = (uint)SelectedPdfPage - 1;

            using (PdfPage page = SchedulePdfFile.GetPage(pageIndex))
            {
                InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();

                // Render the pdf page
                await page.RenderToStreamAsync(stream);

                // Set the preview page source.
                await PdfPagePreviewSource.SetSourceAsync(stream);
            }

            // Deactivate the rendering visuals
            IsPdfRendering = false;
        }

        /// <summary>
        /// Method used to set a file status message based on a <paramref name="messageId"/>.
        /// </summary>
        /// <param name="messageId">Identifier of the StatusMessage resource.</param>
        /// <param name="isGeneric">Defines if the passed identifier is for a generic status message.</param>
        private void SetFileStatusMessage(string messageId, bool isGeneric = true) => SetFormattedStatusMessage(messageId, m => m, isGeneric);

        /// <summary>
        /// Method used to set and format a file status message based on a <paramref name="messageId"/> and a <paramref name="formatter"/> function.
        /// </summary>
        /// <param name="messageId">Identifier of the StatusMessage resource.</param>
        /// <param name="formatter">Function used to format the status message.</param>
        /// <param name="isGeneric">Defines if the passed identifier is for a generic status message.</param>
        private void SetFormattedStatusMessage(string messageId, Func<string, string> formatter, bool isGeneric = true)
        {
            bool statusMessageActive = IsFileStatusMessageActive;

            if (isGeneric)
            {
                FileStatusMessageText = formatter(ResourceManager.GetGenericStatusMessage(messageId));
            }
            else
            {
                FileStatusMessageText = formatter(ResourceManager.GetStatusMessage(messageId));
            }

            if (!statusMessageActive)
            {
                IsFileStatusMessageActive = true;
                OnPropertyChanged(nameof(IsFileStatusMessageActive));
            }
        }

        /// <summary>
        /// Method used to validate the selected file path and update the file error message.
        /// </summary>
        private async Task ValidateFilePath()
        {
            bool issuePresent = false;

            CurrentScheduleFile = null;
            SchedulePdfFile = null;

            SelectedPdfPage = 1;

            bool isPdfFile = Path.GetExtension(SchedulePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(SchedulePath))
            {
                SetFileStatusMessage("FilePathEmptyError");
                issuePresent = true;
            }
            else if (!File.Exists(SchedulePath))
            {
                SetFileStatusMessage("FileNotFoundError");
                issuePresent = true;
            }
            else if (!(ResourceManager.IsValidImageResourcePath(SchedulePath) || isPdfFile))
            {
                SetFormattedStatusMessage("FileTypeNotSupportedFormatted", m => string.Format(m, Path.GetExtension(SchedulePath)));
                issuePresent = true;
            }
            else
            {
                ClearFileStatusMessage();

                // Create a StorageFile from the path
                CurrentScheduleFile = await StorageFile.GetFileFromPathAsync(SchedulePath);

                // Activate the rendering visuals
                IsPdfRendering = true;
                PdfOptionsPanelVisibility = Visibility.Visible;

                if (isPdfFile)
                {
                    // Load the pdf file
                    try
                    {
                        SchedulePdfFile = await PdfDocument.LoadFromFileAsync(CurrentScheduleFile);
                    }
                    catch (Exception ex)
                    {
                        switch (ex.HResult)
                        {
                            case ERROR_WRONG_PASSWORD:
                                SetFileStatusMessage("FilePasswordProtectedError");
                                break;
                            case GENERIC_FAIL:
                                SetFileStatusMessage("FileNotValidPdfError");
                                break;
                            default:
                                SetFileStatusMessage("CannotOpenFileError");
                                break;
                        }

                        SchedulePdfFile = null;
                    }

                    if ((SchedulePdfFile is not null) && SchedulePdfFile.IsPasswordProtected)
                    {
                        SetFileStatusMessage("FilePasswordProtectedError");

                        SchedulePdfFile = null;
                    }

                    issuePresent = SchedulePdfFile is null;

                    if (!issuePresent)
                    {
                        PdfPageCount = (int)SchedulePdfFile!.PageCount;

                        // Refresh the preview
                        await RefreshPdfFilePreview();
                    }
                }
            }

            if (issuePresent)
            {
                // Deactivate the rendering visuals
                IsPdfRendering = false;

                // Reset the file properties
                CurrentScheduleFile = null;
                SchedulePdfFile = null;

                PdfOptionsPanelVisibility = Visibility.Collapsed;
            }

            CanAdd = !issuePresent;
        }

        //   ---   Protected Methods   ---

        /// <summary>
        /// Raises the <see cref="CanAddChanged"/> event.
        /// </summary>
        protected void OnCanAddChanged()
        {
            CanAddChanged?.Invoke(this, CanAdd);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="name">Name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //   ---   Public Methods   ---

        public void Dispose()
        {

        }

        /// <summary>
        /// Method used to add the currently selected image or pdf page as the schedule for the selected <paramref name="resourceName"/>.
        /// </summary>
        /// <param name="resourceName">Name of the dated resource of the schedule image.</param>
        public async Task AddSchedule(string resourceName)
        {
            if ((CurrentScheduleFile is null) || (!CanAdd))
            {
                // TODO: Add error handling and show status messages
                return;
            }

            if (SchedulePdfFile is null)
            {
                throw new InvalidProgramException("Invalid program state reached: The pdf file should be addable, but no pdf file instance exists.");
            }

            DateOnly formattedResourceDate = GetFormattedResourceDate();

            if (SchedulePdfFile is null)
            {
                // Add a simple image file
                ResourceManager.CreateDatedResourceFile(SchedulePath, resourceName, formattedResourceDate);
            }
            else
            {
                using (PdfPage pdfPage = SchedulePdfFile.GetPage((uint)SelectedPdfPage - 1))
                {
                    await ResourceManager.CreateDatedResourceFile(pdfPage, resourceName, formattedResourceDate);
                }
            }
        }
    }
}
