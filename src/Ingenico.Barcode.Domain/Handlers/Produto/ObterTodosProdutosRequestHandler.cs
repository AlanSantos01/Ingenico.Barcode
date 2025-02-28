﻿using Ingenico.Barcode.Domain.Repository;
using Ingenico.Barcode.Shared.Requests;
using Ingenico.Barcode.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace Ingenico.Barcode.Domain.Handlers {
    public class ObterTodosProdutosRequestHandler : IRequestHandler<ObterTodosProdutosRequest, Result<ObterTodosProdutosResponse>> {
        private readonly IProdutoRepository _produtoRepository;

        private readonly ILogger<ObterTodosProdutosRequestHandler> _logger;

        public ObterTodosProdutosRequestHandler(
            IProdutoRepository produtoRepository,

            ILogger<ObterTodosProdutosRequestHandler> logger) {
            _produtoRepository = produtoRepository;
            _logger = logger;
        }

        public async Task<Result<ObterTodosProdutosResponse>> Handle(ObterTodosProdutosRequest request, CancellationToken cancellationToken) {
            var produtos = await _produtoRepository.ObterTodosProdutosAsync();
            if (produtos == null) {
                _logger.LogError("Nenhum produto encontrado");
                return Result.Error<ObterTodosProdutosResponse>(new Exception("Nenhum produto encontrado"));
            }

            var produtoResponses = new List<ObterProdutosResponse>();

            foreach (var produto in produtos) { 

                produtoResponses.Add(new ObterProdutosResponse {
                    ProdutoId = produto.ProdutoId,
                    Nome = produto.Nome,
                    Descricao = produto.Descricao,
                    Marca = produto.Marca,
                    Validade = produto.Validade,
                    Peso = produto.Peso,
                    Preco = produto.Preco,
                    UnidadeMedida = produto.UnidadeMedida,
                    Ingredientes = produto.Ingredientes,
                    PaisOrigem = produto.PaisOrigem,
                    DataFabricacao = produto.DataFabricacao,
                    Lote = produto.Lote
                });
            }
            var response = new ObterTodosProdutosResponse {
                Produtos = produtoResponses
            };

            _logger.LogInformation("Retornando lista de produtos");

            return Result.Success(response);
        }
    }
}
