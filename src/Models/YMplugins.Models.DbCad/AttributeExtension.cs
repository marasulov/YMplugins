using Autodesk.AutoCAD.DatabaseServices;
#if NET8_0_OR_GREATER
using Gile.AutoCAD.R25.Extension;
#else
using Gile.AutoCAD.R20.Extension;
#endif

namespace YMplugins.Models.DbCad
{
    public static class AttributeExtension
    {
        /// <summary>
        /// Get block attribute.
        /// </summary>
        /// <param name="blockReference">The block reference.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>The value.</returns>
        public static string GetBlockAttribute(this BlockReference blockReference, Transaction tr, string tag)
        {
            var attrs = blockReference.GetAttributesValues(tr);

            return attrs.ContainsKey(tag) ? attrs[tag] : null;
        }
    }
}