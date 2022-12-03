using Core.IO.ResourceFormats.XBin.Types;
using Core.IO.ResourceFormats.XBin.Types.CarGearBoxes;
using Core.IO.ResourceFormats.XBin.Types.CarInteriorColors;
using Core.IO.ResourceFormats.XBin.Types.CarTrafficTuning;
using Core.IO.ResourceFormats.XBin.Types.CarTuningModificators;
using Core.IO.ResourceFormats.XBin.Types.CarTuningPackAvailability;
using Core.IO.ResourceFormats.XBin.Types.CharacterCinematics;
using Core.IO.ResourceFormats.XBin.Types.Cities;
using Core.IO.ResourceFormats.XBin.Types.Containers;
using Core.IO.ResourceFormats.XBin.Types.GameMeshBinding;
using Core.IO.ResourceFormats.XBin.Types.GenericSpeechSituations;
using Core.IO.ResourceFormats.XBin.Types.Missions;
using Core.IO.ResourceFormats.XBin.Types.Slot;
using Core.IO.ResourceFormats.XBin.Types.StreamMap;
using Core.IO.ResourceFormats.XBin.Types.String;
using Core.IO.ResourceFormats.XBin.Types.Subtitle;
using Core.IO.ResourceFormats.XBin.Types.Vehicles;

namespace Core.IO.ResourceFormats.XBin
{
    internal static class XBinFactory
    {
        public static ITable ReadXBin(BinaryReader reader, ulong hash)
        {
            switch(hash)
            {
                case 0x5E42EF29E8A3E1D3:
                    return StringTable.ReadFromFile(reader);
                
                case 0xA869F8A3ED0CDAFC: // M1:DE
                case 0xA869F8A3AC7656E1: // M3
                    return VehicleTable.ReadFromFile(reader);
                
                case 0x3759788EC437536C:
                    return CarGearBoxesTable.ReadFromFile(reader);
                
                case 0xF5D56763013A2B0A:
                    return CarInteriorColorsTable.ReadFromFile(reader); // TODO: Broken for both games
                
                case 0x44FA070D73C43CBD: // M1:DE
                case 0x44FA070D55388F65: // M3
                    return CarTrafficTuningTable.ReadFromFile(reader);
                
                case 0x09B5140FA382AF8F:
                    return CarTuningModificatorsTable.ReadFromFile(reader);
                
                case 0x2793BB7847F84081:
                    return GameMeshBindingTable.ReadFromFile(reader);
                
                case 0x493DEA76C9A390F0:
                    return SlotTable.ReadFromFile(reader);
                
                case 0xB77D0A522C8E12A3: // M1:DE
                case 0xB77D0A52FD0225D0: // M3
                    return MissionsTable.ReadFromFile(reader);
                
                case 0x3990790678078A1C: 
                    return GenericSpeechSituationsTable.ReadFromFile(reader);
                
                case 0xEF795C84CA85E193:
                    return CharacterCinematicsTable.ReadFromFile(reader);
                
                case 0x5D69A41C1FBD6565: // M1:DE
                case 0x5D69A41CDC82936C: // M3
                    return TableContainer.ReadFromFile(reader);
                
                case 0x06F579D595DA02AD: // M1:DE
                case 0x06F579D51CE129A5: // M3
                    return GfxContainer.ReadFromFile(reader);
                
                case 0xAD5CF0F7FC3717F0: // M1:DE
                case 0xAD5CF0F764C39370: // M3
                    return GuiContainer.ReadFromFile(reader);
                
                case 0x0E2FBBCF46754F66: // M1:DE and M3
                    return CarTuningPackAvailabilityTable.ReadFromFile(reader);
                
                case 0xA32C16191BC63EEF: // M1:DE
                case 0xA32C1619D5261223: // M3
                    return StreamMapTable.ReadFromFile(reader);
                
                case 0x2B673F12DCA4BBF1:
                case 0x2B673F120D288C9A:
                    return CitiesTable.ReadFromFile(reader);
                
                case 0x8982CC5C46848F6E: // M1:DE
                case 0x8982CC5C78136253: // M3
                    return SubtitleTable.ReadFromFile(reader);
                
                //case 0xDC327944DD83627E: // TODO: Fix for M1: DE. look for 0xA for PaintCombination array entries.
                //    XBinData = new PaintCombinationsTable(); // PaintCombinations
                //    XBinData.ReadFromFile(reader);
                //    break;
                
                case 0xEBD934EE6E87AC4E: // M3
                    throw new NotImplementedException();
                
                case 0xA7A8EB60CE85F97C:
                    throw new NotImplementedException();
                
                case 0xD2B1AD3096FDF2:
                    throw new NotImplementedException();
                
                case 0x543BB56B6772520E:
                    throw new NotImplementedException();
                
                case 0xBE38EA98A1B3B982:
                    throw new NotImplementedException();
                
                case 0xE5D1112869D46046:
                    throw new NotImplementedException();
                
                case 0x3FEE4A7602DB84C2:
                    throw new NotImplementedException();
                
                case 0x941DDDBB8F26254E:
                    throw new NotImplementedException();
                
                case 0x2AC1311BDF935842:
                    throw new NotImplementedException();
                
                case 0x801E2C7A9A0AFBC4:
                    throw new NotImplementedException();
                
                case 0xB9CB5FF488A9211E:
                    throw new NotImplementedException();
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(hash), $"Unknown type for hash: {hash:X}");
            }
        }
    }
}
