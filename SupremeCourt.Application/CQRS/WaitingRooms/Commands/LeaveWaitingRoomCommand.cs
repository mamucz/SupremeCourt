using MediatR;
using SupremeCourt.Application.CQRS.WaitingRooms.Commands;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Mappings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Commands
{
    public record LeaveWaitingRoomCommand(Guid WaitingRoomId, Guid PlayerId) : IRequest<bool>;
}