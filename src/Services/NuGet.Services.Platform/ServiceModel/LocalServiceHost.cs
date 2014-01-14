﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;

namespace NuGet.Services.ServiceModel
{
    public class LocalServiceHost : ServiceHost
    {
        private ServiceHostDescription _description;

        public IList<NuGetService> Services { get; private set; }
        public override ServiceHostDescription Description { get { return _description; } }
        public IDictionary<string, string> Configuration { get; private set; }

        public IObservable<EventEntry> EventSource { get; private set; }

        public LocalServiceHost(ServiceHostName name) : this(name, new Dictionary<string, string>()) { }
        public LocalServiceHost(ServiceHostName name, IDictionary<string, string> configuration)
        {
            _description = new ServiceHostDescription(name, Environment.MachineName);
            Services = new List<NuGetService>();

            Configuration = configuration ?? new Dictionary<string, string>();
        }

        protected override void InitializeLocalLogging()
        {
            var events = new ObservableEventListener();
            events.EnableEvents(SemanticLoggingEventSource.Log, EventLevel.LogAlways);
            events.EnableEvents(ServicePlatformEventSource.Log, EventLevel.LogAlways);
            EventSource = events;
        }

        protected override void InitializeCloudLogging()
        {
        }

        protected override IEnumerable<NuGetService> GetServices()
        {
            return Services;
        }

        protected override NuGetService GetManagementService()
        {
            return null;
        }

        public override string GetConfigurationSetting(string fullName)
        {
            string str;
            if (!Configuration.TryGetValue(fullName, out str))
            {
                return null;
            }
            return str;
        }
    }
}
