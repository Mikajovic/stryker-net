﻿using Stryker.Core.Reporters.Html.Realtime.Events;

namespace Stryker.Core.Reporters.Html.Realtime;

public interface ISseServer
{
    bool IsClientConnected { get; }
    public int Port { get; set; }

    public void OpenSseEndpoint();
    public void SendEvent<T>(SseEvent<T> @event);
    public void CloseSseEndpoint();
}
