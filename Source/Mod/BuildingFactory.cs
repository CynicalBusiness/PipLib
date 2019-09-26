using System.Collections.Generic;
using Database;
using STRINGS;
using static TUNING.BUILDINGS;

namespace PipLib.Mod
{
    public class BuildingFactory : AbstractFactory
    {

        private string name;
        private string description = "";
        private string effect = "";

        public BuildingFactory(PrefixedId id) : base(id)
        {
            name = Id();
        }

        public BuildingFactory SetName(string name)
        {
            this.name = name;
            return this;
        }

        public BuildingFactory SetDescription(string desc)
        {
            description = desc;
            return this;
        }

        public BuildingFactory SetEffect(string effect)
        {
            this.effect = effect;
            return this;
        }

        /// <summary>
        /// Adds a specific technology that must be researched to enable this building to be available
        /// </summary>
        /// <param name="techName">The name of the tech</param>
        /// <returns>This factory</returns>
        public BuildingFactory AddPrerequisiteTech(string techName)
        {
            var tech = Techs.TECH_GROUPING[techName];
            if (tech != null)
            {
                string[] newTechs = new string[tech.Length + 1];
                newTechs[0] = Id();
                tech.CopyTo(newTechs, 1);
                Techs.TECH_GROUPING[techName] = newTechs;
            }
            else
            {
                id.mod.logger.Warning("Could not find tech by name: {0}", techName);
            }
            return this;
        }

        /// <summary>
        /// Adds this building to the plan screen after a given building
        /// </summary>
        /// <param name="categoryName">The name of the category to add to</param>
        /// <param name="afterId">The ID to add after</param>
        /// <returns>This factory</returns>
        public BuildingFactory AddToPlanScreen(string categoryName, string afterId)
        {
            var categoryIndex = GetCategoryIndex(categoryName);
            if (categoryIndex >= 0)
            {
                if (afterId.IsNullOrWhiteSpace())
                {
                    return AddToPlanScreen(categoryName);
                }
                if (PLANORDER[categoryIndex].data is List<string> category)
                {
                    var afterIndex = category.IndexOf(afterId);
                    if (afterIndex < 0)
                    {
                        id.mod.logger.Warning("Tried adding building after '{0}' in '{1}', but no such building exists", afterId, categoryName);
                    }
                    return AddToPlanScreen(categoryIndex, afterIndex);
                }
            }
            return this;
        }

        /// <summary>
        /// Adds this building to the plan, screen optionally at a given index (otherwise to the end).
        /// </summary>
        /// <param name="categoryName">The name of the category to add to</param>
        /// <param name="index">The index to insert at</param>
        /// <returns>This factory</returns>
        public BuildingFactory AddToPlanScreen(string categoryName, int? index = null)
        {
            return AddToPlanScreen(GetCategoryIndex(categoryName), index);
        }

        private BuildingFactory AddToPlanScreen(int categoryIndex, int? positionIndex = null)
        {
            if (categoryIndex >= 0)
            {
                if (PLANORDER[categoryIndex].data is List<string> category)
                {
                    if (positionIndex != null && positionIndex >= 0 && positionIndex < category.Count)
                    {
                        category.Insert(positionIndex.GetValueOrDefault(), Id());
                    }
                    else
                    {
                        category.Add(Id());
                    }
                }
            }
            else
            {
                id.mod.logger.Warning("Attempted to add a building to a plan category that did not exist");
            }
            return this;
        }

        private int GetCategoryIndex(string category)
        {
            int index = PLANORDER.FindIndex(x => x.category.Equals(new HashedString(category)));
            if (index < 0)
            {
                id.mod.logger.Warning("Could not find plan category by name: {0}", category);
            }
            return index;
        }

        internal void RegisterStrings()
        {
            var simId = Id();
            string strName = $"STRINGS.BUILDINGS.PREFABS.{simId.ToUpper()}.NAME";
            string strDesc = $"STRINGS.BUILDINGS.PREFABS.{simId.ToUpper()}.DESC";
            string strEffect = $"STRINGS.BUILDINGS.PREFABS.{simId.ToUpper()}.EFFECT";

            Strings.Add(strName, UI.FormatAsLink(name, Id()));
            Strings.Add(strDesc, description);
            Strings.Add(strEffect, effect);
            id.mod.logger.Info("add Building strings: {0} ({1}): {2} ({3})",
                Strings.Get(strName),
                simId,
                Strings.Get(strDesc).String.Replace("\n", "\\n"),
                Strings.Get(strEffect).String.Replace("\n", "\\n"));
        }
    }
}
