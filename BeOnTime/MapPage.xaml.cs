using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
  public sealed partial class MapPage : Page
  {
    SharedLibraryRoute LibraryRoute = new SharedLibraryRoute();

    public MapPage()
    {
      this.InitializeComponent();
    }

    private void Init()
    {
      #region Для всех страниц
      this.Background = SharedClass.Theme;
      #endregion Для всех страниц      
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      #region Для всех страниц
      Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible;

      var rootFrame = Window.Current.Content as Frame;
      var HeaderPage = ((rootFrame.Content as MainPage).FindName("HeaderPage") as TextBlock); //Get TextBlock
      HeaderPage.Text = SharedClass.HeaderPage8;

      if (e.Parameter != null)
      {
        //var historyItem = (HistoryItem)e.Parameter;
        var locationItem = (LocationItem)e.Parameter;

        //// Для версии программы выше 1.1.8.0, так как в 8 значения координат хранятся, как текст "Lat" и "Long"
        //// После ухода с версии 1.1.8.0 этот код надо убрать
        if (locationItem.locationLatitude == 0 && locationItem.locationLongitude == 0)
        {
          var result = await LibraryRoute.GoogleGeocodeGetPosition(locationItem.locationAddress, SharedClass.LanguageTag);
          if (string.IsNullOrEmpty(result.Item3))
          {
            locationItem.locationLatitude = double.Parse(result.Item1, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
            locationItem.locationLongitude = double.Parse(result.Item2, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
          }
        }

        var locationPosition = new BasicGeoposition() { Latitude = locationItem.locationLatitude, Longitude = locationItem.locationLongitude };
        var locationCenter = new Geopoint(locationPosition);

        var mapIcon1 = new MapIcon
        {
          Location = locationCenter,
          NormalizedAnchorPoint = new Point(0.5, 1.0),
          Title = locationItem.locationAddress,
          ZIndex = 0,
        };

        MapLocation.MapElements.Add(mapIcon1);
        MapLocation.Center = locationCenter;
        MapLocation.ZoomLevel = 14;
        MapLocation.LandmarksVisible = true;
      }

      Init();
      #endregion Для всех страниц
    }

  }
}
