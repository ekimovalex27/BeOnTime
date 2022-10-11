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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BeOnTime
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  /// 
  public sealed partial class AboutPage : Page
  {
    public string ApplicationDisplayName
    {
      get
      {
        var package = Windows.ApplicationModel.Package.Current;
        return $"{package.DisplayName}";
      }
    }

    public string ApplicationFullVersion
    {
      get
      {
        var package = Windows.ApplicationModel.Package.Current;
        var packageId = package.Id;
        var version = packageId.Version;
        return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
      }
    }

    public AboutPage()
    {
      this.InitializeComponent();
    }

    #region AEV Code
    private void Init()
    {
      #region Для всех страниц
      this.Background = SharedClass.Theme;
      #endregion Для всех страниц

      this.ApplicationVersion.Text = ApplicationFullVersion;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      #region Для всех страниц
      Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible;

      var rootFrame = Window.Current.Content as Frame;
      var HeaderPage = ((rootFrame.Content as MainPage).FindName("HeaderPage") as TextBlock); //Get TextBlock
      HeaderPage.Text = SharedClass.HeaderPage4;

      Init();
      #endregion Для всех страниц
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      //if (e.Parameter != null) textBlock1.Text = e.Parameter.ToString();
    }

    #endregion AEV Code
  }
}
