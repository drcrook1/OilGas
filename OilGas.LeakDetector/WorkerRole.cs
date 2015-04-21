using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using OilGas.LeakDetector.Core;

namespace OilGas.LeakDetector
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        public static int InstanceRoleCount;
        public List<int> listenerPartitions;

        public override void Run()
        {
            Trace.TraceInformation("OilGas.LeakDetector is running");

            try
            {
                LeakDetectorCore.Run();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            WorkerRole.MagicVolumeSauce();
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;
            //hook up our listener for if we need our magic volume sauce.
            RoleEnvironment.Changed += (sender, args) =>
                {
                    if(WorkerRole.InstanceRoleCount != RoleEnvironment.CurrentRoleInstance.Role.Instances.Count)
                    {
                        WorkerRole.MagicVolumeSauce();
                    }
                };
            bool result = base.OnStart();

            Trace.TraceInformation("OilGas.LeakDetector has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("OilGas.LeakDetector is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("OilGas.LeakDetector has stopped");
        }
        
        public static void MagicVolumeSauce()
        {
            WorkerRole.InstanceRoleCount = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            var thisInstance = RoleEnvironment.CurrentRoleInstance.Id;
        }
    }
}
