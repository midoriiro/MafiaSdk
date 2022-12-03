using Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands.Vehicle;

namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands
{
    // This is *NOT* a command. This is a Toolkit Util for opening/saving.
    public static class CommandFactory
    {
        public static ICommand ReadCommand(BinaryReader reader, uint commandId)
        {
            ICommand command = null!;

            // Read the CommanID and construct the new Command.
            return commandId switch
            {
                0xB64D9A5D => CloseTraffic.ReadFromFile(reader),
                0x22663242 => LoadSds.ReadFromFile(reader),
                0xA1C05A78 => LoadVehicle.ReadFromFile(reader),
                0xEA186B6 => Save.ReadFromFile(reader),
                0x72386E2B => SetPlayerPositionAndDirection.ReadFromFile(reader),
                0xB40BC168 => Suspend.ReadFromFile(reader),
                0xD7C10363 => OpenSlot.ReadFromFile(reader),
                0x31247C78 => Barrier.ReadFromFile(reader),
                0xD4F4F264 => OpenTraffic.ReadFromFile(reader),
                0x687DAD8B => PlayVideo.ReadFromFile(reader),
                0x665E90F2 => WaitForMovie.ReadFromFile(reader),
                0x90ACE5D5 => PlayCutscene.ReadFromFile(reader),
                0x3B3DD38A => UnlockVehicle.ReadFromFile(reader),
                0xA033FEEB => UnlockSds.ReadFromFile(reader),
                0x1EFE290F => If.ReadFromFile(reader),
                0x20AE48F2 => EndIf.ReadFromFile(reader),
                _ => throw new ArgumentOutOfRangeException(nameof(commandId),
                    $"Unknown type for command ID: {commandId:X}")
            };
        }
    }
}
