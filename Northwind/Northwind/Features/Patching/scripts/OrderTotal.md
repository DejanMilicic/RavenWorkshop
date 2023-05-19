```
from Orders 
update 
{
    this.Total = this.Lines.reduce((partial_sum, l) => partial_sum + (l.Quantity * l.PricePerUnit) * (1 - l.Discount),0)
}
```


