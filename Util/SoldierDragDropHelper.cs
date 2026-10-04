using MilLeadershipBoard.Config;
using MilLeadershipBoard.Models.TroopData;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;

namespace MilLeadershipBoard.Util
{
    /// <summary>
    /// Helper class used to transport <see cref="SoldierData"/> instances through a drag and drop operation.
    /// Only the <see cref="SoldierData.Id"/> values are written into the <see cref="DataPackage"/>; the actual
    /// instances are resolved from <see cref="ConfigData.Soldiers"/> when the payload is read again. This way a
    /// drop always operates on the very same instances the rest of the application is bound to, instead of on a
    /// copy that was marshalled through the clipboard infrastructure.
    /// </summary>
    internal static class SoldierDragDropHelper
    {
        //   ---   Private Constants   ---

        /// <summary>
        /// Constant containing the separator used between the single <see cref="SoldierData.Id"/> values of the payload.
        /// </summary>
        private const char ID_SEPARATOR = ';';

        //   ---   Public Constants   ---

        /// <summary>
        /// Constant containing the key the dragged <see cref="SoldierData"/> identifiers are stored under
        /// within the <see cref="DataPackage.Properties"/> of a drag and drop operation.
        /// </summary>
        public const string SOLDIER_IDS_PROPERTY_KEY = "MilLeadershipBoard.SoldierIds";

        //   ---   Private Methods   ---

        /// <summary>
        /// Method used to try and read the <see cref="SoldierData.Id"/> values of a drag and drop payload.
        /// </summary>
        /// <param name="dataView">The <see cref="DataPackageView"/> of the drag and drop operation.</param>
        /// <param name="ids"><see langword="out"/> array containing the identifiers that could be read from the payload.</param>
        /// <returns><see langword="true"/> if at least one identifier could be read, otherwise <see langword="false"/>.</returns>
        private static bool TryGetSoldierIds(DataPackageView? dataView, out Guid[] ids)
        {
            ids = [];

            if (dataView is null)
            {
                return false;
            }

            IReadOnlyDictionary<string, object> properties = dataView.Properties;

            if (!properties.TryGetValue(SOLDIER_IDS_PROPERTY_KEY, out object? rawValue) || rawValue is not string rawIds)
            {
                return false;
            }

            ids = [.. rawIds.Split(ID_SEPARATOR, StringSplitOptions.RemoveEmptyEntries)
                .Select(idString => Guid.TryParse(idString, out Guid id) ? id : Guid.Empty)
                .Where(id => id != Guid.Empty)];

            return ids.Length != 0;
        }

        //   ---   Public Methods   ---

        /// <summary>
        /// Method used to resolve the <see cref="SoldierData"/> instances a drag and drop operation carries.
        /// Identifiers that cannot be resolved (e.g. because the soldier was deleted while the drag was running)
        /// are silently skipped.
        /// </summary>
        /// <param name="dataView">The <see cref="DataPackageView"/> of the drag and drop operation.</param>
        /// <returns>
        /// A list containing all <see cref="SoldierData"/> instances of the payload or an empty list if the payload
        /// does not contain any soldiers.
        /// </returns>
        public static IReadOnlyList<SoldierData> GetSoldiers(DataPackageView? dataView)
        {
            if (!TryGetSoldierIds(dataView, out Guid[] ids))
            {
                return [];
            }

            return [.. ids
                .Select(id => ConfigManager.Config.Soldiers.FirstOrDefault(soldier => soldier.Id == id))
                .OfType<SoldierData>()];
        }

        /// <summary>
        /// Method used to write the specified <paramref name="soldiers"/> into the <see cref="DataPackage"/> of a
        /// drag and drop operation and to mark the operation as a move operation.
        /// </summary>
        /// <param name="dataPackage">The <see cref="DataPackage"/> of the drag and drop operation.</param>
        /// <param name="soldiers">The <see cref="SoldierData"/> instances that are being dragged.</param>
        public static void SetSoldiers(DataPackage dataPackage, IEnumerable<SoldierData> soldiers)
        {
            IDictionary<string, object> properties = dataPackage.Properties;

            properties[SOLDIER_IDS_PROPERTY_KEY] = string.Join(ID_SEPARATOR, soldiers.Select(soldier => soldier.Id.ToString()));

            // A soldier is never duplicated by a drag and drop operation, it only changes its SoldierLocation.
            dataPackage.RequestedOperation = DataPackageOperation.Move;
        }
    }
}
