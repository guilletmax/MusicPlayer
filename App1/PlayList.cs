using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    class Playlist
    {
        public string name { get; set; }
        public ObservableCollection<Song> songs {get; set; }
        public Windows.UI.Xaml.Controls.ListView listView { get; set; }
    }
}
