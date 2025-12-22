/// <summary>
/// Comment type used for categorizing profit sharing transactions.
/// </summary>
export interface CommentTypeDto {
  id: number;
  name: string;
  /** Indicates whether this comment type is protected from name changes (used in business logic) */
  isProtected: boolean;
  modifiedAtUtc?: string | null;
  userName?: string | null;
}

/// <summary>
/// Request to create a new comment type.
/// </summary>
export interface CreateCommentTypeRequest {
  name: string;
  /** Indicates whether this comment type is protected from changes. Defaults to false. */
  isProtected: boolean;
}

/// <summary>
/// Request to update a comment type name and protection status.
/// </summary>
export interface UpdateCommentTypeRequest {
  id: number;
  name: string;
  /** Can be set from false to true, but cannot be set from true to false (one-way protection) */
  isProtected: boolean;
}

