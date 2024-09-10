using System;
using System.Collections.Generic;
using System.Text;
using YMplugins.ViewModels.Commands;

namespace YMplugins.ViewModels.VM
{
    public class AutoPrintVm :BaseViewModel
    {
        private List<string> _blocksName;
        private List<string> _layers;
        private List<string> _attributes;

        private bool _canExecute = true;
        private int _selectedBlockId;


        public AutoPrintVm(
            GetBlocksNameCommand getBlocksNameCommand, 
            GetLayersCommand getLayersCommand, 
            GetAttributesCommand getAttributesCommand, 
            PrintCommand printCommand,
            SelectBlockCommand selectBlockCommand)
        {
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
            BlocksName = obj;
        }

        public List<string> BlocksName
        {
            get => _blocksName;
            set => Set(ref _blocksName, value);
        }

        public List<string> Layers
        {
            get => _layers;
            set => Set(ref _layers, value);
        }

        public List<string> Attributes
        {
            get => _attributes;
            set => Set(ref _attributes, value);
        }
        public int SelectedBlockId
        {
            get => _selectedBlockId;
            set => Set(ref _selectedBlockId, value);
        }

        public Action CloseAction { get; set; }

        public Action OpenAction { get; set; }

        public GetBlocksNameCommand GetBlocksNameCommand {get;}

        public SelectBlockCommand SelectBlockCommand { get; }

        public GetLayersCommand GetLayersCommand { get; }

        public GetAttributesCommand GetAttributesCommand { get; }

        public PrintCommand PrintCommand { get; }
    }
}
