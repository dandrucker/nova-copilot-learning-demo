namespace WorkOrderTracker.Api.Models;

public enum WorkOrderPriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum WorkOrderStatus
{
    Open,
    InProgress,
    Closed
}

public sealed record WorkOrder(
    int Id,
    string Title,
    string PipelineSegment,
    WorkOrderPriority Priority,
    WorkOrderStatus Status,
    DateTimeOffset OpenedAt,
    string? Assignee,
    DateTimeOffset? ClosedAt = null);

public sealed record CreateWorkOrderRequest(
    string Title,
    string PipelineSegment,
    WorkOrderPriority Priority,
    string? Assignee = null);
