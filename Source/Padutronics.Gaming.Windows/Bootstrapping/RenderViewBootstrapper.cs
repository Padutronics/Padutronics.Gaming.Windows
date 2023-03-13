using Padutronics.Gaming.Bootstrapping;
using Padutronics.Gaming.Ordering;
using Padutronics.Gaming.Windows.Graphics;

namespace Padutronics.Gaming.Windows.Bootstrapping;

public sealed class RenderViewBootstrapper : IBootstrapper
{
    private readonly IRenderViewInitializer renderViewInitializer;

    public RenderViewBootstrapper(IRenderViewInitializer renderViewInitializer)
    {
        this.renderViewInitializer = renderViewInitializer;
    }

    public Order RunOrder => WindowsRunOrders.RenderView;

    public void Run()
    {
        renderViewInitializer.InitializeRenderView();
    }
}