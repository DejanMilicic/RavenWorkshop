```
from "Orders" update {
    var shipper = load(this.ShipVia);
    delete shipper["@metadata"];
    this.Shipper = shipper;
}
```