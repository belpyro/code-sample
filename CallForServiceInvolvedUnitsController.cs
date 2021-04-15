using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using JusticeOne.Api.ActionFilters;
using JusticeOne.Api.Extensions;
using JusticeOne.Api.Models;
using JusticeOne.Business;
using JusticeOne.Business.CallInvolvedUnit;
using JusticeOne.Business.CallInvolvedUnit.Models;
using JusticeOne.Data.Common.Models;
using JusticeOne.Data.Common.Models.CallForService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JusticeOne.Api.Controllers.CallForService
{
    [ApiController]
    [ApiVersion("1.1")]
    [Authorize]
    [LogActionFilter]
    [Route("api/v{version:apiVersion}/calls")]
    public class CallForServiceInvolvedUnitsController : ControllerBase
    {
        /// <summary>
        /// Add new involved unit to call
        /// </summary>
        /// <remarks>Add new involved unit to call for service</remarks>
        /// <param name="involvedUnitManager"></param>
        /// <param name="mapper"></param>
        /// <param name="callId">The id of call for service</param>
        /// <param name="model">Involved unit that needs to be added for the call</param>
        /// <response code="200">Success</response>
        /// <response code="400">Request contains invalid data</response>
        /// <response code="401">Request is not authorized</response>
        /// <response code="403">User does not have the necessary permissions</response>
        /// <response code="404">Requested non-existing resource</response>
        [HttpPost("{callId}/involved-units", Name = "AddCallInvolvedUnit")]
        [Authorize(Policy = ReferenceIds.Permissions.Names.CreateCallInvolvedUnit)]
        [ProducesResponseType((int) HttpStatusCode.OK, Type = typeof(CallInvolvedUnit))]
        public virtual async Task<IActionResult> AddCallInvolvedUnit(
            [FromServices] ICallInvolvedUnitManager involvedUnitManager,
            [FromServices] IMapper mapper,
            [FromRoute] [Required] Guid callId,
            [FromBody] CreateCallInvolvedUnit model)
        {
            Result<CallInvolvedUnitDto> result =
                await involvedUnitManager.AddUnitToCallAsync(callId, mapper.Map<CallInvolvedUnitDto>(model));
            return result.Bind(unit => Result.Success(mapper.Map<CallInvolvedUnit>(unit))).ToHttpResult();
        }

        /// <summary>
        /// Returns all units that involved to call
        /// </summary>
        /// <remarks>Returns all units that involved to the call specified by callId</remarks>
        /// <param name="callInvolvedUnitManager"></param>
        /// <param name="callId">The id of call for service</param>
        /// <response code="200">Success</response>
        /// <response code="401">Request is not authorized</response>
        /// <response code="403">User does not have the necessary permissions</response>
        /// <response code="404">Requested non-existing resource</response>
        [HttpGet("{callId}/involved-units", Name = "GetCallInvolvedUnits")]
        [Authorize(Policy = ReferenceIds.Permissions.Names.ReadCallInvolvedUnit)]
        [ProducesResponseType((int) HttpStatusCode.OK, Type = typeof(Page<CallInvolvedUnit>))]
        public virtual async Task<ActionResult<Page<CallInvolvedUnit>>> GetCallInvolvedUnits(
            [FromServices] ICallInvolvedUnitManager callInvolvedUnitManager, [FromServices] IMapper mapper,
            [FromRoute] [Required] Guid callId,
            [FromQuery] int? page, [FromQuery] int? count)
        {
            Result<PageDto<CallInvolvedUnitDto>> callInvolvedUnits =
                await callInvolvedUnitManager.GetCallInvolvedUnitsAsync(callId, page, count);
            return callInvolvedUnits
                .Bind(selectResult =>
                    Result.Success(mapper.Map<PageDto<CallInvolvedUnitDto>, Page<CallInvolvedUnit>>(selectResult)))
                .ToHttpResult();
        }

        /// <summary>
        /// Update data for involved unit
        /// </summary>
        /// <remarks>Update information for involved unit</remarks>
        /// <param name="callId">The id of call for service</param>
        /// <param name="model">Involved unit object that needs to be updated for the call</param>
        /// <response code="200">Success</response>
        /// <response code="400">Request contains invalid data</response>
        /// <response code="401">Request is not authorized</response>
        /// <response code="403">User does not have the necessary permissions</response>
        /// <response code="404">Requested non-existing resource</response>
        [HttpPut("{callId}/involved-units", Name = "UpdateCallInvolvedUnit")]
        [Authorize(Policy = ReferenceIds.Permissions.Names.UpdateCallInvolvedUnit)]
        [ProducesResponseType((int) HttpStatusCode.OK, Type = typeof(CallInvolvedUnit))]
        public virtual async Task<ActionResult<CallInvolvedUnit>> UpdateCallInvolvedUnit(
            [FromServices] ICallInvolvedUnitManager callInvolvedUnitManager, [FromServices] IMapper mapper,
            [FromRoute] [Required] Guid callId,
            [FromBody] UpdateCallInvolvedUnit model)
        {
            Result<CallInvolvedUnitDto> updateResult =
                await callInvolvedUnitManager.UpdateInvolvedUnit(callId, mapper.Map<CallInvolvedUnitDto>(model));
            return updateResult.Bind(unit => Result.Success(mapper.Map<CallInvolvedUnit>(unit))).ToHttpResult();
        }
    }
}