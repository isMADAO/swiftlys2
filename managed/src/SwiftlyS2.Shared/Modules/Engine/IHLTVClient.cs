namespace SwiftlyS2.Shared.Engine;

public interface IHLTVClient : IServerSideClient
{
    /// <summary>
    /// Gets or sets the password for the HLTV client, which is used to authenticate the client when connecting to the HLTV server. This is typically required for clients that want to access certain features or view specific content on the HLTV server, and it helps ensure that only authorized clients can access sensitive information or perform certain actions on the server.
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// Gets or sets the chat group for the HLTV client, which is used to determine which chat messages the client will receive. This allows the HLTV client to filter chat messages based on specific groups, such as team chat, global chat, or spectator chat, ensuring that the client only receives relevant messages and can focus on the information that is most important for their viewing experience.
    /// </summary>
    public string ChatGroup { get; set; }
    /// <summary>
    /// Gets or sets the last time the HLTV client sent a message to the server, which is used to track the client's activity and manage communication between the client and the server. This information can be used for various purposes, such as rate-limiting messages to prevent spam, tracking client engagement, or implementing features that depend on the timing of client interactions with the server.
    /// </summary>
    public double LastSendTime { get; set; }
    /// <summary>
    /// Gets or sets the last time the HLTV client sent a chat message, which is used to track the client's chat activity and manage communication between the client and the server. This information can be used for various purposes, such as rate-limiting chat messages to prevent spam, tracking client engagement in chat, or implementing features that depend on the timing of client interactions with the chat system on the server.
    /// </summary>
    public double LastChatTime { get; set; }
    /// <summary>
    /// Gets or sets the last tick at which the HLTV client sent a message to the server, which is used to track the client's activity and manage communication between the client and the server. This information can be used for various purposes, such as rate-limiting messages to prevent spam, tracking client engagement, or implementing features that depend on the timing of client interactions with the server based on game ticks.
    /// </summary>
    public int LastSendTick { get; set; }
    /// <summary>
    /// Gets or sets the full frame time for the HLTV client, which is used to track the client's performance and manage communication between the client and the server. This information can be used for various purposes, such as optimizing performance, tracking client engagement, or implementing features that depend on the timing of client interactions with the server based on frame times.
    /// </summary>
    public int FullFrameTime { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the HLTV client has chat disabled, which can be used to prevent the client from receiving or sending chat messages. This is often used for clients that are primarily focused on viewing the game and do not want to be distracted by chat messages, or for clients that want to avoid potential spam or abuse in the chat system while still being able to access other features of the HLTV server.
    /// </summary>
    public bool NoChat { get; set; }
}