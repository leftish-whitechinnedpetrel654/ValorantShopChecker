using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CheckShopApp
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<SkinOffer> _skins = new ObservableCollection<SkinOffer>();
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private int _secondsRemaining;
        private bool _busy;

        public MainWindow()
        {
            InitializeComponent();
            ShopItems.ItemsSource = _skins;

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) => TickCountdown();

            Loaded += async (s, e) => await LoadShopAsync();
        }

        private async Task LoadShopAsync()
        {
            if (_busy) return;
            _busy = true;
            _timer.Stop();
            RefreshButton.IsEnabled = false;
            ShowState(AppState.Loading);

            try
            {
                // Ağ + kimlik işleri arka planda; UI donmaz.
                ShopSnapshot snap = await Task.Run(() =>
                {
                    RiotSession session = RiotAuth.Login();
                    return RiotStore.GetShop(session);
                });

                _skins.Clear();
                foreach (SkinOffer skin in snap.Skins)
                    _skins.Add(skin);

                VpText.Text = snap.Wallet.ValorantPoints.ToString("N0");
                RadText.Text = snap.Wallet.RadianitePoints.ToString("N0");
                RegionText.Text = snap.Region.ToUpperInvariant();

                _secondsRemaining = snap.SecondsRemaining;
                UpdateCountdownLabel();
                _timer.Start();

                ShowState(AppState.Shop);
            }
            catch (Exception ex)
            {
                ErrorText.Text = ex.Message;
                ShowState(AppState.Error);
            }
            finally
            {
                RefreshButton.IsEnabled = true;
                _busy = false;
            }
        }

        private enum AppState { Loading, Shop, Error }

        private void ShowState(AppState state)
        {
            LoadingPanel.Visibility = state == AppState.Loading ? Visibility.Visible : Visibility.Collapsed;
            ShopPanel.Visibility = state == AppState.Shop ? Visibility.Visible : Visibility.Collapsed;
            ErrorPanel.Visibility = state == AppState.Error ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TickCountdown()
        {
            _secondsRemaining--;
            if (_secondsRemaining <= 0)
            {
                _secondsRemaining = 0;
                _timer.Stop();
            }
            UpdateCountdownLabel();
        }

        private void UpdateCountdownLabel()
        {
            TimeSpan t = TimeSpan.FromSeconds(Math.Max(0, _secondsRemaining));
            CountdownText.Text = $"{(int)t.TotalHours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadShopAsync();

        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
