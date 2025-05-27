using System.ComponentModel;

namespace StructuredOutputExample.Models;

public class MovieResult
{
    public required string MessageBack { get; set; }

    public required decimal AverageScoreOfThe10Movies { get; set; } //Example that we still deal with "AIs that can't count" but also that such things are not really needed as it is better to use 'real code' to calculate such

    public decimal RealAverageScore => Top10Movies.Select(x => x.ImdbScore).Average();

    [Description("Order them by IMDB Score")] //Sometimes the Structured output properties need a bit of description to get desired effect
    public required Movie[] Top10Movies { get; set; }
}