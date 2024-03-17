using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.IO;
using System.Media;
using System.Windows.Shapes;
using System.Numerics;
using System.Diagnostics;

namespace Alarm
{
    public partial class MainWindow : Window
    {
        private TimeSpan _timeLeft;
        private Timer _timer;
        private MediaPlayer _player = new MediaPlayer();
        private bool _isPaused = false;
        private bool _isStopped = false;

        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine($"UI thread: {Thread.CurrentThread.ManagedThreadId}");
        }

        private void StartCountdown(object state)
        {
            do
            {
                Dispatcher.Invoke(() =>
                {
                    _timeLeft = TimeSpan.Parse(TimeInput.Text); // Reset the time each day
                    CountdownLabel.Content = _timeLeft.ToString(@"hh\:mm\:ss"); // Update the label with the reset time
                });
                while (_timeLeft.TotalSeconds > 0 && !_isStopped)
                {
                    Thread.Sleep(1000);
                    if (_isStopped) break; // Add this line
                    if (_isPaused)
                    {
                        // If paused, just wait until unpaused
                        while (_isPaused && !_isStopped)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                    _timeLeft = _timeLeft.Add(TimeSpan.FromSeconds(-1));
                    Dispatcher.Invoke(() =>
                    {
                        CountdownLabel.Content = _timeLeft.ToString(@"hh\:mm\:ss");
                    });
                }
                // Alarm function
                Dispatcher.Invoke(() =>
                {
                    if (!_isStopped) // Add this line
                    {
                        var uri = new Uri(@"D:\Over_the_Horizon.mp3", UriKind.RelativeOrAbsolute);
                        if (File.Exists(uri.LocalPath))
                        {
                            _player.Open(uri);
                            _player.Play();
                            MessageBox.Show("Time's up!");
                            _player.Stop();
                        }
                        else
                        {
                            MessageBox.Show("Time's up! There is some sound issues");
                            //open link yt
                            /*System.Diagnostics.Process.Start(new ProcessStartInfo
                            {
                                FileName = "https://www.youtube.com/",
                                UseShellExecute = true
                            });*/
                        }
                    }
                });
                    Thread.Sleep(TimeSpan.FromSeconds(5)); // Wait for 5 secs
            } while ((bool)Dispatcher.Invoke(() => RepeatCheckBox.IsChecked) && !_isStopped); // Repeat if the checkbox is checked
        }



        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate the input
            if (!TimeSpan.TryParse(TimeInput.Text, out _timeLeft))
            {
                MessageBox.Show("Invalid time format. Please enter time in the format hh:mm:ss.");
                return;
            }
            CountdownLabel.Content = _timeLeft.ToString(@"hh\:mm\:ss"); // Update the label with the input time
            _isStopped = false;
            _timer = new Timer(StartCountdown, null, 0, Timeout.Infinite);
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            _isPaused = !_isPaused;
            PauseButton.Content = _isPaused ? "Resume" : "Pause";
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _isStopped = true;
            _player.Stop();
            _timeLeft = TimeSpan.Zero;
            CountdownLabel.Content = _timeLeft.ToString(@"hh\:mm\:ss");
            _timer.Dispose(); // This will stop the timer
        }

        // Add other functions (repeat, event management) here
    }
}
