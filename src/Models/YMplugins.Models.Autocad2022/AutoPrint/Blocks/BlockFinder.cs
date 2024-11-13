using Gile.AutoCAD.Extension;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2022.Contracts;

namespace YMplugins.Models.Autocad2022.AutoPrint.Blocks
{
    public class BlockFinder : IBlockFinder
    {
        private readonly BlockSearchService _blockSearchService;
        private readonly SearchData _data;

        public BlockFinder(BlockSearchService blockSearchService, SearchData data)
        {
            _blockSearchService = blockSearchService;
            _data = data;
        }

        public ObservableCollection<PrintInfo> FindBlocks(SearchData data)
        {
            var blocks = new List<PrintInfo>();
            string searchSpace = default;
            if (data.IsSearchOnLayouts & data.IsSearchOnModel) searchSpace = "Both";
            else if (data.IsSearchOnModel) searchSpace = "Model";
            else searchSpace = "Layout";
            
            // if (!data.IsSearchFromAllDocuments)
            // {
            //     if (data.IsCheckedNumbering)
            //     {
            //         blocks.AddRange(_blockSearchService.SearchAllBlocksInSpaceByName(Active.Database, data.SelectedBlockName, searchSpace, data.NumerationStartValue));
            //     }
            //     else
            //     {
            //         blocks.AddRange(_blockSearchService.SearchAllBlocksInSpaceByName(Active.Database, data.SelectedBlockName, searchSpace));
            //     }
            //         
            // }
            // else
            // {
            //     foreach (Document doc in Application.DocumentManager)
            //     {
            //         if (data.IsCheckedNumbering)
            //         {
            //             blocks.AddRange(_blockSearchService.SearchAllBlocksInSpaceByName(doc.Database, data.SelectedBlockName, searchSpace, data.NumerationStartValue));
            //         }
            //         else
            //         {
            //             blocks.AddRange(_blockSearchService.SearchAllBlocksInSpaceByName(doc.Database, data.SelectedBlockName, searchSpace));
            //         }    
            //     }
            // }

            var databases =
                Application.DocumentManager.Cast<Document>();
                

            foreach (var db in databases)
            {
                if (data.IsCheckedNumbering)
                {
                    blocks.AddRange(_blockSearchService.SearchAllBlocksInSpaceByName(db, data.SelectedBlockName, searchSpace, data.NumerationStartValue));
                }
                else
                {
                    blocks.AddRange(_blockSearchService.SearchAllBlocksInSpaceByName(db, data.SelectedBlockName, searchSpace));
                }
            }
            
            

            return new ObservableCollection<PrintInfo>(blocks);
        }


    }
}