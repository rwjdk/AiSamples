using Microsoft.Extensions.VectorData;

namespace RagExample.Models;
#pragma warning disable SKEXP0001

public class SuperHeroVectorEntity
{
    [VectorStoreKey]
    public required string Id { get; set; }

    [VectorStoreData]
    public required string Name { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public required string Sex { get; set; }

    [VectorStoreData]
    public required string Description { get; set; }

    [VectorStoreVector(1536, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
    public string? DescriptionEmbedding => Description;
}