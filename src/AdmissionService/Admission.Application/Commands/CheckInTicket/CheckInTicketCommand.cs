using Admission.Application.Dtos;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Admission.Application.Commands.CheckInTicket;

public sealed record CheckInTicketCommand(string Code)
    : ICommand<ResultT<AdmissionTicketDto>>;
