﻿using MediatR;
using OperationResult;
using Ingenico.Barcode.Shared.Responses;

namespace Ingenico.Barcode.Shared.Requests
{
    public class ObterProdutoRequest : IRequest<Result<ObterProdutoResponse>>
    {
        public Guid ProdutoId { get; set; }
    }
}
