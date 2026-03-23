using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Shared.Engine;

public interface IServerSideClient : IServerSideClientBase
{
    /// <summary>
    /// Gets the voice stream bit vector for the client, indicating which voice streams the client is currently subscribed to.
    /// </summary>
    public CBitVec64 VoiceStreams { get; }
    /// <summary>
    /// Gets the voice proximity bit vector for the client, indicating which other clients are within voice proximity for this client.
    /// </summary>
    public CBitVec64 VoiceProximity { get; }
    /// <summary>
    /// Gets the last client command quota start time, which is used to determine when the client's command quota will reset. This is typically used for rate-limiting client commands to prevent abuse.
    /// </summary>
    public float LastClientCommandQuotaStart { get; }
    /// <summary>
    /// Gets the time at which the client became fully connected to the server. This can be used to determine how long a client has been connected and is often used for tracking connection durations or implementing time-based features for clients.
    /// </summary>
    public float TimeClientBecameFullyConnected { get; }
    /// <summary>
    /// Gets a value indicating whether the client is currently in voice loopback mode, which means that the client's own voice is being sent back to them. This is often used for testing or debugging voice communication features.
    /// </summary>
    public bool VoiceLoopback { get; }
    /// <summary>
    /// Gets or sets the HLTV replay delay for the client, which determines how much time (in seconds) the HLTV client should delay the replay of events. This is typically used to provide a buffer for viewers watching a live broadcast, allowing them to see events with a slight delay to account for any potential latency or buffering issues.
    /// </summary>
    public int HltvReplayDelay { get; set; }
    /// <summary>
    /// Gets the tick at which the HLTV replay should stop for the client. This is used in conjunction with the replay delay to determine when the replay should end, allowing viewers to see a complete replay of events without missing any crucial moments.
    /// </summary>
    public int HltvReplayStopAt { get; }
    /// <summary>
    /// Gets the tick at which the HLTV replay should start for the client. This is used to determine when the replay should begin, allowing viewers to see the events leading up to a significant moment in the game, such as a kill or an objective capture.
    /// </summary>
    public int HltvReplayStartAt { get; }
    /// <summary>
    /// Gets the tick at which the HLTV replay slowdown should begin for the client. This is used to determine when the replay should start slowing down, allowing viewers to see important moments in slow motion for better analysis and enjoyment.
    /// </summary>
    public int HltvReplaySlowdownBeginAt { get; }
    /// <summary>
    /// Gets the tick at which the HLTV replay slowdown should end for the client. This is used to determine when the replay should stop slowing down, allowing viewers to return to normal speed after seeing important moments in slow motion.
    /// </summary>
    public int HltvReplaySlowdownEndAt { get; }
    /// <summary>
    /// Gets or sets the HLTV replay slowdown rate for the client, which determines how much the replay should be slowed down during the slowdown period. This is typically expressed as a multiplier, where a value of 0.5 would indicate that the replay should be slowed down to half speed, allowing viewers to see important moments in greater detail while still maintaining a reasonable pace for the overall replay.
    /// </summary>
    public float HltvReplaySlowdownRate { get; set; }
    /// <summary>
    /// Gets the tick at which the last HLTV replay request was made for the client. This is used to track when the client last requested a replay, allowing the server to manage replay requests and ensure that clients are not requesting replays too frequently, which could lead to performance issues or abuse of the replay system.
    /// </summary>
    public int HltvLastSendTick { get; }
    /// <summary>
    /// Gets the time at which the last HLTV replay request was made for the client. This is used to track when the client last requested a replay, allowing the server to manage replay requests and ensure that clients are not requesting replays too frequently, which could lead to performance issues or abuse of the replay system.
    /// </summary>
    public float HltvLastReplayRequestTime { get; }
}