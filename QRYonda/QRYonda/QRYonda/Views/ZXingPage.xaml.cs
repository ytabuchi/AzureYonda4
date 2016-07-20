using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.Net.Mobile.Forms;

using Xamarin.Forms;

namespace QRYonda.Views
{
    public partial class ZXingPage : ContentPage
    {
        public ZXingPage()
        {
            InitializeComponent();
        }

        async void ScanButtonClicked(object sender, EventArgs s)
        {
            // スキャナページの設定
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

                System.Diagnostics.Debug.WriteLine(result.Text);

                // スキャンしたデータが正しいか確認し、良ければ元のページに戻る
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var res = await DisplayAlert("Scanned", result.Text, "OK", "Cancel");
                    if (res)
                        await Navigation.PopModalAsync();
                    else
                        scanPage.IsScanning = true;

                });
            };
        }
    }
}
