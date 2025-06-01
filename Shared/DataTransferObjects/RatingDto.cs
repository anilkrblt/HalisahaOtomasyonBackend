using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{

    // Shared/DataTransferObjects/FacilityRatingDtos.cs

    public record FacilityRatingDto(
        int FacilityId,
        int UserId,
        int Stars,
        string? Comment,
        DateTime CreatedAt);

    public record FacilityRatingForCreationDto(
        int Stars,
        string? Comment);

    public record FacilityRatingForUpdateDto(
        int Stars,
        string? Comment);


}