using Microsoft.Extensions.VectorData;

namespace SK_MEAI_Comparison;

public class VectorStoreEntity
{
    [VectorStoreKey]
    public required string Id { get; set; }

    [VectorStoreData]
    public required string Description { get; set; }

    [VectorStoreVector(1536, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
    public string? DescriptionEmbedding => Description;
}