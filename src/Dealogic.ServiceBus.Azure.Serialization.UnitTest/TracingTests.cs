namespace Dealogic.ServiceBus.Azure.Serialization.UnitTest
{
    using System.Diagnostics.Tracing;
    using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Utility;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TracingTests
    {
        [TestMethod]
        public void EventSourceConsistencyTest()
        {
            var eventSource = new PrivateType("Dealogic.ServiceBus.Azure.Serialization", "Dealogic.ServiceBus.Azure.Serialization.Tracing.ServiceBusSerializationEventSource");
            var log = (EventSource)eventSource.GetStaticProperty("Log");
            EventSourceAnalyzer.InspectAll(log);
        }
    }
}