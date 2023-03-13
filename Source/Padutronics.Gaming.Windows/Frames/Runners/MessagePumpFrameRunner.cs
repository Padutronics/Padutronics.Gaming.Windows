using Padutronics.Gaming.Frames.Runners;
using Padutronics.Gaming.Ordering;
using Padutronics.Windows.Win32.Api.WinUser;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Padutronics.Gaming.Windows.Frames.Runners;

public sealed class MessagePumpFrameRunner : IFrameRunner
{
    private readonly IGameExiter gameExiter;

    public MessagePumpFrameRunner(IGameExiter gameExiter)
    {
        this.gameExiter = gameExiter;
    }

    public Order RunOrder => WindowsRunOrders.MessagePump;

    public void Run()
    {
        var message = new MSG();

        while (GlobalFunctions.PeekMessageW(ref message, IntPtr.Zero, 0, 0, PM.PM_NOREMOVE))
        {
            int result = GlobalFunctions.GetMessageW(ref message, IntPtr.Zero, 0, 0);
            if (result == -1)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            else if (result == 0)
            {
                gameExiter.RequestExit();
                break;
            }

            GlobalFunctions.TranslateMessage(ref message);
            GlobalFunctions.DispatchMessageW(ref message);
        }
    }
}