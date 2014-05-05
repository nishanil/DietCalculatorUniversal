using DietCalculator.Common;
using DietCalculator.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace DietCalculator
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class HomePage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        IDietCalculatorModel model;
        IDietCalculatorController controller;
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public HomePage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            // MVVM is not used in this example. We're porting an existing sample from Silverlight MVC
            // we will make use of the existing pattern to achieve the results
            model = new DietCalculatorModel();
            controller = new DietCalculatorController(model);


            // bydefault Male is Selected, hence collapse hips
            txtHips.Visibility = tblHips.Visibility = Visibility.Collapsed;

            cmbGender.SelectionChanged += (sender, args) =>
            {
                var isSelectedItemMale = ((string)cmbGender.SelectedItem == "Male");
                txtHips.Visibility = tblHips.Visibility = isSelectedItemMale ? Visibility.Collapsed : Visibility.Visible;

                // corner case: reset hipsText to empty if the user had typed in hips and later changed the gender
                if (isSelectedItemMale) txtHips.Text = string.Empty;
            };

            // subscribe to IdealBMI and IdealWeight changed events on the model to update the UI
            // as per the implementation in the core project, 
            // any change in IdealBMI or IdealWeight are notified using appropriate event handlers
            model.IdealBMIChanged += (sender, e) =>
            {
                txtIdealBMI.Text = e.IdealBMI.ToString();
            };

            model.IdealWeightChanged += (sender, e) =>
            {
                txtIdealWeight.Text = e.IdealWeight.ToString();
            };

            txtIdealWeight.TextChanged += (sender, e) =>
            {
                controller.SetWeight(StringToNumberUtility.GetDouble(txtWeight.Text, 0.00));
                controller.SetHeight(StringToNumberUtility.GetDouble(txtHeight.Text, 0.00));
                controller.SetIdealWeight(StringToNumberUtility.GetDouble(txtIdealWeight.Text, 0.00));

            };
            txtIdealBMI.TextChanged += (sender, e) =>
            {
                controller.SetWeight(StringToNumberUtility.GetDouble(txtWeight.Text, 0.00));
                controller.SetHeight(StringToNumberUtility.GetDouble(txtHeight.Text, 0.00));
                controller.SetIdealBMI(StringToNumberUtility.GetDouble(txtIdealBMI.Text, 0.00));
            };

        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void CaclculateGridViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            controller.SetAge(StringToNumberUtility.GetInt32(txtAge.Text, 0));
            controller.SetGender((cmbGender.SelectedIndex == 0) ? true : false);
            controller.SetWeight(StringToNumberUtility.GetDouble(txtWeight.Text, 0.00));
            controller.SetHeight(StringToNumberUtility.GetDouble(txtHeight.Text, 0.00));
            controller.SetWaist(StringToNumberUtility.GetDouble(txtWaist.Text, 0.00));
            controller.SetHips(StringToNumberUtility.GetDouble((!string.IsNullOrEmpty(txtHips.Text) ? txtHips.Text : null), 0.00));
            controller.SetIdealWeight(StringToNumberUtility.GetDouble(txtIdealWeight.Text, 0.00));
            controller.SetIdealBMI(StringToNumberUtility.GetDouble(txtIdealBMI.Text, 0.00));
            controller.SetCholesterol(StringToNumberUtility.GetDouble(txtCholestrol.Text, 0.00));
            controller.SetHDL(StringToNumberUtility.GetDouble(txtHDL.Text, 0.00));
            controller.SetNeck(StringToNumberUtility.GetDouble(txtNeck.Text, 0.00));
            var selectedActivity = (string)cmbLevelOfActivity.SelectedItem;
            controller.SetActivity((LevelOfActivity)Enum.Parse(typeof(LevelOfActivity), selectedActivity));

            // HACK: force refresh UI
            resultsControl.DataContext = null;
            resultsControl.DataContext = model;
            resultsControl.Visibility = Visibility.Visible;
            MainContent.Width = 2600;
            MainScroll.ChangeView(1000, null, null);
        }
    }
}
