using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class DynamicBlockService
    {
        // Return dynamic block properties
        public Dictionary<string, object> GetDynamicBlockProperties(BlockReference blockRef)
        {
            Dictionary<string, object> dynamicProps = new Dictionary<string, object>();

            // Get the dynamic properties of the block reference
            DynamicBlockReferencePropertyCollection dynamicPropCollection = blockRef.DynamicBlockReferencePropertyCollection;

            if (dynamicPropCollection != null && dynamicPropCollection.Count > 0)
            {
                foreach (DynamicBlockReferenceProperty prop in dynamicPropCollection)
                {
                    dynamicProps[prop.PropertyName] = prop.Value;
                }
            }

            return dynamicProps;
        }
    }
}
