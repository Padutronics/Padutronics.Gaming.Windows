using Padutronics.Gaming.Graphics;
using Padutronics.Gaming.Windows.Messages;
using Padutronics.Geometry;
using Padutronics.Windows.Win32.Api.WinDef;
using Padutronics.Windows.Win32.Api.WinUser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Padutronics.Gaming.Windows.Graphics;

public sealed class RenderView : IRenderView, IRenderViewInitializer
{
    private const WS WindowStyle = WS.WS_OVERLAPPEDWINDOW;

    private readonly Lazy<IEnumerable<IMessageHandler>> messageHandlers;
    private readonly Lazy<IntPtr> windowHandle;
    private readonly WNDPROC windowProcedure;

    private Size size = Size.Zero;

    public RenderView(IEnumerable<IMessageHandler> additionalMessageHandlers, RenderViewOptions options)
    {
        messageHandlers = new Lazy<IEnumerable<IMessageHandler>>(() => CreateMessageHandlers(additionalMessageHandlers));
        windowHandle = new Lazy<IntPtr>(() => CreateWindowHandle(options.Size));

        // Store window procedure reference in a field, so it won't be garbage collected.
        windowProcedure = WindowProcedure;
    }

    public Size Size
    {
        get => size;
        private set
        {
            if (size != value)
            {
                size = value;
                OnSizeChanged(new SizeEventArgs(Size));
            }
        }
    }

    public event EventHandler<SizeEventArgs>? SizeChanged;

    private Point2 CalculateWindowPosition(Size windowSize)
    {
        int screenWidth = GlobalFunctions.GetSystemMetrics(SM.SM_CXSCREEN);
        int screenHeight = GlobalFunctions.GetSystemMetrics(SM.SM_CYSCREEN);

        int x = (screenWidth - windowSize.Width) / 2;
        int y = (screenHeight - windowSize.Height) / 2;

        return new Point2(x, y);
    }

    private Size CalculateWindowSize(Size desiredSize)
    {
        var desiredRectangle = new RECT
        {
            bottom = desiredSize.Height,
            right = desiredSize.Width
        };

        if (!GlobalFunctions.AdjustWindowRectEx(ref desiredRectangle, WindowStyle, false, 0))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        int width = desiredRectangle.right + -desiredRectangle.left;
        int height = desiredRectangle.bottom + -desiredRectangle.top;

        return new Size(width, height);
    }

    private IEnumerable<IMessageHandler> CreateMessageHandlers(IEnumerable<IMessageHandler> additionalMessageHandlers)
    {
        var defaultMessageHandlers = new[]
        {
            new DelegateMessageHandler(WM.WM_SIZE, HandleSizeMessage)
        };

        // Default message handlers must be placed before additional ones to get higher priority.
        return defaultMessageHandlers
            .Concat(additionalMessageHandlers)
            .ToList();
    }

    private IntPtr CreateWindowHandle(Size desiredSize)
    {
        var windowClass = new WNDCLASSEXW
        {
            cbSize = Marshal.SizeOf<WNDCLASSEXW>(),
            hCursor = GlobalFunctions.LoadCursorW(IntPtr.Zero, new IntPtr(IDC.IDC_ARROW)),
            hInstance = Process.GetCurrentProcess().Handle,
            lpfnWndProc = windowProcedure,
            lpszClassName = "PadutronicsGameWindow",
            style = CS.CS_DBLCLKS | CS.CS_HREDRAW | CS.CS_VREDRAW
        };

        ushort classAtom = GlobalFunctions.RegisterClassExW(ref windowClass);
        if (classAtom == 0)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        Size windowSize = CalculateWindowSize(desiredSize);
        Point2 windowPosition = CalculateWindowPosition(windowSize);

        return GlobalFunctions.CreateWindowExW(
            0,
            new IntPtr(classAtom),
            "Padutronics Game",
            WindowStyle | WS.WS_VISIBLE,
            windowPosition.X,
            windowPosition.Y,
            windowSize.Width,
            windowSize.Height,
            IntPtr.Zero,
            IntPtr.Zero,
            IntPtr.Zero,
            IntPtr.Zero
        );
    }

    private void HandleSizeMessage(IntPtr wParam, IntPtr lParam)
    {
        if ((int)wParam == (int)SIZE.SIZE_RESTORED)
        {
            int width = Macros.LOWORD(lParam);
            int height = Macros.HIWORD(lParam);

            Size = new Size(width, height);
        }
    }

    public void InitializeRenderView()
    {
        // Trigger lazy creation of render view window.
        _ = windowHandle.Value;
    }

    private void OnSizeChanged(SizeEventArgs e)
    {
        SizeChanged?.Invoke(this, e);
    }

    private IntPtr WindowProcedure(IntPtr hwnd, WM uMsg, IntPtr wParam, IntPtr lParam)
    {
        IntPtr result = IntPtr.Zero;

        var isMessageHandled = false;

        foreach (IMessageHandler messageHandler in messageHandlers.Value)
        {
            if (messageHandler.CanHandleMessage(uMsg))
            {
                messageHandler.HandleMessage(uMsg, wParam, lParam);

                isMessageHandled = true;
                break;
            }
        }

        if (!isMessageHandled)
        {
            result = GlobalFunctions.DefWindowProcW(hwnd, uMsg, wParam, lParam);
        }

        return result;
    }
}