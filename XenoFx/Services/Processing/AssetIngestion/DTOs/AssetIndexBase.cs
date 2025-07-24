using System;
using System.Text.Json.Nodes;

namespace XenoFx.Services.Processing.AssetIngestion.DTOs;

public class AssetIndexBase
{
    // Add general purpose data here like

    public Guid AssetId { get; set; } = Guid.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.MinValue;

    // User claims

    // Analysis

    public string FileExtension { get; set; } = string.Empty;
    public string FileMimeType { get; set; } = string.Empty;

    // Metadata

    public long FileSize { get; set; } = 0;
    public DateTimeOffset FileModifiedAt { get; set; } = DateTimeOffset.MinValue;
    public DateTimeOffset FileCreatedAt { get; set; } = DateTimeOffset.MinValue;

    // Integrity

    public byte[] SHA256 { get; set; } = [];
    public byte[] Blake3 { get; set; } = [];
    public byte[] Crumbs { get; set; } = [];

    // Descriptors

    public string Title { get; set; } = string.Empty;
    public JsonObject ExtraData { get; set; } = [];
}