using System.ComponentModel;

namespace RagExample.Models;

public class SuperHero
{
    [Description("Just an incrementatal number (1,2,3,...)")]
    public required string Id { get; set; }

    [Description("Male or Female")]
    public required string Sex { get; set; }

    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Weakness { get; set; }
    public required string Strength { get; set; }
    public required string BackgroundStory { get; set; }
}