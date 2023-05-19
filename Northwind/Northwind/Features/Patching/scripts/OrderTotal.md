# declarative
```
from Orders 
update 
{
    this.Total = this.Lines.reduce((partial_sum, l) => partial_sum + (l.Quantity * l.PricePerUnit) * (1 - l.Discount),0)
}
```

# imperative
```
from Orders 
update {
	var total = 0;
	for(var i = 0; i < this.Lines.length; i++){
		var line = this.Lines[i];
		total += line.Quantity * line.PricePerUnit * (1 - line.Discount);
	}
	this.Total = total;
}
```
