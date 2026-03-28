export interface RelatedItem {
    id: string;          // unique identifier for the asset
    mediaType: string;   // mime type
    title: string;       // title of the asset
    subText: string;     // subtitle or additional text
    thumbText: string;   // text for the thumbnail
    source: string;      // original path of the asset
}

export interface RelatedItemsBucket {
    items: RelatedItem[];
}