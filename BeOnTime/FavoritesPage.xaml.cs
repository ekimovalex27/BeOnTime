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
  public sealed partial class FavoritesPage : Page
  {
    private LocationItem selectedLocationItem = new LocationItem();

    private FavoritesItem selectedItem
    {
      get
      {
        if (lvFavorites.SelectedIndex >= 0)
        {
          return lvFavorites.SelectedItem as FavoritesItem;
        }
        else
        {
          return null;
        }
      }
    }

    public FavoritesPage()
    {
      this.InitializeComponent();
    }

    private void SetStateControls()
    {
      if (lvFavorites.Items.Count > 0)
      {
        lvFavorites.SelectedIndex = 0;

        FavoritesEmptyText.Visibility = Visibility.Collapsed;
        cmdBarFavoritesPage.IsEnabled = true;
      }
      else
      {
        FavoritesEmptyText.Visibility = Visibility.Visible;
        cmdBarFavoritesPage.IsEnabled = false;
      }
    }

    private void Init()
    {
      #region Для всех страниц
      this.Background = SharedClass.Theme;
      #endregion Для всех страниц      

      lvFavorites.ItemsSource = SharedClass.ListFavorites.OrderByDescending(item => item.DateAdd);
      SetStateControls();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      #region Для всех страниц
      Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible;

      var rootFrame = Window.Current.Content as Frame;
      var HeaderPage = ((rootFrame.Content as MainPage).FindName("HeaderPage") as TextBlock); //Get TextBlock
      HeaderPage.Text = SharedClass.HeaderPage2;

      Init();
      #endregion Для всех страниц
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
      var rootFrame = Window.Current.Content as Frame;
      var ScenarioFrame = ((rootFrame.Content as MainPage).FindName("ScenarioFrame") as Frame); //Get Frame in SplitView

      selectedLocationItem.locationLatitude = selectedItem.Latitude;
      selectedLocationItem.locationLongitude = selectedItem.Longitude;
      selectedLocationItem.locationAddress = selectedItem.Address;

      ScenarioFrame.Navigate(typeof(MapPage), selectedLocationItem); //Navigate to MapPage for Show Map
    }

    private void appbarFavoritesEdit_Click(object sender, RoutedEventArgs e)
    {
      var rootFrame = Window.Current.Content as Frame;
      var ScenarioFrame = ((rootFrame.Content as MainPage).FindName("ScenarioFrame") as Frame); //Get Frame in SplitView
      ScenarioFrame.Navigate(typeof(FavoritesEditPage), selectedItem); //Navigate to FavoritesEditPage for Favorites Edit
    }

    private void appbarFavoritesCopyPosition_Click(object sender, RoutedEventArgs e)
    {
      DataPackage dataPackage = new DataPackage();
      dataPackage.RequestedOperation = DataPackageOperation.Copy;
      dataPackage.SetText(selectedItem.Latitude.ToString().Replace(",", ".") + "," + selectedItem.Longitude.ToString().Replace(",", "."));
      Clipboard.SetContent(dataPackage);

      flayoutCopyPosition.ShowAt(spTop);
    }

    private async void appbarFavoritesDelete_Click(object sender, RoutedEventArgs e)
    {
      var deleteDialog = new ContentDialog()
      {
        Content = SharedClass.FavoritesDeleteDialogContent, //"Вы действительно хотите очистить историю мест?",
        PrimaryButtonText = SharedClass.FavoritesDeleteDialogPrimaryButtonText, //Да
        SecondaryButtonText = SharedClass.FavoritesDeleteDialogSecondaryButtonText //Нет
      };

      var result = await deleteDialog.ShowAsync();
      if (result == ContentDialogResult.Primary)
      {
        SharedClass.FavoritesRemove(selectedItem.DateAdd);
        lvFavorites.ItemsSource = SharedClass.ListFavorites.OrderByDescending(item => item.DateAdd);
        SetStateControls();
      }
    }
  }
}
