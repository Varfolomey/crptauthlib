﻿namespace CRPTAuthLib35.Utils
{
    internal class StringUtil
    {
        internal static string Obfuscate(string input)
        {
            return input.Replace("\t", "").Replace("\n", "").Replace(" ", "");
        }
    }
}
