using System;


namespace SigmaKerbalDescriptions
{
    internal static class Extensions
    {
        internal static int Hash(this ProtoCrewMember kerbal, bool useGameSeed)
        {
            string hash = Information.hash;

            if (string.IsNullOrEmpty(hash)) hash = kerbal.name;

            int h = Math.Abs(hash.GetHashCode());

            if (useGameSeed) h += Math.Abs(HighLogic.CurrentGame.Seed);

            Information.hash = h.ToString();

            return h;
        }
    }
}
