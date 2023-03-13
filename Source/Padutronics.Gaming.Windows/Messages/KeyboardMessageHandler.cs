using Padutronics.Conversion.Converters;
using Padutronics.Gaming.Inputs;
using Padutronics.Gaming.Inputs.Keyboards;
using Padutronics.Windows.Win32.Api.WinUser;
using System;
using System.Collections.Generic;

namespace Padutronics.Gaming.Windows.Messages;

public sealed class KeyboardMessageHandler : MessageHandler, IInputDeviceMonitor<KeyboardState>
{
    private readonly IConverter<VK, Key> keyConverter;
    private readonly IKeyboardManager keyboardManager;
    private readonly ICollection<Key> pressedKeys = new List<Key>();

    public KeyboardMessageHandler(IKeyboardManager keyboardManager, IConverter<VK, Key> keyConverter)
    {
        this.keyConverter = keyConverter;
        this.keyboardManager = keyboardManager;
    }

    protected override IReadOnlyDictionary<WM, MessageProcedure> CreateMessageToProcedureMappings()
    {
        return new Dictionary<WM, MessageProcedure>
        {
            [WM.WM_KEYDOWN] = HandleKeyDownMessage,
            [WM.WM_KEYUP] = HandleKeyUpMessage
        };
    }

    public KeyboardState GetState()
    {
        return new KeyboardState(pressedKeys);
    }

    private void HandleKeyDownMessage(IntPtr wParam, IntPtr lParam)
    {
        try
        {
            Key convertedKey = keyConverter.Convert((VK)wParam);
            if (convertedKey != Key.Unspecified)
            {
                if (!pressedKeys.Contains(convertedKey))
                {
                    pressedKeys.Add(convertedKey);
                }

                keyboardManager.RaiseKeyDown(convertedKey);
            }
        }
        catch (ArgumentOutOfRangeException)
        {
            // Do not handle missing keys.
        }
    }

    private void HandleKeyUpMessage(IntPtr wParam, IntPtr lParam)
    {
        try
        {
            Key convertedKey = keyConverter.Convert((VK)wParam);
            if (convertedKey != Key.Unspecified)
            {
                pressedKeys.Remove(convertedKey);

                keyboardManager.RaiseKeyUp(convertedKey);
            }
        }
        catch (ArgumentOutOfRangeException)
        {
            // Do not handle missing keys.
        }
    }
}