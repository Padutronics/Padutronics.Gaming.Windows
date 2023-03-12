using Padutronics.Windows.Win32.Api.WinUser;
using System;

namespace Padutronics.Gaming.Windows.Messages;

public interface IMessageHandler
{
    bool CanHandleMessage(WM message);
    void HandleMessage(WM message, IntPtr wParam, IntPtr lParam);
}