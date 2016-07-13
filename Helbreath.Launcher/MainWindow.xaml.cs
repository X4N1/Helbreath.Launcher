using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RestSharp;
using Path = System.IO.Path;

namespace Helbreath.Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly GameUpdater _gameUpdater;

        private readonly IRestClient _storageRestClient;

        private readonly IRestClient _versionRestClient;

        private readonly string _basePath;

        private readonly IVersionProvider _versionProvider;

        public MainWindow()
        {
            InitializeComponent();

            WebBrowser.Navigate(new Uri("http://helbreathpoland.com"));

            this._storageRestClient = new RestClient("https://s3-eu-west-1.amazonaws.com/helbreath-files/updates/");
            this._versionRestClient = new RestClient("http://version.helbreathpoland.com/");
            this._basePath = Path.Combine(Directory.GetCurrentDirectory(), "Version.json");
            this._versionProvider = new VersionProvider(this._versionRestClient, this._basePath);
            this._gameUpdater = new GameUpdater(this._storageRestClient, Directory.GetCurrentDirectory(),
                this._versionProvider);

            var currentCulture = (System.Globalization.CultureInfo)CultureInfo.CurrentCulture.Clone();
            currentCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;

            this.Main();
        }

        private async void Main()
        {
            this.SetCurrentVersion();
            this.SetRemoteVersion();

            var isUpdate = this._gameUpdater.CheckVersion(this._versionProvider.GetVersionFromInternet().Version,
               this._versionProvider.GetVersionFromFile().Version);

            if (isUpdate)
            {
                var result = await Task.Factory.StartNew<GameUpdaterResult>(() =>
                {
                    return this._gameUpdater.Update();
                });

                if (result.HasSucceed)
                {
                    this.SetStatusOfWork("Game has been updated");
                    StartButton.IsEnabled = true;
                }
                else
                {
                    this.SetStatusOfWork("Game has not been updated");
                    MessageBox.Show("Game has not been updated correctly, please restart launcher");
                }
            }
            else
            {
                StartButton.IsEnabled = true;
                this.SetStatusOfWork("Game has already newest files, you can play");
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("./Helbreath.Client.exe"))
            {
                MessageBox.Show("Client is not present in folder");
                return;
            }

            using (Process proc = Process.Start("Helbreath.Client.exe"))
            {
                proc.WaitForExit();

                // Retrieve the app's exit code
                var exitCode = proc.ExitCode;
            }
        }

        private void SetCurrentVersion()
        {
            var version = this._versionProvider.GetVersionFromFile();
            var message = string.Format("Local version : {0}", version.Version);
            LocalVersionTextBlock.Text = message;
        }

        private void SetRemoteVersion()
        {
            var version = this._versionProvider.GetVersionFromInternet();
            var message = string.Format("Local version : {0}", version.Version);
            RemoteVersionTextBlock.Text = message;
        }

        private void SetStatusOfWork(string message)
        {
            StatusOfWorkTextBlock.Text = message;
        }
    }
}
