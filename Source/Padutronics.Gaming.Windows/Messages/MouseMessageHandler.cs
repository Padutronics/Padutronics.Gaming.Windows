using Padutronics.Gaming.Inputs;
using Padutronics.Gaming.Inputs.Mouses;
using Padutronics.Geometry;
using Padutronics.Windows.Win32.Api.WinUser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Padutronics.Gaming.Windows.Messages;

public sealed class MouseMessageHandler : MessageHandler, IInputDeviceMonitor<MouseState>
{
    private static readonly IReadOnlyDictionary<MK, MouseButton> keyToButtonMappings;

    private readonly IMouseManager mouseManager;

    private Point2F mousePosition = Point2F.Zero;
    private IEnumerable<MouseButton> pressedButtons = Enumerable.Empty<MouseButton>();

    static MouseMessageHandler()
    {
        keyToButtonMappings = new Dictionary<MK, MouseButton>
        {
            [MK.MK_LBUTTON] = MouseButton.Left,
            [MK.MK_MBUTTON] = MouseButton.Middle,
            [MK.MK_RBUTTON] = MouseButton.Right,
            [MK.MK_XBUTTON1] = MouseButton.X1,
            [MK.MK_XBUTTON2] = MouseButton.X2,
        };
    }

    public MouseMessageHandler(IMouseManager mouseManager)
    {
        this.mouseManager = mouseManager;
    }

    protected override IReadOnlyDictionary<WM, MessageProcedure> CreateMessageToProcedureMappings()
    {
        return new Dictionary<WM, MessageProcedure>
        {
            [WM.WM_LBUTTONDBLCLK] = HandleLButtonDoubleClickMessage,
            [WM.WM_LBUTTONDOWN] = HandleLButtonDownMessage,
            [WM.WM_LBUTTONUP] = HandleLButtonUpMessage,
            [WM.WM_MBUTTONDBLCLK] = HandleMButtonDoubleClickMessage,
            [WM.WM_MBUTTONDOWN] = HandleMButtonDownMessage,
            [WM.WM_MBUTTONUP] = HandleMButtonUpMessage,
            [WM.WM_MOUSEMOVE] = HandleMouseMoveMessage,
            [WM.WM_MOUSEWHEEL] = HandleMouseWheelMessage,
            [WM.WM_RBUTTONDBLCLK] = HandleRButtonDoubleClickMessage,
            [WM.WM_RBUTTONDOWN] = HandleRButtonDownMessage,
            [WM.WM_RBUTTONUP] = HandleRButtonUpMessage,
            [WM.WM_XBUTTONDBLCLK] = HandleXButtonDoubleClickMessage,
            [WM.WM_XBUTTONDOWN] = HandleXButtonDownMessage,
            [WM.WM_XBUTTONUP] = HandleXButtonUpMessage
        };
    }

    private IEnumerable<MouseButton> GetMouseButtonsFromMouseKeys(MK keys)
    {
        return keyToButtonMappings
            .Where(keyToButtonMapping => keys.HasFlag(keyToButtonMapping.Key))
            .Select(keyToButtonMapping => keyToButtonMapping.Value)
            .ToList();
    }

    private Point2F GetPosition(IntPtr lParam)
    {
        int x = Macros.LOWORD(lParam);
        int y = Macros.HIWORD(lParam);

        return new Point2F(x, y);
    }

    public MouseState GetState()
    {
        return new MouseState(mousePosition, pressedButtons);
    }

    private MouseButton GetXButton(IntPtr wParam)
    {
        var button = (XBUTTON)Macros.HIWORD(wParam);

        return button switch
        {
            XBUTTON.XBUTTON1 => MouseButton.X1,
            XBUTTON.XBUTTON2 => MouseButton.X2,
            _ => MouseButton.Unspecified
        };
    }

    private void HandleButtonDoubleClickMessage(IntPtr wParam, IntPtr lParam, MouseButton button)
    {
        Point2F position = GetPosition(lParam);

        mouseManager.RaiseMouseDoubleClick(new MouseButtonInfo(position, button));

        HandleButtonDownMessage(wParam, lParam, button);
    }

    private void HandleButtonDownMessage(IntPtr wParam, IntPtr lParam, MouseButton button)
    {
        pressedButtons = GetMouseButtonsFromMouseKeys((MK)wParam);

        Point2F position = GetPosition(lParam);

        mouseManager.RaiseMouseButtonDown(new MouseButtonInfo(position, button));
    }

    private void HandleButtonUpMessage(IntPtr wParam, IntPtr lParam, MouseButton button)
    {
        pressedButtons = GetMouseButtonsFromMouseKeys((MK)wParam);

        Point2F position = GetPosition(lParam);

        mouseManager.RaiseMouseButtonUp(new MouseButtonInfo(position, button));
    }

    private void HandleLButtonDoubleClickMessage(IntPtr wParam, IntPtr lParam)
    {
        HandleButtonDoubleClickMessage(wParam, lParam, MouseButton.Left);
    }

    private void HandleLButtonDownMessage(IntPtr wParam, IntPtr lParam)
    {
        HandleButtonDownMessage(wParam, lParam, MouseButton.Left);
    }

    private void HandleLButtonUpMessage(IntPtr wParam, IntPtr lParam)
    {
        HandleButtonUpMessage(wParam, lParam, MouseButton.Left);
    }

    private void HandleMButtonDoubleClickMessage(IntPtr wParam, IntPtr lParam)
    {
        HandleButtonDoubleClickMessage(wParam, lParam, MouseButton.Middle);
    }

    private void HandleMButtonDownMessage(IntPtr wParam, IntPtr lParam)
    {
        HandleButtonDownMessage(wParam, lParam, MouseButton.Middle);
    }

    private void HandleMButtonUpMessage(IntPtr wParam, IntPtr lParam)
    {
        HandleButtonUpMessage(wParam, lParam, MouseButton.Middle);
    }

    private void HandleMouseMoveMessage(IntPtr wParam, IntPtr lParam)
    {
        Point2F position = GetPosition(lParam);

        mousePosition = position;

        mouseManager.RaiseMouseMove(new MouseMoveInfo(position));
    }

    private void HandleMouseWheelMessage(IntPtr wParam, IntPtr lParam)
    {
        Point2F position = GetPosition(lParam);

        var wheelDelta = new WheelDelta(Macros.HIWORD(wParam));

        mouseManager.RaiseMouseWheel(new MouseWheelInfo(position, wheelDelta));
    }

    private void HandleRButtonDoubleClickMessage(IntPtr wParam, IntPtr lParam)
    {
        HandleButtonDoubleClickMessage(wParam, lParam, MouseButton.Right);
    }

    private void HandleRButtonDownMessage(IntPtr wParam, IntPtr lParam)
    {
        HandleButtonDownMessage(wParam, lParam, MouseButton.Right);
    }

    private void HandleRButtonUpMessage(IntPtr wParam, IntPtr lParam)
    {
        HandleButtonUpMessage(wParam, lParam, MouseButton.Right);
    }

    private void HandleXButtonDoubleClickMessage(IntPtr wParam, IntPtr lParam)
    {
        HandleButtonDoubleClickMessage(wParam, lParam, GetXButton(wParam));
    }

    private void HandleXButtonDownMessage(IntPtr wParam, IntPtr lParam)
    {
        HandleButtonDownMessage(wParam, lParam, GetXButton(wParam));
    }

    private void HandleXButtonUpMessage(IntPtr wParam, IntPtr lParam)
    {
        HandleButtonUpMessage(wParam, lParam, GetXButton(wParam));
    }
}