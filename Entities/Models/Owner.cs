namespace Entities.Models;

public class Owner : ApplicationUser
{
    /* ---- SADECE TESİS SAHİBİ ---- */
    public string? BankAccountInfo { get; set; }

    /*  NAVIGATION  */
    public ICollection<Facility> Facilities { get; set; } = [];
}
