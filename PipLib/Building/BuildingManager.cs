using PipLib.Tech;
using System.Collections.Generic;
using TUNING;

namespace PipLib.Building
{

    public static class BuildingManager
    {

        internal static readonly Logging.ILogger Logger = PipLib.Logger.Fork(nameof(BuildingManager));

        /// <summary>
        /// Adds the given building, by ID, to the given tech
        /// </summary>
        /// <param name="buildingID">The building's ID</param>
        /// <param name="techID">The ID of the tech to add to</param>
        public static void AddToTech (string buildingID, string techID)
        {
            TechTree.AddTechItem(techID, buildingID);
        }

        /// <summary>
        /// Adds the given building ID to the given plan menu category, optionally at a specified index
        /// </summary>
        /// <param name="buildingID">The building ID to add</param>
        /// <param name="category">The building category to add to</param>
        /// <param name="atIndex">The index to add after (or negative values to add to the end)</param>
        public static void AddToPlanMenu(string buildingID, HashedString category, int atIndex = -1)
        {
            var menu = GetPlanMenuByCategory(category);
            if (menu != null)
            {
                if (atIndex < 0)
                {
                    menu.Add(buildingID);
                }
                else
                {
                    menu.Insert(System.Math.Min(atIndex, menu.Count), buildingID);
                }
            }
        }

        /// <summary>
        /// Adds the given building ID to the given plan menu category after the another building. If the ID is not
        /// found, the building will be added to the end
        /// </summary>
        /// <param name="buildingID">The building ID to add</param>
        /// <param name="category">The building category to add to</param>
        /// <param name="afterID">The ID to add after</param>
        public static void AddToPlanMenu(string buildingID, HashedString category, string afterID)
        {
            var menu = GetPlanMenuByCategory(category);
            if (menu != null)
            {
                var i = menu.IndexOf(afterID);
                AddToPlanMenu(buildingID, category, i < 0 ? -1 : i + 1);
            }
        }

        private static List<string> GetPlanMenuByCategory (HashedString category)
        {
            var i = BUILDINGS.PLANORDER.FindIndex(v => category == v.category);
            if (i < 0)
            {
                Logger.Error("Failed to find plan order category by ID: {0}", category);
                return null;
            }

            return (List<string>)BUILDINGS.PLANORDER[i].data;
        }
    }

}
