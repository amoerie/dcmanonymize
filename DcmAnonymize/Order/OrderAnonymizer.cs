using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using DcmAnonymize.Names;
using FellowOakDicom;
using KeyedSemaphores;

namespace DcmAnonymize.Order;

public class OrderAnonymizer
{
    private readonly ConcurrentDictionary<string, AnonymizedOrder> _anonymizedOrders = new ConcurrentDictionary<string, AnonymizedOrder>();
    private int _counter = 1;

    public OrderAnonymizer()
    {
    }

    public async Task AnonymizeAsync(DicomAnonymizationContext context)
    {
        var dicomDataSet = context.Dataset;
        var originalOrderName = dicomDataSet.GetSingleValueOrDefault(DicomTag.PlacerOrderNumberImagingServiceRequest, (string?) null);

        if (originalOrderName == null)
        {
            return;
        }
    
        if (!_anonymizedOrders.TryGetValue(originalOrderName, out var anonymizedOrder))
        {
            using (await KeyedSemaphore.LockAsync($"ORDER_{originalOrderName}"))
            {
                if (!_anonymizedOrders.TryGetValue(originalOrderName, out anonymizedOrder))
                {
                    var orderNumber = $"ORDER{DateTime.Now:yyyyMMddHHmm}{_counter++}";
                    anonymizedOrder = new AnonymizedOrder(orderNumber);
                        
                    _anonymizedOrders[originalOrderName] = anonymizedOrder;
                }
            }
        }

        dicomDataSet.AddOrUpdate(DicomTag.PlacerOrderNumberImagingServiceRequest, anonymizedOrder.OrderNumber);
    }
}
