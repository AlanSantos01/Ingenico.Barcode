﻿using Ingenico.Barcode.Shared.Enums;

namespace Ingenico.Barcode.Shared.Exceptions;

public class RequestInvalidaExcecao : ExceptionAplication
{
    public RequestInvalidaExcecao(IDictionary<string, string[]> erros)
        : base(GenericErrors.DadosInvalidos) =>
        Erros = erros.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}");

    public IEnumerable<string> Erros { get; }
}
