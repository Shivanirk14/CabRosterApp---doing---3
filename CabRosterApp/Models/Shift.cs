using System.ComponentModel.DataAnnotations;

public class Shift
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Shift time is required.")]
    [StringLength(50, ErrorMessage = "Shift time cannot be longer than 50 characters.")]
    public string ShiftTime { get; set; }
}
