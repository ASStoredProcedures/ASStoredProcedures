using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AnalysisServices.AdomdServer;

namespace ASStoredProcs
{
    public class StrToSet
    {
        //NOTE: passing in a Hierarchy object would not have performed well, so pass in the Hierarchy unique name as a string

        //Usage:
        //KeysStrToSet("[Product].[Product]", "1,2,3")
        //KeysStrToSet("[Product].[Product]", "{ 1,2,3 }")
        public static Set KeysStrToSet(string Hierarchy, string KeysCommaSeparated)
        {
            if (KeysCommaSeparated.StartsWith(Hierarchy, StringComparison.InvariantCultureIgnoreCase)
            || KeysCommaSeparated.StartsWith("{ " + Hierarchy, StringComparison.InvariantCultureIgnoreCase)
            || KeysCommaSeparated.StartsWith("{" + Hierarchy, StringComparison.InvariantCultureIgnoreCase))
            {
                //a real set was passed in, so there's no need to deal with keys
                return new Expression(KeysCommaSeparated).CalculateMdxObject(null).ToSet();
            }

            string[] keys = KeysCommaSeparated.Split(new char[] { ',' });
            StringBuilder sSet = new StringBuilder(keys.Length * (Hierarchy.Length + 4) + KeysCommaSeparated.Length + 10);
            sSet.Append("{");
            if (keys.Length > 1 || (keys.Length == 1 && !string.IsNullOrEmpty(keys[0])))
            {
                if (keys[0].StartsWith("{ "))
                    keys[0] = keys[0].Substring(2);
                if (keys[keys.Length - 1].EndsWith(" }"))
                    keys[keys.Length - 1] = keys[keys.Length - 1].Substring(0, keys[keys.Length - 1].Length - 2);

                foreach (string key in keys)
                {
                    if (sSet.Length > 1) sSet.Append(',');
                    sSet.Append(Hierarchy).Append(".&[").Append(key.Replace("]", "]]")).Append(']');
                }
            }
            //TODO: if it's empty, consider doing Head(<hierarchy>.Members,0) so that the set is "typed"?
            sSet.Append("}");
            return new Expression(sSet.ToString()).CalculateMdxObject(null).ToSet();
        }

        //Usage:
        //CompositeKeysStrToSet("[Geography].[City]", "&[Austin]&[TX],&[Baltimore]&[MD]")
        //CompositeKeysStrToSet("[Geography].[City]", "{ &[Austin]&[TX],&[Baltimore]&[MD] }")
        public static Set CompositeKeysStrToSet(string Hierarchy, string KeysCommaSeparated)
        {
            if (KeysCommaSeparated.StartsWith(Hierarchy, StringComparison.InvariantCultureIgnoreCase)
            || KeysCommaSeparated.StartsWith("{ " + Hierarchy, StringComparison.InvariantCultureIgnoreCase)
            || KeysCommaSeparated.StartsWith("{" + Hierarchy, StringComparison.InvariantCultureIgnoreCase))
            {
                //a real set was passed in, so there's no need to deal with keys
                return new Expression(KeysCommaSeparated).CalculateMdxObject(null).ToSet();
            }

            string[] keys = MDXSplit(KeysCommaSeparated, ',');
            StringBuilder sSet = new StringBuilder(keys.Length * (Hierarchy.Length + 1) + KeysCommaSeparated.Length + 10);
            sSet.Append("{");
            if (keys.Length > 1 || (keys.Length == 1 && !string.IsNullOrEmpty(keys[0])))
            {
                if (keys[0].StartsWith("{ "))
                    keys[0] = keys[0].Substring(2);
                if (keys[keys.Length - 1].EndsWith(" }"))
                    keys[keys.Length - 1] = keys[keys.Length - 1].Substring(0, keys[keys.Length - 1].Length - 2);

                foreach (string key in keys)
                {
                    if (sSet.Length > 1) sSet.Append(',');
                    sSet.Append(Hierarchy).Append('.').Append(key);
                }
            }
            sSet.Append("}");
            return new Expression(sSet.ToString()).CalculateMdxObject(null).ToSet();
        }

        #region Internal Helper Functions
        //internal helper function... an escaped key could contain a comma, so we should only split on commas outside of brackets
        private static string[] MDXSplit(string sMDX, char cDelimiter)
        {
            int iPos = 0;
            int iLastSplit = 0;
            bool bInBracket = false;
            string sDelimiter = cDelimiter.ToString();
            System.Collections.Generic.List<string> arrSplits = new System.Collections.Generic.List<string>();
            while (iPos < sMDX.Length)
            {
                if (bInBracket)
                {
                    if (sMDX.Substring(iPos, 1) == "]")
                    {
                        if (sMDX.Length > iPos + 1 && sMDX.Substring(iPos, 2) != "]]")
                        {
                            bInBracket = false;
                        }
                        else
                        {
                            iPos += 1;
                        }
                    }
                }
                else
                {
                    if (sMDX.Substring(iPos, 1) == "[")
                    {
                        bInBracket = true;
                    }
                    else if (sMDX.Substring(iPos, 1) == sDelimiter)
                    {
                        arrSplits.Add(sMDX.Substring(iLastSplit, iPos - iLastSplit));
                        iLastSplit = iPos + 1;
                    }
                }
                iPos++;
            }
            iPos = sMDX.Length;
            if (!string.IsNullOrEmpty(sMDX))
                arrSplits.Add(sMDX.Substring(iLastSplit, iPos - iLastSplit));
            return arrSplits.ToArray();
        }
        #endregion
    }
}
