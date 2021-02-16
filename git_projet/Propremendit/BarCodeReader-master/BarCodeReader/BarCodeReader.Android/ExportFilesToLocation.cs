using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BarCodeReader.Droid;
using BarCodeReader.Interfaces;
using Java.IO;
using Xamarin.Forms;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BarCodeReader.Droid
{
    public class ExportFilesToLocation : IExportFilesToLocation
    {
        public ExportFilesToLocation()
        {
        }

        public string GetFolderLocation()
        {
            string root = null;
            if (Android.OS.Environment.IsExternalStorageEmulated)
            {
                root = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            }
            else
                root = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            File myDir = new File(root + "/Exports");
            if (!myDir.Exists())
                myDir.Mkdir();

            return root + "/Exports/";
        }
    }
}