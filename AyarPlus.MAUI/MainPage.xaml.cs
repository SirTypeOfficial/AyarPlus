namespace AyarPlus.MAUI
{
    public partial class MainPage : ContentPage
    {
        private bool _isPlaying = true;
        private bool _isSliderDragging = false;

        public MainPage()
        {
            InitializeComponent();
            StartProgressUpdater();
        }

        private void OnPlayPauseClicked(object? sender, EventArgs e)
        {
            _isPlaying = !_isPlaying;
            LottieView.IsAnimationEnabled = _isPlaying;
            PlayPauseBtn.Text = _isPlaying ? "⏸️ Pause" : "▶️ Play";
        }

        private void OnRestartClicked(object? sender, EventArgs e)
        {
            LottieView.Progress = TimeSpan.Zero;
            _isPlaying = true;
            LottieView.IsAnimationEnabled = true;
            PlayPauseBtn.Text = "⏸️ Pause";
        }

        private void OnSliderValueChanged(object? sender, ValueChangedEventArgs e)
        {
            _isSliderDragging = true;
            
            var duration = LottieView.Duration;
            if (duration.TotalMilliseconds > 0)
            {
                LottieView.Progress = TimeSpan.FromMilliseconds(duration.TotalMilliseconds * e.NewValue);
            }
            
            ProgressLabel.Text = $"{(int)(e.NewValue * 100)}%";
            _isSliderDragging = false;
        }

        private void StartProgressUpdater()
        {
            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(50), () =>
            {
                if (!_isSliderDragging && LottieView.Duration.TotalMilliseconds > 0)
                {
                    var progress = LottieView.Progress.TotalMilliseconds / LottieView.Duration.TotalMilliseconds;
                    ProgressSlider.Value = progress;
                    ProgressLabel.Text = $"{(int)(progress * 100)}%";
                }
                return true;
            });
        }
    }
}
