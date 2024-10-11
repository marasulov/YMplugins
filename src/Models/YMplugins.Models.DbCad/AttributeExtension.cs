using Autodesk.AutoCAD.DatabaseServices;

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
        public static string GetBlockAttribute(this BlockReference blockReference, string tag)
        {
            var attrs = Gile.AutoCAD.Extension.BlockReferenceExtension.GetAttributesValues(blockReference);

            return attrs.ContainsKey(tag) ? attrs[tag] : null;
        }
    }
}