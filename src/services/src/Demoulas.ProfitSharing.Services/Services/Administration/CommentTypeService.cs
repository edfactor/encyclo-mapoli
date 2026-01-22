using Demoulas.Common.Contracts.Contracts.Request.Audit;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.Common.Data.Services.Entities.Entities.Audit;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Services.Administration;

public sealed class CommentTypeService : ICommentTypeService
{
    private static readonly Error _commentTypeNotFound = Error.EntityNotFound("Comment type");

    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly IProfitSharingAuditService _profitSharingAuditService;
    private readonly ICommitGuardOverride _commitGuardOverride;
    private readonly IAppUser _appUser;
    private readonly ILogger<CommentTypeService> _logger;

    public CommentTypeService(
        IProfitSharingDataContextFactory contextFactory,
        IProfitSharingAuditService profitSharingAuditService,
        ICommitGuardOverride commitGuardOverride,
        IAppUser appUser,
        ILogger<CommentTypeService> logger)
    {
        _contextFactory = contextFactory;
        _profitSharingAuditService = profitSharingAuditService;
        _commitGuardOverride = commitGuardOverride;
        _appUser = appUser;
        _logger = logger;
    }

    public Task<Result<IReadOnlyList<CommentTypeDto>>> GetCommentTypesAsync(CancellationToken cancellationToken)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var results = await ctx.CommentTypes
                .TagWith("Administration-GetCommentTypes")
                .OrderBy(x => x.Id)
                .Select(x => new CommentTypeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsProtected = x.IsProtected,
                    ModifiedAtUtc = x.ModifiedAtUtc,
                    UserName = x.UserName,
                })
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<CommentTypeDto>>.Success(results);
        }, cancellationToken);
    }

    public async Task<Result<CommentTypeDto>> CreateCommentTypeAsync(CreateCommentTypeRequest request, CancellationToken cancellationToken)
    {
        using (_commitGuardOverride.AllowFor(roles: Role.ITDEVOPS))
        {
            return await _contextFactory.UseWritableContext(async ctx =>
            {
                var trimmedName = (request.Name ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(trimmedName))
                {
                    return Result<CommentTypeDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Name)] = ["Name is required."],
                    }));
                }

                if (trimmedName.Length > 255)
                {
                    return Result<CommentTypeDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Name)] = ["Name must be 255 characters or less."],
                    }));
                }

                // Check for duplicate name
                var exists = await ctx.CommentTypes
                    .TagWith("Administration-CheckDuplicateCommentType")
                    .AnyAsync(x => x.Name == trimmedName, cancellationToken);

                if (exists)
                {
                    return Result<CommentTypeDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Name)] = ["A comment type with this name already exists."],
                    }));
                }

                // Get next ID by finding max existing ID and incrementing
                var maxId = await ctx.CommentTypes
                    .TagWith("Administration-GetMaxCommentTypeId")
                    .MaxAsync(x => (byte?)x.Id, cancellationToken) ?? 0;
                var nextId = (byte)(maxId + 1);

                var commentType = new Data.Entities.CommentType
                {
                    Id = nextId, // Manually assign next ID since Oracle doesn't have sequence/identity for this table
                    Name = trimmedName,
                    IsProtected = request.IsProtected,
                    UserName = _appUser.UserName ?? "",
                    ModifiedAtUtc = DateTimeOffset.UtcNow
                };

                ctx.CommentTypes.Add(commentType);
                await ctx.SaveChangesAsync(cancellationToken);

                // Audit log creation
                await _profitSharingAuditService.LogDataChangeAsync(
                    operationName: "Create Comment Type",
                    tableName: "COMMENT_TYPE",
                    auditOperation: AuditEvent.AuditOperations.Create,
                    primaryKey: $"Id:{commentType.Id}",
                    changes:
                    [
                        new AuditChangeEntryInputRequest
                        {
                            ColumnName = "NAME",
                            OriginalValue = null,
                            NewValue = trimmedName,
                        },
                        new AuditChangeEntryInputRequest
                        {
                            ColumnName = "IS_PROTECTED",
                            OriginalValue = null,
                            NewValue = request.IsProtected.ToString(),
                        }
                    ],
                    cancellationToken);

                _logger.LogInformation(
                    "Created comment type {Id}: '{Name}', IsProtected: {IsProtected}",
                    commentType.Id, trimmedName, request.IsProtected);

                return Result<CommentTypeDto>.Success(new CommentTypeDto
                {
                    Id = commentType.Id,
                    Name = commentType.Name,
                    IsProtected = commentType.IsProtected,
                    ModifiedAtUtc = commentType.ModifiedAtUtc,
                    UserName = commentType.UserName,
                });
            }, cancellationToken);
        }
    }

    public async Task<Result<CommentTypeDto>> UpdateCommentTypeAsync(UpdateCommentTypeRequest request, CancellationToken cancellationToken)
    {
        using (_commitGuardOverride.AllowFor(roles: Role.ITDEVOPS))
        {
            return await _contextFactory.UseWritableContext(async ctx =>
            {
                var trimmedName = (request.Name ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(trimmedName))
                {
                    return Result<CommentTypeDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Name)] = ["Name is required."],
                    }));
                }

                if (trimmedName.Length > 255)
                {
                    return Result<CommentTypeDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Name)] = ["Name must be 255 characters or less."],
                    }));
                }

                var commentType = await ctx.CommentTypes
                    .TagWith($"Administration-UpdateCommentType-{request.Id}")
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (commentType is null)
                {
                    return Result<CommentTypeDto>.Failure(_commentTypeNotFound);
                }

                // Validate one-way protection: Cannot remove protected flag once set
                if (commentType.IsProtected && !request.IsProtected)
                {
                    return Result<CommentTypeDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.IsProtected)] = ["Cannot remove protected flag from CommentType. Protected comment types are used in business logic and require direct database update to unprotect."],
                    }));
                }

                var originalName = commentType.Name;
                var originalIsProtected = commentType.IsProtected;
                if (originalName == trimmedName && originalIsProtected == request.IsProtected)
                {
                    return Result<CommentTypeDto>.Success(new CommentTypeDto
                    {
                        Id = commentType.Id,
                        Name = commentType.Name,
                        IsProtected = commentType.IsProtected,
                        ModifiedAtUtc = commentType.ModifiedAtUtc,
                        UserName = commentType.UserName,
                    });
                }

                commentType.Name = trimmedName;
                commentType.IsProtected = request.IsProtected;
                commentType.UserName = _appUser.UserName ?? "";
                commentType.ModifiedAtUtc = DateTimeOffset.UtcNow;
                await ctx.SaveChangesAsync(cancellationToken);

                // Build audit changes list
                var changes = new List<AuditChangeEntryInputRequest>();
                if (originalName != trimmedName)
                {
                    changes.Add(new AuditChangeEntryInputRequest
                    {
                        ColumnName = "NAME",
                        OriginalValue = originalName,
                        NewValue = trimmedName,
                    });
                }
                if (originalIsProtected != request.IsProtected)
                {
                    changes.Add(new AuditChangeEntryInputRequest
                    {
                        ColumnName = "IS_PROTECTED",
                        OriginalValue = originalIsProtected.ToString(),
                        NewValue = request.IsProtected.ToString(),
                    });
                }

                if (changes.Count > 0)
                {
                    await _profitSharingAuditService.LogDataChangeAsync(
                        operationName: "Update Comment Type",
                        tableName: "COMMENT_TYPE",
                        auditOperation: AuditEvent.AuditOperations.Update,
                        primaryKey: $"Id:{request.Id}",
                        changes: changes,
                        cancellationToken);
                }

                _logger.LogInformation(
                    "Updated comment type {Id}: '{OldName}' → '{NewName}', IsProtected: {OldProtected} → {NewProtected}",
                    commentType.Id, originalName, trimmedName, originalIsProtected, request.IsProtected);

                return Result<CommentTypeDto>.Success(new CommentTypeDto
                {
                    Id = commentType.Id,
                    Name = commentType.Name,
                    IsProtected = commentType.IsProtected,
                    ModifiedAtUtc = commentType.ModifiedAtUtc,
                    UserName = commentType.UserName,
                });
            }, cancellationToken);
        }
    }
}
