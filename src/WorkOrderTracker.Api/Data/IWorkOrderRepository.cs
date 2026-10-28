using WorkOrderTracker.Api.Models;

namespace WorkOrderTracker.Api.Data;

public interface IWorkOrderRepository
{
    IEnumerable<WorkOrder> GetAll();
    WorkOrder? GetById(int id);
    WorkOrder Add(CreateWorkOrderRequest request);
    bool Close(int id);
}
