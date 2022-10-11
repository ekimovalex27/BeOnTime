using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Navigation;

using Windows.UI.Notifications;
using NotificationsExtensions.Tiles;
using System.Globalization;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BeOnTime
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class CalcPage : Page
  {
    #region Define vars   

    SharedLibraryRoute LibraryRoute = new SharedLibraryRoute();

    List<string> AddressDeparture = new List<string>();
    List<string> AddressArrival = new List<string>();

    private bool IsModeEnabled = true;

    // A pointer to the main page
    //private MainPage _rootPage = MainPage.Current;

    private uint _desireAccuracyInMetersValue = 3;
    private CancellationTokenSource _cts = null;

    #endregion Define vars   

    public CalcPage()
    {
      this.InitializeComponent();
    }

    #region AEV Code

    private static TileGroup CreateGroup(string TextTraffic1, string TextTraffic2, string TextTraffic3)
    {
      return new TileGroup()
      {
        Children =
        {
            new TileSubgroup()
            {
                Children =
                {
                    new TileText()
                    {
                        Text = TextTraffic1,
                        //Style = TileTextStyle.Subtitle
                        //Style = TileTextStyle.CaptionSubtle
                        Wrap=true
                    },

                    new TileText()
                    {
                        Text = TextTraffic2,
                        Style = TileTextStyle.CaptionSubtle
                    },

                    new TileText()
                    {
                        Text = TextTraffic3,
                        Style = TileTextStyle.CaptionSubtle
                    }
                }
            }
        }
      };
    }

    private void Plitka(string TextWithoutTraffic1, string TextWithoutTraffic2, string TextWithoutTraffic3, string TextWithTraffic1, string TextWithTraffic2, string TextWithTraffic3)
    {
      TileContent content = new TileContent()
      {
        Visual = new TileVisual()
        {
          #region Small
          TileSmall = new TileBinding()
          {
            Content = new TileBindingContentAdaptive()
            {
              Children =
                {

                CreateGroup(
                TextTraffic1: TextWithoutTraffic1,
                TextTraffic2: TextWithoutTraffic2,
                TextTraffic3: TextWithoutTraffic3),

            // For spacing
            new TileText(),

            CreateGroup(
                TextTraffic1: TextWithTraffic1,
                TextTraffic2: TextWithTraffic2,
                TextTraffic3: TextWithTraffic3)
              }
            }
          },
          #endregion Small

          #region Medium
          TileMedium = new TileBinding()
          {
            Content = new TileBindingContentAdaptive()
            {
              Children =
                {

                CreateGroup(
                TextTraffic1: TextWithoutTraffic1,
                TextTraffic2: TextWithoutTraffic2,
                TextTraffic3: TextWithoutTraffic3),

            // For spacing
            new TileText(),

            CreateGroup(
                TextTraffic1: TextWithTraffic1,
                TextTraffic2: TextWithTraffic2,
                TextTraffic3: TextWithTraffic3)
              }
            }
          },
          #endregion Medium

          #region Wide
          TileWide = new TileBinding()
          {
            Content = new TileBindingContentAdaptive()
            {
              Children =
                {

                CreateGroup(
                TextTraffic1: TextWithoutTraffic1,
                TextTraffic2: TextWithoutTraffic2,
                TextTraffic3: TextWithoutTraffic3),

            // For spacing
            new TileText(),

            CreateGroup(
                TextTraffic1: TextWithTraffic1,
                TextTraffic2: TextWithTraffic2,
                TextTraffic3: TextWithTraffic3)
              }
            }
          },

          #endregion Wide

          #region Large
          TileLarge = new TileBinding()
          {
            Content = new TileBindingContentAdaptive()
            {
              Children =
                {

                CreateGroup(
                TextTraffic1: TextWithoutTraffic1,
                TextTraffic2: TextWithoutTraffic2,
                TextTraffic3: TextWithoutTraffic3),

            // For spacing
            new TileText(),

            CreateGroup(
                TextTraffic1: TextWithTraffic1,
                TextTraffic2: TextWithTraffic2,
                TextTraffic3: TextWithTraffic3)
              }
            }
          }
          #endregion Large
        }
      };
      content.Visual.Branding = TileBranding.NameAndLogo;

      var tileNotification = new TileNotification(content.GetXml());
      tileNotification.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(10);
      TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);

      // If the secondary tile is pinned
      if (Windows.UI.StartScreen.SecondaryTile.Exists("MySecondaryTile"))
      {
        // Get its updater
        var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile("MySecondaryTile");

        // And send the notification
        updater.Update(tileNotification);
      }

      //TileUpdateManager.CreateTileUpdaterForApplication().Clear();
    }

    public string XmlEncode(string text)
    {
      System.Text.StringBuilder builder = new System.Text.StringBuilder();
      using (var writer = System.Xml.XmlWriter.Create(builder))
      {
        writer.WriteString(text);
      }

      return builder.ToString();
    }

    private void Plitka2()
    {
      string from = "Jennifer Parker";
      string subject = "Photos from our trip";
      string body = "Check out these awesome photos I took while in New Zealand!";

      string content = $@"
<tile>
    <visual>

        <binding template=&#39;TileMedium&#39;>
            <text>{from}</text>
            <text hint-style=&#39;captionSubtle&#39;>{subject}</text>
            <text hint-style=&#39;captionSubtle&#39;>{body}</text>
        </binding>

        <binding template=&#39;TileWide&#39;>
            <text hint-style=&#39;subtitle&#39;>{from}</text>
            <text hint-style=&#39;captionSubtle&#39;>{subject}</text>
            <text hint-style=&#39;captionSubtle&#39;>{body}</text>
        </binding>

    </visual>
</tile>";

      // Load the string into an XmlDocument
      Windows.Data.Xml.Dom.XmlDocument doc = new Windows.Data.Xml.Dom.XmlDocument();
      doc.LoadXml(content);

      // Then create the tile notification
      var notification = new TileNotification(doc);
    }

    #region Private function

    private void Init()
    {
      #region Для всех страниц
      this.Background = SharedClass.Theme;
      #endregion Для всех страниц

      dateArrival.Date = SharedClass.DateArrival;
      timeArrival.Time = SharedClass.TimeArrival;

      InitStatus();

      txtDeparture.ItemsSource = AddressDeparture;
      txtArrival.ItemsSource = AddressArrival;

      txtDeparture.Text = SharedClass.AddressFrom;
      txtArrival.Text = SharedClass.AddressTo;

      txtDeparture.IsSuggestionListOpen = false;
      txtArrival.IsSuggestionListOpen = false;
    }

    private async void PrepareDepartureTime()
    {
      DateTime StartDateGoogleMaps = new DateTime(1970, 1, 1, 0, 0, 0);

      txtCalculateError.Visibility = Visibility.Collapsed;

      if (CheckEmptyAddress()) return;
      if (CheckEqulAddress()) return;

      var FullDateArrive = new DateTime(dateArrival.Date.Year, dateArrival.Date.Month, dateArrival.Date.Day, timeArrival.Time.Hours, timeArrival.Time.Minutes, 0);
      if (FullDateArrive <= DateTime.Now) return;

      var FullDateArriveUTC = FullDateArrive.Subtract(TimeZoneInfo.Local.BaseUtcOffset);
      var SecondsUTC = FullDateArriveUTC.Subtract(StartDateGoogleMaps).TotalSeconds;
      var departure_time = SecondsUTC.ToString();

      #region Try
      try
      {
        IsModeEnabled = false;
        CalcProgress.IsActive = true;

        #region GoogleDistanceMatrix
        //var returnValue = await LibraryRoute.GoogleDistanceMatrix(
        //  txtDeparture.Text, txtArrival.Text, SharedClass.mode.ToString(), SharedClass.avoid, SharedClass.LanguageTag, "metric", "", departure_time, SharedClass.transit_mode, SharedClass.traffic_model);

        //if (!string.IsNullOrEmpty(returnValue.Item5)) throw new ArgumentException(returnValue.Item5);

        //#region Fill addresses
        //txtDeparture.Text = returnValue.Item1;
        //txtArrival.Text = returnValue.Item2;

        //SharedClass.AddressFrom = txtDeparture.Text;
        //SharedClass.AddressTo = txtArrival.Text;
        //#endregion Fill addresses

        #endregion GoogleDistanceMatrix

        #region GoogleMapsDirections
        var returnValue = await LibraryRoute.GoogleMapsDirections
          (txtDeparture.Text, txtArrival.Text, SharedClass.mode.ToString(), "", "false", SharedClass.avoid, SharedClass.LanguageTag, "metric", "", departure_time, "", SharedClass.transit_mode, SharedClass.traffic_model, "", "");

        if (!string.IsNullOrEmpty(returnValue.Item2)) throw new ArgumentException(returnValue.Item2);

        var parseResultJson = Newtonsoft.Json.Linq.JObject.Parse(returnValue.Item1);

        #region Fill addresses
        txtDeparture.Text = ((string)parseResultJson["routes"].First["legs"].First["start_address"]).Trim();
        txtArrival.Text = ((string)parseResultJson["routes"].First["legs"].First["end_address"]).Trim();

        SharedClass.AddressFrom = txtDeparture.Text;
        SharedClass.AddressTo = txtArrival.Text;
        #endregion Fill addresses

        #region Geo position of location
        //string startLatitude = ((string)parseResultJson["routes"].First["legs"].First["start_location"]["lat"]).Trim();
        //string startLongitude = ((string)parseResultJson["routes"].First["legs"].First["start_location"]["lng"]).Trim();

        string endLatitude = ((string)parseResultJson["routes"].First["legs"].First["end_location"]["lat"]).Trim();
        string endLongitude = ((string)parseResultJson["routes"].First["legs"].First["end_location"]["lng"]).Trim();
        #endregion Geo position of location

        #region Duration
        int duration = (int)parseResultJson["routes"].First["legs"].First["duration"]["value"];

        int duration_in_traffic;
        if (SharedClass.mode == SharedClass.enumMode.driving)
        {
          duration_in_traffic = (int)parseResultJson["routes"].First["legs"].First["duration_in_traffic"]["value"];
        }
        else
        {
          duration_in_traffic = 0;
        }
        #endregion Duration

        #endregion GoogleMapsDirections

        #region Without traffic

        //var timeWithoutTraffic = new TimeSpan(0, 0, returnValue.Item3);
        var timeWithoutTraffic = new TimeSpan(0, 0, duration);

        string CalcTimeWithoutTraffic3;
        if (SharedClass.mode == SharedClass.enumMode.driving)
        {
          CalcTimeWithoutTraffic3 = SharedClass.CalcTimeWithoutTraffic3;
        }
        else
        {
          CalcTimeWithoutTraffic3 = "";          
        }

        ShowResultCalc(FullDateArrive, timeWithoutTraffic, txtDepartureTimeWithoutTraffic1, txtDepartureTimeWithoutTraffic2, txtDepartureTimeWithoutTraffic3, SharedClass.CalcTimeWithoutTraffic1, SharedClass.CalcTimeWithoutTraffic2, CalcTimeWithoutTraffic3);

        #endregion Without traffic

        #region With traffic
        //var timeWithTraffic = new TimeSpan(0, 0, returnValue.Item4);
        var timeWithTraffic = new TimeSpan(0, 0, duration_in_traffic);

        if (SharedClass.mode == SharedClass.enumMode.driving && timeWithTraffic.TotalSeconds >= 60)
        {          
          ShowResultCalc(FullDateArrive, timeWithTraffic, txtDepartureTimeWithTraffic1, txtDepartureTimeWithTraffic2, txtDepartureTimeWithTraffic3, SharedClass.CalcTimeWithTraffic1, SharedClass.CalcTimeWithTraffic2, SharedClass.CalcTimeWithTraffic3);
        }
        #endregion With traffic

        ToggleTraffik();

        if (SharedClass.IsHistorySave)
          SharedClass.HistoryAdd(double.Parse(endLatitude, NumberStyles.Float, CultureInfo.InvariantCulture), double.Parse(endLongitude, NumberStyles.Float, CultureInfo.InvariantCulture), txtArrival.Text);

        //Plitka(txtDepartureTimeWithoutTraffic1.Text, txtDepartureTimeWithoutTraffic2.Text, txtDepartureTimeWithoutTraffic3.Text, txtDepartureTimeWithTraffic1.Text, txtDepartureTimeWithTraffic2.Text, txtDepartureTimeWithTraffic3.Text);
      }
      #endregion Try

      #region Catch
      catch (Exception ex)
      {
        ShowErrorStatus();
      }
      #endregion Catch

      #region Finally
      finally
      {
        this.CalcProgress.IsActive = false;
        IsModeEnabled = true;
      }
      #endregion Finally
    }

    //private void ShowResultCalc(DateTime FullDateArrive, TimeSpan TimeRoute, TextBlock textDeparture1, TextBlock textDeparture2, TextBlock textDeparture3, string RouteTimeOut, string RouteText)
    //{
    //  var DepartureTime = FullDateArrive.Subtract(TimeRoute);

    //  var CurrentTime = DateTime.Now;

    //  if (DepartureTime > CurrentTime)
    //  {
    //    if (TimeRoute.Days > 0)
    //    {
    //      textDeparture1.Text = string.Format("{0} {1} дн. {2} ч. и {3} мин. ", RouteText, TimeRoute.Days, TimeRoute.Hours, TimeRoute.Minutes);
    //      textDeparture3.Text = string.Format("{0:d} в {1:HH':'mm}. ", DepartureTime, DepartureTime);
    //    }
    //    else if (TimeRoute.Hours > 0)
    //    {
    //      textDeparture1.Text = string.Format("{0} {1} ч. и {2} мин.", RouteText, TimeRoute.Hours, TimeRoute.Minutes);
    //      textDeparture3.Text = string.Format("{0:d} в {1:HH':'mm}. ", DepartureTime, DepartureTime);
    //    }
    //    else
    //    {
    //      textDeparture1.Text = string.Format("{0} {1} мин.", RouteText, TimeRoute.Minutes);
    //      textDeparture3.Text = string.Format("{0:d} в {1:HH':'mm}. ", DepartureTime, DepartureTime);
    //    }

    //    var DeltaTime = DepartureTime.Subtract(CurrentTime);
    //    if (DeltaTime.Days > 0)
    //    {
    //      textDeparture2.Text = string.Format("Выезжать через {0} дн. {1} ч. и {2} мин.", DeltaTime.Days, DeltaTime.Hours, DeltaTime.Minutes);
    //    }
    //    else if (DeltaTime.Hours > 0)
    //    {
    //      textDeparture2.Text = string.Format("Выезжать через {0} ч. и {1} мин.", DeltaTime.Hours, DeltaTime.Minutes);
    //    }
    //    else
    //    {
    //      if (DeltaTime.Minutes > 0)
    //      {
    //        textDeparture2.Text = string.Format("Выезжать через {0} мин.", DeltaTime.Minutes);
    //      }
    //      else
    //      {
    //        textDeparture2.Text = string.Format("Выезжать прямо сейчас.", DeltaTime.Minutes);
    //      }

    //    }
    //  }
    //  else
    //  {
    //    textDeparture1.Text = RouteTimeOut;
    //    textDeparture2.Text = "";
    //    textDeparture3.Text = "";
    //  }
    //}

    private void ShowResultCalc(DateTime FullDateArrive, TimeSpan TimeRoute, TextBlock textDeparture1, TextBlock textDeparture2, TextBlock textDeparture3, string RouteTimeOut, string RouteText, string RouteTraffic)
    {
      var DepartureTime = FullDateArrive.Subtract(TimeRoute);

      var CurrentTime = DateTime.Now;

      if (DepartureTime > CurrentTime)
      {
        if (TimeRoute.TotalSeconds <= 60 )
        {
          textDeparture1.Text = SharedClass.CalcTimeNotNeed;
          textDeparture2.Text = "";
          textDeparture3.Text = "";

          return;
        }

        #region textDeparture2 and textDeparture3

        var sdatefmt = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("shortdate");
        var stimefmt = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("shorttime");

        var sdate = sdatefmt.Format(DepartureTime);
        var stime = stimefmt.Format(DepartureTime);

        if (TimeRoute.Days > 0)
        {
          //textDeparture3.Text = string.Format("{0} {1} дн. {2} ч. и {3} мин. ", RouteText, TimeRoute.Days, TimeRoute.Hours, TimeRoute.Minutes);
          textDeparture3.Text = string.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
            RouteText, TimeRoute.Days, SharedClass.CalcTimeDay, TimeRoute.Hours, SharedClass.CalcTimeHour, SharedClass.CalcTimeAnd, TimeRoute.Minutes, SharedClass.CalcTimeMinute);

          //textDeparture2.Text = string.Format("{0:d} в {1:HH':'mm}", DepartureTime, DepartureTime);
          //textDeparture2.Text = string.Format("{0:d} {1} {2:HH':'mm}", DepartureTime, SharedClass.CalcTimeIn, DepartureTime);
          textDeparture2.Text = string.Format("{0} {1} {2}", sdate, SharedClass.CalcTimeIn, stime);
        }
        else if (TimeRoute.Hours > 0)
        {
          //textDeparture3.Text = string.Format("{0} {1} ч. и {2} мин.", RouteText, TimeRoute.Hours, TimeRoute.Minutes);
          textDeparture3.Text = string.Format("{0} {1} {2} {3} {4} {5}", RouteText, TimeRoute.Hours, SharedClass.CalcTimeHour, SharedClass.CalcTimeAnd, TimeRoute.Minutes, SharedClass.CalcTimeMinute);

          //textDeparture2.Text = string.Format("{0:d} в {1:HH':'mm}", DepartureTime, DepartureTime);
          //textDeparture2.Text = string.Format("{0:d} {1} {2:HH':'mm}", DepartureTime, SharedClass.CalcTimeIn, DepartureTime);
          textDeparture2.Text = string.Format("{0} {1} {2}", sdate, SharedClass.CalcTimeIn, stime);
        }
        else
        {
          //textDeparture3.Text = string.Format("{0} {1} мин.", RouteText, TimeRoute.Minutes);
          textDeparture3.Text = string.Format("{0} {1} {2}", RouteText, TimeRoute.Minutes, SharedClass.CalcTimeMinute);

          //textDeparture2.Text = string.Format("{0:d} в {1:HH':'mm}", DepartureTime, DepartureTime);
          //textDeparture2.Text = string.Format("{0:d} {1} {2:HH':'mm}", DepartureTime, SharedClass.CalcTimeIn, DepartureTime);
          textDeparture2.Text = string.Format("{0} {1} {2}", sdate, SharedClass.CalcTimeIn, stime);
        }
        #endregion textDeparture2 and textDeparture3

        #region textDeparture1
        var DeltaTime = DepartureTime.Subtract(CurrentTime);
        if (DeltaTime.Days > 0)
        {
          //textDeparture1.Text = string.Format("Выезжать через {0} дн. {1} ч. и {2} мин. {3}", DeltaTime.Days, DeltaTime.Hours, DeltaTime.Minutes, RouteTraffic);
          textDeparture1.Text = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}",
            SharedClass.CalcDepartureText1, DeltaTime.Days, SharedClass.CalcTimeDay, DeltaTime.Hours, SharedClass.CalcTimeHour, SharedClass.CalcTimeAnd, DeltaTime.Minutes, SharedClass.CalcTimeMinute, RouteTraffic);
        }
        else if (DeltaTime.Hours > 0)
        {
          //textDeparture1.Text = string.Format("Выезжать через {0} ч. и {1} мин. {2}", DeltaTime.Hours, DeltaTime.Minutes, RouteTraffic);
          textDeparture1.Text = string.Format("{0} {1} {2} {3} {4} {5} {6}",
            SharedClass.CalcDepartureText1, DeltaTime.Hours, SharedClass.CalcTimeHour, SharedClass.CalcTimeAnd, DeltaTime.Minutes, SharedClass.CalcTimeMinute, RouteTraffic);
        }
        else
        {
          if (DeltaTime.Minutes > 0)
          {
            //textDeparture1.Text = string.Format("Выезжать через {0} мин. {1}", DeltaTime.Minutes, RouteTraffic);
            textDeparture1.Text = string.Format("{0} {1} {2} {3}", SharedClass.CalcDepartureText1, DeltaTime.Minutes, SharedClass.CalcTimeMinute, RouteTraffic);
          }
          else
          {
            //textDeparture1.Text = string.Format("Выезжать прямо сейчас {0}", RouteTraffic);
            textDeparture1.Text = string.Format("{0} {1}", SharedClass.CalcDepartureText2, RouteTraffic);
          }
        }
        #endregion textDeparture1
      }
      else
      {
        textDeparture1.Text = RouteTimeOut;
        textDeparture2.Text = "";
        textDeparture3.Text = "";
      }
    }

    private void ModeOpacityView(FrameworkElement txtMode)
    {
      txtMode.Opacity = 1;
    }

    private void ModeOpacityHide(FrameworkElement txtMode)
    {
      txtMode.Opacity = 0.5;
    }

    private void ToggleMode(FrameworkElement txtMode, SharedClass.enumMode mode)
    {
      if (!CheckEmptyAddress())
      {
        txtModeDriving.Opacity = 0.5;
        txtModeWalking.Opacity = 0.5;
        txtModeBicycling.Opacity = 0.5;
        txtModeTransit.Opacity = 0.5;

        txtMode.Opacity = 1;

        SharedClass.mode = mode;
      }
    }

    private bool CheckEmptyAddress()
    {
      bool IsAddressEmpty = false;

      if (string.IsNullOrEmpty(this.txtArrival.Text.Trim()))
      {
        IsAddressEmpty = true;
        txtArrivalError.Visibility = Visibility.Visible;
      }

      if (string.IsNullOrEmpty(this.txtDeparture.Text.Trim()))
      {
        IsAddressEmpty = true;
        txtDepartureError.Visibility = Visibility.Visible;
      }

      return IsAddressEmpty;
    }

    private bool CheckEqulAddress()
    {
      bool IsEquilAddress;

      if (this.txtDeparture.Text.Trim() == this.txtArrival.Text.Trim())
      {
        IsEquilAddress = true;
        this.txtCalculateError.Text = SharedClass.CalcEqulAddress;
        this.txtCalculateError.Visibility = Visibility.Visible;
      }
      else
      {
        IsEquilAddress = false;
      }

      return IsEquilAddress;
    }

    private void InitStatus()
    {
      SharedClass.mode = SharedClass.enumMode.none;

      txtDepartureError.Visibility = Visibility.Collapsed;
      txtArrivalError.Visibility = Visibility.Collapsed;

      txtModeDriving.Opacity = 0.5;
      txtModeWalking.Opacity = 0.5;
      txtModeBicycling.Opacity = 0.5;
      txtModeTransit.Opacity = 0.5;

      this.txtDepartureTimeWithoutTraffic1.Visibility = Visibility.Collapsed;
      this.txtDepartureTimeWithoutTraffic2.Visibility = Visibility.Collapsed;
      this.txtDepartureTimeWithoutTraffic3.Visibility = Visibility.Collapsed;
      this.txtDepartureTimeWithTraffic1.Visibility = Visibility.Collapsed;
      this.txtDepartureTimeWithTraffic2.Visibility = Visibility.Collapsed;
      this.txtDepartureTimeWithTraffic3.Visibility = Visibility.Collapsed;

      this.txtDepartureTimeWithoutTraffic1.Text = "";
      this.txtDepartureTimeWithoutTraffic2.Text = "";
      this.txtDepartureTimeWithoutTraffic3.Text = "";

      this.txtDepartureTimeWithTraffic1.Text = "";
      this.txtDepartureTimeWithTraffic2.Text = "";
      this.txtDepartureTimeWithTraffic3.Text = "";

      this.txtCalculateError.Visibility = Visibility.Collapsed;
      this.txtCalculateError.Text = SharedClass.CalcErrorRoute;
    }

    private void ShowErrorStatus()
    {
      this.txtDepartureTimeWithoutTraffic1.Visibility = Visibility.Collapsed;
      this.txtDepartureTimeWithoutTraffic2.Visibility = Visibility.Collapsed;
      this.txtDepartureTimeWithoutTraffic3.Visibility = Visibility.Collapsed;

      this.txtDepartureTimeWithTraffic1.Visibility = Visibility.Collapsed;
      this.txtDepartureTimeWithTraffic2.Visibility = Visibility.Collapsed;
      this.txtDepartureTimeWithTraffic3.Visibility = Visibility.Collapsed;

      this.txtCalculateError.Visibility = Visibility.Visible;
    }

    private void ToggleTraffik()
    {
      this.txtDepartureTimeWithoutTraffic1.Visibility = Visibility.Visible;
      this.txtDepartureTimeWithoutTraffic2.Visibility = Visibility.Visible;
      this.txtDepartureTimeWithoutTraffic3.Visibility = Visibility.Visible;

      switch (SharedClass.mode)
      {
        case SharedClass.enumMode.driving:
          this.txtDepartureTimeWithTraffic1.Visibility = Visibility.Visible;
          this.txtDepartureTimeWithTraffic2.Visibility = Visibility.Visible;
          this.txtDepartureTimeWithTraffic3.Visibility = Visibility.Visible;
          break;
        case SharedClass.enumMode.walking:
          this.txtDepartureTimeWithTraffic1.Visibility = Visibility.Collapsed;
          this.txtDepartureTimeWithTraffic2.Visibility = Visibility.Collapsed;
          this.txtDepartureTimeWithTraffic3.Visibility = Visibility.Collapsed;
          break;
        case SharedClass.enumMode.bicycling:
          this.txtDepartureTimeWithTraffic1.Visibility = Visibility.Collapsed;
          this.txtDepartureTimeWithTraffic2.Visibility = Visibility.Collapsed;
          this.txtDepartureTimeWithTraffic3.Visibility = Visibility.Collapsed;

          break;
        case SharedClass.enumMode.transit:
          this.txtDepartureTimeWithTraffic1.Visibility = Visibility.Collapsed;
          this.txtDepartureTimeWithTraffic2.Visibility = Visibility.Collapsed;
          this.txtDepartureTimeWithTraffic3.Visibility = Visibility.Collapsed;

          break;
      }
    }

    #endregion Private function

    #region Events

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {      
      Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;

      #region Для всех страниц
      //Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible;

      var rootFrame = Window.Current.Content as Frame;
      var HeaderPage = ((rootFrame.Content as MainPage).FindName("HeaderPage") as TextBlock); //Get TextBlock
      HeaderPage.Text = SharedClass.HeaderPage1;

      Init();
      #endregion Для всех страниц
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      //if (e.Parameter != null) textBlock1.Text = e.Parameter.ToString();
    }

    #region Mode Driving

    private void txtModeDriving_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
      if (SharedClass.mode != SharedClass.enumMode.driving) ModeOpacityView((TextBlock)sender);
    }

    private void txtModeDriving_PointerExited(object sender, PointerRoutedEventArgs e)
    {
      if (SharedClass.mode != SharedClass.enumMode.driving) ModeOpacityHide((TextBlock)sender);
    }

    private void txtModeDriving_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
      if (IsModeEnabled)
      {
        //SaveAddress();
        ToggleMode((TextBlock)sender, SharedClass.enumMode.driving);
        PrepareDepartureTime();
      }
    }

    #endregion Mode Driving

    #region Mode Walking
    private void txtModeWalking_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
      if (SharedClass.mode != SharedClass.enumMode.walking) ModeOpacityView((TextBlock)sender);
    }

    private void txtModeWalking_PointerExited(object sender, PointerRoutedEventArgs e)
    {
      if (SharedClass.mode != SharedClass.enumMode.walking) ModeOpacityHide((TextBlock)sender);
    }

    private void txtModeWalking_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
      if (IsModeEnabled)
      {
        //SaveAddress();
        ToggleMode((TextBlock)sender, SharedClass.enumMode.walking);
        PrepareDepartureTime();
      }
    }

    #endregion Mode Walking

    #region Mode Bicycling
    private void txtModeBicycling_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
      if (SharedClass.mode != SharedClass.enumMode.bicycling) ModeOpacityView((Image)sender);
    }

    private void txtModeBicycling_PointerExited(object sender, PointerRoutedEventArgs e)
    {
      if (SharedClass.mode != SharedClass.enumMode.bicycling) ModeOpacityHide((Image)sender);
    }

    private void txtModeBicycling_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
      if (IsModeEnabled)
      {
        //SaveAddress();
        ToggleMode((Image)sender, SharedClass.enumMode.bicycling);
        PrepareDepartureTime();
      }
    }
    #endregion Mode Bicycling

    #region Mode Transit
    private void txtModeTransit_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
      if (SharedClass.mode != SharedClass.enumMode.transit) ModeOpacityView((TextBlock)sender);
    }

    private void txtModeTransit_PointerExited(object sender, PointerRoutedEventArgs e)
    {
      if (SharedClass.mode != SharedClass.enumMode.transit) ModeOpacityHide((TextBlock)sender);
    }

    private void txtModeTransit_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
      if (IsModeEnabled)
      {
        //SaveAddress();
        ToggleMode((TextBlock)sender, SharedClass.enumMode.transit);
        PrepareDepartureTime();
      }
    }

    #endregion Mode Transit

    //private void txtDeparture_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    //{
    //  if (!IsLockAddress)
    //  {
    //    if (!string.IsNullOrEmpty(((TextBox)sender).Text.Trim())) txtDepartureError.Visibility = Visibility.Collapsed;

    //    InitStatus();
    //  }
    //}

    //private void txtArrival_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    //{
    //  if (!IsLockAddress)
    //  {
    //    if (!string.IsNullOrEmpty(((TextBox)sender).Text.Trim())) txtArrivalError.Visibility = Visibility.Collapsed;

    //    InitStatus();
    //  }
    //}

    private async void txtDeparture_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
      if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
      {
        var asb = (sender as AutoSuggestBox);

        if (!string.IsNullOrEmpty(asb.Text.Trim())) txtDepartureError.Visibility = Visibility.Collapsed;

        InitStatus();

        if (asb.Text.Trim().Length > 2)
        {
          var res = await LibraryRoute.GooglePlaceAutocomplete(asb.Text, SharedClass.LanguageTag);
          asb.ItemsSource = res.Item1;
        }
        SharedClass.AddressFrom = asb.Text.Trim();
      }
    }

    private void txtDeparture_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
      //(sender as AutoSuggestBox).Focus(FocusState.Programmatic);
      if (args.ChosenSuggestion != null) SharedClass.AddressFrom = args.ChosenSuggestion.ToString();
    }

    private async void txtArrival_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
      if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
      {
        var asb = (sender as AutoSuggestBox);

        if (!string.IsNullOrEmpty(asb.Text.Trim())) txtArrivalError.Visibility = Visibility.Collapsed;

        InitStatus();

        if (asb.Text.Trim().Length > 2)
        {
          var res = await LibraryRoute.GooglePlaceAutocomplete(asb.Text, SharedClass.LanguageTag);
          asb.ItemsSource = res.Item1;
        }
        SharedClass.AddressTo = asb.Text.Trim();
      }
    }

    private void txtArrival_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
      //(sender as AutoSuggestBox).Focus(FocusState.Programmatic);
      if (args.ChosenSuggestion != null) SharedClass.AddressTo = args.ChosenSuggestion.ToString();
    }

    private void dateArrival_DateChanged(object sender, DatePickerValueChangedEventArgs e)
    {
      InitStatus();
      if (e.OldDate != e.NewDate)
      {
        SharedClass.DateArrival = e.NewDate.DateTime;
      }      
    }

    private void timeArrival_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
    {
      InitStatus();
      
      if (e.OldTime != e.NewTime)
      {
        SharedClass.TimeArrival = e.NewTime;
      }
    }

    #region CommandBar
    private void appbarShowOnMap_Click(object sender, RoutedEventArgs e)
    {
      var rootFrame = Window.Current.Content as Frame;
      var ScenarioFrame = ((rootFrame.Content as MainPage).FindName("ScenarioFrame") as Frame); //Get Frame in SplitView
      ScenarioFrame.Navigate(typeof(MapPage), new LocationItem { locationLatitude = 0, locationLongitude = 0, locationAddress = txtArrival.Text }); //Navigate to MapPage for Show Map
    }

    private void appbarHistory_Click(object sender, RoutedEventArgs e)
    {
      var rootFrame = Window.Current.Content as Frame;
      var ScenarioFrame = ((rootFrame.Content as MainPage).FindName("ScenarioFrame") as Frame); //Get Frame in SplitView
      ScenarioFrame.Navigate(typeof(HistoryPage)); //Navigate to HistoryPage
    }

    async private void appbarDepartureLocation_Click(object sender, RoutedEventArgs e)
    {
      #region Try
      try
      {
        IsModeEnabled = false;
        this.CalcProgress.IsActive = true;
        this.cmdBarMainPage.IsEnabled = false;

        InitStatus();
        //ToggleTraffik(mode_driving);

        var accessStatus = await Geolocator.RequestAccessAsync();

        switch (accessStatus)
        {
          case GeolocationAccessStatus.Allowed:

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;

            var geolocator = new Geolocator { DesiredAccuracyInMeters = _desireAccuracyInMetersValue };
            var position = await geolocator.GetGeopositionAsync().AsTask(token);

            if (position == null) throw new ArgumentException();

            #region Satellite
            if (position.Coordinate.PositionSource == PositionSource.Satellite)
            {
              //this.txtDepartureTimeWithTraffic1.Text = position.Coordinate.SatelliteData.HorizontalDilutionOfPrecision.ToString();
              //this.txtDepartureTimeWithTraffic2.Text = position.Coordinate.SatelliteData.VerticalDilutionOfPrecision.ToString();
              //this.txtDepartureTimeWithTraffic3.Text = position.Coordinate.SatelliteData.PositionDilutionOfPrecision.ToString();

              //this.DepartureTimeWithTraffic.Visibility = Visibility.Visible;
            }
            #endregion Satellite

            string Latitude = position.Coordinate.Point.Position.Latitude.ToString();
            string Longitude = position.Coordinate.Point.Position.Longitude.ToString();

            if (!string.IsNullOrEmpty(Latitude) && !string.IsNullOrEmpty(Longitude))
            {
              var result = await LibraryRoute.GoogleGeocodeGetAddress(Latitude, Longitude, SharedClass.LanguageTag);
              if (!string.IsNullOrEmpty(result.Item2)) throw new ArgumentOutOfRangeException(result.Item2);
              this.txtDeparture.Text = result.Item1;
              this.txtDepartureError.Visibility = Visibility.Collapsed;

              SharedClass.AddressFrom = this.txtDeparture.Text;
            }
            break;

          case GeolocationAccessStatus.Denied:
            //_rootPage.NotifyUser("Access to location is denied.", NotifyType.ErrorMessage);
            throw new ArgumentException(SharedClass.GeolocationDenied);

            //this.txtCalculateError.Visibility = Visibility.Visible;
            //this.txtCalculateError.Text = "GeolocationAccessStatus.Denied";
            break;

          case GeolocationAccessStatus.Unspecified:
            //_rootPage.NotifyUser("Unspecified error.", NotifyType.ErrorMessage);
            throw new ArgumentException(SharedClass.GeolocationUnspecified);

            //this.txtCalculateError.Visibility = Visibility.Visible;
            //this.txtCalculateError.Text = "GeolocationAccessStatus.Unspecified";
            break;
        }

      }
      #endregion Try

      #region Catch
      catch (TaskCanceledException ex)
      {
        //_rootPage.NotifyUser("Canceled.", NotifyType.StatusMessage);

        this.txtCalculateError.Text = SharedClass.GeolocationTaskCanceledException;
        this.txtCalculateError.Visibility = Visibility.Visible;
      }

      catch (ArgumentOutOfRangeException ex)
      {
        this.txtCalculateError.Text = SharedClass.GeolocationArgumentOutOfRangeException;
        this.txtCalculateError.Visibility = Visibility.Visible;
      }

      catch (ArgumentException ex)
      {
        this.txtCalculateError.Text = ex.Message;
        this.txtCalculateError.Visibility = Visibility.Visible;
      }

      catch (Exception ex)
      {
        this.txtCalculateError.Text = ex.Message;
        this.txtCalculateError.Visibility = Visibility.Visible;
        //_rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
      }
      #endregion Catch

      #region Finally
      finally
      {
        _cts = null;

        IsModeEnabled = true;
        this.CalcProgress.IsActive = false;
        this.cmdBarMainPage.IsEnabled = true;
        this.gridRoute.IsTapEnabled = true;
      }
      #endregion Finally
    }

    private void appbarExchangeAddress_Click(object sender, RoutedEventArgs e)
    {
      InitStatus();

      SharedClass.AddressFrom = txtArrival.Text;
      SharedClass.AddressTo = txtDeparture.Text;

      txtDeparture.Text = SharedClass.AddressFrom;
      txtArrival.Text = SharedClass.AddressTo;

      //var Exchange = this.txtDeparture.Text;
      //this.txtDeparture.Text = this.txtArrival.Text;
      //this.txtArrival.Text = Exchange;
    }
    #endregion CommandBar

    private void rootPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if ((sender as Pivot).SelectedIndex == 0)
      {
        this.cmdBarMainPage.Visibility = Visibility.Visible;
      }
      else
      {
        this.cmdBarMainPage.Visibility = Visibility.Collapsed;
      }
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
      //SaveAddress();
    }

    #endregion Events

    #endregion AEV Code    
  }
}
