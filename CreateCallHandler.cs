using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using JetBrains.Annotations;
using JusticeOne.Business.Calls.Commands;
using JusticeOne.Core.Aspects;
using JusticeOne.Data.Common.Models.CallForService;
using JusticeOne.Data.Repositories;
using JusticeOne.Data.Tenant.Entities.Context;
using JusticeOne.Data.Tenant.Entities.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JusticeOne.Business.Calls.Handlers
{
    public sealed class CreateCallHandler : IRequestHandler<CreateCallCommand, Result<CallDto>>, IRequestHandler<CreateTrafficStopCommand, Result<CallDto>>
    {
        private readonly IMapper _mapper;
        private readonly JusticeOneContext _context;
        private readonly IValidator<NewCallDto> _validator;

        public CreateCallHandler([NotNull]IDbContextResolver resolver,
            [NotNull]IMapper mapper,
            [NotNull]IValidator<NewCallDto> validator)
        {
            _mapper = mapper;
            _context = resolver.DbContext;
            _validator = validator;
        }

        [MethodLog]
        public async Task<Result<CallDto>> Handle([NotNull]CreateCallCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.Data, ruleSet: "CreateCallValidation", cancellationToken);
            var dbModel = _mapper.Map<CallDb>(request.Data);
            return await CreateCall(dbModel, cancellationToken);
        }

        [MethodLog]
        public async Task<Result<CallDto>> Handle([NotNull]CreateTrafficStopCommand request, CancellationToken cancellationToken)
        {
            //TODO: implement validation
            var dbModel = _mapper.Map<CallDb>(request.Data);
            return await CreateCall(dbModel, cancellationToken);
        }

        [MethodLog]
        private async Task<Result<CallDto>> CreateCall(CallDb model, CancellationToken cancellationToken)
        {
            await using (_context)
            {
                _context.Calls.Add(model);
                try
                {
                    await _context.SaveChangesAsync(cancellationToken);
                    return Result.Success(_mapper.Map<CallDto>(model));
                }
                catch (Exception ex) when (ex is DbUpdateException)
                {
                    return Result.Failure<CallDto>($"Cannot create call. {ex.Message}");
                }
            }
        }
    }
}
