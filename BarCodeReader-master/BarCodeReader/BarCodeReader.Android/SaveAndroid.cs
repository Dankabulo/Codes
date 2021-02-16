using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BarCodeReader.Interfaces;
using System.IO;
using BarCodeReader.Droid;
using Android.Content;
using Java.IO;
using Xamarin.Forms;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

[assembly: Dependency(typeof(SaveAndroid))]

namespace BarCodeReader.Droid
{

    class SaveAndroid : ISave
    {
        //Method to save document as a file in Android and view the saved document.
        public void SaveAndView(string fileName, String contentType, MemoryStream stream)
        {
            string exception = string.Empty;
            string root = null;

            //Get the root path of android device.
            if (Android.OS.Environment.IsExternalStorageEmulated)
            {
                root = Android.OS.Environment.ExternalStorageDirectory.ToString();
            }
            else
                root = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            //Create directory and file.
            Java.IO.File myDir = new Java.IO.File(root + "/Syncfusion");
            myDir.Mkdir();

            Java.IO.File file = new Java.IO.File(myDir, fileName);

            //Remove the file if exists.
            if (file.Exists()) file.Delete();

            try
            {
                FileOutputStream outs = new FileOutputStream(file);
                outs.Write(stream.ToArray());

                outs.Flush();
                outs.Close();
            }
            catch (Exception e)
            {
                exception = e.ToString();
            }

            //Launch the saved file for viewing in default viewer.
            //if (file.Exists())
            //{
            //    Android.Net.Uri path = Android.Net.Uri.FromFile(file);
            //    string extension = Android.Webkit.MimeTypeMap.GetFileExtensionFromUrl(Android.Net.Uri.FromFile(file).ToString());
            //    string mimeType = Android.Webkit.MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);
            //    Intent intent = new Intent(Intent.ActionView);
            //    intent.SetDataAndType(path, mimeType);
            //    context.StartActivity(Intent.CreateChooser(intent, "Choose App"));
            //}
        }
    }
}