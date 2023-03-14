using Padutronics.Disposing;
using Padutronics.Extensions.System;
using Padutronics.Windows.Win32.Api.D2D1;
using Padutronics.Windows.Win32.Api.DWrite;
using Padutronics.Windows.Win32.Api.Dxgi;
using Padutronics.Windows.Win32.Api.WinCodec;
using Padutronics.Windows.Win32.Api.WTypesBase;
using System;

using ComBaseApi = Padutronics.Windows.Win32.Api.ComBaseApi;
using D2D1 = Padutronics.Windows.Win32.Api.D2D1;
using DWrite = Padutronics.Windows.Win32.Api.DWrite;
using Dxgi = Padutronics.Windows.Win32.Api.Dxgi;

namespace Padutronics.Gaming.Windows.Graphics;

public sealed class DeviceFactoryProvider : DisposableObject, IDeviceFactoryProvider
{
    private readonly Lazy<ID2D1Factory> direct2DFactory;
    private readonly Lazy<IDWriteFactory> directWriteFactory;
    private readonly Lazy<IDXGIFactory> dxgiFactory;
    private readonly Lazy<IWICImagingFactory> imagingFactory;

    public DeviceFactoryProvider()
    {
        direct2DFactory = new Lazy<ID2D1Factory>(CreateDirect2DFactory);
        directWriteFactory = new Lazy<IDWriteFactory>(CreateDirectWriteFactory);
        dxgiFactory = new Lazy<IDXGIFactory>(CreateDxgiFactory);
        imagingFactory = new Lazy<IWICImagingFactory>(CreateImagingFactory);
    }

    public ID2D1Factory Direct2DFactory => direct2DFactory.Value;

    public IDWriteFactory DirectWriteFactory => directWriteFactory.Value;

    public IDXGIFactory DxgiFactory => dxgiFactory.Value;

    public IWICImagingFactory ImagingFactory => imagingFactory.Value;

    private ID2D1Factory CreateDirect2DFactory()
    {
        Guid factoryGuid = typeof(ID2D1Factory).GUID;
        var factoryOptions = new D2D1_FACTORY_OPTIONS();
#if DEBUG
        factoryOptions.debugLevel |= D2D1_DEBUG_LEVEL.D2D1_DEBUG_LEVEL_INFORMATION;
#endif

        D2D1.GlobalFunctions.D2D1CreateFactory(D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_MULTI_THREADED, ref factoryGuid, ref factoryOptions, out IntPtr factoryPointer);

        return new D2D1Factory(factoryPointer);
    }

    private IDWriteFactory CreateDirectWriteFactory()
    {
        Guid factoryGuid = typeof(IDWriteFactory).GUID;

        DWrite.GlobalFunctions.DWriteCreateFactory(DWRITE_FACTORY_TYPE.DWRITE_FACTORY_TYPE_SHARED, ref factoryGuid, out IntPtr factoryPointer);

        return new DWriteFactory(factoryPointer);
    }

    private IDXGIFactory CreateDxgiFactory()
    {
        Guid factoryGuid = typeof(IDXGIFactory).GUID;

        Dxgi.GlobalFunctions.CreateDXGIFactory(ref factoryGuid, out IntPtr factoryPointer);

        return new DXGIFactory(factoryPointer);
    }

    private IWICImagingFactory CreateImagingFactory()
    {
        Guid classGuid = CLSID.CLSID_WICImagingFactory;
        Guid factoryGuid = typeof(IWICImagingFactory).GUID;

        ComBaseApi.GlobalFunctions.CoCreateInstance(ref classGuid, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, ref factoryGuid, out IntPtr factoryPointer);

        return new WICImagingFactory(factoryPointer);
    }

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            direct2DFactory.Dispose();
            directWriteFactory.Dispose();
            dxgiFactory.Dispose();
            imagingFactory.Dispose();
        }
    }
}