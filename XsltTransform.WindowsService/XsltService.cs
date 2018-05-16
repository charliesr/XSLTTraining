using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Xsl;
using System.Runtime.InteropServices;
using log4net;
using System.Reflection;

namespace XsltTransform.WindowsService
{
    public partial class XsltService : ServiceBase
    {
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        private readonly string xsltFileName, xmlFileName,finalFileName;
        private readonly ILog logger;
        public XsltService()
        {
            InitializeComponent();
            this.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());
            this.xsltFileName ="\\transformer.xslt";
            this.xmlFileName = "\\catalog.xml";
            this.finalFileName = "\\final.html";

        }

        protected override void OnStart(string[] args)
        {
            try
            {
                this.logger.Debug("Starting Service");
                this.logger.Debug("xmlfilename " + this.xsltFileName);
                this.logger.Debug("xsltFileName " + this.xsltFileName);
                this.logger.Debug("finalFileName " + this.finalFileName);
                // Update the service state to Start Pending.  
                ServiceStatus serviceStatus = new ServiceStatus();
                serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
                serviceStatus.dwWaitHint = 100000;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);

                var timer = new Timer();
                timer.Interval = 20000;
                timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
                timer.Start();

                // Update the service state to Running.  
                serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex.Message, ex);
                throw;
            }

        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                this.logger.Debug("executing transform");
                var xslt = new XslTransform();
                xslt.Load(this.xsltFileName);
                xslt.Transform(this.xmlFileName, this.finalFileName);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex.Message, ex);
                throw;
            }

        }

        protected override void OnStop()
        {
            try
            {
                this.logger.Debug("Stopping");
                ServiceStatus serviceStatus = new ServiceStatus();
                serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex.Message, ex);
                throw;
            }

        }
            
    }
}
