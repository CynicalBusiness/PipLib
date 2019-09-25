using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PipLib.Mod
{
    public class PipModImpl : PipMod
    {
        public override string Name => "PipLib";

        public override void Initialize()
        {
            logger.Info("ok!");
        }
    }
}
