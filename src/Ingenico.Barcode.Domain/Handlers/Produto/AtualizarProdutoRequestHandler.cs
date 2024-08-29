﻿using Ingenico.Barcode.Domain.Repository;
using Ingenico.Barcode.Shared.Requests;
using Ingenico.Barcode.Shared.Responses;
using Ingenico.Barcode.Domain.Entites;
using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ingenico.Barcode.Shared.Exceptions;
using Ingenico.Barcode.Shared.Enums;

namespace Ingenico.Barcode.Domain.Handlers
{


        public class AtualizarProdutoRequestHandler : IRequestHandler<AtualizarProdutoRequest, Result<AtualizarProdutoResponse>>
        {
            private readonly IProdutoRepository _produtoRepository;
            private readonly ICategoriaRepository _categoriaRepository;
            private readonly ITagRepository _tagRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ILogger<AtualizarProdutoRequestHandler> _logger;

            public AtualizarProdutoRequestHandler(
                IProdutoRepository produtoRepository,
                ICategoriaRepository categoriaRepository,
                ITagRepository tagRepository,
                IUnitOfWork unitOfWork,
                ILogger<AtualizarProdutoRequestHandler> logger)
            {
                _produtoRepository = produtoRepository;
                _categoriaRepository = categoriaRepository;
                _tagRepository = tagRepository;
                _unitOfWork = unitOfWork;
                _logger = logger;
            }

            public async Task<Result<AtualizarProdutoResponse>> Handle(AtualizarProdutoRequest request, CancellationToken cancellationToken)
            {
                var produto = await _produtoRepository.ObterProdutoAsync(request.ProdutoId);
                if (produto == null)
                {
                    _logger.LogWarning("Produto não encontrado: {ProdutoId}", request.ProdutoId);
                    return Result.Error<AtualizarProdutoResponse>(new ExceptionAplication(AuthError.UsuarioNaoEncontrado));
                }

                // Atualizar propriedades básicas
                produto.Nome = request.Nome;
                produto.Descricao = request.Descricao;
                produto.Marca = request.Marca;
                produto.Validade = request.Validade;
                produto.Preco = request.Preco;
                produto.Peso = request.Peso;
                produto.UnidadeMedida = request.UnidadeMedida;
                produto.Ingredientes = request.Ingredientes;
                produto.PaisOrigem = request.PaisOrigem;

                // Atualizar categorias
                produto.ProdutoCategoria.Clear();
                foreach (var categoriaRequest in request.Categorias)
                {
                    var categoria = await _categoriaRepository.ObterCategoriaPorNomeAsync(categoriaRequest.Nome);
                    if (categoria == null)
                    {
                        categoria = new CategoriaEntity
                        {
                            Nome = categoriaRequest.Nome
                        };
                        await _categoriaRepository.CadastrarCategoriaAsync(categoria);
                    }

                    produto.ProdutoCategoria.Add(new ProdutoCategoria
                    {
                        Categoria = categoria
                    });
                }

                // Atualizar tags
                produto.ProdutoTag.Clear();
                foreach (var tagRequest in request.Tags)
                {
                    var tag = await _tagRepository.ObterTagPorNomeAsync(tagRequest.NomeTag);
                    if (tag == null)
                    {
                        tag = new TagEntity
                        {
                            Nome = tagRequest.NomeTag
                        };
                        await _tagRepository.CadastrarTagAsync(tag);
                    }

                    produto.ProdutoTag.Add(new ProdutoTag
                    {
                        Tag = tag
                    });
                }

                await _produtoRepository.AtualizarProdutoAsync(produto);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Produto atualizado com categorias e tags");

                return Result.Success(new AtualizarProdutoResponse
                {
                    ProdutoId = produto.ProdutoId,
                    Nome = produto.Nome,
                    Descricao = produto.Descricao,
                    Marca = produto.Marca,
                    Peso = produto.Peso,
                    Preco = produto.Preco,
                    UnidadeMedida = produto.UnidadeMedida,
                    Ingredientes = produto.Ingredientes,
                    PaisOrigem = produto.PaisOrigem,
                    Categorias = produto.ProdutoCategoria.Select(pc => new ObterCategoriaResponse
                    {
                        CategoriaId = pc.Categoria.CategoriaId,
                        Nome = pc.Categoria.Nome
                    }).ToList(),
                    Tags = produto.ProdutoTag.Select(pt => new ObterTagResponse
                    {
                        TagId = pt.Tag.TagId,
                        NomeTag = pt.Tag.Nome
                    }).ToList()
                });
            }
        }

    }

