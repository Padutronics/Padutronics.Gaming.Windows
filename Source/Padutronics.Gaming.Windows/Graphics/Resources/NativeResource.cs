using Padutronics.Gaming.Graphics.Resources;
using System;

namespace Padutronics.Gaming.Windows.Graphics.Resources;

public abstract class NativeResource<TResource> : Resource, INativeResource<TResource>
    where TResource : IDisposable
{
    protected NativeResource(TResource resource)
    {
        Resource = resource;
    }

    public TResource Resource { get; }

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            Resource.Dispose();
        }
    }
}