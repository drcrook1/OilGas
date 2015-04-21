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
using System.Collections.Concurrent;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        public static object padLock = new object();
        public static ConcurrentBag<int> SpokenForInstances = new ConcurrentBag<int>();
        public static ConcurrentBag<int> AvailableInstances = new ConcurrentBag<int>();
        public static int InstanceRoleCount;
        public List<int> MyInstancesToManage;

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole1 is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            //necessary Initialization stuff
            if(WorkerRole.AvailableInstances.Count < 1)
            {
                lock(padLock)
                {
                    if(WorkerRole.AvailableInstances.Count < 1)
                    {
                        for (int i = 0; i < 32; i++)
                        {
                            WorkerRole.AvailableInstances.Add(i + 1);
                        }
                    }
                }
            }

            this.MyInstancesToManage = WorkerRole.MagicVolumeSauce();

            
            //hook up our listener for if we need our magic volume sauce.
            RoleEnvironment.Changed += (sender, args) =>
            {
                if (WorkerRole.InstanceRoleCount != RoleEnvironment.CurrentRoleInstance.Role.Instances.Count)
                {
                    this.MyInstancesToManage = WorkerRole.MagicVolumeSauce();
                }
            };
            bool result = base.OnStart();

            Trace.TraceInformation("OilGas.LeakDetector has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }

        public static List<int> MagicVolumeSauce()
        {
            if (WorkerRole.SpokenForInstances.Count > 0)
            {
                lock (padLock)
                {
                    if (WorkerRole.SpokenForInstances.Count > 0)
                    {
                        foreach (var i in WorkerRole.SpokenForInstances)
                        {
                            int r;
                            WorkerRole.SpokenForInstances.TryTake(out r);
                            WorkerRole.AvailableInstances.Add(r);
                        }
                    }
                }
            }
            WorkerRole.InstanceRoleCount = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            int numToClaim = WorkerRole.AvailableInstances.Count / WorkerRole.InstanceRoleCount;
            List<int> InstancesToManage = new List<int>();
            for(int i = 0; i < numToClaim; i++)
            {
                int instanceToClaim = i;
                WorkerRole.AvailableInstances.TryTake(out instanceToClaim);
                if (instanceToClaim == 0)
                    continue;
                WorkerRole.SpokenForInstances.Add(instanceToClaim);
                InstancesToManage.Add(instanceToClaim);
            }
            return InstancesToManage;
        }
    }
}
