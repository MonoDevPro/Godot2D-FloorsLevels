using Godot;

namespace Game.Shared.Resources;

// ========== CLASSE CUSTOMIZADA PARA DATA ==========
[GlobalClass]
public partial class GodotData : Resource
{
    [Export] public int Year { get; set; } = 2024;
    [Export] public int Month { get; set; } = 1;
    [Export] public int Day { get; set; } = 1;
    [Export] public int Hour { get; set; } = 0;
    [Export] public int Minute { get; set; } = 0;
    [Export] public int Second { get; set; } = 0;
        
    public DateTime ToDateTime()
    {
        try
        {
            return new DateTime(Year, Month, Day, Hour, Minute, Second);
        }
        catch (ArgumentOutOfRangeException)
        {
            return DateTime.Now;
        }
    }
        
    public void FromDateTime(DateTime dateTime)
    {
        Year = dateTime.Year;
        Month = dateTime.Month;
        Day = dateTime.Day;
        Hour = dateTime.Hour;
        Minute = dateTime.Minute;
        Second = dateTime.Second;
    }
        
    public override string ToString()
    {
        return $"{Year:0000}-{Month:00}-{Day:00} {Hour:00}:{Minute:00}:{Second:00}";
    }
}
