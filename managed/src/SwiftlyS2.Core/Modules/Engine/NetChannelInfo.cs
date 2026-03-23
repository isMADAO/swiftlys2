using SwiftlyS2.Shared.Engine;
using SwiftlyS2.Shared.Natives;
using SwiftlyS2.Core.Extensions;

namespace SwiftlyS2.Core.Engine;

internal class NetChannelInfo : INetChannelInfo
{
    private nint Ptr { get; set; } = 0;
    private nint VTable => Ptr.Read<nint>();

    public string Name => throw new NotImplementedException();

    public string Address => throw new NotImplementedException();

    public NetworkAddress RemoteAddress => throw new NotImplementedException();

    public float Time => throw new NotImplementedException();

    public float TimeConnected => throw new NotImplementedException();

    public int DataRate => throw new NotImplementedException();

    public bool IsLocalHost => throw new NotImplementedException();

    public bool IsLoopback => throw new NotImplementedException();

    public bool IsTimingOut => throw new NotImplementedException();

    public bool IsPlayback => throw new NotImplementedException();

    public float AvgLatency => throw new NotImplementedException();

    public float EngineLatency => throw new NotImplementedException();

    public float TimeSinceLastReceived => throw new NotImplementedException();

    public (float FrameTime, float FrameTimeStdDeviation, float FrameStartTimeStdDeviation) RemoteFramerate => throw new NotImplementedException();

    public float TimeoutSeconds => throw new NotImplementedException();

    public float TimeUntilTimeout => throw new NotImplementedException();

    public float GetAvgChoke( NetworkFlow flow )
    {
        throw new NotImplementedException();
    }

    public float GetAvgData( NetworkFlow flow )
    {
        throw new NotImplementedException();
    }

    public float GetAvgLoss( NetworkFlow flow )
    {
        throw new NotImplementedException();
    }

    public float GetAvgPacketBytes( NetworkFlow flow )
    {
        throw new NotImplementedException();
    }

    public float GetAvgPackets( NetworkFlow flow )
    {
        throw new NotImplementedException();
    }

    public int GetSequenceNr( NetworkFlow flow )
    {
        throw new NotImplementedException();
    }

    public ulong GetTotalData( NetworkFlow flow )
    {
        throw new NotImplementedException();
    }

    public int GetTotalPackets( NetworkFlow flow )
    {
        throw new NotImplementedException();
    }

    public void SetInterpolationAmount( float InterpolationAmount, float UpdateRate )
    {
        throw new NotImplementedException();
    }

    public void SetNumPredictionErrors( int num )
    {
        throw new NotImplementedException();
    }

    public void SetShowNetMessages( bool show )
    {
        throw new NotImplementedException();
    }
}