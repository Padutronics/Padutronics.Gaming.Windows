using Padutronics.Gaming.Bootstrapping;
using Padutronics.Gaming.Ordering;

namespace Padutronics.Gaming.Windows.Bootstrapping;

internal static class WindowsRunOrders
{
    public static Order RenderView { get; } = RunOrders.StartScene.After();
}