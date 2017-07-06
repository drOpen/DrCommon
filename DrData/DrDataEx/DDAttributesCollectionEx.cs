using DrOpen.DrCommon.DrData.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrOpen.DrCommon.DrData
{
    public static class DDAttributesCollectionEx
    {
        #region ContainsAttributesOtherwiseThrow

        /// <summary>
        /// Determines whether the Attribute Collection contains an elements with the specified names and will be throw new <exception cref="ContainsAttributesException"/> if one of these attributes was not found
        /// </summary>
        /// <param name="attr">collection of DDValue that can be accessed by name.</param>
        /// <param name="names">list of names of mandatory attributes</param>
        /// <exception cref="DDMissingSomeOfAttributesException"/>
        public static void ContainsAttributesOtherwiseThrow(this DDAttributesCollection attr, params string[] names)
        {
            containsAttributesOtherwiseThrow(attr, names);
        }
        /// <summary>
        /// Determines whether the Attribute Collection contains an elements with the specified names and will be throw new <exception cref="ContainsAttributesException"/> if one of these attributes was not found
        /// </summary>
        /// <param name="attr">collection of DDValue that can be accessed by name.</param>
        /// <param name="names">list of names of mandatory attributes</param>
        /// <exception cref="DDMissingSomeOfAttributesException"/>
        private static void containsAttributesOtherwiseThrow(this DDAttributesCollection attr, IEnumerable<string> names)
        {
            var nel = new List<string>();
            foreach (var name in names)
            {
                if (!attr.Contains(name)) nel.Add(name);
            }
            if (nel.Count > 0) throw new DDMissingSomeOfAttributesException(nel.ToArray());
        }
        #endregion ContainsAttributesOtherwiseThrow
    }
}
