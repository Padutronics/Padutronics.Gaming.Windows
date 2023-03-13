using Padutronics.Windows.Win32.Api.WinUser;
using System;
using System.Collections.Generic;

namespace Padutronics.Gaming.Windows.Messages;

public sealed class DestroyMessageHandler : MessageHandler
{
    protected override IReadOnlyDictionary<WM, MessageProcedure> CreateMessageToProcedureMappings()
    {
        return new Dictionary<WM, MessageProcedure>
        {
            [WM.WM_DESTROY] = HandleDestroyMessage
        };
    }

    private void HandleDestroyMessage(IntPtr wParam, IntPtr lParam)
    {
        GlobalFunctions.PostQuitMessage(0);
    }
}