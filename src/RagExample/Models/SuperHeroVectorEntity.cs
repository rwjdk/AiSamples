using Microsoft.Extensions.VectorData;

namespace RagExample.Models;
#pragma warning disable SKEXP0001

public class SuperHeroVectorEntity
{
    [VectorStoreKey]
    public string Id { get; set; }

    [VectorStoreData]
    public string Name { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public string Sex { get; set; }

    [VectorStoreData]
    public string Description { get; set; }

    [VectorStoreVector(1536, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
    public string? DescriptionEmbedding => Description;
}