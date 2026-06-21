using System;

namespace DoorsWeb.Shared.Enums
{
    /// <summary>
    /// Active alarm conditions decoded from a ping reply's (B,2) Status byte 2. A door can be in
    /// several at once, hence <see cref="FlagsAttribute"/>. The bit positions mirror the protocol's
    /// status-byte-2 layout (see <c>DoorStatusDecoder</c>); bit 3 is reserved by the protocol.
    /// </summary>
    [Flags]
    public enum DoorAlarmFlags
    {
        /// <summary>No alarm conditions.</summary>
        None = 0,

        /// <summary>Fire alarm (bit 0) — releases the door.</summary>
        Fire = 1 << 0,

        /// <summary>Intruder alarm (bit 1).</summary>
        Intruder = 1 << 1,

        /// <summary>Tamper detected (bit 2).</summary>
        Tamper = 1 << 2,

        /// <summary>Duress code entered (bit 4).</summary>
        Duress = 1 << 4,

        /// <summary>Premature / Door-Open-too-long alarm (bit 5).</summary>
        Pdo = 1 << 5,

        /// <summary>Door forced open without a valid release (bit 6).</summary>
        Forced = 1 << 6,

        /// <summary>Hacker / repeated invalid attempts (bit 7).</summary>
        Hacker = 1 << 7
    }
}
