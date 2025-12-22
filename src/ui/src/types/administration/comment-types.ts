/// <summary>
/// Comment type used for categorizing profit sharing transactions.
/// </summary>
export interface CommentTypeDto {
  id: number;
  name: string;
  modifiedAtUtc?: string | null;
  userName?: string | null;
}

/// <summary>
/// Request to update a comment type name.
/// </summary>
export interface UpdateCommentTypeRequest {
  id: number;
  name: string;
}
