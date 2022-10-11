using System;
using System.Linq;

using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;

public class SharedLibraryRoute
{
  private readonly string GoogleAPIKey1 = "***";
  private readonly string GoogleAPIKey2 = "***";

  public class RouteStep
  {
    string distance_text { get; set; }
    string distance_value { get; set; }
    string duration_text { get; set; }
    string duration_value { get; set; }
    string html_instructions { get; set; }
    string start_location_lat { get; set; }
    string start_location_lng { get; set; }
    string travel_mode { get; set; }
  }

  #region Google

  public async Task<Tuple<string, string, int, int, string>> GoogleDistanceMatrix(
    string origins, string destinations, string mode, string avoid, string language, string units, string region, string departure_time, string transit_mode, string traffic_model)
  {
    #region Define vars
    string query;
    int duration_in_traffic;
    Tuple<string, string, int, int, string> returnValue;
    #endregion Define vars

    #region Try
    try
    {
      #region Set query
      query = string.Format("origins={0}&destinations={1}&departure_time={2}&mode={3}&language={4}&units={5}&key={6}",
        origins, destinations, departure_time, mode, language, units, GoogleAPIKey1);

      if (!string.IsNullOrEmpty(avoid))
      {
        query += string.Format("&avoid={0}", avoid);
      }

      if (mode == "driving" && !string.IsNullOrEmpty(traffic_model))
      {
        query += string.Format("&traffic_model={0}", traffic_model);
      }

      if (mode == "transit" && !string.IsNullOrEmpty(transit_mode))
      {
        query += string.Format("&transit_mode={0}", transit_mode);
      }
      #endregion Set query

      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri("https://maps.googleapis.com");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync("maps/api/distancematrix/json?" + query);
        response.EnsureSuccessStatusCode();

        var resultJson = await response.Content.ReadAsStringAsync();
        var parseResultJson = Newtonsoft.Json.Linq.JObject.Parse(resultJson);

        string origin_addresses = ((string)parseResultJson["origin_addresses"][0]).Trim();
        string destination_addresses = ((string)parseResultJson["destination_addresses"][0]).Trim();

        int duration = (int)parseResultJson["rows"].First.First.First.First.First.Next.First.First.Next;

        if (mode == "driving")
        {
          duration_in_traffic = (int)parseResultJson["rows"].First.First.First.First.First.Next.Next.First.First.Next;
        }
        else
        {
          duration_in_traffic = 0;
        }

        var ErrorMessage = "";
        returnValue = new Tuple<string, string, int, int, string>(origin_addresses, destination_addresses, duration, duration_in_traffic, ErrorMessage);
      }
    }
    #endregion Try

    #region Catch
    catch (Exception ex)
    {
      returnValue = new Tuple<string, string, int, int, string>("", "", 0, 0, ex.Message);
    }
    #endregion Catch

    return returnValue;
  }

  //public async Task<Tuple<string, string, int, int, string>> GoogleMapsDirections(
  public async Task<Tuple<string, string>> GoogleMapsDirections(
    string origin, string destination, string mode, string waypoints, string alternatives, string avoid, string language, string unit, string region, string departure_time, string arrival_time, string transit_mode, string traffic_model, string transit_routing_preference, string optimize)
  {
    #region Define vars
    Tuple<string, string> returnValue;
    //Tuple<string, string, int, int, string> returnValue;
    #endregion Define vars

    #region Try
    try
    {
      #region Set query
      var query = string.Format("origin={0}&destination={1}&key={2}&mode={3}&alternatives={4}&language={5}&unit={6}&departure_time={7}",
        origin, destination, GoogleAPIKey1, mode, alternatives, language, unit, departure_time);

      if (!string.IsNullOrEmpty(avoid))
      {
        query += string.Format("&avoid={0}", avoid);
      }

      if (mode == "driving" && !string.IsNullOrEmpty(traffic_model))
      {
        query += string.Format("&traffic_model={0}", traffic_model);
      }

      if (mode == "transit" && !string.IsNullOrEmpty(transit_mode))
      {
        query += string.Format("&transit_mode={0}", transit_mode);
      }
      #endregion Set query

      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri("https://maps.googleapis.com");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync("maps/api/directions/json?" + query);
        response.EnsureSuccessStatusCode();

        string resultJson = await response.Content.ReadAsStringAsync();

        #region Calculation is transfered to calling procedure
        //var parseResultJson = Newtonsoft.Json.Linq.JObject.Parse(resultJson);

        //#region Fill addresses
        //string origin_addresses = ((string)parseResultJson["routes"].First["legs"].First["start_address"]).Trim();
        //string destination_addresses = ((string)parseResultJson["routes"].First["legs"].First["end_address"]).Trim();
        //#endregion Fill addresses

        //#region Geo position of location
        //string startLatitude = ((string)parseResultJson["routes"].First["legs"].First["start_location"]["lat"]).Trim();
        //string startLongitude = ((string)parseResultJson["routes"].First["legs"].First["start_location"]["lng"]).Trim();

        //string endLatitude = ((string)parseResultJson["routes"].First["legs"].First["end_location"]["lat"]).Trim();
        //string endLongitude = ((string)parseResultJson["routes"].First["legs"].First["end_location"]["lng"]).Trim();
        //#endregion Geo position of location

        //#region Duration
        //int duration = (int)parseResultJson["routes"].First["legs"].First["duration"]["value"];

        //int duration_in_traffic;
        //if (mode == "driving")
        //{
        //  duration_in_traffic = (int)parseResultJson["routes"].First["legs"].First["duration_in_traffic"]["value"];
        //}
        //else
        //{
        //  duration_in_traffic = 0;
        //}
        #endregion Calculation is transfered to calling procedure

        var ErrorMessage = "";
        returnValue = new Tuple<string, string>(resultJson, ErrorMessage);
        //returnValue = new Tuple<string, string>(origin_addresses, destination_addresses, duration, duration_in_traffic, ErrorMessage);
      }
    }
    #endregion Try

    #region Catch
    catch (Exception ex)
    {
      //returnValue = new Tuple<string, string, int, int, string>("", "", 0, 0, ex.Message);
      returnValue = new Tuple<string, string>("", ex.Message);
    }
    #endregion Catch

    return returnValue;
  }

  //https://developers.google.com/maps/documentation/geocoding/start?hl=ru
  public async Task<Tuple<string, string>> GoogleGeocodeGetAddress(string Latitude, string Longitude, string language)
  {
    #region Define vars
    Tuple<string, string> returnValue;
    #endregion Define vars

    var latlng = Latitude.Replace(",", ".") + "," + Longitude.Replace(",", ".");

    var query = string.Format("latlng={0}&language={1}&key={2}", latlng, language, GoogleAPIKey1);

    #region Try
    try
    {
      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri("https://maps.googleapis.com");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync("maps/api/geocode/json?" + query);
        response.EnsureSuccessStatusCode();

        var resultJson = await response.Content.ReadAsStringAsync();
        var parseResultJson = Newtonsoft.Json.Linq.JObject.Parse(resultJson);

        var formatted_address = parseResultJson.First.Children()[0].First().Children().First().Next.First.ToString();

        if (string.IsNullOrEmpty(formatted_address)) throw new ArgumentException("formatted_address is empty");

        var ErrorMessage = "";
        returnValue = new Tuple<string, string>(formatted_address, ErrorMessage);
      }
    }
    #endregion Try

    #region Catch
    catch (Exception ex)
    {
      returnValue = new Tuple<string, string>("", ex.Message);
    }
    #endregion Catch

    return returnValue;
  }

  //https://developers.google.com/maps/documentation/geocoding/start?hl=ru
  // Метод не доделан - нет аналаза разбора json
  public async Task<Tuple<string, string, string>> GoogleGeocodeGetPosition(string Address, string Language)
  {
    #region Define vars
    Tuple<string, string, string> returnValue;
    #endregion Define vars

    var query = string.Format("address={0}&language={1}&key={2}", Address, Language, GoogleAPIKey1);

    #region Try
    try
    {
      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri("https://maps.googleapis.com");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync("maps/api/geocode/json?" + query);
        response.EnsureSuccessStatusCode();

        var resultJson = await response.Content.ReadAsStringAsync();
        var parseResultJson = Newtonsoft.Json.Linq.JObject.Parse(resultJson);

        string Latitude = (string)parseResultJson["results"].First["geometry"]["location"]["lat"];
        string Longitude = (string)parseResultJson["results"].First["geometry"]["location"]["lng"];

        if (string.IsNullOrEmpty(Latitude) || string.IsNullOrEmpty(Longitude)) throw new ArgumentException("location is empty");

        var ErrorMessage = "";
        returnValue = new Tuple<string, string, string>(Latitude, Longitude, ErrorMessage);
      }
    }
    #endregion Try

    #region Catch
    catch (Exception ex)
    {
      returnValue = new Tuple<string, string, string>("0", "0", ex.Message);
    }
    #endregion Catch

    return returnValue;
  }

  public async Task<Tuple<List<string>, string>> GooglePlaceAutocomplete(string input, string language)
  {
    #region Define vars
    Tuple<List<string>, string> returnValue;    
    #endregion Define vars

    var query = string.Format("input={0}&language={1}&key={2}&types=address", input, language, GoogleAPIKey2);

    #region Try
    try
    {
      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri("https://maps.googleapis.com");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync("maps/api/place/autocomplete/json?" + query);
        response.EnsureSuccessStatusCode();

        var resultJson = await response.Content.ReadAsStringAsync();
        var parseResultJson = Newtonsoft.Json.Linq.JObject.Parse(resultJson);

        var ListAddress = new List<string>();
        foreach (var item in parseResultJson.First.First)
        {
          ListAddress.Add(item["description"].ToString());
        }

        var ErrorMessage = "";
        returnValue = new Tuple<List<string>, string>(ListAddress, ErrorMessage);
      }
    }
    #endregion Try

    #region Catch
    catch (Exception ex)
    {
      returnValue = new Tuple<List<string>, string>(new List<string>(), ex.Message);
    }
    #endregion Catch

    return returnValue;
  }

  #endregion Google

  #region Microsoft
  ////Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
  ////Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx) 

  //private static async Task<ListDictionary> GetLanguagesNativeAsync(string locale, string[] languageCodes)
  //{
  //  string[] languageNames;
  //  ListDictionary ListNative;

  //  try
  //  {
  //    var admAuth = new AdmAuthentication(UserData.clientID, UserData.clientSecret);
  //    var admToken = admAuth.GetAccessToken();
  //    var headerValue = "Bearer " + admToken.access_token;

  //    var uri = "http://api.microsofttranslator.com/v2/Http.svc/GetLanguageNames?locale=" + locale;
  //    // create the request
  //    var request = WebRequest.Create(uri) as HttpWebRequest;
  //    request.Headers.Add("Authorization", headerValue);
  //    request.ContentType = "text/xml";
  //    request.Method = "POST";
  //    var dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String[]"));
  //    using (var stream = await request.GetRequestStreamAsync())
  //    {
  //      dcs.WriteObject(stream, languageCodes);
  //    }

  //    WebResponse response = null;
  //    try
  //    {
  //      response = request.GetResponse();

  //      using (var stream = response.GetResponseStream())
  //      {
  //        languageNames = (string[])dcs.ReadObject(stream);
  //      }

  //      ListNative = new ListDictionary();
  //      for (int i = 0; i < languageCodes.Length; i++)
  //      {
  //        //ListNative.Add(languageCodes[i], languageNames[i]);
  //        ListNative.Add(languageCodes[i], languageNames[i].Substring(0, 1).ToUpperInvariant() + languageNames[i].Substring(1));
  //      }
  //    }
  //    catch
  //    {
  //      ListNative = null;
  //    }
  //    finally
  //    {
  //      if (response != null)
  //      {
  //        response.Close();
  //        response = null;
  //      }
  //    }

  //  }
  //  catch (Exception)
  //  {
  //    ListNative = null;
  //  }

  //  return ListNative;
  //}
  #endregion Microsoft
}

namespace SharedLibrary
{
  //public class SharedLibrary : ContentPage
  //{
  //  public SharedLibrary()
  //  {
  //    //Original
  //    //var button = new Button
  //    //{
  //    //  Text = "Click Me!",
  //    //  VerticalOptions = LayoutOptions.CenterAndExpand,
  //    //  HorizontalOptions = LayoutOptions.CenterAndExpand,
  //    //};

  //    //int clicked = 0;
  //    //button.Clicked += (s, e) => button.Text = "Clicked: " + clicked++;

  //    //Content = button;


  //  }

  //}
}
