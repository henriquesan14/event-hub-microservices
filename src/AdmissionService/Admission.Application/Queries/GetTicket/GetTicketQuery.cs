using Admission.Application.Dtos;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Admission.Application.Queries.GetTicket;

public sealed record GetTicketQuery(Guid Id) : IQuery<ResultT<AdmissionTicketDto>>;
