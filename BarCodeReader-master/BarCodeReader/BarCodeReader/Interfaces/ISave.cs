using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Android.Content;

namespace BarCodeReader.Interfaces
{
    public interface ISave
    {
        void SaveAndView(string filename, string contentType, MemoryStream stream);
    }
}
