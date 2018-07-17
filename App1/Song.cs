using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace App1
{
    class Song
    {
        public int id { get; set; }
        public string title { get; set; }
        public string artist { get; set; }
        public StorageFile file { get; set; }
        public Song next { get; set; }
        public Song previous { get; set; }
    }
}
