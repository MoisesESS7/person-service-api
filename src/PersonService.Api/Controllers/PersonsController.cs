using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonService.Api.Builders;
using PersonService.Api.Contracts.Requests.Persons;
using PersonService.Api.Models;
using PersonService.Application.Features.Persons.Commands.Create;
using PersonService.Application.Features.Persons.Commands.Delete;
using PersonService.Application.Features.Persons.Commands.Update;
using PersonService.Application.Features.Persons.Queries.GetById;
using PersonService.Application.Features.Persons.Queries.PagedSearch;
using PersonService.Shared.Results;
using ApiPersonResponse = PersonService.Api.Contracts.Responses.Persons.PersonResponse;

namespace PersonService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonsController : ApiControllerBase
    {
        private readonly ILogger<PersonsController> _logger;
        private readonly ILinkBuilder _linkBuilder;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public PersonsController(
            ILogger<PersonsController> logger,
            IMediator mediator,
            IMapper mapper,
            ILinkBuilder linkBuilder)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
            _linkBuilder = linkBuilder;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync([FromRoute] string id)
        {
            _logger.LogInformation("Retrieving person with ID {Id}.", id);
            var query = _mapper.Map<GetPersonByIdQuery>(id);

            var result = await _mediator.Send(query);

            if (result.IsFailure)
                return ToActionResult(result);

            var response = _mapper.Map<ApiPersonResponse>(result.Value);

            _logger.LogInformation("Person with ID {Id} retrieved successfully.", id);
            return ToActionResult(ResultOfT<ApiPersonResponse>.Ok(response));
        }

        [HttpGet(Name = "Paged")]
        public async Task<ActionResult> GetPagedAsync(SearchParamsQuery searchParams)
        {
            _logger.LogInformation("Retrieving all persons from databse.");
            var query = _mapper.Map<PagedSearchQuery>(searchParams);

            var result = await _mediator.Send(query);

            if (result.IsFailure)
                return ToActionResult(result);

            var response = _mapper.Map<PagedResponse<ApiPersonResponse>>(result.Value);

            var links = _linkBuilder.Build(
                "Paged",
                searchParams.PageNumber,
                searchParams.PageSize,
                result.Value?.Meta.PageMeta?.TotalPages ?? 0
            );

            response.SetLinks(links);

            _logger.LogInformation(
                "Retrieved {Count} persons (page {Page}/{TotalPages})",
                response.Data.Count,
                searchParams.PageNumber,
                result.Value?.Meta.PageMeta?.TotalPages ?? 0
            );

            return ToActionResult(ResultOfT<PagedResponse<ApiPersonResponse>>.Ok(response));
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreatePersonRequest request)
        {
            _logger.LogInformation("Creating new person with name: {Name}", request.Name);
            var command = _mapper.Map<CreatePersonCommand>(request);

            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return ToActionResult(result);

            var response = _mapper.Map<ApiPersonResponse>(result.Value);

            _logger.LogInformation("Person created successfully with ID {ID}", response.Id);

            return CreatedAtAction("Get", new
            {
                response.Id
            },
            response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync([FromRoute] string id, [FromBody] UpdatePersonRequest request)
        {
            _logger.LogInformation("Updating person with ID {Id}", id);
            request.SetId(id);

            var command = _mapper.Map<UpdatePersonCommand>(request);

            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return ToActionResult(result);

            var response = _mapper.Map<ApiPersonResponse>(result.Value);

            _logger.LogInformation("Person with ID {Id} updated successfully.", id);
            return ToActionResult(ResultOfT<ApiPersonResponse>.Ok(response));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync([FromRoute] string id)
        {
            _logger.LogInformation("Deleting person with ID {Id}", id);
            var command = _mapper.Map<DeletePersonCommand>(id);

            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return ToActionResult(result);

            _logger.LogInformation("Person with ID {Id} deleted successfully.", id);
            return ToActionResult(result);
        }
    }
}
