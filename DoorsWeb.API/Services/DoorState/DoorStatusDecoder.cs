using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Enums;

namespace DoorsWeb.API.Services.DoorState
{
    /// <summary>
    /// Translates raw controller packets into a <see cref="DoorLiveStatus"/> for the floorplan.
    ///
    /// State is driven by two sources: the well-documented event-log codes (P4 protocol, Doc 4002
    /// appendix), and the ping reply (B,2) whose two status bytes carry the live relay and alarm
    /// state. <see cref="DecodePing"/> turns a reply into a <see cref="PingResult"/> (relay states,
    /// alarms, firmware, supply voltage and the controller's clock); <see cref="LiveStatusFromPing"/>
    /// folds the bits into the single colour-driving <see cref="DoorLiveStatus"/>.
    /// </summary>
    public static class DoorStatusDecoder
    {
        // ---- Event-log command group (Command 1 = 0x0D). ----
        // The connector pulls unread entries with a request/reply/ack handshake (legacy Coms.bas):
        //   D,1 (we send)  — read the oldest unread entry; data = [1,0].
        //   D,3 (we get)   — one decoded event entry (16 data bytes; see layout below).
        //   D,2 (we send)  — ack: mark that entry read and advance the controller's queue.
        //   D,4 (we get)   — "Event Log Data End": no more unread entries, stop draining.
        public const byte EventLogGroup = 0x0D;     // Command 1
        public const byte EventLogRequest = 0x01;   // Command 2 (D,1 — read oldest unread)
        public const byte EventLogAck = 0x02;       // Command 2 (D,2 — ack / mark-read / advance)
        public const byte EventLogReply = 0x03;     // Command 2 (D,3 — one event entry)
        public const byte EventLogEnd = 0x04;       // Command 2 (D,4 — no more unread entries)
        public const int EventTypeIndex = 14;       // data[14] = event type byte

        // ---- Event-log reply (D,3) data-byte layout (0-indexed; P4 protocol / legacy Coms.bas). ----
        // Time fields are packed BCD; the card number is three big-endian 16-bit words.
        private const int EvtSecondIndex = 0;
        private const int EvtMinuteIndex = 1;
        private const int EvtHourIndex = 2;
        private const int EvtDayIndex = 3;
        private const int EvtMonthIndex = 4;
        private const int EvtYearIndex = 5;     // 2-digit year (=> 20xx)
        private const int EvtCardAHiIndex = 6;  // card word A (most significant pair), high byte
        private const int EvtCardALoIndex = 7;  //                                       low byte
        private const int EvtCardBHiIndex = 8;  // card word B
        private const int EvtCardBLoIndex = 9;
        private const int EvtCardCHiIndex = 10; // card word C
        private const int EvtCardCLoIndex = 11;
        private const int EvtReaderIndex = 12;  // reader (0x01 = A, 0x02 = B)
        // data[13] = milliseconds (unused); data[14] = event type (EventTypeIndex above).

        // Event types that carry a real card number (legacy Coms.bas keep-list). Every other event
        // type is logged with card number 0, matching the connector's "000000000000" sentinel.
        private static readonly HashSet<int> CardBearingEvents = new()
        {
            0, 1, 10, 15, 16, 17, 18, 25, 26, 27, 28, 44, 46, 48, 51, 57, 58, 60, 65, 67, 68
        };

        // ---- Ping (B,1 request / B,2 reply) ----
        public const byte PingGroup = 0x0B;       // Command 1
        public const byte PingRequest = 0x01;     // Command 2 (B,1 — what we send)
        public const byte PingReply = 0x02;       // Command 2 (B,2 — what we decode)

        // ---- Command acknowledgement (legacy "command received" reply). ----
        // After a controller accepts a command it answers with command group 0x11, command 2 = 0x01.
        // The legacy connector correlated an ack to the most recent command sent to that controller
        // address (Last_command_address = Source_Address); the block-sequence number was not used for
        // matching. We use the same address-based correlation to resolve pending commands.
        public const byte AckGroup = 0x11;        // Command 1 (controller -> PC "command received")
        public const byte AckReply = 0x01;        // Command 2

        // ---- Ping reply (B,2) data-byte layout (P4 UDP Protocol v4.68, Doc 4002) ----
        // data length is 15 (no supply voltage) or 16 (with supply voltage).
        private const int CardBlocksLowIndex = 0;   // free card blocks, low byte
        private const int CardBlocksHighIndex = 1;  // free card blocks, high byte
        private const int UnreadLogsHighIndex = 2;  // unread-log count, BCD digits 1&2 (high pair)
        private const int UnreadLogsLowIndex = 3;   // unread-log count, BCD digits 3&4 (low pair)
        private const int StatusByte1Index = 4;     // relay / monitor bits
        private const int StatusByte2Index = 5;     // alarm bits
        private const int RtcDayIndex = 6;          // controller clock (BCD): day
        private const int RtcMonthIndex = 7;        //                          month
        private const int RtcYearIndex = 8;         //                          year (2-digit)
        private const int RtcSecondIndex = 9;       //                          seconds
        private const int RtcMinuteIndex = 10;      //                          minutes
        private const int RtcHourIndex = 11;        //                          hours
        private const int DayOfWeekIndex = 12;      //                          day-of-week
        private const int VersionMajorIndex = 13;   // firmware version major
        private const int VersionMinorIndex = 14;   // firmware version minor
        private const int SupplyVoltageIndex = 15;  // supply voltage in 100mV units (137 => 13.7V)

        // ---- Status byte 1 bits (1 = active). ----
        private const int RelayABit = 0;  // Relay A / lock relay (1 = open/released)
        private const int RelayBBit = 1;  // Relay B / auxiliary relay (1 = open)
        // bit2 door-release monitor, bit3 RQE, bit4 program, bit5 interlock, bit6 PWR, bit7 reserved.

        // ---- Status byte 2 bits (alarms; 1 = active). ----
        private const int FireBit = 0;
        private const int IntruderBit = 1;
        private const int TamperBit = 2;
        // bit3 reserved.
        private const int DuressBit = 4;
        private const int PdoBit = 5;     // premature / door-open-too-long
        private const int ForcedBit = 6;  // door forced
        private const int HackerBit = 7;

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

        // ---- Event-log reply (D,3) decoding ------------------------------------------

        /// <summary>One decoded event-log entry pulled from a controller (D,3).</summary>
        /// <param name="TimestampLocal">The controller's local wall-clock time for the event.</param>
        /// <param name="TimestampValid">False when the BCD time was implausible and a fallback was used.</param>
        /// <param name="CardNumber">The numeric card number (0 for events that don't carry a card).</param>
        /// <param name="CardId">The raw assembled card string, or null when the event carries no card.</param>
        /// <param name="ReaderId">Reader the event came from (1 = A, 2 = B).</param>
        /// <param name="EventType">The P4 event-type code (see <see cref="EventName"/>).</param>
        public readonly record struct EventLogRecord(
            DateTime TimestampLocal,
            bool TimestampValid,
            long CardNumber,
            string? CardId,
            int ReaderId,
            int EventType);

        /// <summary>
        /// Decodes a D,3 event-log reply's data bytes into a single <see cref="EventLogRecord"/>.
        /// The card number is assembled exactly as the legacy connector did: three big-endian 16-bit
        /// words formatted as zero-padded 4-digit groups and concatenated C|B|A (the least-significant
        /// group truncated to its last four digits). Only the legacy keep-list event types carry a
        /// card; all others are logged with card 0. Tolerates short payloads (missing bytes read as 0).
        /// </summary>
        public static EventLogRecord DecodeEvent(byte[] data)
        {
            int eventType = Get(data, EventTypeIndex);
            int reader = Get(data, EvtReaderIndex);

            long cardNumber = 0;
            string? cardId = null;
            if (CardBearingEvents.Contains(eventType))
            {
                int a = (Get(data, EvtCardAHiIndex) << 8) | Get(data, EvtCardALoIndex);
                int b = (Get(data, EvtCardBHiIndex) << 8) | Get(data, EvtCardBLoIndex);
                int c = (Get(data, EvtCardCHiIndex) << 8) | Get(data, EvtCardCLoIndex);

                string aStr = a.ToString("0000");
                if (aStr.Length > 4) aStr = aStr[^4..]; // legacy Right(Format(a,"0000"), 4)
                cardId = c.ToString("0000") + b.ToString("0000") + aStr;
                long.TryParse(cardId, out cardNumber);
            }

            var (when, valid) = TryDecodeEventTime(data);
            return new EventLogRecord(when, valid, cardNumber, cardId, reader, eventType);
        }

        // Decodes the packed-BCD event timestamp. Any implausible field means we don't trust the
        // controller's clock and fall back to "now" so the entry is still recorded in order.
        private static (DateTime When, bool Valid) TryDecodeEventTime(byte[] data)
        {
            int second = Bcd(Get(data, EvtSecondIndex));
            int minute = Bcd(Get(data, EvtMinuteIndex));
            int hour = Bcd(Get(data, EvtHourIndex));
            int day = Bcd(Get(data, EvtDayIndex));
            int month = Bcd(Get(data, EvtMonthIndex));
            int year = Bcd(Get(data, EvtYearIndex));

            if (month is < 1 or > 12 || day is < 1 or > 31) return (DateTime.Now, false);
            if (hour > 23 || minute > 59 || second > 59) return (DateTime.Now, false);

            try { return (new DateTime(2000 + year, month, day, hour, minute, second, DateTimeKind.Local), true); }
            catch (ArgumentOutOfRangeException) { return (DateTime.Now, false); }
        }

        // ---- Ping reply (B,2) decoding -----------------------------------------------

        /// <summary>The decoded contents of a ping reply (B,2).</summary>
        public readonly record struct PingResult(
            DoorLiveStatus Status,
            DoorHardwareStatusDto Hardware,
            DateTime? ControllerTimeLocal);

        /// <summary>
        /// Decodes a ping reply's (B,2) data bytes into the live status plus the hardware detail
        /// (relay states, alarms, firmware, supply voltage, diagnostics) and the controller clock.
        /// Tolerates short payloads: fields past the end of <paramref name="data"/> are left unset.
        /// </summary>
        public static PingResult DecodePing(byte[] data, DateTime nowUtc)
        {
            byte s1 = Get(data, StatusByte1Index);
            byte s2 = Get(data, StatusByte2Index);

            var hardware = new DoorHardwareStatusDto
            {
                RelayA = Bit(s1, RelayABit) ? RelayState.Open : RelayState.Closed,
                RelayB = Bit(s1, RelayBBit) ? RelayState.Open : RelayState.Closed,
                Alarms = AlarmsFrom(s2),
                Status1 = s1,
                Status2 = s2,
                LastPolledUtc = nowUtc
            };

            if (data.Length > CardBlocksHighIndex)
                hardware.CardBlocks = Get(data, CardBlocksLowIndex) | (Get(data, CardBlocksHighIndex) << 8);

            if (data.Length > UnreadLogsLowIndex)
                hardware.UnreadLogCount = Bcd(Get(data, UnreadLogsHighIndex)) * 100 + Bcd(Get(data, UnreadLogsLowIndex));

            if (data.Length > VersionMinorIndex)
                hardware.FirmwareVersion = $"{Get(data, VersionMajorIndex)}.{Get(data, VersionMinorIndex)}";

            if (data.Length > SupplyVoltageIndex)
                hardware.SupplyVoltage = Get(data, SupplyVoltageIndex) / 10.0;

            return new PingResult(LiveStatusFromPing(s1, s2), hardware, TryDecodeRtc(data));
        }

        /// <summary>
        /// Folds the two status bytes into the single colour-driving <see cref="DoorLiveStatus"/>.
        /// Alarm conditions win over the plain relay state: forced &gt; held-open &gt; fire-release,
        /// otherwise the lock relay tells us locked vs. released.
        /// </summary>
        public static DoorLiveStatus LiveStatusFromPing(byte status1, byte status2)
        {
            if (Bit(status2, ForcedBit)) return DoorLiveStatus.ForcedOpen;
            if (Bit(status2, PdoBit)) return DoorLiveStatus.HeldOpen;
            if (Bit(status2, FireBit)) return DoorLiveStatus.Unlocked; // fire releases the door
            return Bit(status1, RelayABit) ? DoorLiveStatus.Unlocked : DoorLiveStatus.Locked;
        }

        private static DoorAlarmFlags AlarmsFrom(byte status2)
        {
            var flags = DoorAlarmFlags.None;
            if (Bit(status2, FireBit)) flags |= DoorAlarmFlags.Fire;
            if (Bit(status2, IntruderBit)) flags |= DoorAlarmFlags.Intruder;
            if (Bit(status2, TamperBit)) flags |= DoorAlarmFlags.Tamper;
            if (Bit(status2, DuressBit)) flags |= DoorAlarmFlags.Duress;
            if (Bit(status2, PdoBit)) flags |= DoorAlarmFlags.Pdo;
            if (Bit(status2, ForcedBit)) flags |= DoorAlarmFlags.Forced;
            if (Bit(status2, HackerBit)) flags |= DoorAlarmFlags.Hacker;
            return flags;
        }

        // The controller clock is packed BCD. Decode defensively: any implausible field means we
        // don't trust the clock and return null rather than persist a bogus timestamp.
        private static DateTime? TryDecodeRtc(byte[] data)
        {
            if (data.Length <= DayOfWeekIndex) return null;

            int day = Bcd(Get(data, RtcDayIndex));
            int month = Bcd(Get(data, RtcMonthIndex));
            int year = Bcd(Get(data, RtcYearIndex));
            int second = Bcd(Get(data, RtcSecondIndex));
            int minute = Bcd(Get(data, RtcMinuteIndex));
            int hour = Bcd(Get(data, RtcHourIndex));

            if (month is < 1 or > 12 || day is < 1 or > 31) return null;
            if (hour > 23 || minute > 59 || second > 59) return null;

            try { return new DateTime(2000 + year, month, day, hour, minute, second, DateTimeKind.Local); }
            catch (ArgumentOutOfRangeException) { return null; }
        }

        private static bool Bit(byte value, int bit) => (value & (1 << bit)) != 0;

        private static byte Get(byte[] data, int index)
            => index >= 0 && index < data.Length ? data[index] : (byte)0;

        // Decodes one packed-BCD byte (e.g. 0x37 => 37) into 0..99.
        private static int Bcd(byte value) => ((value >> 4) & 0x0F) * 10 + (value & 0x0F);
    }
}
