using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int count = 0;
        private MediaElement playMusic = new MediaElement();
        private Boolean playing = false;
        private Boolean songChanged = true;
        private ObservableCollection<Song> songs = new ObservableCollection<Song>();
        private ObservableCollection<Playlist> playlists = new ObservableCollection<Playlist>();
        private Song nowPlayingSong = new Song();

        public MainPage()
        {
            this.InitializeComponent();
            ObservableCollection<Song> storedSongs = (ObservableCollection<Song>)SerializeHelper.ReadData<ObservableCollection<Song>>("songs");
            if (storedSongs != null)
            {
                songs = storedSongs;
            }
            Songs_ListView.ItemsSource = songs;
            PlayList_ListView.ItemsSource = playlists;
        }

        private async Task<StorageFile> OpenFilePickerAsync()
        {
            FileOpenPicker filePicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.MusicLibrary
            };
            filePicker.FileTypeFilter.Add(".mp3");
            return await filePicker.PickSingleFileAsync();
        }

        private void SaveSong(StorageFile x)
        {
            Song song = new Song()
            {
                id = count,
                artist = x.DisplayName.Substring(0, x.DisplayName.IndexOf("-")),
                title = x.DisplayName.Substring(x.DisplayName.IndexOf("-") + 2),
                file = x
            };
            songs.Add(song);
            SerializeHelper.SaveData<ObservableCollection<Song>>("songs", songs);
            DisplaySongs();
            Saved_Song_Count_TextBlock.Text = (count += 1).ToString();
            Song_Info_TextBlock.Text = song.title;
        }

        private async void Save_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            StorageFile x = await OpenFilePickerAsync();
            SaveSong(x);
        }

        private void createQueue()
        {
            Song temp = nowPlayingSong;
            for (int i = nowPlayingSong.id + 1; i < count; i++)
            {
                temp.next = songs[i];
                temp = temp.next;
            }
            for (int i = 0; i < nowPlayingSong.id; i++)
            {
                temp.next = songs[i];
                temp = temp.next;
            }
        }

        private async void Play_Pause_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (songChanged)
            {
                if (Songs_ListView.SelectedItem == null)
                {
                    Song_Info_TextBlock.Text = "select a song";
                }
                else
                {
                    Boolean needNewQueue = true;
                    Song selectedSong = (Song)Songs_ListView.SelectedItem;
                    //if (nowPlayingSong.file != null) //just add selected song to queue
                    //{
                    //    selectedSong.previous = nowPlayingSong;
                    //    selectedSong.next = nowPlayingSong.next;
                    //    needNewQueue = false;
                    //}
                    if (needNewQueue)
                    {
                        createQueue();
                    }
                    nowPlayingSong = selectedSong;
                    playMusic.SetSource(await nowPlayingSong.file.OpenAsync(FileAccessMode.Read), nowPlayingSong.file.ContentType);
                    playMusic.Play();
                    playing = true;
                    Play_Pause_Button.Content = "Pause";
                    songChanged = false;
                }
            }
            else if (playing)
            {
                playMusic.Pause();
                playing = false;
                Play_Pause_Button.Content = "Play";
            }
            else
            {
                playMusic.Play();
                playing = true;
                Play_Pause_Button.Content = "Pause";
            }
        }

        private void CreatePlaylist()
        {
            if (NewPlaylist_TextBox.Text == "")
            {
                NewPlaylist_TextBox.Text = "NAME PLAYLIST";
            }
            else
            {
                Playlist playlist = new Playlist()
                {
                    name = NewPlaylist_TextBox.Text,
                    songs = new ObservableCollection<Song>(),
                    listView = new ListView()
                };
                playlists.Add(playlist);
                if (playlist.name.Equals("ALL_SONGS"))
                {
                    playlist.songs = songs;
                }
            }
        }

        private void DisplaySongs()
        {
            Playlist_StackPanel.Visibility = Visibility.Collapsed;
            Songs_ListView.Visibility = Visibility.Visible;
        }

        private void DisplayPlaylists()
        {
            Songs_ListView.Visibility = Visibility.Collapsed;
            Playlist_StackPanel.Visibility = Visibility.Visible;
        }

        private void DisplaySongs_Button_Click(object sender, RoutedEventArgs e)
        {
            DisplaySongs();
        }

        private void DisplayPlaylists_Button_Click(object sender, RoutedEventArgs e)
        {
            DisplayPlaylists();
        }

        private void NewPlaylist_Button_Click(object sender, RoutedEventArgs e)
        {
            CreatePlaylist();
        }

        private void Songs_ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            songChanged = true;
            Play_Pause_Button.Content = "Play";
        }

        private async void Next_Song_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (nowPlayingSong.next == null)
            {
                Song_Info_TextBlock.Text = "No songs in queue";
            }
            else
            {
                Song nextSong = nowPlayingSong.next;
                nextSong.previous = nowPlayingSong;
                nowPlayingSong = nextSong;
                Songs_ListView.SelectedIndex = nowPlayingSong.id;
                playMusic.SetSource(await nowPlayingSong.file.OpenAsync(FileAccessMode.Read), nowPlayingSong.file.ContentType);
                playMusic.Play();
                playing = true;
                Play_Pause_Button.Content = "Pause";
                songChanged = false;
            }
        }

        private async void Previous_Song_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (nowPlayingSong.previous == null)
            {
                Song_Info_TextBlock.Text = "No songs in stack";
            }
            else
            {
                Song previousSong = nowPlayingSong.previous;
                previousSong.next = nowPlayingSong;
                nowPlayingSong = previousSong;
                Songs_ListView.SelectedIndex = nowPlayingSong.id;
                playMusic.SetSource(await nowPlayingSong.file.OpenAsync(FileAccessMode.Read), nowPlayingSong.file.ContentType);
                playMusic.Play();
                playing = true;
                Play_Pause_Button.Content = "Pause";
                songChanged = false;
            }
        }
    }
}
