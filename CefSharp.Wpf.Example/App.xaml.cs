// Copyright © 2011 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using CefSharp.Example;
using CefSharp.Example.Handlers;
using CefSharp.Wpf.Example.Handlers;

namespace CefSharp.Wpf.Example
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //System.AppContext.SetSwitch("Switch.System.Windows.Input.Stylus.DisableStylusAndTouchSupport", true);
            System.AppContext.SetSwitch("Switch.System.Windows.Input.Stylus.EnablePointerSupport", true);


#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                //MessageBox.Show("When running this Example outside of Visual Studio please make sure you compile in `Release` mode.", "Warning");
            }
#endif

            const bool multiThreadedMessageLoop = true;

            IBrowserProcessHandler browserProcessHandler;

            if (multiThreadedMessageLoop)
            {
                browserProcessHandler = new BrowserProcessHandler();
            }
            else
            {
                browserProcessHandler = new WpfBrowserProcessHandler(Dispatcher);
            }

            var settings = new CefSettings();
            settings.MultiThreadedMessageLoop = multiThreadedMessageLoop;
            settings.ExternalMessagePump = !multiThreadedMessageLoop;
            //Disable GPU Acceleration
            settings.CefCommandLineArgs.Add("disable-gpu");
            //settings.CefCommandLineArgs.Add("disable-gpu-compositing");
            // Enable System Wide Flash Installation
            settings.CefCommandLineArgs.Add("enable-system-flash", "1");
            settings.CefCommandLineArgs.Add("plugin-policy", "allow");
            settings.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)";
            var cacheFolder = $"{DateTime.Now:yyyyMMdd_HHmmssfff}_{Process.GetCurrentProcess().Id}";
            settings.CachePath = Path.Combine(Path.GetTempPath(), "CefSharp\\Cache", cacheFolder);

            CefExample.Init(settings, browserProcessHandler: browserProcessHandler);

            var contx = Cef.GetGlobalRequestContext();
            Cef.UIThreadTaskFactory.StartNew(delegate
            {
                contx.SetPreference("profile.default_content_setting_values.plugins", 1, out string err);
            });

            base.OnStartup(e);
        }
    }
}
