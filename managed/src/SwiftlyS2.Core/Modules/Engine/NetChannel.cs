using SwiftlyS2.Shared.Engine;

namespace SwiftlyS2.Core.Engine;

internal class NetChannel : NetChannelInfo, INetChannel
{
    public bool CanPacket => throw new NotImplementedException();

    public bool IsOverflowed => throw new NotImplementedException();

    public bool HasPendingReliableData => throw new NotImplementedException();

    public bool IsTimedOut => throw new NotImplementedException();

    public bool IsSuppressingTransmit => throw new NotImplementedException();

    public void SetMaxDataRate( int rate )
    {
        throw new NotImplementedException();
    }

    public void SetMinDataRate( int rate )
    {
        throw new NotImplementedException();
    }

    public void SetTimeout( float seconds, bool forceExact = false )
    {
        throw new NotImplementedException();
    }

    public void SuppressTransmit( bool suppress )
    {
        throw new NotImplementedException();
    }
}