namespace Entities.Exceptions
{
    public class FacilityNotFoundException : NotFoundException
    {
        public FacilityNotFoundException(int facilityId)
         : base($"Facility with id: {facilityId} doesn't exist in the database.")
        {
        }
    }
}