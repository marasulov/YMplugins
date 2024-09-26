using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Contracts.Dto.Enums;
using YMplugins.ViewModels.Commands;

namespace YMplugins.ViewModels.VM
{
    public class AutoPrintVm : BaseViewModel
    {
        private ObservableCollection<string> _blocksNames;
        private List<string> _layers;
        private List<BlockAttribute> _attributes;
        private bool _canExecute = true;
        private string _selectedBlockOnScreen;
        private readonly ZoomToPointCommand _zoomToPointCommand;
        private IAttributesService _attributesService;
        private readonly ISearchService _searchService;
        private ObservableCollection<PrintInfo> _blockDataCollection;
        private BlockAttribute _selectedAttr;
        private bool _isUpdatingAttributes;
        private int _numerationStartValue;

        //private string _selectedBlock;

        public AutoPrintVm(
            GetBlocksNameCommand getBlocksNameCommand,
            GetLayersCommand getLayersCommand,
            GetAttributesCommand getAttributesCommand,
            PrintCommand printCommand,
            SelectBlockCommand selectBlockCommand,
            ZoomToPointCommand zoomToPointCommand,
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
            ZoomToPointCommand = zoomToPointCommand;
        }

        private void GetLayersCommandOnResultObtained(List<string> obj)
        {
            Layers = obj;
        }

        private void BlocksCommand_ResultObtained(ObservableCollection<string> obj)
        {
            BlocksNames = obj;
        }

        public ObservableCollection<string> BlocksNames
        {
            get => _blocksNames;
            set
            {
                Set(ref _blocksNames, value);
                UpdateAttributes();
            }
        }

        // Свойство для выбора между блоком и полилинией
        private PrintByOption _selectedPrintByOption;
        private string _prefix;
        private string _suffix;

        public PrintByOption SelectedPrintByOption
        {
            get => _selectedPrintByOption;
            set
            {
                if (Set(ref _selectedPrintByOption, value))
                {
                    if (_selectedPrintByOption == PrintByOption.ByPolyline)
                    {
                        BlockDataCollection.Clear();  
                    }
                    else if (_selectedPrintByOption == PrintByOption.ByBlock)
                    {
                        UpdateAttributes();
                        UpdateBlockCollection();
                    }

                }
            }
        }

        // Свойство для выбора порядка печати
        public PrintingOrder SelectedPrintingOrder { get; set; }


        public List<string> Layers
        {
            get => _layers;
            set => Set(ref _layers, value);
        }

        public List<BlockAttribute> Attributes
        {
            get => _attributes;
            set => Set(ref _attributes, value);
        }

        public ObservableCollection<PrintInfo> BlockDataCollection
        {
            get => _blockDataCollection;
            set
            {
                Set(ref _blockDataCollection, value);
            }
        }

        public string SelectedBlockOnScreen
        {
            get => _selectedBlockOnScreen;
            set
            {
                if (!Set(ref _selectedBlockOnScreen, value)) return;
                UpdateAttributes(); 
                UpdateBlockCollection();
            }
        }

        public Action CloseAction { get; set; }
        public Action OpenAction { get; set; }
        public GetBlocksNameCommand GetBlocksNameCommand { get; }
        public SelectBlockCommand SelectBlockCommand { get; }
        public GetLayersCommand GetLayersCommand { get; }
        public GetAttributesCommand GetAttributesCommand { get; }
        public PrintCommand PrintCommand { get; }
        public ZoomToPointCommand ZoomToPointCommand { get; }

        public BlockAttribute SelectedAttr
        {
            get => _selectedAttr;
            set
            {
                if (Set(ref _selectedAttr, value))
                {
                    UpdateBlockCollection();
                }
            } 
        }
        public string Prefix
        {
            get => _prefix;
            set
            {
                Set(ref _prefix, value);
                
                UpdateBlockCollection();
            }
        }

        public string Suffix
        {
            get => _suffix;
            set
            {
                Set(ref _suffix, value);
                
                UpdateBlockCollection();
            }
        }
        public bool IsSearchOnModel { get; set; } = true;
        public bool IsSearchOnLayout { get; set; }

        public int NumerationStartValue
        {
            get =>_numerationStartValue;
            set
            {
                if (_numerationStartValue == value)
                {
                    return;
                }

                if (IsCheckedNumbering & value == null)
                {
                    _numerationStartValue = 0;
                }
                else
                {
                    _numerationStartValue = value;
                }
                OnPropertyChanged();
                UpdateBlockCollection();
            }
        }
        public bool IsCheckedNumbering { get; set; }


        private void UpdateAttributes()
        {
            if (_isUpdatingAttributes || string.IsNullOrEmpty(_selectedBlockOnScreen))
                return;

            Attributes = _attributesService.GetAttributesForBlock(_selectedBlockOnScreen);
            

            var printData = new SearchData
            {
                SelectedPrintByOption = SelectedPrintByOption,
                SelectedPrintingOrder = SelectedPrintingOrder,
                IsSearchOnModel = IsSearchOnModel,
                IsSearchOnLayouts = IsSearchOnLayout,
                SelectedBlockName = SelectedBlockOnScreen,
                AttributeName = SelectedAttr?.AttributeName,
                NumerationStartValue = NumerationStartValue,
                Prefix = Prefix,
                Suffix = Suffix
            };
            BlockDataCollection = _searchService.FindObjects(printData);
            _isUpdatingAttributes = false;
        }

        private void UpdateBlockCollection()
        {
            if (SelectedAttr != null && BlockDataCollection != null)
            {
                BlockDataCollection = new ObservableCollection<PrintInfo>(
                    _attributesService.GetPrintInfosForBlock(BlockDataCollection, SelectedAttr.AttributeName, NumerationStartValue, Prefix, Suffix));
            }
        }
    }
}