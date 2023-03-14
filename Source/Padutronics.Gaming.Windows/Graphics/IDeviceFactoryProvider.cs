using Padutronics.Windows.Win32.Api.D2D1;
using Padutronics.Windows.Win32.Api.DWrite;
using Padutronics.Windows.Win32.Api.Dxgi;
using Padutronics.Windows.Win32.Api.WinCodec;

namespace Padutronics.Gaming.Windows.Graphics;

public interface IDeviceFactoryProvider
{
    ID2D1Factory Direct2DFactory { get; }
    IDWriteFactory DirectWriteFactory { get; }
    IDXGIFactory DxgiFactory { get; }
    IWICImagingFactory ImagingFactory { get; }
}