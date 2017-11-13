using System;

namespace XenoCore.Engine.Services.Input
{
    public interface ITextInputProvider
    {
        event Action<Char> OnCharacterEntered;
    }
}
