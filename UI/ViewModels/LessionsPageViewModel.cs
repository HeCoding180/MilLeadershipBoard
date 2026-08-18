using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MilLeadershipBoard.Config;
using MilLeadershipBoard.Resources;
using MilLeadershipBoard.UI.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MilLeadershipBoard.UI.ViewModels
{
    internal class LessionsPageViewModel : IDisposable, INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the value of the <see cref="AddCommand"/> property.
        /// </summary>
        private RelayCommand _addCommand;

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the <see cref="ICommand"/> used to add new lessions.
        /// </summary>
        public ICommand AddCommand => _addCommand;

        /// <summary>
        /// Sets or gets the <see cref="Microsoft.UI.Xaml.XamlRoot"/> instance of the partent page.
        /// Used to display Popups.
        /// </summary>
        public XamlRoot? XamlRoot { set; get; }

        //   ---   Public Events   ---

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="BreaksPageViewModel"/> class.
        /// </summary>
        public LessionsPageViewModel()
        {
            ConfigManager.Config.PropertyChanged += Config_PropertyChanged;

            _addCommand = new RelayCommand(AddLession);
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Method used to add a new lession.
        /// </summary>
        private async void AddLession()
        {
            if (XamlRoot is null)
            {
                throw new InvalidProgramException($"{nameof(LessionsPageViewModel)} did not load properly. No value set for {nameof(XamlRoot)}.");
            }

            AddLessionPage contentPage = new AddLessionPage();

            ContentDialog dialog = new ContentDialog()
            {
                Content = contentPage,
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonCommand = contentPage.AddLessionCommand,
                PrimaryButtonText = ResourceManager.GetString("LessionsPage/AddLessionContentDialog/PrimaryButtonText"),
                SecondaryButtonText = ResourceManager.GetString("LessionsPage/AddLessionContentDialog/SecondaryButtonText"),
                XamlRoot = XamlRoot
            };

            await dialog.ShowAsync();
        }

        /// <summary>
        /// Callback method for the <see cref="ConfigData.PropertyChanged"/> event of the <see cref="ConfigManager"/>'s <see cref="ConfigData"/> instance.
        /// </summary>
        private void Config_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ConfigData):
                    break;
            }
        }

        //   ---   Protected Methods   ---

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="name">Name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //   ---   Public Methods   ---

        /// <summary>
        /// Method used to dispose of this <see cref="BreaksPageViewModel"/> instance.
        /// </summary>
        void IDisposable.Dispose()
        {
            ConfigManager.Config.PropertyChanged -= Config_PropertyChanged;
        }
    }
}
