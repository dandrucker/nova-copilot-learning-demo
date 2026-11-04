using WorkOrderTracker.Api.Models;

namespace WorkOrderTracker.Api.Data;

public interface IWorkOrderRepository
{
    Task<IReadOnlyList<WorkOrder>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<WorkOrder>> GetOverdueAsync(CancellationToken cancellationToken);
    Task<WorkOrder?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<WorkOrder> AddAsync(CreateWorkOrderRequest request, CancellationToken cancellationToken);
    Task<WorkOrder?> CloseAsync(int id, CancellationToken cancellationToken);
}
