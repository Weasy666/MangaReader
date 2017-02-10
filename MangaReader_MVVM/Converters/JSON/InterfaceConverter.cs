using Newtonsoft.Json.Converters;
using System;

namespace MangaReader_MVVM.Converters.JSON
{
    class InterfaceConverter<TInterface, TConcrete> : CustomCreationConverter<TInterface>
    where TConcrete : TInterface, new()
    {
        public override TInterface Create(Type objectType)
        {
            return new TConcrete();
        }
    }
}
