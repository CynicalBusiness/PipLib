using Database;
using System.Collections.Generic;
using UnityEngine;

namespace PipLib.Tech
{
    public sealed class TechTree
    {

        public const float X = 350;
        public const float X0 = 200;

        public const float Y = 250;
        public const float Y0 = 100;

        public const string LOCNAME_TITLE = "STRINGS.RESEARCH.TREES.TITLE";
        public const string LOCNAME_TECH = "STRINGS.RESEARCH.TECHS.";

        public static readonly ResourceTreeNode ORIGIN = new ResourceTreeNode(){ nodeX = 0, nodeY = 0, width = 250, height = 72 };

        public static readonly Dictionary<string, string> titleAssociations = new Dictionary<string, string>();

        public static TechTree Instance { get; private set; }

        private static Logging.ILogger Logger = PipLib.Logger.Fork(nameof(TechTree));

        /// <summary>
        /// Adds a new tech tier using the given cost map
        /// </summary>
        /// <param name="tierCosts">The costs</param>
        /// <param name="before">Optionally, the tier to add this tier before (defaults to the end)</param>
        /// <returns>The index of the newly added tier</returns>
        public static int AddTier (Dictionary<string, float> tierCosts, int? before)
        {
            var tiers = GetTechTiers();
            var tier = before ?? tiers.Count;

            var tierCostList = new List<Tuple<string, float>>();
            foreach (var tierCostEntry in tierCosts)
            {
                tierCostList.Add(new Tuple<string, float>(tierCostEntry.Key, tierCostEntry.Value));
            }
            tiers.Insert(tier, tierCostList);
            return tier;
        }

        /// <summary>
        /// Gets the cost map of a given tech tier
        /// </summary>
        /// <param name="tier">The tier to fetch</param>
        /// <returns>The tier costs</returns>
        /// <exception cref="OutOfBoundsException">If the tier is greater than the total number of tiers</exception>
        public static Dictionary<string, float> GetTierCosts (int tier)
        {
            var tierCostList = GetTechTiers()[tier];

            var tierCosts = new Dictionary<string, float>();
            foreach (var tierCost in tierCostList)
            {
                tierCosts.Add(tierCost.first, tierCost.second);
            }

            return tierCosts;
        }

        public static global::Tech GetTech (string id)
        {
            return Instance.db.Techs.Get(id);
        }

        public static global::Tech CreateTech (string id)
        {
            string locname = LOCNAME_TECH + id.ToUpper();
            var tech = new global::Tech(id, Instance.Techs, Strings.Get(locname + ".NAME"), Strings.Get(locname + ".DESC"), new ResourceTreeNode(){
                nodeX = ORIGIN.nodeX,
                nodeY = ORIGIN.nodeY,
                width = ORIGIN.width,
                height = ORIGIN.height
            });
            return tech;
        }

        public static void AddRequirement (global::Tech unlocked, global::Tech required)
        {
            required.unlockedTech.Add(unlocked);
            unlocked.requiredTech.Add(required);

            SetTier(unlocked, required.tier + 1);

            var unlockedNode = GetNode(unlocked);
            var requiredNode = GetNode(required);
            var edge = new ResourceTreeNode.Edge(requiredNode, unlockedNode, ResourceTreeNode.Edge.EdgeType.GenericEdge);
            unlockedNode.edges.Add(edge);
            requiredNode.edges.Add(edge);
        }

        public static void SetTier (global::Tech tech, int tier)
        {
            tech.tier = tier;
            tech.costsByResearchTypeID = GetTierCosts(tier);
        }

        internal static void Create (Techs techs)
        {
            Instance = Instance ?? new TechTree();
            Logger.Info("Created new TechTree");
        }

        private static ResourceTreeNode GetNode (global::Tech tech)
        {
            return (ResourceTreeNode)Harmony.Traverse.Create(tech).Field("node").GetValue();
        }

        private static List<List<Tuple<string, float>>> GetTechTiers ()
        {
            return (List<List<Tuple<string, float>>>)Harmony.Traverse.Create(Instance.Techs).Field("TECH_TIERS").GetValue();
        }

        private readonly Db db;

        public readonly Techs Techs;

        public TechTreeTitles TechTreeTitles { get; private set; }

        private TechTree ()
        {
            db = Db.Get();
            Techs = db.Techs;
            TechTreeTitles = db.TechTreeTitles;

            titleAssociations.Add("_InteriorDecor", "_ColonyDevelopment");
            titleAssociations.Add("_GasPiping", "_Gases");
            titleAssociations.Add("_LiquidPiping", "_Liquids");
            titleAssociations.Add("_MedicineI", "_Medicine");
            titleAssociations.Add("_Jobs", "_Solids");
            titleAssociations.Add("_PowerRegulation", "_Power");
            titleAssociations.Add("_FarmingTech", "_Food");
        }

        public void ResetArrangement ()
        {
            Logger.Verbose("Resetting tech tree arrangement...");

            // clear titles
            db.TechTreeTitles.resources.Clear();

            // set all nodes to 0,0
            foreach (var tech in Techs.resources)
            {
                var node = GetNode(tech);
                node.nodeX = 0;
                node.nodeY = 0;
            }
        }

        public void RebuildArragement ()
        {
            Logger.Info("Rebuilding tech tree (this can take time)...");
            ResetArrangement();

            // this is a little silly, don't you think?
            var techTable = new Dictionary<int, Dictionary<int, List<global::Tech>>>();
            var techRows = new Dictionary<string, int>();
            var techRowTitles = new Dictionary<int, string>();
            int techRowNum = 0;

            // calculate row nums
            foreach (var tech in Techs.resources)
            {
                if (tech.requiredTech.Count == 0 && !techRows.ContainsKey(tech.Id))
                {
                    techRowTitles.Add(techRowNum, tech.Id);
                    techRows.Add(tech.Id, techRowNum++);
                }
            }

            // push the techs into a 2D dictionary(?) of tech lists
            foreach (var tech in Techs.resources)
            {
                var row = GetRow(tech, techRows);
                var col = GetCol(tech);

                Dictionary<int, List<global::Tech>> techRow;
                if (!techTable.TryGetValue(row, out techRow))
                {
                    techRow = new Dictionary<int, List<global::Tech>>();
                    techTable.Add(row, techRow);
                }

                List<global::Tech> techCol;
                if (!techRow.TryGetValue(col, out techCol))
                {
                    techCol = new List<global::Tech>();
                    techRow.Add(col, techCol);
                }

                techCol.Add(tech);
            }

            // space the techs out in their 2D table
            float rowHeight = Y0;
            List<Tuple<string, ResourceTreeNode>> titles = new List<Tuple<string, ResourceTreeNode>>(techTable.Count);
            foreach (var techTableEntry in techTable)
            {
                var techRowI = techTableEntry.Key;
                var techRow = techTableEntry.Value;

                float colMaxHeight = 0;
                foreach (var techCol in techRow.Values)
                {
                    colMaxHeight = Mathf.Max(techCol.Count * Y, colMaxHeight);
                }
                colMaxHeight += ORIGIN.height;

                var titleID = "_" + techRowTitles[techRowI];
                if (titleAssociations.ContainsKey(titleID))
                {
                    titleID = titleAssociations[titleID];
                }
                titles.Insert(techRowI, new Tuple<string, ResourceTreeNode>(titleID, new ResourceTreeNode(){
                    nodeX = -X0,
                    nodeY = -(rowHeight - (ORIGIN.height * 2)),
                    width = ORIGIN.width,
                    height = ORIGIN.height
                }));

                foreach (var techRowEntry in techRow)
                {
                    var techColI = techRowEntry.Key;
                    var techCol = techRowEntry.Value;

                    var colHeight = (techCol.Count + 1) * Y;
                    var colOffset = (colMaxHeight - colHeight) / 2;

                    for (int techI = 0; techI < techCol.Count; techI++)
                    {
                        var tech = techCol[techI];
                        var node = GetNode(tech);
                        node.nodeX = (techColI * X) + X0;
                        node.nodeY = -(rowHeight + (techI * Y) + colOffset);
                    }
                }

                rowHeight += colMaxHeight;
            }

            foreach (var titleData in titles)
            {
                var titleID = titleData.first;
                var node = titleData.second;
                var title = new TechTreeTitle(titleID, TechTreeTitles, Strings.Get(LOCNAME_TITLE + titleID.ToUpper()), node);
            }

            Logger.Info("Completed TechTree rebuild: {0} techs, {1} titles", Techs.resources.Count, titles.Count);
        }

        private int GetRow (global::Tech tech, Dictionary<string, int> rows)
        {
            return tech.requiredTech.Count > 0
                ? GetRow(tech.requiredTech[0], rows)
                : rows[tech.Id];
        }

        private int GetCol (global::Tech tech)
        {
            return tech.tier;
        }
    }
}
