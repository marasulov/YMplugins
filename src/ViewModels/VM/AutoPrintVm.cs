using System;
using System.Collections.Generic;
using System.Linq;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Contracts.Dto.Enums;
using YMplugins.ViewModels.Commands;

namespace YMplugins.ViewModels.VM
{
    public class AutoPrintVm : BaseViewModel
    {
        private List<string> _blocksNames;
        private List<string> _layers;
        private IEnumerable<BlockAttribute> _attributes;

        private bool _canExecute = true;
        private string _selectedBlockOnScreen;
        private IAttributesService _attributesService;
        private readonly ISearchService _searchService;

        private IEnumerable<PrintInfo> _blockDataCollection;
        //private string _selectedBlock;

        public AutoPrintVm(
            GetBlocksNameCommand getBlocksNameCommand,
            GetLayersCommand getLayersCommand,
            GetAttributesCommand getAttributesCommand,
            PrintCommand printCommand,
            SelectBlockCommand selectBlockCommand,
            IAttributesService attributesService, ISearchService searchService)
        {
            _attributesService = attributesService;
            _searchService = searchService;
            GetBlocksNameCommand = getBlocksNameCommand;
            getBlocksNameCommand.ResultObtained += BlocksCommand_ResultObtained;
            GetLayersCommand = getLayersCommand;
            getLayersCommand.ResultObtained += GetLayersCommandOnResultObtained;
            GetAttributesCommand = getAttributesCommand;
            PrintCommand = printCommand;
            SelectBlockCommand = selectBlockCommand;
        }

        private void GetLayersCommandOnResultObtained(List<string> obj)
        {
            Layers = obj;
        }

        private void BlocksCommand_ResultObtained(List<string> obj)
        {
            BlocksNames = obj;
        }

        public List<string> BlocksNames
        {
            get => _blocksNames;
            set
            {
                Set(ref _blocksNames, value);
                UpdateAttributes();
            }
        }

        //public string SelectedBlock
        //{
        //    get => _selectedBlock;
        //    set
        //    {
        //        Set(ref _selectedBlock, value);

        //    }
        //}

        // Свойство для выбора между блоком и полилинией
        public PrintByOption SelectedPrintByOption { get; set; }

        // Свойство для выбора порядка печати
        public PrintingOrder SelectedPrintingOrder { get; set; }


        public List<string> Layers
        {
            get => _layers;
            set => Set(ref _layers, value);
        }

        public IEnumerable<BlockAttribute> Attributes
        {
            get => _attributes;
            set => Set(ref _attributes, value);
        }

        public IEnumerable<PrintInfo> BlockDataCollection
        {
            get => _blockDataCollection;
            set => Set(ref _blockDataCollection, value);
        }

        public string SelectedBlockOnScreen
        {
            get => _selectedBlockOnScreen;
            set
            {
                Set(ref _selectedBlockOnScreen, value);
                //if (value != null)
                //{
                //    _attributesService.GetAttributesForBlock(value);
                //}

                UpdateAttributes();
            }
        }

        public Action CloseAction { get; set; }
        public Action OpenAction { get; set; }

        public GetBlocksNameCommand GetBlocksNameCommand { get; }

        public SelectBlockCommand SelectBlockCommand { get; }

        public GetLayersCommand GetLayersCommand { get; }

        public GetAttributesCommand GetAttributesCommand { get; }

        public PrintCommand PrintCommand { get; }
        public string SelectedAttr { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public bool IsSearchOnModel { get; set; } = true;
        public bool IsSearchOnLayout { get; set; }
        public int RenumberStartValue { get; set; }

        public object FileName { get; }

        private void UpdateAttributes()
        {
            if (string.IsNullOrEmpty(_selectedBlockOnScreen))
            {
                Attributes = Enumerable.Empty<BlockAttribute>();
                return;
            }

            Attributes = _attributesService.GetAttributesForBlock(_selectedBlockOnScreen);
            BlockDataCollection = _searchService.FindObjects(SelectedBlockOnScreen);
        }
    }
}