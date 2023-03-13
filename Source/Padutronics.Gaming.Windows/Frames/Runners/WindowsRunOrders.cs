using Padutronics.Gaming.Frames.Runners;
using Padutronics.Gaming.Ordering;

namespace Padutronics.Gaming.Windows.Frames.Runners;

internal static class WindowsRunOrders
{
    public static Order MessagePump { get; } = RunOrders.Updatable.Before();
}