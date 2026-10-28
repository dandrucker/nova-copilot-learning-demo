using WorkOrderTracker.Api.Models;

namespace WorkOrderTracker.Api.Data;

public class InMemoryWorkOrderRepository : IWorkOrderRepository
{
    private readonly List<WorkOrder> _workOrders = new();
    private readonly object _lock = new();
    private int _nextId = 1;

    public InMemoryWorkOrderRepository()
    {
        Seed("Replace corroded valve at compressor station", "Segment 12", WorkOrderPriority.Critical, 10);
        Seed("Inspect cathodic protection rectifier", "Segment 4", WorkOrderPriority.High, 9);
        Seed("Clear right-of-way vegetation", "Segment 7", WorkOrderPriority.Low, 20);
        Seed("Recalibrate flow meter", "Segment 2", WorkOrderPriority.Medium, 3);
        Seed("Repair leak detection sensor", "Segment 9", WorkOrderPriority.High, 2);
    }

    public IEnumerable<WorkOrder> GetAll()
    {
        lock (_lock)
        {
            return _workOrders.ToList();
        }
    }

    public WorkOrder? GetById(int id)
    {
        lock (_lock)
        {
            return _workOrders.FirstOrDefault(w => w.Id == id);
        }
    }

    public WorkOrder Add(CreateWorkOrderRequest request)
    {
        lock (_lock)
        {
            var workOrder = new WorkOrder
            {
                Id = _nextId++,
                Title = request.Title,
                PipelineSegment = request.PipelineSegment,
                Priority = request.Priority,
                Status = WorkOrderStatus.Open,
                OpenedAt = DateTime.UtcNow
            };
            _workOrders.Add(workOrder);
            return workOrder;
        }
    }

    public bool Close(int id)
    {
        lock (_lock)
        {
            var workOrder = _workOrders.FirstOrDefault(w => w.Id == id);
            if (workOrder == null)
            {
                return false;
            }

            workOrder.Status = WorkOrderStatus.Closed;
            return true;
        }
    }

    private void Seed(string title, string segment, WorkOrderPriority priority, int daysOpen)
    {
        _workOrders.Add(new WorkOrder
        {
            Id = _nextId++,
            Title = title,
            PipelineSegment = segment,
            Priority = priority,
            Status = WorkOrderStatus.Open,
            OpenedAt = DateTime.UtcNow.AddDays(-daysOpen)
        });
    }
}
