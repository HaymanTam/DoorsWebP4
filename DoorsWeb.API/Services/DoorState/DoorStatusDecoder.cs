using DoorsWeb.Shared.Enums;

namespace DoorsWeb.API.Services.DoorState
{
    /// <summary>
    /// Translates raw controller packets into a <see cref="DoorLiveStatus"/> for the floorplan.
    ///
    /// State is driven primarily by the well-documented event-log codes (P4 protocol, Doc 4002
    /// appendix). A ping reply (B,2) only tells us the controller is alive; we do not try to
    /// reverse-engineer the bit layout of its status1/status2 bytes here, so a freshly-seen door
    /// is shown as <see cref="DoorLiveStatus.Locked"/> (secured) until an event says otherwise.
    /// </summary>
    public static class DoorStatusDecoder
    {
        // ---- Event-log command (D,3) data layout (see P4Protocol.DecodePayload) ----
        public const byte EventLogGroup = 0x0D;   // Command 1
        public const byte EventLogReply = 0x03;   // Command 2
        public const int EventTypeIndex = 14;     // data[14] = event type byte

        // ---- Ping (B,2) ----
        public const byte PingGroup = 0x0B;       // Command 1
        public const byte PingReply = 0x02;       // Command 2

        // ---- Event type codes that move a door between live states (P4 appendix). ----
        private const int E_CARD_OK = 0;
        private const int E_DOOR_PC_RELEASE = 3;
        private const int E_DOOR_FORCED = 5;
        private const int E_PDO = 6;                // Premature / Door-Open-Too-Long => held open
        private const int E_FIRE_ALARM = 7;
        private const int E_CARD_AND_PIN_OK = 16;
        private const int E_DOOR_PC_CLOSE = 19;
        private const int E_RQE = 22;               // Request to exit
        private const int E_CODE_OK = 23;
        private const int E_DOOR_PC_OPEN = 29;
        private const int E_TZ_DOOR_OPEN = 30;
        private const int E_FIRE_RESET = 24;
        private const int E_TZ_DOOR_CLOSE = 38;
        private const int E_DOOR_PC_OPEN_B = 102;
        private const int E_DOOR_PC_CLOSE_B = 103;

        /// <summary>
        /// The live state an event implies, or null if the event does not change the door's state
        /// (e.g. an invalid-card read at a still-locked door).
        /// </summary>
        public static DoorLiveStatus? FromEventType(int eventType) => eventType switch
        {
            E_DOOR_FORCED => DoorLiveStatus.ForcedOpen,
            E_PDO => DoorLiveStatus.HeldOpen,
            E_FIRE_ALARM => DoorLiveStatus.Unlocked,   // fire releases the door

            E_CARD_OK or E_CARD_AND_PIN_OK or E_CODE_OK or E_RQE
                or E_DOOR_PC_RELEASE or E_DOOR_PC_OPEN or E_TZ_DOOR_OPEN
                or E_DOOR_PC_OPEN_B => DoorLiveStatus.Unlocked,

            E_DOOR_PC_CLOSE or E_TZ_DOOR_CLOSE or E_DOOR_PC_CLOSE_B
                or E_FIRE_RESET => DoorLiveStatus.Locked,

            _ => null
        };

        /// <summary>Friendly, human-readable name for an event type code (for the hover "last event").</summary>
        public static string EventName(int eventType) => eventType switch
        {
            E_CARD_OK => "Card OK",
            E_DOOR_PC_RELEASE => "Released",
            E_DOOR_FORCED => "Door forced",
            E_PDO => "Held open",
            E_FIRE_ALARM => "Fire alarm",
            E_CARD_AND_PIN_OK => "Card + PIN OK",
            E_DOOR_PC_CLOSE => "Locked",
            E_RQE => "Exit request",
            E_CODE_OK => "Code OK",
            E_FIRE_RESET => "Fire reset",
            E_DOOR_PC_OPEN => "Opened",
            E_TZ_DOOR_OPEN => "Time-zone open",
            E_TZ_DOOR_CLOSE => "Time-zone close",
            E_DOOR_PC_OPEN_B => "Opened (B)",
            E_DOOR_PC_CLOSE_B => "Closed (B)",
            1 => "Invalid card",
            2 => "Hacker",
            18 => "Unknown card",
            25 => "Card expired",
            26 => "Invalid PIN",
            _ => $"Event {eventType}"
        };

        /// <summary>True when this status should grab attention on the board (flash/pulse).</summary>
        public static bool IsAlarm(DoorLiveStatus status)
            => status is DoorLiveStatus.ForcedOpen or DoorLiveStatus.HeldOpen;
    }
}
