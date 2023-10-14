using Newtonsoft.Json;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Html.Realtime.Events;
using Stryker.Core.Reporters.Json.SourceFiles;
using System.Collections.Generic;

namespace Stryker.Core.Reporters.Html.Realtime
{
    public class RealtimeMutantHandler : IRealtimeMutantHandler
    {
        private Queue<Mutant> _mutantsToReport = new Queue<Mutant>();
        public int Port => _server.Port;

        private readonly ISseServer _server;

        public RealtimeMutantHandler(StrykerOptions options, ISseServer server = null)
            => _server = server ?? new SseServer();

        public void OpenSseEndpoint() => _server.OpenSseEndpoint();

        public void CloseSseEndpoint()
        {
            _server.SendEvent(new SseEvent<string> { Event = SseEventType.Finished, Data = "" });
            _server.CloseSseEndpoint();
        }

        public void SendMutantTestedEvent(IReadOnlyMutant testedMutant)
        {
            var jsonMutant = new JsonMutant(testedMutant);
            Send(jsonMutant);
        }

        private void Send(object data)
        {
            if (_server.IsClientConnected)
            {
                // Verzend alle opgeslagen mutanten uit de wachtrij
                while (_mutantsToReport.Count > 0)
                {
                    var queuedMutant = _mutantsToReport.Dequeue();
                    _server.SendEvent(new SseEvent<object> { Event = SseEventType.MutantTested, Data = queuedMutant });
                }

                // Verzend de huidige data
                _server.SendEvent(new SseEvent<object> { Event = SseEventType.MutantTested, Data = data });
            }
            else
            {
                // Plaats de data in de wachtrij om later te worden gerapporteerd
                _mutantsToReport.Enqueue(data as Mutant);
            }
        }
    }
}

