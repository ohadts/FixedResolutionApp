using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using System.Threading;
using System.Runtime.InteropServices;

namespace FixedResolutionApp
{
    class Program
    {

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("Kernel32")]
        private static extern IntPtr GetConsoleWindow();

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        private static string resolution;
        private static int width;
        private static int hight;
        private static int frequency;
        private static Boolean isFatalErrorFound = false;
        public static string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);


        static void Main(string[] args)
        {
            
            
           string mutex_id = "MY_APP";
           using (Mutex mutex = new Mutex(false, mutex_id))
           {
               if (!mutex.WaitOne(0, false))
               {
                   MessageBox.Show("Instance Already Running!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                   return;
               }
               else
               {
                   IntPtr hwnd;
                   hwnd = GetConsoleWindow();
                   ShowWindow(hwnd, SW_HIDE);
               }

               try
               {
                   //reading the config file and saving it content to a variable
                   using (System.IO.StreamReader file = new System.IO.StreamReader(exePath + "\\config.cfg"))
                   {
                       resolution = file.ReadLine();
                   }
               }
               catch (Exception e)
               {
                   MessageBox.Show(e.Message);
               }

               //if unable to read the config file, there's no point to continue...
               if (string.IsNullOrEmpty(resolution))
               {
                   MessageBox.Show("Can't read config.cfg file !\nClosing... ");
                   isFatalErrorFound = true;
               }
               if (!isFatalErrorFound)
               {
                   string[] dim = resolution.Split(' ');
                   int.TryParse(dim[0], out width);
                   int.TryParse(dim[1], out hight);
                   int.TryParse(dim[2], out frequency);

                   if (width < 640 || hight < 480)
                   {
                       MessageBox.Show("Invaild resolution detected !\nClosing... ");
                       isFatalErrorFound = true;
                   }


               }

               //the config file content is readable, so start the event
               if (!isFatalErrorFound)
               {


                   Resolution.CResolution ChangeRes = new Resolution.CResolution(width, hight, frequency);
                   // Set the SystemEvents class to receive event notification when a user  
                   // preference changes, the palette changes, or when display settings change.
                   SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);



                   
               }
           }
           Console.Read();
            
        }

        static void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                Resolution.CResolution ChangeRes = new Resolution.CResolution(width, hight, frequency);
            }
        }

       

        
    }
}
