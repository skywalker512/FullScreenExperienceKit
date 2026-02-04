using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FullScreenExperienceShell
{

    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial ObservableCollection<ObservableAppItem> Applications { get; set; } = [];

        public List<AppItem> AppItems = [];
    }

    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel = new();

        public MainPage()
        {
            InitializeComponent();
        }

        public async Task RefreshAppList()
        {
            await Task.Run(() =>
            {
                AppsFolder.GetApplications(ViewModel.AppItems);
            });

            AppsFolder.InitApplicationList(ViewModel.AppItems, ViewModel.Applications);

        }

        private async void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            await RefreshAppList();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await RefreshAppList();
        }

        private void TreeView_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            var appItem = (ObservableAppItem)args.InvokedItem;
            if (appItem.Type == AppItemType.Container)
            {
                appItem.Expanded = !appItem.Expanded;
            }
            else
            {
                if (string.IsNullOrEmpty(appItem.ParsingPath))
                {
                    return;
                }

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = $@"shell:appsfolder\{appItem.ParsingPath}",
                    UseShellExecute = true
                };
                Process.Start(processStartInfo);
            }
        }
    }
}
