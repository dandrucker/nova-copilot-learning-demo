using WorkOrderTracker.Api.Models;

namespace WorkOrderTracker.Api.Data;

public class InMemoryWorkOrderRepository : IWorkOrderRepository
{
    private readonly List<WorkOrder> _workOrders;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private int _nextId;

    public InMemoryWorkOrderRepository()
    {
        var now = DateTimeOffset.UtcNow;
        _workOrders =
        [
            new(1, "Replace corroded valve at compressor station", "Segment 12", WorkOrderPriority.Critical, WorkOrderStatus.Open, now.AddDays(-10), "Alex Morgan"),
            new(2, "Inspect cathodic protection rectifier", "Segment 4", WorkOrderPriority.High, WorkOrderStatus.Open, now.AddDays(-9), "Jordan Lee"),
            new(3, "Clear right-of-way vegetation", "Segment 7", WorkOrderPriority.Low, WorkOrderStatus.Open, now.AddDays(-20), "Casey Taylor"),
            new(4, "Recalibrate flow meter", "Segment 2", WorkOrderPriority.Medium, WorkOrderStatus.Open, now.AddDays(-3), "Riley Chen"),
            new(5, "Repair leak detection sensor", "Segment 9", WorkOrderPriority.High, WorkOrderStatus.Open, now.AddDays(-2), "Sam Patel")
        ];
        _nextId = _workOrders.Count + 1;
    }

    public async Task<IReadOnlyList<WorkOrder>> GetAllAsync(CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            return _workOrders.ToList();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<IReadOnlyList<WorkOrder>> GetOverdueAsync(CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var overdueBefore = DateTimeOffset.UtcNow.AddDays(-7);
            return _workOrders
                .Where(workOrder =>
                    workOrder.Status == WorkOrderStatus.Open
                    && (workOrder.Priority is WorkOrderPriority.High or WorkOrderPriority.Critical)
                    && workOrder.OpenedAt < overdueBefore)
                .ToList();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<WorkOrder?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            return _workOrders.FirstOrDefault(w => w.Id == id);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<WorkOrder> AddAsync(CreateWorkOrderRequest request, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var workOrder = new WorkOrder(
                _nextId++,
                request.Title,
                request.PipelineSegment,
                request.Priority,
                WorkOrderStatus.Open,
                DateTimeOffset.UtcNow,
                request.Assignee);
            _workOrders.Add(workOrder);
            return workOrder;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<WorkOrder?> CloseAsync(int id, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var index = _workOrders.FindIndex(workOrder => workOrder.Id == id);
            if (index < 0)
            {
                return null;
            }

            var workOrder = _workOrders[index] with
            {
                Status = WorkOrderStatus.Closed,
                ClosedAt = _workOrders[index].ClosedAt ?? DateTimeOffset.UtcNow
            };
            _workOrders[index] = workOrder;
            return workOrder;
        }
        finally
        {
            _lock.Release();
        }
    }
}
