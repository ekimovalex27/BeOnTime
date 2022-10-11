using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BeOnTime
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class HistoryPage : Page
  {
    private LocationItem selectedLocationItem = new LocationItem();

    private HistoryItem selectedItem
    {
      get
      {
        if (lvHistory.SelectedIndex >= 0)
        {
          return lvHistory.SelectedItem as HistoryItem;
        }
        else
        {
          return null;
        }
      }
    }

    public HistoryPage()
    {
      InitializeComponent();     
    }

    private void SetStateControls()
    {
      if (lvHistory.Items.Count > 0)
      {
        lvHistory.SelectedIndex = 0;

        HistoryEmptyText.Visibility = Visibility.Collapsed;
        HistoryGotoSettings.Visibility = Visibility.Collapsed;
        cmdHistoryGotoSettings.Visibility = Visibility.Collapsed;
        cmdBarHistoryPage.IsEnabled = true;
      }
      else
      {
        HistoryEmptyText.Visibility = Visibility.Visible;

        if (SharedClass.IsHistorySave)
        {
          HistoryGotoSettings.Visibility = Visibility.Collapsed;
          cmdHistoryGotoSettings.Visibility = Visibility.Collapsed;
        }
        else
        {
          HistoryGotoSettings.Visibility = Visibility.Visible;
          cmdHistoryGotoSettings.Visibility = Visibility.Visible;
        }

        cmdBarHistoryPage.IsEnabled = false;
      }
    }

    private void Init()
    {
      #region Для всех страниц
      this.Background = SharedClass.Theme;
      #endregion Для всех страниц      

      lvHistory.ItemsSource = SharedClass.ListHistory.OrderByDescending(item => item.DateAdd);
      SetStateControls();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      #region Для всех страниц
      Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible;

      var rootFrame = Window.Current.Content as Frame;
      var HeaderPage = ((rootFrame.Content as MainPage).FindName("HeaderPage") as TextBlock); //Get TextBlock
      HeaderPage.Text = SharedClass.HeaderPage6;

      Init();
      #endregion Для всех страниц
    }
  
    private void cmdHistoryGotoSettings_Click(object sender, RoutedEventArgs e)
    {
      var rootFrame = Window.Current.Content as Frame;      
      var ScenarioFrame = ((rootFrame.Content as MainPage).FindName("ScenarioFrame") as Frame); //Get Frame in SplitView
      ScenarioFrame.Navigate(typeof(SettingsPage), 4); //Navigate to SettingsPage to Tab History
    }

    private void appbarCalc_Click(object sender, RoutedEventArgs e)
    {
      SharedClass.AddressTo = selectedItem.Address;

      var rootFrame = Window.Current.Content as Frame;
      var ScenarioFrame = ((rootFrame.Content as MainPage).FindName("ScenarioFrame") as Frame); //Get Frame in SplitView
      ScenarioFrame.Navigate(typeof(CalcPage)); //Navigate to CalcPage
    }

    private void appbarShowOnMap_Click(object sender, RoutedEventArgs e)
    {
      selectedLocationItem.locationLatitude = selectedItem.Latitude;
      selectedLocationItem.locationLongitude = selectedItem.Longitude;
      selectedLocationItem.locationAddress = selectedItem.Address;

      var rootFrame = Window.Current.Content as Frame;
      var ScenarioFrame = ((rootFrame.Content as MainPage).FindName("ScenarioFrame") as Frame); //Get Frame in SplitView
      ScenarioFrame.Navigate(typeof(MapPage), selectedLocationItem); //Navigate to MapPage for Show Map
    }

    private void appbarHistoryCopyPosition_Click(object sender, RoutedEventArgs e)
    {
      DataPackage dataPackage = new DataPackage();
      dataPackage.RequestedOperation = DataPackageOperation.Copy;
      dataPackage.SetText(selectedItem.Latitude.ToString().Replace(",", ".") + "," + selectedItem.Longitude.ToString().Replace(",", "."));
      Clipboard.SetContent(dataPackage);

      PositionInClipboard.Visibility = Visibility.Visible;
      HistoryAddToFavoritesNote.Visibility = Visibility.Collapsed;
      flayoutCopyPosition.ShowAt(spTop);
    }

    private void appbarHistoryDelete_Click(object sender, RoutedEventArgs e)
    {
      SharedClass.HistoryRemove(selectedItem.DateAdd);
      lvHistory.ItemsSource = SharedClass.ListHistory.OrderByDescending(item => item.DateAdd);
      SetStateControls();
    }

    private async void appbarHistoryClear_Click(object sender, RoutedEventArgs e)
    {
      var clearDialog = new ContentDialog()
      {
        Content = SharedClass.SettingsHistoryClearDialogContent, //"Вы действительно хотите очистить историю мест?",
        PrimaryButtonText = SharedClass.SettingsHistoryClearDialogPrimaryButtonText, //Да
        SecondaryButtonText = SharedClass.SettingsHistoryClearDialogSecondaryButtonText //Нет
      };

      var result = await clearDialog.ShowAsync();
      if (result == ContentDialogResult.Primary)
      {
        SharedClass.HistoryClear();
        lvHistory.ItemsSource = SharedClass.ListHistory.OrderByDescending(item => item.DateAdd);
        SetStateControls();
      }
    }

    private void StackPanel_LostFocus(object sender, RoutedEventArgs e)
    {
      flayoutCopyPosition.Hide();
    }

    private void appbarHistoryFavorite_Click(object sender, RoutedEventArgs e)
    {
      SharedClass.FavoritesAdd(SharedClass.HistoryAddToFavoritesTitle, selectedItem.Latitude, selectedItem.Longitude, selectedItem.Address, "", false);

      PositionInClipboard.Visibility = Visibility.Collapsed;
      HistoryAddToFavoritesNote.Visibility = Visibility.Visible;
      flayoutCopyPosition.ShowAt(spTop);
    }
  }
}
