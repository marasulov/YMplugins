using System;

namespace YMplugins.ViewModels.Commands;

public interface ISelectBlockService
{
    Tuple<long, string> SelectBlock();
}