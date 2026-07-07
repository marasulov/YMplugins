namespace YMplugins.Contracts
{
    /// <summary>
    ///     Позволяет пользователю указать полилинию на экране и возвращает имя
    ///     слоя, на котором она лежит.
    /// </summary>
    public interface ISelectLayerService
    {
        string SelectLayer();
    }
}
