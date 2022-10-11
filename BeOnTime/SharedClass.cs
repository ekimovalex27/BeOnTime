using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml.Media;
using System.Reflection;
using Windows.Storage;
using Windows.ApplicationModel.Resources;
using Windows.Globalization;

using System.Collections.ObjectModel;

namespace BeOnTime
{
  class NavigationItem
  {
    public string Glyph { get; set; }
    public string Text { get; set; }
    public string Visibility { get; set; }
    public string PageName { get; set; }
  }

  class LocalizationItem
  {
    public string NativeName { get; set; }
    public string Culture { get; set; }
  }

  class LocationItem
  {
    public double locationLatitude { get; set; }
    public double locationLongitude { get; set; }
    public string locationAddress { get; set; }
  }

  class FavoritesItem
  {
    public string Name { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime DateInUse { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Address { get; set; }
    public string Note { get; set; }
    public bool IsPinned { get; set; }
  }

  class HistoryItem
  {
    public DateTime DateAdd { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Address { get; set; }
  }

  class SharedClass
  {
    #region Define vars

    private static ResourceLoader ResourceApp = new ResourceLoader();

    //private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
    private static ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;

    private static ApplicationDataContainer favoritesContainer = roamingSettings.CreateContainer("Favorites", ApplicationDataCreateDisposition.Always);
    private static ApplicationDataContainer historyContainer = roamingSettings.CreateContainer("History", ApplicationDataCreateDisposition.Always);
    private static object objLockListHistory = new object();

    public enum enumMode : int { none = 0, driving = 1, walking = 2, bicycling = 3, transit = 4 };

    public enum enumTrafficModel : int { best_guess = 1, pessimistic = 2, optimistic = 3 };

    private enum enumAvoid : int { tolls = 1, highways = 2, ferries = 3, indoor = 4 };

    private enum enumTransitMode : int { bus = 1, subway = 2, train = 3, tram = 4, rail = 5 };

    public enum enumTheme : int { Blue = 1, Green = 2, Black = 3 };

    #endregion Define vars

    #region Localization

    public static List<LocalizationItem> ListLocalization
    {
      get
      {
        var listLocalization = new List<LocalizationItem>();        
        Language LanguageDescription;

        foreach (var CultureName in ApplicationLanguages.ManifestLanguages)
        {
          LanguageDescription = new Language(CultureName);
          listLocalization.Add(new LocalizationItem() { Culture = CultureName.ToLower(), NativeName = LanguageDescription.NativeName });
        }
        return listLocalization;
      }
    }

    public static string LanguageTag
    {
      get
      {
        try
        {
          if (string.IsNullOrEmpty(ApplicationLanguages.PrimaryLanguageOverride))
          {
            var topUserLanguage = ApplicationLanguages.Languages[0];
            var LanguageDescription = new Language(topUserLanguage);

            return LanguageDescription.LanguageTag.ToLower();
          }
          else
          {
            return ApplicationLanguages.PrimaryLanguageOverride;
          }
        }
        catch (Exception)
        {
          var topUserLanguage = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
          var LanguageDescription = new Language(topUserLanguage);

          return LanguageDescription.LanguageTag.ToLower();
        }

        //System.Text.StringBuilder sb1 = new System.Text.StringBuilder();

        //foreach (var item in Windows.Globalization.ApplicationLanguages.Languages)
        //{
        //  sb1.Append(item);
        //  sb1.Append("_");
        //}

        //System.Text.StringBuilder sb2 = new System.Text.StringBuilder();
        //foreach (var item in Windows.System.UserProfile.GlobalizationPreferences.Languages)
        //{
        //  sb2.Append(item);
        //  sb2.Append("_");
        //}

        //if (string.IsNullOrEmpty(ApplicationLanguages.PrimaryLanguageOverride))
        //{
        //  var topUserLanguage = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
        //  var LanguageDescription = new Language(topUserLanguage);

        //  return LanguageDescription.LanguageTag.ToLower();
        //}
        //else
        //{
        //  return ApplicationLanguages.PrimaryLanguageOverride;
        //}
      }
    }

    #endregion Localization

    #region Date and Time Arrival

    public static void InitializeDateTimeArrival()
    {
      DateTime NewDateTimeArrival;
      var CurrentDateTime = DateTime.Now;

      var TimeArrival = new TimeSpan(CurrentDateTime.Hour + 2, 0, 0);
      if (TimeArrival > CurrentDateTime.TimeOfDay)
      {
        NewDateTimeArrival = new DateTime(CurrentDateTime.Year, CurrentDateTime.Month, CurrentDateTime.Day, TimeArrival.Hours, TimeArrival.Minutes, TimeArrival.Seconds);
      }
      else
      {
        NewDateTimeArrival = new DateTime(CurrentDateTime.Year, CurrentDateTime.Month, CurrentDateTime.Day, TimeArrival.Hours, TimeArrival.Minutes, TimeArrival.Seconds).AddDays(1);
      }

      DateTimeArrival = NewDateTimeArrival;
    }

    private static DateTime DateTimeArrival
    {
      get
      {
        return new DateTime(
          Convert.ToInt32(roamingSettings.Values["DateTimeArrival_Year"]),
          Convert.ToInt32(roamingSettings.Values["DateTimeArrival_Month"]),
          Convert.ToInt32(roamingSettings.Values["DateTimeArrival_Day"]),
          Convert.ToInt32(roamingSettings.Values["DateTimeArrival_Hour"]),
          Convert.ToInt32(roamingSettings.Values["DateTimeArrival_Minute"]),
          Convert.ToInt32(roamingSettings.Values["DateTimeArrival_Second"]));
      }

      set
      {
        roamingSettings.Values["DateTimeArrival_Year"] = value.Year.ToString();
        roamingSettings.Values["DateTimeArrival_Month"] = value.Month.ToString();
        roamingSettings.Values["DateTimeArrival_Day"] = value.Day.ToString();
        roamingSettings.Values["DateTimeArrival_Hour"] = value.Hour.ToString();
        roamingSettings.Values["DateTimeArrival_Minute"] = value.Minute.ToString();
        roamingSettings.Values["DateTimeArrival_Second"] = value.Second.ToString();
      }
    }

    public static DateTime DateArrival
    {
      get
      {
        return DateTimeArrival.Date;
      }

      set
      {
        var OldDateTimeArrival = DateTimeArrival;

        var NewDateTimeArrival = new DateTime(value.Year, value.Month, value.Day, OldDateTimeArrival.TimeOfDay.Hours, OldDateTimeArrival.TimeOfDay.Minutes, OldDateTimeArrival.TimeOfDay.Seconds);
        DateTimeArrival = NewDateTimeArrival;
      }
    }

    public static TimeSpan TimeArrival
    {
      get
      {
        return DateTimeArrival.TimeOfDay;
      }

      set
      {
        var OldDateTimeArrival = DateTimeArrival;

        var NewDateTimeArrival = new DateTime(OldDateTimeArrival.Year, OldDateTimeArrival.Month, OldDateTimeArrival.Day, value.Hours, value.Minutes, value.Seconds);
        DateTimeArrival = NewDateTimeArrival;
      }
    }

    #endregion Date and Time Arrival

    #region Selected pivot

    public static string HeaderPage1
    {
      get
      {
        string resource = ResourceApp.GetString("HeaderPage1");
        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string HeaderPage2
    {
      get
      {
        string resource = ResourceApp.GetString("HeaderPage2");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "Favorites";
        }
      }
    }

    public static string HeaderPage3
    {
      get
      {
        string resource = ResourceApp.GetString("HeaderPage3");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string HeaderPage4
    {
      get
      {
        string resource = ResourceApp.GetString("HeaderPage4");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string HeaderPage5
    {
      get
      {
        string resource = ResourceApp.GetString("HeaderPage5");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string HeaderPage6
    {
      get
      {
        string resource = ResourceApp.GetString("HeaderPage6");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string HeaderPage7
    {
      get
      {
        string resource = ResourceApp.GetString("HeaderPage7");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string HeaderPage8
    {
      get
      {
        string resource = ResourceApp.GetString("HeaderPage8");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "Map";
        }
      }
    }

    public static string HeaderPage9
    {
      get
      {
        string resource = ResourceApp.GetString("HeaderPage9");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "Редактирование Избранного";
        }
      }
    }
    #endregion Selected pivot

    #region Show Time Without Traffic
    public static string CalcTimeWithoutTraffic1
    {
      get
      {
        string resource = ResourceApp.GetString("CalcTimeWithoutTraffic1");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string CalcTimeWithoutTraffic2
    {
      get
      {
        string resource = ResourceApp.GetString("CalcTimeWithoutTraffic2");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string CalcTimeWithoutTraffic3
    {
      get
      {
        string resource = ResourceApp.GetString("CalcTimeWithoutTraffic3");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    #endregion Show Time Without Traffic

    #region Show Time With Traffic
    public static string CalcTimeWithTraffic1
    {
      get
      {
        string resource = ResourceApp.GetString("CalcTimeWithTraffic1");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string CalcTimeWithTraffic2
    {
      get
      {
        string resource = ResourceApp.GetString("CalcTimeWithTraffic2");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string CalcTimeWithTraffic3
    {
      get
      {
        string resource = ResourceApp.GetString("CalcTimeWithTraffic3");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    #endregion Show Time With Traffic

    #region Show Calculation Time Result
    public static string CalcTimeIn
    {
      get
      {
        string resource = ResourceApp.GetString("CalcTimeIn");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string CalcTimeDay
    {
      get
      {
        string resource = ResourceApp.GetString("CalcTimeDay");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string CalcTimeHour
    {
      get
      {
        string resource = ResourceApp.GetString("CalcTimeHour");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string CalcTimeMinute
    {
      get
      {
        string resource = ResourceApp.GetString("CalcTimeMinute");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string CalcTimeNotNeed
    {
      get
      {
        string resource = ResourceApp.GetString("CalcTimeNotNeed");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string CalcTimeAnd
    {
      get
      {
        string resource = ResourceApp.GetString("CalcTimeAnd");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string CalcDepartureText1
    {
      get
      {
        string resource = ResourceApp.GetString("CalcDepartureText1");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string CalcDepartureText2
    {
      get
      {
        string resource = ResourceApp.GetString("CalcDepartureText2");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string CalcErrorRoute
    {
      get
      {
        string resource = ResourceApp.GetString("CalcErrorRoute");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string CalcEqulAddress
    {
      get
      {
        string resource = ResourceApp.GetString("CalcEqulAddress");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    #endregion Show Calculation Time Result

    #region Address
    public static string AddressFrom
    {
      get
      {
        if (roamingSettings.Values["AddressFrom"] != null)
        {
          return (string)roamingSettings.Values["AddressFrom"];
        }
        else
        {
          return "";
        }
      }
      set
      {
        roamingSettings.Values["AddressFrom"] = value;
      }
    }

    public static string AddressTo
    {
      get
      {
        if (roamingSettings.Values["AddressTo"] != null)
        {
          return (string)roamingSettings.Values["AddressTo"];
        }
        else
        {
          return "";
        }
      }
      set
      {
        roamingSettings.Values["AddressTo"] = value;
      }
    }

    public static string DefaultCity
    {
      get
      {
        if (roamingSettings.Values["DefaultCity"] != null)
        {
          return (string)roamingSettings.Values["DefaultCity"];
        }
        else
        {
          return "";
        }
      }
      set
      {
        roamingSettings.Values["DefaultCity"] = value;
      }
    }

    #endregion Address

    #region Mode
    public static enumMode mode { get; set; }
    #endregion Mode

    #region Traffic Model

    public static bool IsModeBestGuess
    {
      get
      {
        if (roamingSettings.Values["IsModeBestGuess"] != null)
        {
          return (bool)roamingSettings.Values["IsModeBestGuess"];
        }
        else
        {
          return false;
        }
      }

      set
      {
        roamingSettings.Values["IsModeBestGuess"] = value;
      }
    }

    public static bool IsModePessimistic
    {
      get
      {
        if (roamingSettings.Values["IsModePessimistic"] != null)
        {
          return (bool)roamingSettings.Values["IsModePessimistic"];
        }
        else
        {
          if (!IsModeBestGuess && !IsModeOptimistic)
            return true;
          else
            return false;
        }
      }

      set
      {
        roamingSettings.Values["IsModePessimistic"] = value;
      }
    }

    public static bool IsModeOptimistic
    {
      get
      {
        if (roamingSettings.Values["IsModeOptimistic"] != null)
        {
          return (bool)roamingSettings.Values["IsModeOptimistic"];
        }
        else
        {
          return false;
        }
      }

      set
      {
        roamingSettings.Values["IsModeOptimistic"] = value;
      }
    }

    public static string traffic_model
    {
      get
      {
        string traffic_model_total = enumTrafficModel.pessimistic.ToString();

        if (IsModeBestGuess) traffic_model_total = enumTrafficModel.best_guess.ToString();
        if (IsModePessimistic) traffic_model_total = enumTrafficModel.pessimistic.ToString();
        if (IsModeOptimistic) traffic_model_total = enumTrafficModel.optimistic.ToString();

        return traffic_model_total;
      }
    }

    #endregion Traffic Model

    #region Avoid
    public static bool IsAvoidTolls
    {
      get
      {
        if (roamingSettings.Values["IsAvoidTolls"] != null)
        {
          return (bool)roamingSettings.Values["IsAvoidTolls"];
        }
        else
        {
          return false;
        }
      }
      set
      {
        roamingSettings.Values["IsAvoidTolls"] = value;
      }
    }

    public static bool IsAvoidHighways
    {
      get
      {
        if (roamingSettings.Values["IsAvoidHighways"] != null)
        {
          return (bool)roamingSettings.Values["IsAvoidHighways"];
        }
        else
        {
          return false;
        }
      }
      set
      {
        roamingSettings.Values["IsAvoidHighways"] = value;
      }
    }

    public static bool IsAvoidFerries
    {
      get
      {
        if (roamingSettings.Values["IsAvoidFerries"] != null)
        {
          return (bool)roamingSettings.Values["IsAvoidFerries"];
        }
        else
        {
          return false;
        }
      }
      set
      {
        roamingSettings.Values["IsAvoidFerries"] = value;
      }
    }

    public static bool IsAvoidIndoor
    {
      get
      {
        if (roamingSettings.Values["IsAvoidIndoor"] != null)
        {
          return (bool)roamingSettings.Values["IsAvoidIndoor"];
        }
        else
        {
          return false;
        }
      }
      set
      {
        roamingSettings.Values["IsAvoidIndoor"] = value;
      }
    }

    public static string avoid
    {
      get
      {
        string total = "";

        if (IsAvoidTolls) total += enumAvoid.tolls.ToString();
        if (IsAvoidHighways) total += "|" + enumAvoid.highways.ToString();
        if (IsAvoidFerries) total += "|" + enumAvoid.ferries.ToString();
        if (IsAvoidIndoor) total += "|" + enumAvoid.indoor.ToString();

        if (total.Length > 0 && total.Substring(0, 1) == "|") total = total.Substring(1);

        return total;
      }
    }

    #endregion Avoid

    #region Transit Mode
    public static bool IsTransitModeBus
    {
      get
      {
        if (roamingSettings.Values["IsTransitModeBus"] != null)
        {
          return (bool)roamingSettings.Values["IsTransitModeBus"];
        }
        else
        {
          return false;
        }
      }
      set
      {
        roamingSettings.Values["IsTransitModeBus"] = value;
      }
    }

    public static bool IsTransitModeSubway
    {
      get
      {
        if (roamingSettings.Values["IsTransitModeSubway"] != null)
        {
          return (bool)roamingSettings.Values["IsTransitModeSubway"];
        }
        else
        {
          return false;
        }
      }
      set
      {
        roamingSettings.Values["IsTransitModeSubway"] = value;
      }
    }

    public static bool IsTransitModeTrain
    {
      get
      {
        if (roamingSettings.Values["IsTransitModeTrain"] != null)
        {
          return (bool)roamingSettings.Values["IsTransitModeTrain"];
        }
        else
        {
          return false;
        }
      }
      set
      {
        roamingSettings.Values["IsTransitModeTrain"] = value;
      }
    }

    public static bool IsTransitModeTram
    {
      get
      {
        if (roamingSettings.Values["IsTransitModeTram"] != null)
        {
          return (bool)roamingSettings.Values["IsTransitModeTram"];
        }
        else
        {
          return false;
        }
      }
      set
      {
        roamingSettings.Values["IsTransitModeTram"] = value;
      }
    }

    public static bool IsTransitModeRail
    {
      get
      {
        if (roamingSettings.Values["IsTransitModeRail"] != null)
        {
          return (bool)roamingSettings.Values["IsTransitModeRail"];
        }
        else
        {
          return false;
        }
      }
      set
      {
        roamingSettings.Values["IsTransitModeRail"] = value;
      }
    }

    public static string transit_mode
    {
      get
      {
        string total = "";
        if (IsTransitModeBus) total += enumTransitMode.bus.ToString();
        if (IsTransitModeSubway) total += "|" + enumTransitMode.subway.ToString();
        if (IsTransitModeTrain) total += "|" + enumTransitMode.train.ToString();
        if (IsTransitModeTram) total += "|" + enumTransitMode.tram.ToString();

        if (total.Length > 0 && total.Substring(0, 1) == "|") total = total.Substring(1);

        return total;
      }
    }

    #endregion Transit Mode

    #region Theme

    public static bool IsThemeBlue
    {
      get
      {
        if (roamingSettings.Values["IsThemeBlue"] != null)
        {
          return (bool)roamingSettings.Values["IsThemeBlue"];
        }
        else
        {
          return false;
        }
      }
      set
      {
        roamingSettings.Values["IsThemeBlue"] = value;
      }
    }

    public static bool IsThemeGreen
    {
      get
      {
        if (roamingSettings.Values["IsThemeGreen"] != null)
        {
          return (bool)roamingSettings.Values["IsThemeGreen"];
        }
        else
        {
          return false;
        }
      }
      set
      {
        roamingSettings.Values["IsThemeGreen"] = value;
      }
    }

    public static bool IsThemeBlack
    {
      get
      {
        if (roamingSettings.Values["IsThemeBlack"] != null)
        {
          return (bool)roamingSettings.Values["IsThemeBlack"];
        }
        else
        {
          return false;
        }
      }
      set
      {
        roamingSettings.Values["IsThemeBlack"] = value;
      }
    }

    public static SolidColorBrush Theme
    {
      get
      {
        SolidColorBrush returnBrush;

        string Theme_total = enumTheme.Black.ToString();

        if (IsThemeBlue) Theme_total = enumTheme.Blue.ToString();
        if (IsThemeGreen) Theme_total = enumTheme.Green.ToString();
        if (IsThemeBlack) Theme_total = enumTheme.Black.ToString();

        var prop = typeof(Colors).GetRuntimeProperty(Theme_total);
        if (prop != null)
        {
          var c = (Color)prop.GetValue(null);
          returnBrush = new SolidColorBrush(c);
        }
        else
        {
          returnBrush = new SolidColorBrush(Colors.Black);
        }

        return returnBrush;
      }
    }

    public static string SettingsThemeDialogTitle
    {
      get
      {
        string resource = ResourceApp.GetString("SettingsThemeDialogTitle");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string SettingsThemeDialogText
    {
      get
      {
        string resource = ResourceApp.GetString("SettingsThemeDialogText");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    #endregion Theme

    #region Favorites

    private static string GetFavoritesIdFormat()
    {
      return "yyyyMMddHHmmssfffffff";
    }

    private static string GetFavoritesId(DateTime locationDate)
    {
      return locationDate.ToString(GetFavoritesIdFormat());
    }

    public static string FavoritesDeleteDialogContent
    {
      get
      {
        string resource = ResourceApp.GetString("FavoritesDeleteDialogContent");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string FavoritesDeleteDialogPrimaryButtonText
    {
      get
      {
        string resource = ResourceApp.GetString("FavoritesDeleteDialogPrimaryButtonText");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "Yes";
        }
      }
    }

    public static string FavoritesDeleteDialogSecondaryButtonText
    {
      get
      {
        string resource = ResourceApp.GetString("FavoritesDeleteDialogSecondaryButtonText");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "No";
        }
      }
    }

    public static List<FavoritesItem> ListFavorites
    {
      get
      {
        var listFavorites = new List<FavoritesItem>();
        foreach (var item in favoritesContainer.Values)
        {
          var favoritesItem = item.Value as ApplicationDataCompositeValue;

          listFavorites.Add(new FavoritesItem
          {
            Name = (string)favoritesItem["Name"],            
            DateAdd = DateTime.ParseExact((string)favoritesItem["DateAdd"], GetFavoritesIdFormat(), CultureInfo.InvariantCulture),
            DateInUse = DateTime.ParseExact((string)favoritesItem["DateInUse"], GetFavoritesIdFormat(), CultureInfo.InvariantCulture),
            Latitude= ConvertToDouble(favoritesItem["Latitude"]),
            Longitude = ConvertToDouble(favoritesItem["Longitude"]),
            Address = (string)favoritesItem["Address"],
            Note = (string)favoritesItem["Note"],
            IsPinned = (bool)favoritesItem["IsPinned"]
          });
        }

        return listFavorites;
      }
    }

    public static void FavoritesAdd(string Name, double Latitude, double Longitude, string Address, string Note, bool IsPinned)
    {
      var locationDateId = GetFavoritesId(DateTime.Now);
      var DateInUse = GetFavoritesId(DateTime.MinValue);

      var composite = new ApplicationDataCompositeValue
      {
        ["Name"] = Name,        
        ["DateAdd"] = locationDateId,
        ["DateInUse"] = DateInUse,
        ["Latitude"] = Latitude,
        ["Longitude"] = Longitude,
        ["Address"] = Address,
        ["Note"] = Note,
        ["IsPinned"] = IsPinned
      };

      favoritesContainer.Values.Add("FavoritesItem" + locationDateId, composite);
    }

    public static void FavoritesEdit(DateTime DateAdd, string Name)
    {
      var locationDateId = "FavoritesItem" + GetFavoritesId(DateAdd);

      favoritesContainer.Values.TryGetValue(locationDateId, out var item);
      var favoritesItem = item as ApplicationDataCompositeValue;

      favoritesContainer.Values.Remove(locationDateId);

      FavoritesAdd(Name, ConvertToDouble(favoritesItem["Latitude"]), ConvertToDouble(favoritesItem["Longitude"]), (string)favoritesItem["Address"], (string)favoritesItem["Note"], (bool)favoritesItem["IsPinned"]);
    }

    public static void FavoritesRemove(DateTime locationDate)
    {
      favoritesContainer.Values.Remove("FavoritesItem" + GetFavoritesId(locationDate));
    }

    public static void FavoritesUpdateDateInUse(DateTime DateAdd)
    {
      favoritesContainer.Values.TryGetValue("FavoritesItem" + GetFavoritesId(DateAdd), out var item);
      var favoritesItem = item as ApplicationDataCompositeValue;
    }

    #endregion Favorites

    #region History
    private static string GetHistoryIdFormat()
    {
      return "yyyyMMddHHmmssfffffff";
    }

    private static string GetHistoryId(DateTime locationDate)
    {
      return locationDate.ToString(GetHistoryIdFormat());
    }

    public static bool IsHistorySave
    {
      get
      {
        if (roamingSettings.Values["IsHistorySave"] != null)
        {
          return (bool)roamingSettings.Values["IsHistorySave"];
        }
        else
        {
          return false;
        }
      }

      set
      {
        roamingSettings.Values["IsHistorySave"] = value;
      }
    }

    public static string SettingsHistoryClearDialogContent
    {
      get
      {
        string resource = ResourceApp.GetString("SettingsHistoryClearDialogContent");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string SettingsHistoryClearDialogPrimaryButtonText
    {
      get
      {
        string resource = ResourceApp.GetString("SettingsHistoryClearDialogPrimaryButtonText");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string SettingsHistoryClearDialogSecondaryButtonText
    {
      get
      {
        string resource = ResourceApp.GetString("SettingsHistoryClearDialogSecondaryButtonText");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string HistoryAddToFavoritesTitle
    {
      get
      {
        string resource = ResourceApp.GetString("HistoryAddToFavoritesTitle");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string HistoryAddToFavoritesNote
    {
      get
      {
        string resource = ResourceApp.GetString("HistoryAddToFavoritesNote");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static List<HistoryItem> ListHistory
    {
      get
      {
        CultureInfo provider = CultureInfo.InvariantCulture;

        var listHistory = new List<HistoryItem>();        
        foreach (var item in historyContainer.Values)
        {
          var historyItem = item.Value as ApplicationDataCompositeValue;

          listHistory.Add(new HistoryItem {
            DateAdd = DateTime.ParseExact((string)historyItem["locationDate"], GetHistoryIdFormat(), provider),
            Latitude = ConvertToDouble(historyItem["locationLatitude"]),
            Longitude = ConvertToDouble(historyItem["locationLongitude"]),
            Address = (string)historyItem["locationAddress"] });
        }

        return listHistory;

        // Для версии программы выше 1.1.8.0, так как в 8 значения координат хранятся, как текст "Lat" и "Long"
        // После ухода с версии 1.1.8.0 этот код надо убрать
      }
    }

    public static void HistoryAdd(double locationLatitude, double locationLongitude, string locationAddress)
    {
      foreach (var item in historyContainer.Values)
      {
        var historyItem = item.Value as ApplicationDataCompositeValue;

        if ((string)historyItem["locationAddress"] == locationAddress)
        {
          historyContainer.Values.Remove(item.Key);
        }
      }

      var locationDateId = GetHistoryId(DateTime.Now);

      var composite = new ApplicationDataCompositeValue
      {
        ["locationDate"] = locationDateId,
        ["locationLatitude"] = locationLatitude,
        ["locationLongitude"] = locationLongitude,
        ["locationAddress"] = locationAddress
      };

      historyContainer.Values.Add("HistoryItem" + locationDateId, composite);
    }

    public static void HistoryRemove(DateTime locationDate)
    {
      historyContainer.Values.Remove("HistoryItem" + GetHistoryId(locationDate));
    }

    public static void HistoryClear()
    {
      lock (objLockListHistory)
      {
        historyContainer.Values.Clear();
      }
    }

    #endregion History

    #region Location

    public static string GeolocationArgumentOutOfRangeException
    {
      get
      {
        string resource = ResourceApp.GetString("GeolocationArgumentOutOfRangeException");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string GeolocationDenied
    {
      get
      {
        string resource = ResourceApp.GetString("GeolocationDenied");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string GeolocationTaskCanceledException
    {
      get
      {
        string resource = ResourceApp.GetString("GeolocationTaskCanceledException");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    public static string GeolocationUnspecified
    {
      get
      {
        string resource = ResourceApp.GetString("GeolocationUnspecified");

        if (resource != null)
        {
          return resource;
        }
        else
        {
          return "";
        }
      }
    }

    #endregion Location

    private static double ConvertToDouble(object objectPosition)
    {
      double doublePosition;
      if (!double.TryParse(objectPosition.ToString(), out doublePosition)) doublePosition = 0;
      return doublePosition;
    }
  }
}
