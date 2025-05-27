using System.ComponentModel;

namespace RagExample.Models;

public class SuperHero
{
    [Description("Just an incrementatal number (1,2,3,...)")]
    public string Id { get; set; }

    [Description("Male or Female")]
    public string Sex { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public string Weakness { get; set; }
    public string Strenght { get; set; }
    public string BackgroundStory { get; set; }
}