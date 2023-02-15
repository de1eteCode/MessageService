using Application.Common.Interfaces;

namespace Application.Services;

internal class DateTimeService : IDateTime {
    public DateTime Now => DateTime.Now;
}