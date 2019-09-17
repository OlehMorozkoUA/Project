using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MP3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Tracks> tracks;
        DispatcherTimer timer;
        MediaPlayer mediaPlayer = new MediaPlayer();
        bool isActive = false;
        string filename;

        TimeSpan time;
        string minutes = "";
        string seconds = "";
        int min = 0;
        int sec = 0;

        int index = 0;
        int number = 0;

        LinearGradientBrush linearGradient1;
        LinearGradientBrush linearGradient2;

        Thread thread;
        public MainWindow()
        {
            InitializeComponent();
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            timer.Start();

            tracks = new List<Tracks>();
            ListTrack.ItemsSource = tracks;

            linearGradient1 = new LinearGradientBrush();
            linearGradient1.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF343434"), 0.15));
            linearGradient1.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF535353"), 0.5));
            linearGradient1.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF343434"), 0.85));

            linearGradient2 = new LinearGradientBrush();
            linearGradient2.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF9FFF50"), 0.15));
            linearGradient2.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFC9FF6C"), 0.5));
            linearGradient2.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF9FFF50"), 0.85));

            thread = new Thread(() =>
            {
                Action actionR = blinckR;
                Action actionG = blinckG;
                Action actionB = blinckB;
                while (true)
                {
                    this.Dispatcher.BeginInvoke(actionR);
                    Thread.Sleep(400);
                    this.Dispatcher.BeginInvoke(actionG);
                    Thread.Sleep(400);
                    this.Dispatcher.BeginInvoke(actionB);
                    Thread.Sleep(400);
                }
            });

            thread.Start();
        }

        private void blinckR()
        {
            BorderPlayerShadow.Color = (Color)ColorConverter.ConvertFromString("red");
        }
        private void blinckG()
        {
            BorderPlayerShadow.Color = (Color)ColorConverter.ConvertFromString("green");
        }
        private void blinckB()
        {
            BorderPlayerShadow.Color = (Color)ColorConverter.ConvertFromString("blue");
        }

        private void blick(byte r, byte g, byte b)
        {
            BorderPlayerShadow.Color = Color.FromRgb(r, g, b);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            slider_time.Value = mediaPlayer.Position.TotalSeconds;
            time = TimeSpan.FromSeconds(mediaPlayer.Position.TotalSeconds);
            setTextBoxTime((int)time.TotalSeconds);

            if (tracks.Count > 0 && slider_time.Maximum == slider_time.Value)
            {
                number++;
                if (number >= tracks.Count) number = 0;
                mediaPlayer.Open(new Uri(tracks[number].Path));
                TrackName.Text = tracks[number].Name;
            }
        }

        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                slider_time.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                mediaPlayer.Play();
            }
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void BtnPower_Click(object sender, RoutedEventArgs e)
        {
            thread.Abort();
            this.Close();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (tracks.Count > 0)
            {
                number--;
                if (number < 0) number = tracks.Count - 1;
                mediaPlayer.Open(new Uri(tracks[number].Path));
                TrackName.Text = tracks[number].Name;
            }
        }

        private void BtnLeft_Click(object sender, RoutedEventArgs e)
        {
            if (tracks.Count > 0)
            {
                mediaPlayer.Position = time - new TimeSpan(0, 0, 10);
            }
        }

        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (tracks.Count > 0)
            {
                if (isActive)
                {
                    PackIconPlayPause.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                    PackIconPlayPause.Foreground = linearGradient1;
                    ShadowPlayPause.Color = (Color)ColorConverter.ConvertFromString("#FFF3B100");
                    mediaPlayer.Play();
                }
                else
                {
                    PackIconPlayPause.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                    PackIconPlayPause.Foreground = linearGradient2;
                    ShadowPlayPause.Color = (Color)ColorConverter.ConvertFromString("#FFC9FF6C");
                    mediaPlayer.Pause();
                }
                isActive = !isActive;
            }
        }

        private void BtnRight_Click(object sender, RoutedEventArgs e)
        {
            if (tracks.Count > 0)
            {
                mediaPlayer.Position = time + new TimeSpan(0, 0, 10);
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (tracks.Count > 0)
            {
                number++;
                if (number >= tracks.Count) number = 0;
                mediaPlayer.Open(new Uri(tracks[number].Path));
                TrackName.Text = tracks[number].Name;
            }
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                DefaultExt = ".mp3"
            };

            bool? dialogOk = openFileDialog.ShowDialog();
            if (dialogOk == true)
            {
                filename = openFileDialog.FileName;
                mediaPlayer.Open(new Uri(filename));
                TrackName.Text = System.IO.Path.GetFileName(filename);

                tracks.Add(new Tracks() { Id = index + 1, Name = System.IO.Path.GetFileName(filename), Path = filename });
                ListTrack.ItemsSource = null;
                ListTrack.ItemsSource = tracks;
                number = index;
                index++;

                if (isActive)
                {
                    PackIconPlayPause.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                    PackIconPlayPause.Foreground = linearGradient1;
                    ShadowPlayPause.Color = (Color)ColorConverter.ConvertFromString("#FFF3B100");
                    mediaPlayer.Play();
                    isActive = !isActive;
                }
            }
        }

        private void Slider_vol_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = slider_vol.Value;
        }

        private void setTextBoxTime(int totalSeconds)
        {
            min = totalSeconds / 60;
            sec = totalSeconds % 60;

            if (min < 10) minutes = "0" + min.ToString();
            else minutes = min.ToString();

            if (sec < 10) seconds = "0" + sec.ToString();
            else seconds = sec.ToString();

            TextBoxTime.Text = minutes + ":" + seconds;
        }

        private string getTextBoxTime(int totalSeconds)
        {
            min = totalSeconds / 60;
            sec = totalSeconds % 60;

            if (min < 10) minutes = "0" + min.ToString();
            else minutes = min.ToString();

            if (sec < 10) seconds = "0" + sec.ToString();
            else seconds = sec.ToString();

            return minutes + ":" + seconds;
        }

        private void Slider_time_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(slider_time.Value);
        }

        private void ListTrack_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                mediaPlayer.Open(new Uri(tracks[ListTrack.SelectedIndex].Path));
                TrackName.Text = tracks[ListTrack.SelectedIndex].Name;
                number = ListTrack.SelectedIndex;
                if (isActive)
                {
                    PackIconPlayPause.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                    PackIconPlayPause.Foreground = linearGradient1;
                    ShadowPlayPause.Color = (Color)ColorConverter.ConvertFromString("#FFF3B100");
                    isActive = !isActive;
                }
            }
            catch(Exception ex)
            {
                mediaPlayer.Open(new Uri(tracks[index].Path));
                TrackName.Text = tracks[index].Name;
                number = index;
            }
        }

        private void Slider_time_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (!isActive) mediaPlayer.Play();
        }

        private void Slider_time_GotMouseCapture(object sender, MouseEventArgs e)
        {
            if (!isActive) mediaPlayer.Pause();
        }

        private void BtnLeft_LostMouseCapture(object sender, MouseEventArgs e)
        {
            ButtonLeftPackIcon.Foreground = linearGradient1;
            ButtonLeftShadow.Color = (Color)ColorConverter.ConvertFromString("#FFF3B100");
        }

        private void BtnLeft_GotMouseCapture(object sender, MouseEventArgs e)
        {
            ButtonLeftPackIcon.Foreground = linearGradient2;
            ButtonLeftShadow.Color = (Color)ColorConverter.ConvertFromString("#FFC9FF6C");
        }

        private void BtnRight_LostMouseCapture(object sender, MouseEventArgs e)
        {
            ButtonRightPachIcon.Foreground = linearGradient1;
            ButtonRightShadow.Color = (Color)ColorConverter.ConvertFromString("#FFF3B100");
        }

        private void BtnRight_GotMouseCapture(object sender, MouseEventArgs e)
        {
            ButtonRightPachIcon.Foreground = linearGradient2;
            ButtonRightShadow.Color = (Color)ColorConverter.ConvertFromString("#FFC9FF6C");
        }
    }
}
