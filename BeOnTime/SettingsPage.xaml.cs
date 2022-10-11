using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BeOnTime
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class SettingsPage : Page
  {
    #region Define vars
    int SelectedIndex;
    #endregion Define vars

    public SettingsPage()
    {
      InitializeComponent();
    }

    #region AEV

    private void Init()
    {
      this.Background = SharedClass.Theme;

      #region General

      #region Theme
      if (SharedClass.IsThemeBlue)
        this.ThemeBlue.IsChecked = true;
      else if (SharedClass.IsThemeGreen)
        this.ThemeGreen.IsChecked = true;
      else if (SharedClass.IsThemeBlack)
        this.ThemeBlack.IsChecked = true;
      else
      {
        SharedClass.IsThemeBlue = true;
        this.ThemeBlue.IsChecked = true;
      }
      #endregion Theme

      #region Select current language in list localization
      this.comboLocalization.ItemsSource = SharedClass.ListLocalization;
      var LanguageTag = SharedClass.LanguageTag;
      var SelectedIndex = (this.comboLocalization.ItemsSource as List<LocalizationItem>).FindIndex(item => item.Culture == LanguageTag);
      if (SelectedIndex < 0)
      {
        var aLanguageTag = LanguageTag.Split(new char[] { '-' });
        if (aLanguageTag.Length > 0)
        {
          LanguageTag = aLanguageTag[0].ToLower();
          SelectedIndex = (this.comboLocalization.ItemsSource as List<LocalizationItem>).FindIndex(item => item.Culture == LanguageTag);
        }          
      }
      this.comboLocalization.SelectedIndex = SelectedIndex;
      this.comboLocalization.SelectionChanged += comboLocalization_SelectionChanged;
      #endregion Select current language in list localization

      //this.txtDefaultCity.Text = SharedClass.DefaultCity;
      #endregion General

      #region Traffic Model
      this.TrafficModelBestGuess.IsChecked = SharedClass.IsModeBestGuess;
      this.TrafficModelPessimistic.IsChecked = SharedClass.IsModePessimistic;
      this.TrafficModelOptimistic.IsChecked = SharedClass.IsModeOptimistic;
      #endregion Traffic Model

      #region Avoid
      this.AvoidTolls.IsChecked = SharedClass.IsAvoidTolls;
      this.AvoidHighways.IsChecked = SharedClass.IsAvoidHighways;
      this.AvoidFerries.IsChecked = SharedClass.IsAvoidFerries;
      this.AvoidIndoor.IsChecked = SharedClass.IsAvoidIndoor;
      #endregion Avoid

      #region Transit Mode
      this.TransitBus.IsChecked = SharedClass.IsTransitModeBus;
      this.TransitSubway.IsChecked = SharedClass.IsTransitModeSubway;
      this.TransitTrain.IsChecked = SharedClass.IsTransitModeTrain;
      this.TransitTram.IsChecked = SharedClass.IsTransitModeTram;
      #endregion Transit Mode

      #region History
      this.HistoryToggle.IsOn = SharedClass.IsHistorySave;
      //HistoryClearText.Visibility = Visibility.Collapsed;
      #endregion History
    }

    #region Events

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      #region Для всех страниц
      Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible;

      var rootFrame = Window.Current.Content as Frame;
      var HeaderPage = ((rootFrame.Content as MainPage).FindName("HeaderPage") as TextBlock); //Get TextBlock
      HeaderPage.Text = SharedClass.HeaderPage3;

      if (e.Parameter != null)
      {
        rootPivot.SelectedIndex = (int)e.Parameter;
      }
      else
      {
        rootPivot.SelectedIndex = 0; // After change Theme
      }

      Init();
      #endregion Для всех страниц
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
    }

    private void rootPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      //HistoryClearText.Visibility = Visibility.Collapsed;

      ////(((sender as Pivot).SelectedItem as PivotItem).

      ////var PivotEvent = sender as Pivot;

      ////if (PivotEvent.SelectedIndex != SelectedIndex)
      ////{
      ////  (PivotEvent.Items[SelectedIndex] as PivotItem).Opacity = 0.5;
      ////  SelectedIndex = PivotEvent.SelectedIndex;
      ////  (PivotEvent.SelectedItem as PivotItem).Opacity = 0;
      ////}
    }

    #region General
    //private void txtDefaultCity_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    //{
    //  SharedClass.DefaultCity = ((TextBox)sender).Text.Trim();
    //}

    private async void Theme_Checked(object sender, RoutedEventArgs e)
    {
      try
      {
        if ((e.OriginalSource as RadioButton).FocusState != FocusState.Unfocused)
        {
          var pressed = sender as RadioButton;

          #region Set theme
          switch ((SharedClass.enumTheme)Convert.ToInt32(pressed.Tag))
          {
            case SharedClass.enumTheme.Blue:
              SharedClass.IsThemeBlue = true;
              SharedClass.IsThemeGreen = false;
              SharedClass.IsThemeBlack = false;
              break;
            case SharedClass.enumTheme.Green:
              SharedClass.IsThemeBlue = false;
              SharedClass.IsThemeGreen = true;
              SharedClass.IsThemeBlack = false;
              break;
            case SharedClass.enumTheme.Black:
              SharedClass.IsThemeBlue = false;
              SharedClass.IsThemeGreen = false;
              SharedClass.IsThemeBlack = true;
              break;
          }
          #endregion Set theme

          #region Refresh theme

          await RefreshPage();

          #region Рабочий вариант
          //var rootFrame = Window.Current.Content as Frame;
          //if (rootFrame != null)
          //{
          //  var SaveCurrentSourcePage = this.Frame.CurrentSourcePageType; //Save current page type and state
          //  rootFrame.Navigate(rootFrame.Content.GetType()); //Refresh theme
          //  var ScenarioFrame = ((rootFrame.Content as MainPage).FindName("ScenarioFrame") as Frame); //Get Frame in SplitView
          //  ScenarioFrame.Navigate(SaveCurrentSourcePage); //Navigate back to saved page in SplitView

          //  //ScenarioFrame.Navigate(ScenarioFrame.Content.GetType());
          //}
          #endregion Рабочий вариант

          #region Refresh theme as dialog
          //var noWifiDialog = new ContentDialog()
          //{
          //  Title = SharedClass.SettingsThemeDialogTitle,
          //  Content = SharedClass.SettingsThemeDialogText,
          //  PrimaryButtonText = "Ok"
          //};
          //await noWifiDialog.ShowAsync();
          #endregion Refresh theme as dialog

          #endregion Refresh theme
        }
      }
      catch (Exception ex)
      {
      }
    }

    private async void comboLocalization_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var LanguageTag = (sender as ComboBox).SelectedValue.ToString();

      //SharedClass.LanguageTag = LanguageTag;
      Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = LanguageTag;

      await RefreshPage();

      #region Рабочий вариант
      //Это ОБЯЗАТЕЛЬНО!!! Без этого ничего не работает почему-то.
      //await System.Threading.Tasks.Task.Delay(100);

      //var rootFrame = Window.Current.Content as Frame;
      //if (rootFrame != null)
      //{
      //  var SaveCurrentSourcePage = this.Frame.CurrentSourcePageType; //Save current page type and state
      //  rootFrame.Navigate(rootFrame.Content.GetType()); //Refresh theme
      //  var ScenarioFrame = ((rootFrame.Content as MainPage).FindName("ScenarioFrame") as Frame); //Get Frame in SplitView
      //  ScenarioFrame.Navigate(SaveCurrentSourcePage); //Navigate back to saved page in SplitView
      //}
      #endregion Рабочий вариант

      //rootFrame.Navigate(typeof(MainPage));

      //Windows.ApplicationModel.Resources.Core.ResourceContext.GetForViewIndependentUse().Reset();

      //Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView();      
      ////Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Reset();

      //var type = Frame.CurrentSourcePageType;
      //try
      //{
      //  Frame.Navigate(type);
      //}
      //finally
      //{
      //  Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);
      //}

      //this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LocalizedResourceMap"));
      // Notify code about the changes
      //this.LanguageChanged?.Invoke(this, new EventArgs());
    }

    private async System.Threading.Tasks.Task RefreshPage()
    {
      //Это ОБЯЗАТЕЛЬНО!!! Без этого ничего не работает почему-то.
      await System.Threading.Tasks.Task.Delay(100);

      var rootFrame = Window.Current.Content as Frame;
      if (rootFrame != null)
      {
        var SaveCurrentSourcePage = this.Frame.CurrentSourcePageType; //Save current page type and state
        rootFrame.Navigate(rootFrame.Content.GetType()); //Refresh theme
        var ScenarioFrame = ((rootFrame.Content as MainPage).FindName("ScenarioFrame") as Frame); //Get Frame in SplitView
        ScenarioFrame.Navigate(SaveCurrentSourcePage); //Navigate back to saved page in SplitView
      }
    }

    #endregion General

    #region Traffic Model
    private void RadioButton_Checked(object sender, RoutedEventArgs e)
    {
      var pressed = sender as RadioButton;

      switch ((SharedClass.enumTrafficModel)Convert.ToInt32(pressed.Tag))
      {
        case SharedClass.enumTrafficModel.best_guess:
          SharedClass.IsModeBestGuess = true;
          SharedClass.IsModePessimistic = false;
          SharedClass.IsModeOptimistic = false;
          break;
        case SharedClass.enumTrafficModel.pessimistic:
          SharedClass.IsModeBestGuess = false;
          SharedClass.IsModePessimistic = true;
          SharedClass.IsModeOptimistic = false;
          break;
        case SharedClass.enumTrafficModel.optimistic:
          SharedClass.IsModeBestGuess = false;
          SharedClass.IsModePessimistic = false;
          SharedClass.IsModeOptimistic = true;
          break;
      }
    }

    #endregion Traffic Model

    #region Avoid

    #region Tolls
    private void AvoidTolls_Checked(object sender, RoutedEventArgs e)
    {
      SharedClass.IsAvoidTolls = true;
    }

    private void AvoidTolls_Unchecked(object sender, RoutedEventArgs e)
    {
      SharedClass.IsAvoidTolls = false;
    }
    #endregion Tolls

    #region Highways
    private void AvoidHighways_Checked(object sender, RoutedEventArgs e)
    {
      SharedClass.IsAvoidHighways = true;
    }

    private void AvoidHighways_Unchecked(object sender, RoutedEventArgs e)
    {
      SharedClass.IsAvoidHighways = false;
    }
    #endregion Highways

    #region Ferries
    private void AvoidFerries_Checked(object sender, RoutedEventArgs e)
    {
      SharedClass.IsAvoidFerries = true;
    }

    private void AvoidFerries_Unchecked(object sender, RoutedEventArgs e)
    {
      SharedClass.IsAvoidFerries = false;
    }
    #endregion Ferries

    #region Indoor
    private void AvoidIndoor_Checked(object sender, RoutedEventArgs e)
    {
      SharedClass.IsAvoidIndoor = true;
    }

    private void AvoidIndoor_Unchecked(object sender, RoutedEventArgs e)
    {
      SharedClass.IsAvoidIndoor = false;
    }
    #endregion Indoor

    #endregion Avoid

    #region Transit Mode 

    #region Bus
    private void TransitBus_Checked(object sender, RoutedEventArgs e)
    {
      SharedClass.IsTransitModeBus = true;
    }

    private void TransitBus_Unchecked(object sender, RoutedEventArgs e)
    {
      SharedClass.IsTransitModeBus = false;
    }
    #endregion Bus

    #region Subway
    private void TransitSubway_Checked(object sender, RoutedEventArgs e)
    {
      SharedClass.IsTransitModeSubway = true;
    }

    private void TransitSubway_Unchecked(object sender, RoutedEventArgs e)
    {
      SharedClass.IsTransitModeSubway = false;
    }
    #endregion Subway

    #region Train
    private void TransitTrain_Checked(object sender, RoutedEventArgs e)
    {
      SharedClass.IsTransitModeTrain = true;
    }

    private void TransitTrain_Unchecked(object sender, RoutedEventArgs e)
    {
      SharedClass.IsTransitModeTrain = false;
    }
    #endregion Train

    #region Tram
    private void TransitTram_Checked(object sender, RoutedEventArgs e)
    {
      SharedClass.IsTransitModeTram = true;
    }

    private void TransitTram_Unchecked(object sender, RoutedEventArgs e)
    {
      SharedClass.IsTransitModeTram = false;
    }
    #endregion Tram

    #endregion Transit Mode 

    #region History
    private void HistoryToggle_Toggled(object sender, RoutedEventArgs e)
    {
      SharedClass.IsHistorySave = (sender as ToggleSwitch).IsOn;
    }

    #endregion History

    #endregion Events

    #endregion AEV
  }
}
