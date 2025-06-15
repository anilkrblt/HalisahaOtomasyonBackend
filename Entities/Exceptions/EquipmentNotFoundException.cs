namespace Entities.Exceptions
{
    public class EquipmentNotFoundException : NotFoundException
    {
        public EquipmentNotFoundException(int equipmentId) : base($"Equipment with id {equipmentId} not found.")
        {
        }
    }
}
