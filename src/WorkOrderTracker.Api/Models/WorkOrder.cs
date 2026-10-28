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

public class WorkOrder
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PipelineSegment { get; set; } = string.Empty;
    public WorkOrderPriority Priority { get; set; }
    public WorkOrderStatus Status { get; set; }
    public DateTime OpenedAt { get; set; }
}

public class CreateWorkOrderRequest
{
    public string Title { get; set; } = string.Empty;
    public string PipelineSegment { get; set; } = string.Empty;
    public WorkOrderPriority Priority { get; set; }
}
