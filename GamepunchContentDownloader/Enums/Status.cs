using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamepunchContentDownloader.Enums
{
    enum Status
    {
        Waiting = 0,
        Pending = 1,
        Downloading = 2,
        Decompressing = 3,
        Done = 4,
        Error = 5
    }
}
