﻿using System.Collections;
using System.Collections.Generic;

namespace PipLib.Mod
{
    /// <summary>
    /// The main abstract PipLib mod.
    /// <example>
    /// Implmented as such:
    /// <code>
    /// public class YourMod : PipMod
    /// {
    ///     public YourMod () : base("YourMod", "YM") { }
    ///     
    ///     public void Load ()
    ///     {
    ///         // Do your stuff here
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public abstract class PipMod
    {
        public readonly string name;
        public readonly string prefix;

        public readonly ILogger logger;

        protected internal readonly List<ElementFactory> elements = new List<ElementFactory>();

        public PipMod(string name, string prefix = null)
        {
            this.name = name;
            this.prefix = prefix ?? name;
            logger = GlobalLogger.Get().Fork(name);
        }

        public abstract void Load();

        public ElementFactory CreateElement(string name)
        {
            var element = PipLib.CreateElement(this, name);
            elements.Add(element);
            return element;
        }

        internal void RegisterSimHashes(Dictionary<SimHashes, string> hashTable, Dictionary<string, object> hashTableReverse)
        {
            foreach (var e in elements)
            {
                e.RegisterSimHashes(hashTable, hashTableReverse);
            }
        }

        internal void RegisterSubstances(Hashtable substanceList, SubstanceTable substanceTable)
        {
            foreach (var e in elements)
            {
                e.RegisterSubstances(substanceList, substanceTable);
            }
        }

        internal void RegisterStrings()
        {
            foreach (var e in elements)
            {
                e.RegisterStrings();
            }
        }

        internal void RegisterAttributes()
        {
            foreach (var e in elements)
            {
                e.RegisterAttributes();
            }
        }

        public override string ToString()
        {
            return name;
        }

        public override bool Equals(object obj)
        {
            return obj is PipMod mod &&
                   name == mod.name;
        }

        public override int GetHashCode()
        {
            // generated by Visual Studio
            return 363513814 + EqualityComparer<string>.Default.GetHashCode(name);
        }
    }

    /// <summary>
    /// Core implementation of <see cref="PipMod"/> used for internal mod management.
    /// </summary>
    public class BasePipMod : PipMod
    {
        public const string NAME = "BasePipMod";

        public const string MISSING_ANIM_NAME = "missinganim";
        public const string MISSING_TEX_NAME = "missingtex";

        public static BasePipMod instance;

        public static void OnLoad()
        {
            PipLib.Add(instance);
        }

        static BasePipMod()
        {
            instance = new BasePipMod();
        }

        public BasePipMod() : base(NAME, "Pip") { }

        public override void Load()
        {
            CreateElement("DebugElement")
                .SetUnlocalizedName("Debug Element")
                .AddUnlocalizedDescription("Internal debugging element for PipLib. Not intended for normal gameplay.")
                .SetColor(new UnityEngine.Color32(255, 80, 255, 255))
                .AddState(Element.State.Solid)
                .AddState(Element.State.Liquid)
                .AddState(Element.State.Gas)
                .AddAttributeModifier(db => db.BuildingAttributes.Decor.Id, 1f, isMultiplier: true)
                .AddAttributeModifier(db => db.BuildingAttributes.OverheatTemperature.Id, 1000f);
        }
    }
}
