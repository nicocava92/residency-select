using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using PPI.Core;

namespace PPI.Hogan.Service
{
    public partial class Feed : ServiceBase
    {
        protected System.IO.FileSystemWatcher fileWatcher;
        
        public Feed()
        {
            InitializeComponent();            
        }
        [Log]
        protected override void OnStart(string[] args)
        {
            System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "OnStart.txt");
            var Watcher = new PPI.Hogan.Service.Utility.Util();
            Watcher.StartWatching(fileWatcher);
        }

        internal void OnDebug()
        {
            OnStart(null); // Starts the service
        }

        [Log]
        protected override void OnStop()
        {
            System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "OnStop.txt");
            if (fileWatcher != null)
            {
                fileWatcher.EnableRaisingEvents = false;
                fileWatcher.Dispose();
            }            
        }        
    }
}
