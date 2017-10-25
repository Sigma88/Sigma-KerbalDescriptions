using System;


namespace SigmaKerbalDescriptions
{
    internal static class Extensions
    {
        internal static int Hash(this ProtoCrewMember kerbal)
        {
            string hash = Information.hash;

            if (string.IsNullOrEmpty(hash)) hash = kerbal.name;

            int h = Math.Abs(hash.GetHashCode());

            Information.hash = h.ToString();

            return h;
        }
    }
}
