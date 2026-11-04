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
    DateTime OpenedAt,
    string? Assignee);

public sealed record CreateWorkOrderRequest(
    string Title,
    string PipelineSegment,
    WorkOrderPriority Priority,
    string? Assignee = null);
