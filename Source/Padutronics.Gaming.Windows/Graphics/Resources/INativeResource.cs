using Padutronics.Gaming.Graphics.Resources;
using System;

namespace Padutronics.Gaming.Windows.Graphics.Resources;

public interface INativeResource<out TResource> : IResource
    where TResource : IDisposable
{
    TResource Resource { get; }
}