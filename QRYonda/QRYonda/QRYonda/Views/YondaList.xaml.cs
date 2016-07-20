using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;
using QRYonda.Models;

namespace QRYonda.Views
{
    public partial class YondaList : ContentPage
    {
        TodoItemManager manager;
        ItemLookup itemLookup = new ItemLookup();
        string str = "";
        //string itemInfo = "";
        TodoItem itemInfo = new TodoItem();

        public YondaList()
        {
            InitializeComponent();

            manager = TodoItemManager.DefaultManager;

            // OnPlatform<T> doesn't currently support the "Windows" target platform, so we have this check here.
            if (manager.IsOfflineEnabled &&
                (Device.OS == TargetPlatform.Windows || Device.OS == TargetPlatform.WinPhone))
            {
                var syncButton = new Button
                {
                    Text = "Sync items",
                    MinimumHeightRequest = 15
                };
                syncButton.Clicked += OnSyncItems;

                buttonsPanel.Children.Add(syncButton);
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Set syncItems to true in order to synchronize the data on startup when running in offline mode
            await RefreshItems(true, syncItems: false);
        }

        // Data methods
        async Task AddItem(TodoItem item)
        {
            await manager.SaveTaskAsync(item);
            todoList.ItemsSource = await manager.GetTodoItemsAsync();
        }

        async Task CompleteItem(TodoItem item)
        {
            item.Done = true;
            await manager.SaveTaskAsync(item);
            todoList.ItemsSource = await manager.GetTodoItemsAsync();
        }

        /// <summary>
        /// Click method for Add Button. 
        /// </summary>
        /// <returns>The add.</returns>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        public async void OnAdd(object sender, EventArgs e)
        {
            str = newItemName.Text;
            

            if (string.IsNullOrWhiteSpace(str))
            {
                var scanPage = new ZXingScannerPage()
                {
                    Title = "ScanPage",
                    DefaultOverlayTopText = "Scan a ISBN barcode.",
                    DefaultOverlayBottomText = "",
                };
                // スキャナページを表示
                await Navigation.PushModalAsync(scanPage);

                // データが取れると発火
                scanPage.OnScanResult += (result) =>
                {
                    // スキャン停止
                    scanPage.IsScanning = false;

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        str = result.Text;

                        itemInfo = await itemLookup.Lookup(str);

                        //if (itemInfo != "")
                        if (itemInfo != null)
                        {
                            if (await DisplayAlert("Add", $"Add {itemInfo.Name} to azure?", "Yes", "No"))
                            {
                                //var todo = new TodoItem { Name = itemInfo.Name, Image = itemInfo.Image, Url = itemInfo.Url };
                                await AddItem(itemInfo);
                            }
                        }
                        else
                            await DisplayAlert("Error", "Invalid Number", "OK");

                        await Navigation.PopModalAsync();

                        newItemName.Text = string.Empty;
                        newItemName.Unfocus();

                    });
                };
            }
            else 
            {
                itemInfo = await itemLookup.Lookup(str);

                if (itemInfo != null)
                {
                    if (await DisplayAlert("Add", $"Add {itemInfo.Name} to azure?", "Yes", "No"))
                    {
                        await AddItem(itemInfo);
                    }
                }
                else
                    await DisplayAlert("Error", "Invalid Number", "OK");

            }

        }

        // Event handlers
        public void OnSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var todo = e.SelectedItem as TodoItem;
            todoList.SelectedItem = null;
            // Open amazon link with browser.

            if (todo != null)
                Device.OpenUri(new Uri(todo.Url));

            //if (Device.OS != TargetPlatform.iOS && todo != null)
            //{
            //    // Not iOS - the swipe-to-delete is discoverable there
            //    if (Device.OS == TargetPlatform.Android)
            //    {
            //        await DisplayAlert(todo.Name, "Press-and-hold to complete task " + todo.Name, "Got it!");
            //    }
            //    else
            //    {
            //        // Windows, not all platforms support the Context Actions yet
            //        if (await DisplayAlert("Mark completed?", "Do you wish to complete " + todo.Name + "?", "Complete", "Cancel"))
            //        {
            //            await CompleteItem(todo);
            //        }
            //    }
            //}

            // prevents background getting highlighted

        }


        // http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/listview/#context
        public async void OnComplete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var todo = mi.CommandParameter as TodoItem;
            await CompleteItem(todo);
        }

        // http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/listview/#pulltorefresh
        public async void OnRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            Exception error = null;
            try
            {
                await RefreshItems(false, true);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                list.EndRefresh();
            }

            if (error != null)
            {
                await DisplayAlert("Refresh Error", "Couldn't refresh data (" + error.Message + ")", "OK");
            }
        }

        public async void OnSyncItems(object sender, EventArgs e)
        {
            await RefreshItems(true, true);
        }

        private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                todoList.ItemsSource = await manager.GetTodoItemsAsync(syncItems);
            }
        }

        private class ActivityIndicatorScope : IDisposable
        {
            private bool showIndicator;
            private ActivityIndicator indicator;
            private Task indicatorDelay;

            public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
            {
                this.indicator = indicator;
                this.showIndicator = showIndicator;

                if (showIndicator)
                {
                    indicatorDelay = Task.Delay(2000);
                    SetIndicatorActivity(true);
                }
                else
                {
                    indicatorDelay = Task.FromResult(0);
                }
            }

            private void SetIndicatorActivity(bool isActive)
            {
                this.indicator.IsVisible = isActive;
                this.indicator.IsRunning = isActive;
            }

            public void Dispose()
            {
                if (showIndicator)
                {
                    indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }
    }
}

