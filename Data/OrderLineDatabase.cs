using ErpCli.Models;
using Mysqlx.Crud;


namespace ErpCli.Data
{
    public partial class Database
    {
        List<OrderLine> OrderLineList = new List<OrderLine>()
        {
            new OrderLine {Id = 1},
            new OrderLine {Id = 2}
        };
        public OrderLine? GetOrderLine(int id)
        {
            for (int i = 0; i < OrderLineList.Count; i++)
            {
                OrderLine orderLine = OrderLineList[i];
                if (id == orderLine.Id)
                {
                    return orderLine;
                }
            }
            return null;
        }
        public List<OrderLine> GetAllOrderLine(List<int> Ids)
        {
            List<OrderLine> OrderLineCopy = new();
            for (int i = 0; i < OrderLineList.Count; i++)
            {
                OrderLine orderLine = OrderLineList[i];

                foreach (var id in Ids)
                {
                    if(id == orderLine.Id)
                    {
                        OrderLineCopy.Add(orderLine);
                    }
                }
            }
            return OrderLineCopy;
        }
        public void AddOrderLine(OrderLine orderLine, int headerId)
        {
            SalesOrderHeader? header = GetSalesOrderHeader(headerId);
            if (orderLine.Id != 0) return;
            orderLine.Id = OrderLineList.Count + 1;
            OrderLineList.Add(orderLine);
            header?.OrderLineIdList.Add(orderLine.Id);
        }
        public void UpdateOrderLine(OrderLine editOrderLine)
        {
            for (int i = 0; i < OrderLineList.Count; i++)
            {
                OrderLine orderLine = OrderLineList[i];
                if (orderLine.Id == editOrderLine.Id)
                    OrderLineList[i] = editOrderLine;
            }
        }
        public void DeleteOrderLine(int id)
        {
            for (int i = 0; i < OrderLineList.Count; i++)
            {
                OrderLine orderLine = OrderLineList[i];
                if (orderLine.Id == id)
                {
                    OrderLineList.RemoveAt(i);
                    break;
                }
            }
        }

        
    }
    
}