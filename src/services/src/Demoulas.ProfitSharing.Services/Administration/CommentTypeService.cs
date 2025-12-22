using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Request.Audit;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.ProfitSharing.Data.Entities.Comments;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Administration;

public sealed class CommentTypeService : ICommentTypeService
{
    private static readonly Error s_commentTypeNotFound = Error.EntityNotFound("Comment type");

    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly IAuditService _auditService;
    private readonly ICommitGuardOverride _commitGuardOverride;
    private readonly IAppUser _appUser;
    private readonly ILogger<CommentTypeService> _logger;

    public CommentTypeService(
        IProfitSharingDataContextFactory contextFactory,
        IAuditService auditService,
        ICommitGuardOverride commitGuardOverride,
        IAppUser appUser,
        ILogger<CommentTypeService> logger)
    {
        _contextFactory = contextFactory;
        _auditService = auditService;
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
                    DateModified = x.DateModified,
                    UserModified = x.UserModified,
                })
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<CommentTypeDto>>.Success(results);
        }, cancellationToken);
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
                    return Result<CommentTypeDto>.Failure(s_commentTypeNotFound);
                }

                var originalName = commentType.Name;
                if (originalName == trimmedName)
                {
                    return Result<CommentTypeDto>.Success(new CommentTypeDto
                    {
                        Id = commentType.Id,
                        Name = commentType.Name,
                        DateModified = commentType.DateModified,
                        UserModified = commentType.UserModified,
                    });
                }

                commentType.Name = trimmedName;
                commentType.UserModified = _appUser.UserName ?? "";
                commentType.DateModified = DateOnly.FromDateTime(DateTime.UtcNow);

                await ctx.SaveChangesAsync(cancellationToken);

                await _auditService.LogDataChangeAsync(
                    operationName: "Update Comment Type",
                    tableName: "COMMENT_TYPE",
                    auditOperation: AuditEvent.AuditOperations.Update,
                    primaryKey: $"Id:{request.Id}",
                    changes:
                    [
                        new AuditChangeEntryInput
                        {
                            ColumnName = "NAME",
                            OriginalValue = originalName,
                            NewValue = trimmedName,
                        },
                    ],
                    cancellationToken);

                _logger.LogInformation(
                    "Updated comment type {Id}: '{OldName}' → '{NewName}'",
                    commentType.Id, originalName, trimmedName);

                return Result<CommentTypeDto>.Success(new CommentTypeDto
                {
                    Id = commentType.Id,
                    Name = commentType.Name,
                    DateModified = commentType.DateModified,
                    UserModified = commentType.UserModified,
                });
            }, cancellationToken);
        }
    }
}
