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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BeOnTime
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class FavoritesEditPage : Page
  {
    FavoritesItem favoritesItem;

    public FavoritesEditPage()
    {
      this.InitializeComponent();
    }

    private void Init()
    {
      #region Для всех страниц
      this.Background = SharedClass.Theme;
      #endregion Для всех страниц      
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      #region Для всех страниц
      Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible;

      var rootFrame = Window.Current.Content as Frame;
      var HeaderPage = ((rootFrame.Content as MainPage).FindName("HeaderPage") as TextBlock); //Get TextBlock
      HeaderPage.Text = SharedClass.HeaderPage9;

      if (e.Parameter != null)
      {
        favoritesItem = (FavoritesItem)e.Parameter;
        txtName.Text = favoritesItem.Name;
      }

      Init();
      #endregion Для всех страниц
    }

    private void cmdOk_Click(object sender, RoutedEventArgs e)
    {
      SharedClass.FavoritesEdit(favoritesItem.DateAdd, txtName.Text);

      var rootFrame = Window.Current.Content as Frame;
      var ScenarioFrame = ((rootFrame.Content as MainPage).FindName("ScenarioFrame") as Frame); //Get Frame in SplitView
      ScenarioFrame.Navigate(typeof(FavoritesPage)); //Navigate to FavoritesPage
    }

    private void cmdCancel_Click(object sender, RoutedEventArgs e)
    {
      var rootFrame = Window.Current.Content as Frame;
      var ScenarioFrame = ((rootFrame.Content as MainPage).FindName("ScenarioFrame") as Frame); //Get Frame in SplitView
      ScenarioFrame.Navigate(typeof(FavoritesPage)); //Navigate to FavoritesPage
    }
  }
}
