using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Contracts.Dto.Enums;
using YMplugins.ViewModels.Commands;

namespace YMplugins.ViewModels.VM
{
    public class AutoPrintVm : BaseViewModel
    {
        private readonly IAttributesService _attributesService;
        private readonly ISearchService _searchService;
        private List<string> _blocksNames;
        private List<string> _layers;
        private List<BlockAttribute> _attributes;
        private string _selectedBlockOnScreen;
        private string _selectLayerOnScreen;
        private BlockAttribute _selectedAttr;
        private bool _isUpdatingAttributes;
        private int _numerationStartValue;
        private PrintByOption _selectedPrintByOption;
        private string _prefix;
        private string _suffix;
        private bool _isCheckedNumbering = true;
        private string _outputFileName;
        private bool _isAllPrintSelected = true;
        private List<PrintInfo> _printDataCollection;
        private int _selectedPrintCount;
        private string _error;
        private bool _isUpdatingBlockCollection;
        private bool _isSearchOnLayout;
        private bool _isSearchOnModel = true;
        private string _fileName;

        public AutoPrintVm(
            GetBlocksNameCommand getBlocksNameCommand,
            GetLayersCommand getLayersCommand,
            GetAttributesCommand getAttributesCommand,
            PrintCommand printCommand,
            SelectBlockCommand selectBlockCommand,
            ZoomToPointCommand zoomToPointCommand,
            IAttributesService attributesService,
            ISearchService searchService)
        {
            _attributesService = attributesService ?? throw new ArgumentNullException(nameof(attributesService));
            _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));

            InitializeCommands(getBlocksNameCommand, getLayersCommand, getAttributesCommand,
                               printCommand, selectBlockCommand, zoomToPointCommand);
        }

        public List<string> BlocksNames
        {
            get => _blocksNames;
            set
            {
                if (Set(ref _blocksNames, value))
                {
                    UpdateAttributes();
                }
            }
        }

        public List<string> Layers
        {
            get => _layers;
            set
            {
                if (Set(ref _layers, value))
                {
                    SearchPolylinesInLayer();
                }
            }
        }

        public List<BlockAttribute> Attributes
        {
            get => _attributes;
            set => Set(ref _attributes, value);
        }

        public List<PrintInfo> PrintDataCollection
        {
            get => _printDataCollection;
            set => SetPrintDataCollection(value);
        }

        public string SelectedBlockOnScreen
        {
            get => _selectedBlockOnScreen;
            set
            {
                if (Set(ref _selectedBlockOnScreen, value))
                {
                    UpdateAttributes();
                    UpdateBlockCollection();
                }
            }
        }

        public string SelectedLayerOnScreen
        {
            get => _selectLayerOnScreen;
            set
            {
                if (Set(ref _selectLayerOnScreen, value))
                {
                    SearchPolylinesInLayer();
                }
            }
        }

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

        public PrintByOption SelectedPrintByOption
        {
            get => _selectedPrintByOption;
            set => SetSelectedPrintByOption(value);
        }

        public PrintingOrder SelectedPrintingOrder { get; set; }

        public string Prefix
        {
            get => _prefix;
            set
            {
                if (Set(ref _prefix, value))
                {
                    UpdateBlockCollection();
                }
            }
        }

        public string Suffix
        {
            get => _suffix;
            set
            {
                if (Set(ref _suffix, value))
                {
                    UpdateBlockCollection();
                }
            }
        }

        public bool IsSearchOnModel
        {
            get => _isSearchOnModel;
            set
            {
                Set(ref _isSearchOnModel, value);
                if (SelectedBlockOnScreen == null) return;
                UpdateAttributes();
                UpdateBlockCollection();
            }
        }

        public bool IsSearchOnLayout
        {
            get => _isSearchOnLayout;
            set
            {
                Set(ref _isSearchOnLayout, value);
                if (SelectedBlockOnScreen == null) return;
                UpdateAttributes();
                UpdateBlockCollection();
            }

        }

        public int NumerationStartValue
        {
            get => _numerationStartValue;
            set => SetNumerationStartValue(value);
        }

        public bool IsCheckedNumbering
        {
            get => _isCheckedNumbering;
            set
            {
                if (Set(ref _isCheckedNumbering, value))
                {
                    UpdateBlockCollection();
                }
            }
        }

        public bool IsCombinePdf { get; set; }

        public string OutputFileName
        {
            get => _outputFileName;
            set => Set(ref _outputFileName, value);
        }

        public bool IsAllPrintSelected
        {
            get => _isAllPrintSelected;
            set => SetIsAllPrintSelected(value);
        }

        public int SelectedPrintCount
        {
            get => _selectedPrintCount;
            set
            {
                if (_selectedPrintCount == value) return;
                _selectedPrintCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HeaderContent));
            }
        }

        public string HeaderContent => $"Blocks found {PrintDataCollection?.Count ?? 0} | Selected for Printing: {SelectedPrintCount}";

        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        public string FileName
        {
            get => _fileName;
            set => Set(ref _fileName, value);
        }

        public Action CloseAction { get; set; }
        public Action OpenAction { get; set; }
        public GetBlocksNameCommand GetBlocksNameCommand { get; private set; }
        public SelectBlockCommand SelectBlockCommand { get; private set; }
        public GetLayersCommand GetLayersCommand { get; private set; }
        public GetAttributesCommand GetAttributesCommand { get; private set; }
        public PrintCommand PrintCommand { get; private set; }
        public ZoomToPointCommand ZoomToPointCommand { get; private set; }
        public int PlineScale { get; }

        private void InitializeCommands(
            GetBlocksNameCommand getBlocksNameCommand,
            GetLayersCommand getLayersCommand,
            GetAttributesCommand getAttributesCommand,
            PrintCommand printCommand,
            SelectBlockCommand selectBlockCommand,
            ZoomToPointCommand zoomToPointCommand)
        {
            GetBlocksNameCommand = getBlocksNameCommand ?? throw new ArgumentNullException(nameof(getBlocksNameCommand));
            GetBlocksNameCommand.ResultObtained += BlocksCommand_ResultObtained;

            GetLayersCommand = getLayersCommand ?? throw new ArgumentNullException(nameof(getLayersCommand));
            GetLayersCommand.ResultObtained += GetLayersCommandOnResultObtained;

            GetAttributesCommand = getAttributesCommand ?? throw new ArgumentNullException(nameof(getAttributesCommand));
            PrintCommand = printCommand ?? throw new ArgumentNullException(nameof(printCommand));
            SelectBlockCommand = selectBlockCommand ?? throw new ArgumentNullException(nameof(selectBlockCommand));
            ZoomToPointCommand = zoomToPointCommand ?? throw new ArgumentNullException(nameof(zoomToPointCommand));
        }

        private void BlocksCommand_ResultObtained(List<string> obj)
        {
            BlocksNames = obj;
        }

        private void GetLayersCommandOnResultObtained(List<string> obj)
        {
            Layers = obj;
        }

        private void OnBlockDataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PrintInfo.IsPrint))
            {
                UpdateSelectedPrintCount();
            }
        }

        private void UpdateSelectedPrintCount()
        {
            SelectedPrintCount = PrintDataCollection?.Count(b => b.IsPrint) ?? 0;
        }

        private void UpdateAttributes()
        {
            if (_isUpdatingAttributes || string.IsNullOrEmpty(_selectedBlockOnScreen))
                return;

            _isUpdatingAttributes = true;
            Attributes = _attributesService.GetAttributesForBlock(_selectedBlockOnScreen);
            PrintDataCollection = _searchService.FindObjects(CreateSearchData());
            _isUpdatingAttributes = false;
        }

        private void UpdateBlockCollection()
        {
            if (_isUpdatingBlockCollection) return;

            _isUpdatingBlockCollection = true;

            try
            {
                if (SelectedPrintByOption == PrintByOption.ByPolyline)
                {
                    PrintDataCollection = new List<PrintInfo>(NamingPolylines(PrintDataCollection));
                }
                if (SelectedPrintByOption == PrintByOption.ByBlock)
                {
                    //if (SelectedAttr != null && PrintDataCollection != null)
                    //{
                        PrintDataCollection = new List<PrintInfo>(
                            _attributesService.GetPrintInfosForBlock(PrintDataCollection, SelectedAttr?.AttributeName,
                                NumerationStartValue, Prefix, Suffix, IsCheckedNumbering));
                    //}
                    //else
                    //{
                    //    PrintDataCollection = new List<PrintInfo>(NamingPolylines(PrintDataCollection));
                    //}

                }

                UpdateSelectedPrintCount();
            }
            finally
            {
                _isUpdatingBlockCollection = false;
            }
        }

        private void SearchPolylinesInLayer()
        {
            PrintDataCollection = _searchService.FindObjects(CreateSearchData());
        }

        private SearchData CreateSearchData()
        {
            return new SearchData
            {
                SelectedPrintByOption = SelectedPrintByOption,
                SelectedPrintingOrder = SelectedPrintingOrder,
                IsSearchOnModel = IsSearchOnModel,
                IsSearchOnLayouts = IsSearchOnLayout,
                SelectedBlockName = SelectedBlockOnScreen,
                AttributeName = SelectedAttr?.AttributeName,
                NumerationStartValue = NumerationStartValue,
                Prefix = Prefix,
                Suffix = Suffix,
                IsCheckedNumbering = IsCheckedNumbering,
                SelectedLayer = SelectedLayerOnScreen, 
                PlineScale = PlineScale
            };
        }

        private void SetPrintDataCollection(List<PrintInfo> value)
        {
            if (_printDataCollection == value) return;

            if (_printDataCollection != null)
            {
                foreach (var item in _printDataCollection)
                {
                    item.PropertyChanged -= OnBlockDataPropertyChanged;
                }
            }

            _printDataCollection = value;

            if (_printDataCollection != null)
            {
                foreach (var item in _printDataCollection)
                {
                    item.PropertyChanged += OnBlockDataPropertyChanged;
                }
            }

            OnPropertyChanged(nameof(PrintDataCollection));
            OnPropertyChanged(nameof(HeaderContent));
        }

        private void SetSelectedPrintByOption(PrintByOption value)
        {
            if (!Set(ref _selectedPrintByOption, value)) return;

            if (_selectedPrintByOption == PrintByOption.ByPolyline)
            {
                PrintDataCollection?.Clear();
                UpdateSelectedPrintCount();
            }
            else if (_selectedPrintByOption == PrintByOption.ByBlock)
            {
                PrintDataCollection?.Clear();
                if (SelectedBlockOnScreen == null) return;
                UpdateAttributes();
                UpdateBlockCollection();
                UpdateSelectedPrintCount();
            }
        }

        private void SetNumerationStartValue(int value)
        {
            if (_numerationStartValue == value) return;

            _numerationStartValue = IsCheckedNumbering && value == 0 ? 0 : value;
            OnPropertyChanged(nameof(NumerationStartValue));
            UpdateBlockCollection();
        }

        private void SetIsAllPrintSelected(bool value)
        {
            _isAllPrintSelected = value;
            OnPropertyChanged(nameof(IsAllPrintSelected));

            if (PrintDataCollection != null)
            {
                foreach (var printInfo in PrintDataCollection)
                {
                    printInfo.IsPrint = value;
                }
            }

            UpdateSelectedPrintCount();
        }

        private List<PrintInfo> NamingPolylines(List<PrintInfo> dataCollection)
        {
            var i = NumerationStartValue;
            var newdata = new List<PrintInfo>();
            foreach (var printInfo in dataCollection)
            {
                printInfo.FileName = Prefix + i + Suffix;
                i++;
                newdata.Add(printInfo);
            }

            return newdata;
        }
    }
}