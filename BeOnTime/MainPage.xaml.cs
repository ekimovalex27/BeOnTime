using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

using System;
using Windows.ApplicationModel.Background;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BeOnTime
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  /// 

  public sealed partial class MainPage : Page
  {
    #region Define vars
    //private SolidColorBrush PaneBackgroundSelected;
    //private SolidColorBrush PaneBackgroundUnSelected;

    //private SolidColorBrush PaneForegroundSelected = new SolidColorBrush(Windows.UI.Colors.White);
    //private SolidColorBrush PaneForegroundUnSelected = new SolidColorBrush(Windows.UI.Colors.Black);

    #endregion Define vars

    public MainPage()
    {
      InitializeComponent();
    }

    private const string taskName = "BlogFeedBackgroundTask";
    private const string taskEntryPoint = "BackgroundTasks.BlogFeedBackgroundTask";

    private async void RegisterBackgroundTask()
    {
      var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

      //if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
      //    backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
      if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
          backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
      {
        foreach (var task in BackgroundTaskRegistration.AllTasks)
        {
          if (task.Value.Name == taskName)
          {
            task.Value.Unregister(true);
          }
        }

        BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
        taskBuilder.Name = taskName;
        taskBuilder.TaskEntryPoint = taskEntryPoint;
        taskBuilder.SetTrigger(new TimeTrigger(5, false));
        var registration = taskBuilder.Register();
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      //this.RegisterBackgroundTask();

      Background = SharedClass.Theme;
      cmdHamburger.Background = SharedClass.Theme;
      //PaneBackgroundSelected= SharedClass.Theme;
      //PaneBackgroundUnSelected = (SolidColorBrush)this.Splitter.PaneBackground;

      ScenarioFrame.Navigate(typeof(CalcPage));

      //// This page is always at the top of our in-app back stack.
      //// Once it is reached there is no further back so we can always disable the title bar back UI when navigated here.
      //// If you want to you can always to the Frame.CanGoBack check for all your pages and act accordingly.
      //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
    }

    private void cmdHamburger_Click(object sender, RoutedEventArgs e)
    {
      Splitter.IsPaneOpen = (Splitter.IsPaneOpen == true) ? false : true;
    }

    private async void ListViewNavigation_ItemClick(object sender, ItemClickEventArgs e)
    {
      #region Manage splitter
      if (VisualStateGroup.CurrentState.Name.Contains("narrow"))
      {
        Splitter.IsPaneOpen = false;
      }
      #endregion Manage splitter

      var NavigationItem = (e.ClickedItem as NavigationItem);

      #region Switch to selected page
      switch (NavigationItem.PageName)
      {
        case "CalcPage":
          ScenarioFrame.Navigate(typeof(CalcPage));
          break;
        case "FavoritesPage":
          ScenarioFrame.Navigate(typeof(FavoritesPage));
          break;
        case "HistoryPage":
          ScenarioFrame.Navigate(typeof(HistoryPage));
          break;
        case "SettingsPage":
          ScenarioFrame.Navigate(typeof(SettingsPage));
          break;
        case "WhatIsNewPage":
          ScenarioFrame.Navigate(typeof(AboutPage));
          break;
        case "AboutPage":
          ScenarioFrame.Navigate(typeof(AboutPage));
          break;
        case "FeedbackPage":
          await SendFeedback();
          break;
        default:
          ScenarioFrame.Navigate(typeof(CalcPage), this.HeaderPage);
          break;
      }
      #endregion Switch to selected page

      if (ScenarioFrame.BackStackDepth >= 2)
      {
        if (ScenarioFrame.BackStack[ScenarioFrame.BackStackDepth - 1].SourcePageType == ScenarioFrame.CurrentSourcePageType)
        {
          //this.ScenarioFrame.GoBack();
          ScenarioFrame.BackStack.RemoveAt(ScenarioFrame.BackStackDepth - 1);
        }
      }

      #region test
      //  if (this.Frame.CurrentSourcePageType == typeof(SettingsPage) && this.Frame.BackStack.Count > 0)
      //  {
      //    this.Frame.BackStack.RemoveAt(this.Frame.BackStack.Count - 1);
      //  }

      //this.Frame.BackStack.Remove(this.Frame.BackStack.Last());

      //if (this.ScenarioFrame.CurrentSourcePageType== typeof(SettingsPage))
      //{
      //  if (Frame.BackStack.Count > 0)
      //  {
      //    this.Frame.BackStack.RemoveAt(this.Frame.BackStack.Count - 1);
      //  }
      //}
      #endregion test
    }

    private async Task SendFeedback()
    {
      var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
      await launcher.LaunchAsync();
    }
  }
}
