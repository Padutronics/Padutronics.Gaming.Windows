using Padutronics.Windows.Win32.Api.WinUser;
using System;
using System.Collections.Generic;

namespace Padutronics.Gaming.Windows.Messages;

public abstract class MessageHandler : IMessageHandler
{
    private readonly Lazy<IReadOnlyDictionary<WM, MessageProcedure>> messageToProcedureMappings;

    protected MessageHandler()
    {
        messageToProcedureMappings = new Lazy<IReadOnlyDictionary<WM, MessageProcedure>>(CreateMessageToProcedureMappings);
    }

    public bool CanHandleMessage(WM message)
    {
        return messageToProcedureMappings.Value.ContainsKey(message);
    }

    protected abstract IReadOnlyDictionary<WM, MessageProcedure> CreateMessageToProcedureMappings();

    public void HandleMessage(WM message, IntPtr wParam, IntPtr lParam)
    {
        if (messageToProcedureMappings.Value.TryGetValue(message, out MessageProcedure? procedure))
        {
            procedure(wParam, lParam);
        }
    }
}