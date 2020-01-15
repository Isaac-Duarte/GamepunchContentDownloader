using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamepunchContentDownloader.Enums
{
    enum Status
    {
        Pending = 0,
        Downloading = 1,
        Decompressing = 2,
        Done = 3,
        Error = 4
    }
}
