using Padutronics.Windows.Win32.Api.WinUser;
using System;

namespace Padutronics.Gaming.Windows.Messages;

internal sealed class DelegateMessageHandler : IMessageHandler
{
    private readonly WM message;
    private readonly MessageProcedure procedure;

    public DelegateMessageHandler(WM message, MessageProcedure procedure)
    {
        this.message = message;
        this.procedure = procedure;
    }

    public bool CanHandleMessage(WM message)
    {
        return message == this.message;
    }

    public void HandleMessage(WM message, IntPtr wParam, IntPtr lParam)
    {
        procedure(wParam, lParam);
    }
}