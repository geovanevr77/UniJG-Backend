using Elastic.Apm;
using Elastic.Apm.Api;

namespace UniJG.Application.Abstractions.Helpers
{
    public static class ElasticLogHelper
    {
        public static void LogJsonContent(
            string type,
            string content)
        {
            ISpan span = Agent.Tracer.CurrentTransaction?.StartSpan(type, "http", "internal", content);

            if (span != null)
            {
                span.Context.Message = new Message
                {
                    Body = content
                };
            }

            span?.End();
        }
    }
}