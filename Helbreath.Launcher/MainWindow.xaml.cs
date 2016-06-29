using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Helbreath.Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double CurrentVersion = 0.1;
        private const double VersionFromInternet = 0.2;

        public MainWindow()
        {
            InitializeComponent();

            //open file VERSION.txt
            // read from file current version
            // if version from internet is greater download changes
            // copy zip
            // unzip
            // finish
        }

        public double GetCurrentVersion()
        {
            return CurrentVersion;
        }

        public void GetVersionFromInternet()
        {
            // read from webody =bsite
        }
    }
}
