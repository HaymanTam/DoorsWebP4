using DoorsWeb.API.Services.DoorState;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.API.Services.Protocol;
using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services
{
    /// <summary>
    /// Builds the controller programming packets for a door and queues them for delivery through the
    /// existing pending-command retry queue.
    ///
    /// Two documented commands carry a door's configuration:
    ///   - 1,6 "Engineers Pack" - split into sub-blocks selected by Data[0]:
    ///        0x00 reader/keypad hardware, 0x01 lock/relay behaviour, 0x02 valid-from/to window.
    ///   - 1,5 "Users Pack"     - Data[0] 0x00 carries the keypad access (v-card) code.
    ///
    /// Each packet is 16 data bytes. We reuse <see cref="IPendingCommandService"/> so programming is
    /// retried until the controller acks with the same (0x11/0x01) "command received" reply that
    /// lock/unlock commands already correlate against; the pending-command Action/Relay fields are
    /// irrelevant here (the UI shows only the Description), so neutral placeholders are passed.
    /// </summary>
    public sealed class DoorConfigSyncService : IDoorConfigSyncService
    {
        // 1,6 Engineers Pack (reader/lock/valid-window sub-blocks) and 1,5 Users Pack (access code).
        private const byte EngPackGroup = 0x01;
        private const byte EngPackNumber = 0x06;
        private const byte UserPackGroup = 0x01;
        private const byte UserPackNumber = 0x05;

        // Data[0] sub-block selectors within the Engineers Pack.
        private const byte BlockReader = 0x00;
        private const byte BlockLock = 0x01;
        private const byte BlockValidWindow = 0x02;
        // Data[0] sub-block selector within the Users Pack.
        private const byte BlockAccessCode = 0x00;

        // Legacy sentinel meaning "no time zone" (frmDoors lcNoTimeZone); on the wire the controller
        // expects 0xFF ("time-zone disabled") for a relay/override that has no active window.
        private const int NoTimeZone = 10000;
        private const byte TzDisabled = 0xFF;

        // Placeholders: the pending queue requires an Action/Relay, but the Door Manager renders only
        // the Description for these programming rows, so the exact values don't matter.
        private const DoorCommandAction ProgAction = DoorCommandAction.Lock;
        private const DoorRelay ProgRelay = DoorRelay.RelayA;

        private readonly IPendingCommandService _pending;
        private readonly ILogger<DoorConfigSyncService> _logger;

        public DoorConfigSyncService(IPendingCommandService pending, ILogger<DoorConfigSyncService> logger)
        {
            _pending = pending;
            _logger = logger;
        }

        public async Task SyncDoorAsync(Doors door, CancellationToken ct = default)
        {
            try
            {
                var host = door.DoorIpaddress?.Trim();
                if (string.IsNullOrWhiteSpace(host)) return;                 // no IP - nothing to program
                if (!uint.TryParse(door.ControllerId, out var address) || address == 0) return;

                var name = door.Name;
                var packets = new (ProtocolPacket Packet, string Description)[]
                {
                    (BuildEngPack(address, BuildReaderBlock(door)),       "Program reader/keypad"),
                    (BuildEngPack(address, BuildLockBlock(door)),         "Program lock/relay"),
                    (BuildEngPack(address, BuildValidWindowBlock(door)),  "Program valid window"),
                    (BuildUserPack(address, BuildAccessCodeBlock(door)),  "Program access code"),
                };

                foreach (var (packet, description) in packets)
                {
                    await _pending.EnqueueAsync(door.Door, name, address, host!,
                        packet, ProgAction, ProgRelay, description, ct);
                }

                _logger.LogInformation("Queued programming for door {Door} ({Ip}).", door.Door, host);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to queue programming for door {Door}.", door.Door);
            }
        }

        // ---- packet builders ---------------------------------------------------------

        private static ProtocolPacket BuildEngPack(uint address, byte[] data) => new()
        {
            DestinationAddress = address,
            SourceAddress = 0, // PC
            CommandGroup = EngPackGroup,
            CommandNumber = EngPackNumber,
            Data = data
        };

        private static ProtocolPacket BuildUserPack(uint address, byte[] data) => new()
        {
            DestinationAddress = address,
            SourceAddress = 0, // PC
            CommandGroup = UserPackGroup,
            CommandNumber = UserPackNumber,
            Data = data
        };

        // 1,6 sub-block 0x00 - reader & keypad hardware.
        private static byte[] BuildReaderBlock(Doors d)
        {
            var data = NewBlock(BlockReader);
            data[1] = B(d.RdrBrightnessA, 5);
            data[2] = B(d.RdrBrightnessB, 5);
            data[3] = B(d.RdrVolumeA, 15);
            data[4] = B(d.RdrVolumeB, 15);
            data[5] = B(d.KeypadStarMode);
            data[6] = B(d.RandomSearchFreq);
            data[7] = B(d.ConFbVolume, 15);
            data[8] = B(d.ConAlmVolume, 15);
            data[9] = B(d.LockDriveMode);
            data[10] = Lo(d.IdSequenceA);
            data[11] = Hi(d.IdSequenceA);
            data[12] = Lo(d.IdSequenceB);
            data[13] = Hi(d.IdSequenceB);
            return data;
        }

        // 1,6 sub-block 0x01 - lock / relay behaviour.
        private static byte[] BuildLockBlock(Doors d)
        {
            var data = NewBlock(BlockLock);
            data[1] = B(d.ReleaseTime);
            data[2] = B(d.Pdo);
            data[3] = (byte)((d.AutoRelock ?? false) ? 1 : 0);
            data[4] = B(d.ReleaseTimeB);
            data[5] = Tz(d.RelayBtimeZone);
            data[6] = Tz(d.TimeLock);
            data[7] = Tz(d.CardandPintimeZone);
            data[8] = 0;
            data[9] = B(d.AutoDelayVal);
            data[10] = B(d.RelayBmode);
            data[11] = B(d.ReleaseDelay);
            return data;
        }

        // 1,6 sub-block 0x02 - valid-from / valid-to window.
        private static byte[] BuildValidWindowBlock(Doors d)
        {
            var data = NewBlock(BlockValidWindow);
            data[1] = B(d.ValidFromTimeHh);
            data[2] = B(d.ValidFromTimeMm);
            data[3] = B(d.ValidToTimeHh);
            data[4] = B(d.ValidToTimeMm);
            return data;
        }

        // 1,5 sub-block 0x00 - keypad access (v-card) code, one BCD digit per byte.
        private static byte[] BuildAccessCodeBlock(Doors d)
        {
            var data = NewBlock(BlockAccessCode);
            data[1] = B(d.AccessCodeLen);
            data[2] = B(d.AccessCodeDig1);
            data[3] = B(d.AccessCodeDig2);
            data[4] = B(d.AccessCodeDig3);
            data[5] = B(d.AccessCodeDig4);
            data[6] = B(d.AccessCodeDig5);
            data[7] = B(d.AccessCodeDig6);
            data[8] = B(d.AccessCodeDig7);
            data[9] = B(d.AccessCodeDig8);
            return data;
        }

        // ---- byte helpers ------------------------------------------------------------

        // A fresh 16-byte block whose Data[0] is the sub-block selector; remaining bytes default to 0.
        private static byte[] NewBlock(byte selector)
        {
            var data = new byte[ProtocolPacket.MaxDataLength];
            data[0] = selector;
            return data;
        }

        private static byte B(int? value, byte fallback = 0)
        {
            if (value is null) return fallback;
            if (value < 0) return 0;
            if (value > 255) return 255;
            return (byte)value.Value;
        }

        // A time-zone reference: 0xFF disables it (null or the legacy "no time zone" sentinel);
        // otherwise the clamped zone number.
        private static byte Tz(int? value)
            => value is null || value == NoTimeZone ? TzDisabled : B(value);

        private static byte Lo(int? value) => (byte)(Clamp16(value) & 0xFF);
        private static byte Hi(int? value) => (byte)((Clamp16(value) >> 8) & 0xFF);

        private static int Clamp16(int? value)
        {
            if (value is null || value < 0) return 0;
            return value > 0xFFFF ? 0xFFFF : value.Value;
        }
    }
}
