```
from "Orders" update {
    var shipper = load(this.ShipVia);
    delete shipper["@metadata"];
    this.Shipper = shipper;
}
```

```
from Orders update {
    let company = load(this.Company);
    this.CompanyCountry = company.Address.Country;
}
```