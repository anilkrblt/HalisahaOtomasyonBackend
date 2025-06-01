using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public record PhotoDto
    {
        public int Id { get; init; }
        public string Url { get; init; }
        public string? Description { get; init; }
    }

    public record PhotoForCreationDto
    {
        public string Url { get; init; }
        public string? Description { get; init; }
    }

}